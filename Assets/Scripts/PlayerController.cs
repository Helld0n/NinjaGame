using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 8f;
    public float runSpeed = 14f;
    public float jumpForce = 12f;

    [Header("Проверка земли")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Атака")]
    public bool blockMovementDuringAttack = true;
    public float attackDuration = 0.7f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;
    private bool isRunning;
    private bool isAttacking = false;
    private int attackCounter = 0; // 0 = Attack 1, 1 = Attack 2

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        moveInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && isGrounded)
        {
            if (attackCounter == 0)
            {
                animator.SetTrigger("Attack");   // Attack 1
                attackCounter = 1;
            }
            else
            {
                animator.SetTrigger("Attack2");  // Attack 2
                attackCounter = 0;
            }

            isAttacking = true;

            Invoke("OnAttackFinished", attackDuration);

            if (blockMovementDuringAttack)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isAttacking && blockMovementDuringAttack)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
    }

    void UpdateAnimator()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelocityY", rb.velocity.y);
    }

    public void OnAttackFinished()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}