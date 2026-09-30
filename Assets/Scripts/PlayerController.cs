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
    public int attackDamage = 1;

    [Header("Зона атаки")]
    public Transform attackPoint;
    public float attackRadius = 0.4f;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;
    private bool isRunning;
    private bool isAttacking = false;
    private int attackCounter = 0;

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

        // АТАКА
        if (Input.GetMouseButtonDown(0) && !isAttacking && isGrounded)
        {
            if (attackCounter == 0)
            {
                animator.SetTrigger("Attack");
                attackCounter = 1;
            }
            else
            {
                animator.SetTrigger("Attack2");
                attackCounter = 0;
            }

            isAttacking = true;

            DealDamage();

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

    void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy == null) continue;

            EnemySkeleton skeleton = enemy.GetComponent<EnemySkeleton>();
            if (skeleton != null && !skeleton.IsDead())
            {
                skeleton.TakeDamage(attackDamage);
            }
        }
    }

    public void OnAttackFinished()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}