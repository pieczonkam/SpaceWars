using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private AudioManager audioManager;
    private ObjectPooler objectPooler;

    private Vector2 size;
    private Vector2 sizeExact;

    [SerializeField] string impactEffectTag;
    [SerializeField] GameObject impactEffect;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private int damage = 20;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        rb.velocity = transform.up * speed;
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
        objectPooler = ObjectPooler.instance;
        
        size = sr.bounds.size;
        sizeExact = bc.bounds.size;
    }

    private void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        if (transform.position.y - size.y / 2.0f > screenSize.y / 2.0f)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        Enemy enemy = collision.GetComponent<Enemy>();
        Boss boss = collision.GetComponent<Boss>();

        if (asteroid != null || enemy != null || boss != null)
        {
            Vector2 impactPosition = new Vector2(transform.position.x, transform.position.y + sizeExact.y / 2.0f);
            objectPooler.SpawnFromPool(impactEffectTag, impactPosition, transform.rotation);
            if (enemy != null)
                enemy.ApplyDamage(damage);
            if (boss != null)
                boss.ApplyDamage(damage);
            if (asteroid != null)
                audioManager.Play("PlayerBulletImpact");

            gameObject.SetActive(false);
        }
    }
}
