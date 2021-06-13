using System;
using System.Collections.Generic;
// using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RuntimeAnimatorController))]
public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D playerRB;

    Dictionary<string, GameObject> playerStateColliders = new Dictionary<string, GameObject>();

    [SerializeField]
    Animator playerAnimCtrl;

    [SerializeField]
    SpriteRenderer blobbySpriteRen;

    bool canMove = true;

    bool isJumping = false;

    Collider2D currentObstacle;

    float jumpForce;

    float moveForce;

    float maxMoveVelocity;

    float inAirForce;

    Vector2 playerSlowdownForce = new Vector2(-1.5f, 0f);

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
        if (Physics2D.Raycast(this.transform.position, Vector2.down, 1f).collider)
        {
            isJumping = false;
            playerAnimCtrl.SetBool("fall", false);
            playerAnimCtrl.SetBool("onground", true);

            // Debug.Log("FALLING");
        }

        if (playerRB.velocity.y < Vector2.zero.y)
        {
            playerAnimCtrl.SetBool("fall", true);
        }

        if (canMove)
        {
            Vector2 direction = Vector2.zero;
            Vector2 playerVelocity = playerRB.velocity;

            if ((Input.GetKey("right") || Input.GetKey("d")) && playerVelocity.x <= maxMoveVelocity)
            {
                // To the right...
                blobbySpriteRen.flipX = false;
                if (!isJumping) playerAnimCtrl.SetBool("blobby_Moving", true);
                direction = Vector2.right;
                playerRB.AddForce(direction * moveForce);
            }
            else if ((Input.GetKey("left") || Input.GetKey("a")) && playerVelocity.x >= (maxMoveVelocity * -1))
            {
                // To the left ...
                blobbySpriteRen.flipX = true;
                if (!isJumping) playerAnimCtrl.SetBool("blobby_Moving", true);
                direction = Vector2.left;
                playerRB.AddForce(direction * moveForce);
            }
            else if (Input.GetKey("down") || Input.GetKey("s"))
            {
                //? how low can you go?
                //* not very.... I'm gettin' old, fam :/
                //* gotta start doing those stretches everyday. Yoga is apparently good.
            }
            else
            {
                playerAnimCtrl.SetBool("blobby_Moving", false);
                if (playerVelocity != Vector2.zero && !isJumping)
                    playerRB.AddForce(playerVelocity * playerSlowdownForce);
            }

            if (Input.GetKey("space") && !isJumping)
            {
                Debug.Log("Jump!");
                //Stop HORIZ movement anim...
                playerAnimCtrl.SetBool("blobby_Moving", false);
                playerAnimCtrl.SetBool("jump", true);
                // One hop this time...
                playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = true;
            }
        }
    }

    public void Initialize(float playerJumpImpulseForce, float playerMoveForce, float playerMaxMoveVelocity, float playerAirForce)
    {
        jumpForce = playerJumpImpulseForce;
        moveForce = playerMoveForce;
        maxMoveVelocity = playerMaxMoveVelocity;
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
                isJumping = false;
                // playerRB.velocity = Vector2.zero;
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
