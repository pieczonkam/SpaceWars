using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioManager audioManager;

    private Vector2 size;

    [SerializeField] GameObject impactEffect;
    [SerializeField] private float speed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        rb.velocity = -transform.up * speed;
        size = sr.bounds.size;
        audioManager.Play("EnemyBulletShot");
    }

    private void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();

        if (transform.position.y > screenSize.y / 2.0f + size.y / 2.0f 
            || transform.position.y < -screenSize.y / 2.0f - size.y / 2.0f 
            || transform.position.x > screenSize.x / 2.0f + size.x / 2.0f 
            || transform.position.x < -screenSize.x / 2.0f - size.x / 2.0f) 
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        Player player = collision.GetComponent<Player>();

        if (asteroid != null || (player != null && !player.immortal))
        {
            if (asteroid != null)
                audioManager.Play("EnemyBulletImpact");
            Instantiate(impactEffect, rb.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
