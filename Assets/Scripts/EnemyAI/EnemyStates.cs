using UnityEngine;

public class EnemyIdleState : IState
{
    private readonly EnemyFSMController enemy;
    private Vector3 spawnPosition;
    private int patrolDirection = 1;

    public EnemyIdleState(EnemyFSMController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        spawnPosition = enemy.transform.position;

            enemy.facingMode = enemy.enablePatrol
            ? EnemyFSMController.FacingMode.Patrol
            : EnemyFSMController.FacingMode.Player;
        enemy.patrolFacingDirection = patrolDirection;
    }

    public void Tick()
    {
        if (enemy.enablePatrol)
        {
            Patrol();
        }

        if (enemy.DistanceToPlayer() <= enemy.detectRange)
        {
            enemy.GoToAttack();
        }
    }

    public void Exit() { }

    private void Patrol()
    {
        float currentX = enemy.GetX();

        // Cek posisi SEKARANG dulu terhadap batas, baru tentukan arah -> gak kebablasan lewat batas.
        if (currentX >= spawnPosition.x + enemy.patrolRange) patrolDirection = -1;
        else if (currentX <= spawnPosition.x - enemy.patrolRange) patrolDirection = 1;

        float newX = currentX + patrolDirection * enemy.patrolSpeed * Time.deltaTime;

        enemy.patrolFacingDirection = patrolDirection;
        enemy.MoveHorizontalTo(newX);
    }
}

// Musuh berhenti jalan (kalau lagi patrol), menembak berkala selama player masih dalam jangkauan.
// Kalau player keluar jangkauan, balik ke Idle/Patrol.
public class EnemyAttackState : IState
{
    private readonly EnemyFSMController enemy;
    private float timer;
    private float nextShootDelay;

    public EnemyAttackState(EnemyFSMController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        timer = 0f;
        PickNextShootDelay();

        // Pas nyerang, sprite tetap ngadep player kayak semula.
        enemy.facingMode = EnemyFSMController.FacingMode.Player;
    }

    public void Tick()
    {
        if (enemy.DistanceToPlayer() > enemy.detectRange)
        {
            enemy.GoToIdle();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= nextShootDelay)
        {
            enemy.Shoot();
            timer = 0f;
            PickNextShootDelay();
        }
    }

    public void Exit() { }

    private void PickNextShootDelay()
    {
        nextShootDelay = Random.Range(enemy.shootCooldownMin, enemy.shootCooldownMax);
    }
}

// Musuh mati: matikan collider, mainkan animasi mati, lalu destroy.
public class EnemyDeadState : IState
{
    private readonly EnemyFSMController enemy;

    public EnemyDeadState(EnemyFSMController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.isDead = true;
        enemy.animator.enabled = true;
        enemy.animator.SetBool("isDead", true);

        Collider2D col = enemy.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Object.Destroy(enemy.gameObject, 1.25f);
    }

    public void Tick() { }
    public void Exit() { }
}