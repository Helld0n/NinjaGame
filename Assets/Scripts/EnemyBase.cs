using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Настройки движения")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 2.5f;
    public float patrolDistance = 2f;

    [Header("Границы патрулирования")]
    public bool usePatrolBounds = true;
    public float leftBound = 0f;
    public float rightBound = 0f;

    [Header("Обнаружение игрока")]
    public float detectionRange = 6f;
    public float losePlayerRange = 8f;

    [Header("Настройки атаки")]
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    public float attackDuration = 0.8f;

    [Header("Настройки щита")]
    public bool canUseShield = false;
    public float shieldDuration = 2f;
    public float shieldCooldown = 6f;
    public float shieldChance = 0.5f;
    public float shieldTriggerRange = 3f;

    [Header("Настройки получения урона")]
    public float hitDuration = 0.5f;

    [Header("Дроп хилки")]
    public bool canDropHealth = true;
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.3f;

    [Header("Ссылки")]
    public Transform groundCheck;
    public Transform edgeCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    public Transform player;

    [Header("HP")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Имена анимаций")]
    public string prefix = "SK_";
    public string idleName = "Idle";
    public string moveName = "Walk";
    public string attackName = "Attack";
    public string hitName = "Hit";
    public string deathName = "Death";
    public string shieldName = "Shield";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector3 startPosition;
    private int direction = 1;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isShielding = false;
    private bool isHit = false;
    private bool isChasing = false;
    private bool isStunned = false;
    private float lastAttackTime;
    private float attackEndTime;
    private float lastShieldTime;
    private float shieldEndTime;
    private float hitEndTime;
    private float stunEndTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        currentHealth = maxHealth;
        lastShieldTime = -shieldCooldown;

        if (usePatrolBounds && leftBound == 0f && rightBound == 0f)
        {
            leftBound = startPosition.x - patrolDistance;
            rightBound = startPosition.x + patrolDistance;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        // СТАН
        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;
                animator.Play(prefix + idleName, 0, 0f);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
        }

        // ТАЙМЕРЫ
        if (isAttacking && Time.time >= attackEndTime) EndAttack();
        if (isShielding && Time.time >= shieldEndTime) EndShield();
        if (isHit && Time.time >= hitEndTime) EndHit();

        if (isHit)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float distanceToPlayer = player != null
            ? Vector2.Distance(transform.position, player.position)
            : 999f;

        // ОБНАРУЖЕНИЕ
        if (!isChasing && distanceToPlayer <= detectionRange)
            isChasing = true;
        else if (isChasing && distanceToPlayer > losePlayerRange)
            isChasing = false;

        // ЩИТ
        if (isShielding)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (canUseShield && !isShielding && !isAttacking && isChasing &&
            distanceToPlayer <= shieldTriggerRange &&
            Time.time - lastShieldTime > shieldCooldown)
        {
            if (UnityEngine.Random.value < shieldChance)
            {
                StartShield();
                return;
            }
            else
            {
                lastShieldTime = Time.time;
            }
        }

        // АТАКА
        if (isChasing && !isAttacking &&
            distanceToPlayer <= attackRange &&
            Time.time - lastAttackTime > attackCooldown)
        {
            FacePlayer();
            Attack();
            return;
        }

        // ДВИЖЕНИЕ
        if (isChasing) ChasePlayer(distanceToPlayer);
        else Patrol();

        UpdateAnimator();
    }

    void Patrol()
    {
        bool atEdgeUnderFeet = groundCheck != null &&
            !Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Vector3 edgePos = edgeCheck != null ? edgeCheck.position :
                          transform.position + new Vector3(direction * 0.4f, -0.5f, 0);
        bool atEdgeAhead = !Physics2D.OverlapCircle(edgePos, groundCheckRadius, groundLayer);

        bool atLeftBound = transform.position.x <= leftBound;
        bool atRightBound = transform.position.x >= rightBound;

        if (direction > 0 && (atRightBound || atEdgeAhead || atEdgeUnderFeet)) direction = -1;
        else if (direction < 0 && (atLeftBound || atEdgeAhead || atEdgeUnderFeet)) direction = 1;

        float targetSpeed = direction * patrolSpeed;
        rb.velocity = new Vector2(
            Mathf.Lerp(rb.velocity.x, targetSpeed, Time.deltaTime * 5f),
            rb.velocity.y
        );

        spriteRenderer.flipX = direction < 0;
    }

    void ChasePlayer(float distanceToPlayer)
    {
        if (player == null) return;

        if (distanceToPlayer <= attackRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            FacePlayer();
            return;
        }

        int directionToPlayer = player.position.x > transform.position.x ? 1 : -1;

        Vector3 edgePos = transform.position + new Vector3(directionToPlayer * 0.4f, -0.5f, 0);
        bool atEdgeAhead = !Physics2D.OverlapCircle(edgePos, groundCheckRadius, groundLayer);

        if (atEdgeAhead)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            FacePlayer();
            return;
        }

        float targetSpeed = directionToPlayer * chaseSpeed;
        rb.velocity = new Vector2(
            Mathf.Lerp(rb.velocity.x, targetSpeed, Time.deltaTime * 5f),
            rb.velocity.y
        );

        spriteRenderer.flipX = directionToPlayer < 0;
    }

    void FacePlayer()
    {
        if (player == null) return;
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        attackEndTime = Time.time + attackDuration;
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (canUseShield)
            animator.SetBool("IsShielding", false);

        animator.Play(prefix + attackName, 0, 0f);
        Invoke("DealDamageToPlayer", attackDuration / 2f);
    }

    void DealDamageToPlayer()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.3f)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        if (Mathf.Abs(rb.velocity.x) > 0.1f) animator.Play(prefix + moveName, 0, 0f);
        else animator.Play(prefix + idleName, 0, 0f);
    }

    void StartShield()
    {
        isShielding = true;
        lastShieldTime = Time.time;
        shieldEndTime = Time.time + shieldDuration;
        rb.velocity = new Vector2(0, rb.velocity.y);
        FacePlayer();
        animator.SetBool("IsShielding", true);
        animator.Play(prefix + shieldName, 0, 0f);
    }

    void EndShield()
    {
        isShielding = false;
        animator.SetBool("IsShielding", false);
        animator.Play(prefix + idleName, 0, 0f);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        if (isShielding) return;
        if (isHit) return;
        if (isStunned) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartHit();
    }

    void StartHit()
    {
        isAttacking = false;
        isShielding = false;
        if (canUseShield) animator.SetBool("IsShielding", false);

        isHit = true;
        hitEndTime = Time.time + hitDuration;
        rb.velocity = new Vector2(0, rb.velocity.y);

        animator.Play(prefix + hitName, 0, 0f);
    }

    void EndHit()
    {
        isHit = false;

        if (Mathf.Abs(rb.velocity.x) > 0.1f)
            animator.Play(prefix + moveName, 0, 0f);
        else
            animator.Play(prefix + idleName, 0, 0f);
    }

    public void Stun(float duration)
    {
        if (isDead) return;

        isAttacking = false;
        isShielding = false;
        isHit = false;
        if (canUseShield) animator.SetBool("IsShielding", false);

        isStunned = true;
        stunEndTime = Time.time + duration;
        rb.velocity = new Vector2(0, rb.velocity.y);

        animator.Play(prefix + idleName, 0, 0f);
    }

    public bool IsStunned() => isStunned;
    public bool IsDead() => isDead;

    void Die()
    {
        isDead = true;
        isShielding = false;
        isHit = false;
        isAttacking = false;
        isStunned = false;
        rb.velocity = Vector2.zero;

        if (canUseShield) animator.SetBool("IsShielding", false);

        animator.Play(prefix + deathName, 0, 0f);

        if (LevelManager.instance != null)
            LevelManager.instance.AddKill();

        // Дроп хилки
        TryDropHealth();

        Destroy(gameObject, 2f);
    }

    void TryDropHealth()
    {
        if (!canDropHealth) return;
        if (healthPickupPrefab == null) return;

        if (UnityEngine.Random.value < dropChance)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }
    }

    void UpdateAnimator()
    {
        if (isDead || isAttacking || isShielding || isHit || isStunned) return;
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (edgeCheck != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(edgeCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (usePatrolBounds)
        {
            Gizmos.color = Color.green;
            Vector3 leftPoint = new Vector3(leftBound, transform.position.y, 0);
            Vector3 rightPoint = new Vector3(rightBound, transform.position.y, 0);
            Gizmos.DrawLine(leftPoint, rightPoint);
        }
    }
}