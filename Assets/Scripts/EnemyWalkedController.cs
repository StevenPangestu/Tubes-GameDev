using UnityEngine;

public class EnemyWalkedController : MonoBehaviour
{
    public static int enemyCount = 0;
    public static int enemiesKilled = 0;
    public GameObject bulletPrefab;
    public float moveSpeed = 5f;
    private int health = 3;

    // Movement tracking
    private float distanceMoved = 0f;
    private Vector3 lastPosition;
    public float shootDistance = 10f;

    void Start()
    {
        enemyCount++;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move enemy forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Calculate how far the enemy has moved
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Check if the enemy has moved enough to shoot
        if (distanceMoved >= shootDistance)
        {
            EnemyShoot();
            distanceMoved = 0f; // Reset the moved distance
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
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > 16f) return;

        Vector3 playerPosition = player.transform.position;
        Vector3 shootDirection = (playerPosition - transform.position).normalized;
        Vector3 spawnPosition = transform.position + shootDirection * 1.5f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
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
            Destroy(gameObject);
        }
    }
}
