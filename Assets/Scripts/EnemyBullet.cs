using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private Vector3 direction;
    Animator animator;
    private bool isEnemyHit = false;
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isEnemyHit && other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {   
                Debug.Log("Enemy bullet hit the player");
                player.TakeDamage(1);
            }
            else if (!isEnemyHit && other.gameObject.CompareTag("Barrier"))
            {   
                Debug.Log("Enemy bullet hit the barrier");
                TriggerHit();
            }
            else if (isEnemyHit && other.gameObject.CompareTag("Ground"))
            {
                TriggerHit();
            }
        }

        Destroy(gameObject); // Destroy self on any collision
    }
    
    private void TriggerHit()
    {
        isEnemyHit = true;
        animator.SetBool("isEnemyHit", true);
        // speed = 0f;

        //get bullet prefab from the EnemyController
       
        Destroy(gameObject, 0.5f);

    }
}
