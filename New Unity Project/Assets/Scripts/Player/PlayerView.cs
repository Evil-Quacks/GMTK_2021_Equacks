using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimatorController))]
public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D playerRB;

    Dictionary<string, GameObject> playerStateColliders = new Dictionary<string, GameObject>();

    Animator playerAnimCtrl;

    bool canMove = true;

    bool isNotJumping = true;

    Collider2D currentObstacle;

    float jumpForce;

    float moveForce;

    float inAirForce;

    private void Start()
    {
        Debug.Log("PV ==> Created");

        EventManager.instance.AddListener<PlayerEvents.RespawnPlayer>(OnRespawnPlayer);
    }

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Check if falling
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, 2f);
        if (!Physics2D.Raycast(this.transform.position, Vector2.down, 2f))
        {
            isNotJumping = true;
            Debug.Log("FALLING");
        }

        if (canMove)
        {
            Vector2 direction = Vector2.zero;
            if ((Input.GetKey("right") || Input.GetKey("d")))
            {
                // To the right...
                direction = Vector2.right;
            }
            else if ((Input.GetKey("left") || Input.GetKey("a")))
            {
                // To the left ...
                direction = Vector2.left;
            }
            else if (Input.GetKey("space") && isNotJumping)
            {
                // One hop this time...
                direction = Vector2.up;
                isNotJumping = false;
            }
            else if (Input.GetKey("down") || Input.GetKey("s"))
            {
                //? how low can you go?
                //* not very.... I'm gettin' old, fam :/
            }

            if (direction != Vector2.zero)
                UpdateMovement(direction, Time.fixedDeltaTime);
        }
    }

    private void UpdateMovement(Vector2 direction, float delta)
    {
        Vector2 currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);

        if (isNotJumping)
        {
            playerRB.MovePosition(currentPosition + direction * moveForce * delta);
        }
        else
        {
            if (direction != Vector2.up)
            {
                // Obey gravity if we're not >.<
                playerRB.AddForce(direction * inAirForce);
            }
            else
            {
                // We're jumping upwards :D
                playerRB.AddForce(direction * jumpForce);
            }
        }
    }

    public void Initialize(float playerJumpSpeed, float playerMoveSpeed, float playerAirForce)
    {
        jumpForce = playerJumpSpeed;
        moveForce = playerMoveSpeed;
        inAirForce = playerAirForce;

        playerAnimCtrl = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Check to make sure we're not trying to pass over the same obstacle every frame of movement
        if (other.collider != currentObstacle)
        {
            if (other.gameObject.tag == "OBST")
            {
                canMove = false;
                currentObstacle = other.collider;
                //Fire Off Event
                EventManager.instance.QueueEvent(new PlayerEvents.ObsCollision(other.gameObject, AllowMoveThrough));
            }
            else if (other.gameObject.tag == "GRND")
            {
                isNotJumping = true;
                playerRB.velocity = Vector2.zero;
                //Raycast out
                if (ReturnDirection(this.gameObject, other.gameObject) != HitDirection.Bottom)
                {
                    currentObstacle = other.collider;
                    AllowMoveThrough(false);
                }
            }
            else if (other.gameObject.tag == "Finish")
            {
                currentObstacle = other.collider;
                // Fire off event
                EventManager.instance.QueueEvent(new PlayerEvents.ObsCollision(other.gameObject, CanUnlockDoor));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider == currentObstacle)
        {
            //We left the currentObstacle.
            currentObstacle = null;
            canMove = true;
        }
    }

    private void OnRespawnPlayer(PlayerEvents.RespawnPlayer @event)
    {
        playerRB.velocity = Vector2.zero;
    }
    private void AllowMoveThrough(bool canPass)
    {
        if (canPass)
        {
            //Resume Moving
            canMove = true;
        }
        else
        {
            //Prevent Moving - push them out of the trigger
            Vector2 force = this.transform.position - currentObstacle.transform.position;

            force.Normalize();
            playerRB.AddForce(-force, ForceMode2D.Impulse);
            currentObstacle = null;
        }
    }

    private void CanUnlockDoor(bool canUnlock)
    {
        if (canUnlock)
        {
            Debug.Log("YOU WON!");
            playerRB.Sleep();
            playerAnimCtrl.Play("TRANSFORM");
            return;
        }

        Debug.Log("Sorry, but that's not a key to this door");
    }

    // TOOK THIS CODE FROM UNITY FORUMS - NEEDS REVIEWED AND POSSIBLY REVISED
    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }
    private HitDirection ReturnDirection(GameObject player, GameObject objCollision)
    {

        HitDirection hitDirection = HitDirection.None;
        RaycastHit MyRayHit;
        Vector3 direction = (player.transform.position - objCollision.transform.position).normalized;
        Ray MyRay = new Ray(objCollision.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {

            if (MyRayHit.collider != null)
            {

                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up) { hitDirection = HitDirection.Top; }
                if (MyNormal == -MyRayHit.transform.up) { hitDirection = HitDirection.Bottom; }
                if (MyNormal == MyRayHit.transform.forward) { hitDirection = HitDirection.Forward; }
                if (MyNormal == -MyRayHit.transform.forward) { hitDirection = HitDirection.Back; }
                if (MyNormal == MyRayHit.transform.right) { hitDirection = HitDirection.Right; }
                if (MyNormal == -MyRayHit.transform.right) { hitDirection = HitDirection.Left; }
            }
        }
        return hitDirection;
    }
}
