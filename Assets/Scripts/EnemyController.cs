using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static int enemyCount = 0;
    public static int enemiesKilled = 0;
    public GameObject bulletPrefab;
    public float moveSpeed = 5f;
    private int health = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyCount++;
        InvokeRepeating("EnemyShoot", Random.Range(1f, 5f), Random.Range(1f, 8f)); // Call EnemyShoot every second
    }

    // Update is called once per frame
    void Update()
    {

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
            return; // Don't shoot if the player is too far away
        }
        //get the shoot direction according the player position
        Vector3 playerPosition = player.transform.position;
        Vector3 shootDirection = (playerPosition - transform.position).normalized;

        // Calculate spawn position with a gap of 1.5f in the shoot direction
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
