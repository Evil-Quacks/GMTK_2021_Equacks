using UnityEngine;

public class SpriteBob : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    float height = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        float newY = Mathf.Sin(Time.time * speed);
        transform.position = new Vector2(pos.x, newY) * height;
    }
}
