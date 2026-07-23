using UnityEngine;
using System.Collections;

// Class tetap bernama BossScript (sama seperti sebelumnya) supaya BulletForward.cs,
// ThrowBomb.cs, dan GameController.cs TIDAK perlu diubah sama sekali.
// Yang berubah cuma isi otaknya: dari bool-chain manual (isAttacking/isBackingAway)
// jadi Behavior Tree eksplisit lewat method BuildBehaviorTree().
public class BossScript : MonoBehaviour
{
    private Animator animator;
    public bool isBossDead = false;
    private int maxHealth = 15;
    private int health = 15;
    private float attackCooldown = 2f;
    private float lastAttackTime = 0f;
    private float followDistance = 10f;
    private float stopDistance = 3f;
    private float attackRange = 3f;
    private float attackDamage = 1f;
    private bool isBackingAway = false;
    private float backAwayDuration = 1f;
    private enum AttackType { Melee, Ranged }
    private AttackType currentAttack;
    public static int bossDefeated = 0;
    private GameObject player;
    private AudioManager audioManager;

    [Header("Debug")]
    public bool showDebugLogs = false; // centang di Inspector buat lihat cabang BT mana yang aktif di Console

    [Header("Low HP Behavior")]
    [Range(0f, 1f)] public float lowHealthPercent = 0.3f; // di bawah persentase ini, boss prioritas mundur+cast

    private IBTNode behaviorTree;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioManager = FindObjectOfType<AudioManager>();

        behaviorTree = BuildBehaviorTree();
    }

    void Update()
    {
        UpdateFacingDirection();

        if (isBossDead || player == null) return;

        behaviorTree.Tick();
    }

    // ===== Behavior Tree =====
    // Dicoba dari atas ke bawah tiap frame, dipakai cabang PERTAMA yang valid.
    private IBTNode BuildBehaviorTree()
    {
        return new Selector(
            // 1. Kalau lagi mundur untuk cast spell (coroutine BackAwayThenCast jalan),
            //    jangan proses cabang lain dulu tick ini.
            new ActionNode(() =>
            {
                if (!isBackingAway) return NodeStatus.Failure;
                Log("1. Sedang mundur+cast (Running)");
                return NodeStatus.Running;
            }),

            // 2. Player kabur / terlalu jauh -> diam
            new Sequence(
                new ConditionNode(() => DistanceToPlayer() > followDistance),
                new ActionNode(() =>
                {
                    Log("2. Player di luar followDistance -> diam");
                    SetXVelocity(0f);
                    return NodeStatus.Success;
                })
            ),

            // 3. Player masih di luar jarak berhenti -> kejar
            new Sequence(
                new ConditionNode(() => DistanceToPlayer() > stopDistance),
                new ActionNode(() =>
                {
                    Log("3. Kejar player");
                    MoveTowardPlayer();
                    return NodeStatus.Success;
                })
            ),

            // 4. HP rendah, dekat, & cooldown siap -> PAKSA mundur+cast (bukan random lagi)
            new Sequence(
                new ConditionNode(() => health <= maxHealth * lowHealthPercent
                                         && DistanceToPlayer() <= attackRange
                                         && Time.time >= lastAttackTime + attackCooldown),
                new ActionNode(() =>
                {
                    Log($"4. HP rendah ({health}/{maxHealth}) -> paksa mundur+cast");
                    SetXVelocity(0f);
                    ForceRangedAttack();
                    lastAttackTime = Time.time;
                    return NodeStatus.Success;
                })
            ),

            // 5. Player dekat & cooldown serangan sudah siap -> serang (acak melee/ranged)
            new Sequence(
                new ConditionNode(() => DistanceToPlayer() <= attackRange
                                         && Time.time >= lastAttackTime + attackCooldown),
                new ActionNode(() =>
                {
                    Log("5. Serang (acak melee/ranged)");
                    SetXVelocity(0f);
                    AttackPlayer();
                    lastAttackTime = Time.time;
                    return NodeStatus.Success;
                })
            ),

            // 6. Fallback: cukup dekat tapi belum boleh nyerang lagi -> berhenti & tunggu
            new ActionNode(() =>
            {
                Log("6. Fallback -> berhenti nunggu cooldown");
                SetXVelocity(0f);
                return NodeStatus.Success;
            })
        );
    }

    private void Log(string message)
    {
        if (showDebugLogs) Debug.Log($"[BossBT] {message}");
    }

    // ===== Helper yang dipanggil node-node BT di atas =====

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private void SetXVelocity(float value)
    {
        animator.SetFloat("xVelocity", value);
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 3.5f);
        animator.SetFloat("xVelocity", player.transform.position.x - transform.position.x);
    }

    // ===== Sisa logika asli, tidak berubah =====

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

    // Dipakai khusus oleh cabang HP-rendah di BT: paksa Ranged, gak diundi lagi.
    void ForceRangedAttack()
    {
        currentAttack = AttackType.Ranged;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("CastSpell");

        if (!isBackingAway)
        {
            StartCoroutine(BackAwayThenCast());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isBossDead) return;

        health -= damage;
        animator.SetTrigger("Hit");
        Debug.Log("Boss took damage! Current health: " + health);
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

        StartCoroutine(DelayedDisable());
    }

    IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        this.enabled = false;
    }
}
