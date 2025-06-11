using UnityEngine;

public class BulletForward : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Vector3 direction; // Default direction
    private Animator BulletAnimator;
    private bool isHit = false;
    private bool isHitBarrier = false;
    private Vector3 startPosition;
    [SerializeField] private float maxRange = 14f;
    //private Rrigidbody2D rb;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        BulletAnimator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //rb.linearVelocity = direction * speed;
    }

    void Update()
    {

        if (!isHit)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            if (distanceTravelled >= maxRange)
            {
                TriggerHit(); // Bullet auto-destroys after exceeding range
            }
        }

        if (!isHit && isHitBarrier)
        {
            TriggerHit();
        }
    }

    private void TriggerHit()
    {
        isHit = true;
        BulletAnimator.SetBool("isHit", true);
        speed = 0f;
        Destroy(gameObject, 0.5f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isHit && other.gameObject.CompareTag("Enemy"))
        {
            TriggerHit();
            EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
            enemyController.TakeDamage(1);
        }
        else if (!isHit && other.gameObject.CompareTag("Barrier"))
        {
            isHitBarrier = true;
            TriggerHit();
        }
    }
}
