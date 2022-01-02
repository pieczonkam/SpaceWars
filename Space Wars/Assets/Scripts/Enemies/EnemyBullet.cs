using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private AudioManager audioManager;
    private ObjectPooler objectPooler;

    private Vector2 size;

    [SerializeField] private string impactEffectTag;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float speed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        rb.velocity = -transform.up * speed;
        size = sr.bounds.size;
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        objectPooler = ObjectPooler.instance;
    }

    private void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();

        if (transform.position.y > screenSize.y / 2.0f + size.y / 2.0f
            || transform.position.y < -screenSize.y / 2.0f - size.y / 2.0f
            || transform.position.x > screenSize.x / 2.0f + size.x / 2.0f
            || transform.position.x < -screenSize.x / 2.0f - size.x / 2.0f)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        Player player = collision.GetComponent<Player>();

        if (asteroid != null || (player != null && !player.immortal))
        {
            if (asteroid != null)
                audioManager.Play("EnemyBulletImpact");
            objectPooler.SpawnFromPool(impactEffectTag, rb.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
