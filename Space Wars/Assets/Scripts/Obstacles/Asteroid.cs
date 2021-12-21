using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 size;

    [SerializeField] private PolygonCollider2D[] colliders;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float scaleMin = 0.4f;
    [SerializeField] private float scaleMax = 1.0f;
    [SerializeField] private float velocity = 0.9f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        float scale = Random.Range(scaleMin, scaleMax);
        int i = Random.Range(0, sprites.Length);

        size = sr.bounds.size;
        transform.localScale = new Vector2(scale, scale);
        rb.velocity = -transform.up * velocity;

        sr.sprite = sprites[i];
        colliders[i].enabled = true;
    }

    private void Update()
    {
        Vector2 screenSize = CameraManager.GetScreenSize();
        if (transform.position.y < -(screenSize.y / 2.0f + size.y / 2.0f))
            Destroy(gameObject);
    }
}
