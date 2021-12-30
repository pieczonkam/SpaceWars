using UnityEngine;

public class Background : MonoBehaviour
{
    private static Background instance = null;

    private SpriteRenderer sr;

    private float height;
    private float initPosY;

    [SerializeField] private float velocity = 0.002f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        sr = GetComponent<SpriteRenderer>();        
    }

    private void Start()
    {
        height = sr.bounds.size.y;
        initPosY = transform.position.y;
    }

    private void FixedUpdate()
    {
        if (transform.position.y <= initPosY - height / 2.0f) 
            transform.position = new Vector2(0.0f, initPosY);
        else 
            transform.position = new Vector2(transform.position.x, transform.position.y - velocity);
    }
}
