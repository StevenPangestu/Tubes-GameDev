using UnityEngine;

// Pengganti EnemyController.cs. Otak musuh sekarang FSM eksplisit:
// Idle/Patrol -> Attack -> Dead
//
// Default-nya musuh DIAM di tempat (persis perilaku lama).
// Centang "Enable Patrol" di Inspector kalau mau musuh ini jalan
// bolak-balik di sekitar titik spawn-nya selama belum lihat player.
public class EnemyFSMController : MonoBehaviour
{
    public static int enemyCount = 0;
    public static int enemiesKilled = 0;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public int maxHealth = 3;
    public float detectRange = 16f;      // dulu hardcoded "distanceToPlayer > 16f"
    public float shootCooldownMin = 1f;  // dulu Random.Range(1f, 6f)
    public float shootCooldownMax = 3f;

    [Header("Movement (opsional)")]
    public bool enablePatrol = false;    // false = perilaku lama, diam total
    public float patrolSpeed = 1.5f;
    public float patrolRange = 2f;       // jarak maksimum dari titik spawn

    [Header("Sprites")]
    public Sprite aimForwardSprite;
    public Sprite aimUpSprite;
    public Sprite aimDownSprite;

    [HideInInspector] public int health;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public bool isDead;

    private Rigidbody2D rb;
    private StateMachine stateMachine;
    private EnemyIdleState idleState;
    private EnemyAttackState attackState;
    private EnemyDeadState deadState;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // boleh null kalau musuh gak pakai Rigidbody2D, tetap aman
        health = maxHealth;

        stateMachine = new StateMachine();
        idleState = new EnemyIdleState(this);
        attackState = new EnemyAttackState(this);
        deadState = new EnemyDeadState(this);
    }

    void Start()
    {
        animator.enabled = false; // animator baru aktif saat mati, sama seperti sebelumnya

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        enemyCount++;
        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        UpdateFacingAndAim();
        stateMachine.Tick();
    }

    void OnDestroy()
    {
        if (enemyCount > 0)
        {
            enemyCount--;
            enemiesKilled++;
        }
    }

    // ===== Dipanggil oleh state-state di EnemyStates.cs =====

    public float DistanceToPlayer()
    {
        if (playerTransform == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, playerTransform.position);
    }

    public float GetX()
    {
        return rb != null ? rb.position.x : transform.position.x;
    }

    // Gerak horizontal yang aman dipakai bareng Rigidbody2D.
    // Kalau ada Rigidbody2D, pakai MovePosition supaya gak tabrakan/ketimpa physics step.
    // Kalau gak ada Rigidbody2D, fallback set transform langsung (untuk musuh tanpa fisik).
    public void MoveHorizontalTo(float targetX)
    {
        if (rb != null)
        {
            rb.MovePosition(new Vector2(targetX, rb.position.y));
        }
        else
        {
            Vector3 pos = transform.position;
            pos.x = targetX;
            transform.position = pos;
        }
    }

    public void GoToIdle() => stateMachine.ChangeState(idleState);
    public void GoToAttack() => stateMachine.ChangeState(attackState);
    public void GoToDead() => stateMachine.ChangeState(deadState);

    public void Shoot()
    {
        if (playerTransform == null) return;

        Vector3 shootDirection = (playerTransform.position - transform.position).normalized;
        Vector3 spawnPosition = transform.position + shootDirection * 1.5f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null) audioManager.playSFX(audioManager.enemyShoot);
            enemyBullet.SetDirection(shootDirection);
        }

        bullet.SetActive(true);
    }

    // ===== Sisa logika asli, tidak berubah =====

    void UpdateFacingAndAim()
    {
        if (playerTransform == null || isDead) return;

        float yTolerance = 1f;
        float yDifference = playerTransform.position.y - transform.position.y;

        if (yDifference > yTolerance) spriteRenderer.sprite = aimUpSprite;
        else if (yDifference < -yTolerance) spriteRenderer.sprite = aimDownSprite;
        else spriteRenderer.sprite = aimForwardSprite;

        transform.localScale = new Vector3(
            playerTransform.position.x > transform.position.x ? -1.5f : 1.5f,
            1.5f, 1);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0) GoToDead();
    }
}
