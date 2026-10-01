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
    public bool allowMovementDuringAttack = true;
    public float attackMoveSpeedMultiplier = 0.5f;
    public float attackDuration = 0.7f;
    public int attackDamage = 1;

    [Header("Зона атаки")]
    public Transform attackPoint;
    public float attackRadius = 0.4f;
    public LayerMask enemyLayer;

    [Header("Слайд")]
    public float slideSpeed = 12f;
    public float slideDuration = 0.5f;
    public float slideFriction = 18f;
    public float stunRadius = 1.5f;
    public float enemyStunExtra = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private float moveInput;
    private bool isRunning;
    private bool isAttacking = false;
    private bool isSliding = false;
    private int attackCounter = 0;
    private float slideEndTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Проверка окончания слайда
        if (isSliding && Time.time >= slideEndTime)
        {
            EndSlide();
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking && !isSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // СЛАЙД (ПКМ)
        if (Input.GetMouseButtonDown(1) && !isSliding && !isAttacking && isGrounded)
        {
            StartSlide();
        }

        // АТАКА (ЛКМ) — может прервать слайд
        if (Input.GetMouseButtonDown(0) && !isAttacking && isGrounded)
        {
            if (isSliding)
            {
                isSliding = false;   // Прерываем слайд
            }

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

            // Урон в середине анимации атаки
            Invoke("DealDamage", attackDuration / 2f);
            Invoke("OnAttackFinished", attackDuration);

            // Тормозим персонажа после слайда
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Поворот персонажа + AttackPoint
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            SetAttackPointDirection(1);
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            SetAttackPointDirection(-1);
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (isSliding)
        {
            float newSpeed = Mathf.MoveTowards(rb.velocity.x, 0, slideFriction * Time.fixedDeltaTime);
            rb.velocity = new Vector2(newSpeed, rb.velocity.y);

            StunNearbyEnemies();
            return;
        }

        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        if (isAttacking && allowMovementDuringAttack)
        {
            currentSpeed *= attackMoveSpeedMultiplier;
        }
        else if (isAttacking && !allowMovementDuringAttack)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
    }

    void StartSlide()
    {
        isSliding = true;
        slideEndTime = Time.time + slideDuration;

        int dir = spriteRenderer.flipX ? -1 : 1;
        rb.velocity = new Vector2(dir * slideSpeed, rb.velocity.y);

        animator.Play("Slide", 0, 0f);
    }

    void EndSlide()
    {
        isSliding = false;
        animator.Play("Idle", 0, 0f);
    }

    void StunNearbyEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            stunRadius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            if (enemy == null) continue;

            EnemySkeleton skeleton = enemy.GetComponent<EnemySkeleton>();
            if (skeleton != null && !skeleton.IsDead() && !skeleton.IsStunned())
            {
                skeleton.Stun(slideDuration + enemyStunExtra);
            }
        }
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

    public bool IsSliding()
    {
        return isSliding;
    }

    void SetAttackPointDirection(int dir)
    {
        if (attackPoint == null) return;

        Vector3 pos = attackPoint.localPosition;
        pos.x = Mathf.Abs(pos.x) * dir;
        attackPoint.localPosition = pos;
    }

    void UpdateAnimator()
    {
        if (isSliding) return;

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelocityY", rb.velocity.y);
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}