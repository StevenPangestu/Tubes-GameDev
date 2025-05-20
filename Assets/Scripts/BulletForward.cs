using UnityEngine;

public class BulletForward : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Vector3 direction = Vector3.right; // Default direction to the right
    Animator animator;

    private bool isHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }
    // Update is called once per frame
    void Update()
    {


        transform.Translate(direction * speed * Time.deltaTime);



        if (!isHit && (transform.position.x > 9.0f || transform.position.x < -15.0f))
        {
            TriggerHit();
        }
    }
    // private void FixedUpdate()
    // {

    // }
    private void TriggerHit()
    {
        isHit = true;
        animator.SetBool("isHit", true);
        speed = 0f;
        Destroy(gameObject,0.5f);
     
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isHit && other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            TriggerHit();
        }
    }
}
