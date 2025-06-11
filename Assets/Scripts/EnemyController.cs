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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
        enemyCount++;
        InvokeRepeating("EnemyShoot", Random.Range(1f, 5f), Random.Range(1f, 8f)); // Call EnemyShoot every second
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAimingAnimation();
    }
    void UpdateAimingAnimation()
    {
      
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        float yTolerance = 2f;
        float yDifference = player.transform.position.y - transform.position.y;

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

        // Tambahkan ini jika ingin flip saat musuh menghadap ke kanan atau kiri
        if (player.transform.position.x > transform.position.x)
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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > 16f)
        {
            return;
        }

        Vector3 playerPosition = player.transform.position;
        Vector3 shootDirection = (playerPosition - transform.position).normalized;

        Vector3 spawnPosition = transform.position + shootDirection * 1.5f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
        //set the animation up


        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(shootDirection);
        }
        bullet.SetActive(true);
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took damage, health remaining: " + health);

        if (health <= 0)
        {
            Debug.Log("Enemy is dead");
            animator.SetBool("isDead", true);
            Destroy(gameObject, 1.25f);
        }

    }
}
