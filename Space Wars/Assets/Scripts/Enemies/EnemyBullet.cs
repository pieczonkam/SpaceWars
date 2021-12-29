using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CircleCollider2D cc;

    private Vector2 size;
    private Vector2 sizeExact;

    [SerializeField] GameObject impactEffect;
    [SerializeField] private float speed = 2.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        rb.velocity = -transform.up * speed;

        size = sr.bounds.size;
        sizeExact = cc.bounds.size;
        FindObjectOfType<AudioManager>().Play("EnemyBulletShot");
    }

    private void Update()
    {
        Vector2 screenSize = CameraController.GetScreenSize();
        if (transform.position.y > screenSize.y / 2.0f + size.y / 2.0f 
            || transform.position.y < -screenSize.y / 2.0f - size.y / 2.0f 
            || transform.position.x > screenSize.x / 2.0f + size.x / 2.0f 
            || transform.position.x < -screenSize.x / 2.0f - size.x / 2.0f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Asteroid asteroid = collision.GetComponent<Asteroid>();
        Player player = collision.GetComponent<Player>();

        if (asteroid != null || (player != null && !player.immortal))
        {
            if (asteroid != null)
                FindObjectOfType<AudioManager>().Play("EnemyBulletImpact");
            Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
