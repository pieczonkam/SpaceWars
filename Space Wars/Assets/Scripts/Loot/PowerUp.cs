using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 size;

    public string forPlayer;
    [SerializeField] private float velocity = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        size = sr.bounds.size;
        rb.velocity = -transform.up * velocity;
    }

    void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        if (transform.position.y + size.y / 2.0f < -screenSize.y / 2.0f)
            gameObject.SetActive(false);
    }
}
