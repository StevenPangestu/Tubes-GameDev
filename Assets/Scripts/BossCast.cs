using UnityEngine;

public class BossCast : MonoBehaviour
{
    public int damage = 2;
    public float destroyAfter = 1.5f; // Animation duration

    void Start()
    {
        Destroy(gameObject, destroyAfter); // Clean up after animation
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
