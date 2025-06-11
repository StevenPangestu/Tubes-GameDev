using UnityEngine;

public class ThrowBomb : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float explosionDelay = 1f;
    [SerializeField] private Vector2 explosionRadius = new Vector2(5f, 3f);
    [SerializeField] private int damage = 3;

    private Vector3 direction;
    private Animator grenadeAnimator;
    private bool isHit = false;
    private bool hasExploded = false;
    private float moveTimer = 0f;

    void Start()
    {
        grenadeAnimator = GetComponent<Animator>();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        if (!isHit && moveTimer < explosionDelay)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            moveTimer += Time.deltaTime;
        }
        else if (!isHit)
        {
            // Auto-explode if time runs out
             grenadeAnimator.SetBool("isBombHit", true); // Start hit/explosion animation

            TriggerHit();
        }
    }

    private void TriggerHit()
    {
        if (isHit) return;

        isHit = true;
        speed = 0f;

        Explode(); // fallback if no animator

    }

    // Called from animation event at end of explosion animation
    // public void OnExplosionEnd()
    // {
    //     Explode();
    //     Destroy(gameObject);
    // }

    void Explode()
    {
        // grenadeAnimator.SetBool("isBombHit", false); // Start hit/explosion animation
        Destroy(gameObject, 1.5f); // Destroy after a short delay
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position, explosionRadius, 0f);
        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyController ec = col.GetComponent<EnemyController>();
                if (ec != null)
                {
                    ec.TakeDamage(damage);
                }
            }
        }
    }

    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (isHit) return;

    //     if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Barrier"))
    //     {
    //         TriggerHit();
    //     }
    // }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireCube(transform.position, explosionRadius);
    }
}
