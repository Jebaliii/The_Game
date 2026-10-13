using UnityEngine;

// Hero movement (top-down, 8 directions) + sword attack.
// Put this script on the Player object.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;          // walking speed

    [Header("Sword")]
    public float attackRange = 0.6f;      // size of the sword hit zone
    public float attackDistance = 0.6f;   // how far in front of the hero the hit zone is
    public int attackDamage = 1;          // damage per hit
    public float attackCooldown = 0.4f;   // time between two attacks
    public LayerMask monsterLayer;        // choose the "Monster" layer in the Inspector

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 mobileInput;          // filled by mobile buttons (later)
    private Vector2 lastDirection = Vector2.down; // direction the hero is facing
    private float nextAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;      // top-down game: no gravity
        rb.freezeRotation = true;  // the hero must not spin when hitting walls
    }

    void Update()
    {
        // Keyboard: arrows or WASD / ZQSD
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        input = new Vector2(x, y);

        // If no keyboard input, use mobile buttons
        if (input == Vector2.zero) input = mobileInput;

        // Same speed in diagonal
        input = Vector2.ClampMagnitude(input, 1f);

        // Remember where the hero looks (for the sword)
        if (input != Vector2.zero) lastDirection = input.normalized;

        // Attack with Space
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    void FixedUpdate()
    {
        // Cancel pushes from collisions, otherwise the hero slides away
        rb.velocity = Vector2.zero;
        rb.MovePosition(rb.position + input * moveSpeed * Time.fixedDeltaTime);
    }

    // Public so a mobile "Attack" button can call it
    public void Attack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        // Hit zone in front of the hero
        Vector2 hitPoint = (Vector2)transform.position + lastDirection * attackDistance;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint, attackRange, monsterLayer);

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (monster != null) monster.TakeDamage(attackDamage);
        }
    }

    // Called by mobile direction buttons (we will add them later)
    public void SetMobileInput(Vector2 direction)
    {
        mobileInput = direction;
    }

    // Draws the sword zone in red in the Scene view (to help you adjust it)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 dir = Application.isPlaying ? lastDirection : Vector2.down;
        Gizmos.DrawWireSphere((Vector2)transform.position + dir * attackDistance, attackRange);
    }
}
