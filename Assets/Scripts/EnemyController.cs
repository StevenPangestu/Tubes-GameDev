using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static int enemyCount = 0;
    public static int enemiesKilled = 0;

    public GameObject bulletPrefab;
    public float moveSpeed = 5f;
    private int health = 3;

    public Sprite aimForwardSprite;
    public Sprite aimUpSprite;
    public Sprite aimDownSprite;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.enabled = false; // Nonaktifkan animator awalnya

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        enemyCount++;
        InvokeRepeating("EnemyShoot", Random.Range(1f, 6f), Random.Range(1f, 6f));
    }

    void Update()
    {
        UpdateAimingSprite();
    }

    void UpdateAimingSprite()
    {
        if (playerTransform == null) return;
        if (animator != null && animator.GetBool("isDead")) return;

        float yTolerance = 1f;
        float yDifference = playerTransform.position.y - transform.position.y;

        if (yDifference > yTolerance)
        {
            spriteRenderer.sprite = aimUpSprite;
        }
        else if (yDifference < -yTolerance)
        {
            spriteRenderer.sprite = aimDownSprite;
        }
        else
        {
            spriteRenderer.sprite = aimForwardSprite;
        }

        // Flip arah berdasarkan posisi player
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }
        else
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
    }

    void OnDestroy()
    {
        if (enemyCount > 0)
        {
            enemyCount--;
            enemiesKilled++;
        }
    }

    void EnemyShoot()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > 16f) return;

        Vector3 shootDirection = (playerTransform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + shootDirection * 1.5f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            //audio
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager.playSFX(audioManager.enemyShoot);
            enemyBullet.SetDirection(shootDirection);
        }

        bullet.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        if (animator.GetBool("isDead")) return;

        health -= damage;

        if (health <= 0)
        {
            CancelInvoke("EnemyShoot"); // Hentikan tembakan musuh
            animator.enabled = true;
            animator.SetBool("isDead", true);
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 1.25f);
        }
    }

}
