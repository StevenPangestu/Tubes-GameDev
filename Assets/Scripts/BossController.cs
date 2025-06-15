using UnityEngine;
using System.Collections;

public class BossScript : MonoBehaviour
{
    private Animator animator;
    public bool isBossDead = false;
    private int health = 15;
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    private float followDistance = 10f;
    private float stopDistance = 3f;
    private float attackRange = 3f;
    private float attackDamage = 1f;
    private bool isBackingAway = false;
    private float backAwayDuration = 1f;
    private bool isAttacking = false;
    private enum AttackType { Melee, Ranged }
    private AttackType currentAttack;
    public static int bossDefeated = 0;
    private GameObject player;
    private AudioManager audioManager;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        audioManager = FindObjectOfType<AudioManager>();

    }

    void Update()
    {
        UpdateFacingDirection();
        if (isBossDead || player == null || isBackingAway || isAttacking) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= followDistance)
        {
            if (distanceToPlayer > stopDistance)
            {
                // Move toward player
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 3.5f);
                animator.SetFloat("xVelocity", player.transform.position.x - transform.position.x);
            }
            else
            {
                // Stop movement
                animator.SetFloat("xVelocity", 0);

                // Try to attack if in range
                if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            // Player too far, idle
            animator.SetFloat("xVelocity", 0);
        }
    }
    public bool IsDead()
    {
        return isBossDead;
    }
    void UpdateFacingDirection()
    {
        if (player == null) return;

        float direction = player.transform.position.x - transform.position.x;

        if (Mathf.Abs(direction) > 0.1f) // Hanya ubah arah jika cukup berbeda
        {
            // Menghadap kanan jika player di kanan, kiri jika player di kiri
            transform.localScale = new Vector3(
                direction > 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    IEnumerator BackAwayThenCast()
    {
        isBackingAway = true;
        float timer = 0f;
        PlayerController playerController = player.GetComponent<PlayerController>();
        while (timer < backAwayDuration)
        {
            Vector3 directionAway = (transform.position - player.transform.position).normalized;
            transform.position += directionAway * Time.deltaTime * 3f;

            animator.SetFloat("xVelocity", directionAway.x);
            timer += Time.deltaTime;
            yield return null;
        }
        playerController.TakeDamage((int)attackDamage);
        // Stop movement and cast
        animator.SetFloat("xVelocity", 0);
        animator.SetTrigger("CastSpell");
        audioManager.playSFX(audioManager.BossCast);

        isBackingAway = false;
    }

    void AttackPlayer()
    {
        int randomAttack = Random.Range(0, 2);
        currentAttack = (randomAttack == 0) ? AttackType.Melee : AttackType.Ranged;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("CastSpell");

        if (currentAttack == AttackType.Melee)
        {
            animator.SetTrigger("Attack");

            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage((int)attackDamage);
                audioManager.playSFX(audioManager.BossSlash);
            }
        }
        else
        {
            if (!isBackingAway)
            {
                StartCoroutine(BackAwayThenCast());
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isBossDead) return;

        health -= damage;
        animator.SetTrigger("Hit");

        if (health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isBossDead = true;
        bossDefeated++;

        animator.SetTrigger("Die");
        audioManager.playSFX(audioManager.BossDeath);

        GameObject portal = GameObject.Find("Portal");
        portal?.SetActive(true);

        GetComponent<Collider2D>().enabled = false;

        // Tunggu animasi selesai baru nonaktifkan
        StartCoroutine(DelayedDisable());
    }

    IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(2f); // Sesuaikan dengan durasi animasi "Die"
        gameObject.SetActive(false);
        this.enabled = false;
    }

}
