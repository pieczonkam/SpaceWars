using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 size;

    [SerializeField] private float velocity = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        size = sr.bounds.size;
        rb.velocity = -transform.up * velocity;
    }

    void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        if (transform.position.y + size.y / 2.0f < -screenSize.y / 2.0f) 
            Destroy(gameObject);
    }
}
