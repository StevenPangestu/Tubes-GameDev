using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;

    private float horizontalInput;
    private float jumpInput;
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    private bool isGrounded = false;
    public Rigidbody2D rb;
    private bool isLookingRight = true;
    // private float leftPosLimit = -20.0f;
    public static int health = 5;
    public static int grenadeOwned = 0;
    private bool isUsingGrenade = false;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    Animator animator;
    void Start()
    {
        bulletPrefab.SetActive(false);
        //grenadePrefab.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetAxis("Jump");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mousePos - transform.position).normalized;
        
        if (mousePos.x > transform.position.x)
        {
            transform.localScale = new Vector3(0.3f, 0.3f, 1);
            isLookingRight = true;
        }
        else
        {
            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
            isLookingRight = false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            BulletShot();
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager.playSFX(audioManager.shoot);
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (grenadeOwned > 0)
            {
                grenadeOwned--;
                GameController gameController = FindObjectOfType<GameController>();
                gameController.UpdateGrenade(grenadeOwned);
                isUsingGrenade = true;
                BulletShot();
            }

        }
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Koreksi angle jika menghadap kiri agar sesuai arah sprite
        float adjustedAngle = angle;
        if (!isLookingRight)
        {
            adjustedAngle = 180 - angle;
            if (adjustedAngle > 180) adjustedAngle -= 360;
        }

        // Deteksi apakah sedang aim ke atas/bawah
        if (Mathf.Abs(adjustedAngle) > 15)
        {
            animator.SetBool("isAimingUpDown", true);
        }
        else
        {
            animator.SetBool("isAimingUpDown", false);
        }
        animator.SetFloat("yAim", angle);

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

        transform.Translate(Vector2.right * horizontalInput * moveSpeed * Time.deltaTime);
    }
    private void FixedUpdate()
    {

        animator.SetFloat("xVelocity", Mathf.Abs(horizontalInput));
        animator.SetFloat("yVelocity", Mathf.Abs(jumpInput));

    }
    public void TakeDamage(int damage)
    {
        GameController gameController = FindObjectOfType<GameController>();
        health -= damage;
        gameController.UpdateHealth(health);
        Debug.Log("Player took damage, health remaining: " + health);

        if (health <= 0)
        {
            Debug.Log("Player is dead");
            animator.SetBool("isDead", true);
            Destroy(gameObject, 1f);
            gameController.showFailed();
        }
    }
    void Heal()
    {
        GameController gameController = FindObjectOfType<GameController>();
        if (health < gameController.maxHealth)
        {
            health++;
            gameController.UpdateHealth(health);
            Debug.Log("Player healed, health remaining: " + health);
        }
    }
    void BulletShot()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = (mousePos - transform.position).normalized;

        float offsetDistance = 2.0f;
        Vector3 spawnPosition = transform.position + direction * offsetDistance;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (isUsingGrenade)
        {
            isUsingGrenade = false;
            // Instantiate grenade
            GameObject grenade = Instantiate(grenadePrefab, spawnPosition, Quaternion.identity);
            //if success in instantiating grenade
           
            grenade.transform.rotation = Quaternion.Euler(0, 0, angle);
            grenade.GetComponent<ThrowBomb>().SetDirection(direction);
            grenade.SetActive(true);
            return; // Exit after throwing grenade
        }
        //bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
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

        if (other.gameObject.CompareTag("HealPotion"))
        {
            Heal();
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("GrenadeSpawn"))
        {
            grenadeOwned++;
            GameController gameController = FindObjectOfType<GameController>();
            gameController.UpdateGrenade(grenadeOwned);
            Destroy(other.gameObject);
        }
    }
}