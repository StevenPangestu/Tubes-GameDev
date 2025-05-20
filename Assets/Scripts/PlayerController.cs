using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private float horizontalInput;
    private float jumpInput;
    public float moveSpeed = 10f;
    public float jumpForce = 0f;
    private bool isGrounded = false;
    public Rigidbody2D rb;
    private bool isLookingRight = true;
    Animator animator;
    void Start()
    {
        bulletPrefab.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");

        if (isGrounded)
        {
            if (jumpInput > 0)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                animator.SetBool("isJumping", true);
            }
            else
            {
                isGrounded = true;
            }

        }

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(0.3f, 0.3f, 1);
            isLookingRight = true;
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
            isLookingRight = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            BulletShot(isLookingRight);
        }

        if (transform.position.x < -20.0f)
        {

            transform.position = new Vector3(-10.0f, transform.position.y, 0);

        }
        else if (transform.position.x > 8.0f)
        {
            transform.position = new Vector3(8.0f, transform.position.y, 0);
        }

        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {

        animator.SetFloat("xVelocity", Mathf.Abs(horizontalInput));
        animator.SetFloat("yVelocity",  Mathf.Abs(jumpInput));
        //animator.SetBool("isGrounded", isGrounded);
    }
    void BulletShot(bool isLookingRight)
    {
        Vector3 spawnPosition = transform.position + new Vector3(isLookingRight ? 1.3f : -1.3f, 0, 0);
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        Vector2 direction = isLookingRight ? Vector2.right : Vector2.left;

        // Atur arah peluru
        bullet.GetComponent<BulletForward>().SetDirection(direction);
        bullet.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Ground"))
        {

            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
        else
        {
            isGrounded = false;
        }
    }
}