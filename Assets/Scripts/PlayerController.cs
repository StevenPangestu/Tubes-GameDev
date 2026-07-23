using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;

    private float horizontalInput;
    private float jumpInput;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 30f;
    public float deceleration = 30f;  
    private float currentSpeed = 0f;  

    public float jumpForce = 15f;

    [Header("Jump - Coyote Time, Buffer, Variable Height")]
    public float coyoteTime = 0.15f;     
    public float jumpBufferTime = 0.15f;   
    public float jumpCutMultiplier = 0.5f; 
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool isGrounded = false;
    public Rigidbody2D rb;
    private bool isLookingRight = true;
    // private float leftPosLimit = -20.0f;
    public static int health = 500;
    public static int grenadeOwned = 0;
    private bool isUsingGrenade = false;

    [Header("Debug")]
    public bool showDebugInfo = false;
   
    Animator animator;
    void Start()
    {
        //set the spawn position of the player to the spawn point
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        transform.position = spawnPoint.transform.position;
        
        bulletPrefab.SetActive(false);
        //grenadePrefab.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); 
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

        // ===== COYOTE TIME =====
        // Selama masih di ground, counter selalu di-reset.
        // Begitu lepas dari ground, counter mulai berkurang -> masih ada waktu singkat untuk tetap bisa lompat.
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ===== INPUT BUFFERING =====
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetBool("isJumping", true);

            // reset biar tidak ter-trigger berulang dari input/coyote yang sama
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // ===== VARIABLE JUMP HEIGHT =====
        // Kalau tombol lompat dilepas lebih awal saat karakter masih naik, potong kecepatan vertikalnya.
        // Tekan sebentar = lompatan pendek, tahan = lompatan full.
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // ===== ACCELERATION & DECELERATION =====
        float targetSpeed = horizontalInput * moveSpeed;
        float accelRate = (Mathf.Abs(horizontalInput) > 0.01f) ? acceleration : deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.deltaTime);

        transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
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

        if (health <= 0)
        {
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
            //audio
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager.playSFX(audioManager.heal);
        }

        if (other.gameObject.CompareTag("GrenadeSpawn"))
        {
            grenadeOwned++;
            Destroy(other.gameObject);
            GameController gameController = FindObjectOfType<GameController>();

            gameController.UpdateGrenade(grenadeOwned);

        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        // begitu player benar-benar lepas dari ground (bukan cuma saat melompat).
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.yellow;

        GUI.Label(new Rect(10, 10, 400, 25), "isGrounded: " + isGrounded, style);
        GUI.Label(new Rect(10, 35, 400, 25), "coyoteTimeCounter: " + coyoteTimeCounter.ToString("F2"), style);
        GUI.Label(new Rect(10, 60, 400, 25), "jumpBufferCounter: " + jumpBufferCounter.ToString("F2"), style);
        GUI.Label(new Rect(10, 85, 400, 25), "currentSpeed: " + currentSpeed.ToString("F2"), style);
        GUI.Label(new Rect(10, 110, 400, 25), "rb.velocity.y: " + rb.linearVelocity.y.ToString("F2"), style);
    }
}