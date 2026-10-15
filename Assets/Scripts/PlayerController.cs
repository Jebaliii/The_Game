using System.Collections;
using UnityEngine;

// Hero movement (top-down, 8 directions) + sword attack.
// Put this script on the Player object.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;          // walking speed
    public float acceleration = 35f;      // how fast the hero reaches full speed (smooth start)
    public float deceleration = 50f;      // how fast the hero stops (smooth stop)

    [Header("Sword")]
    public float attackRange = 0.6f;      // size of the sword hit zone
    public float attackDistance = 0.6f;   // how far in front of the hero the hit zone is
    public int attackDamage = 1;          // damage per hit
    public float attackCooldown = 0.4f;   // time between two attacks
    public LayerMask monsterLayer;        // choose the "Monster" layer in the Inspector

    [Header("Sword swing (the child object called \"Sword\")")]
    public float swingTime = 0.2f;        // how long one swing lasts
    public float swingAngle = 140f;       // size of the arc in degrees
    public float swingReach = 0.55f;      // distance of the sword from the hero while swinging

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform sword;
    private SpriteRenderer swordSr;
    private Vector3 swordRest;            // where the sword is when not swinging (right side)
    private bool swinging = false;

    private Vector2 input;
    private Vector2 mobileInput;          // filled by mobile buttons (later)
    private Vector2 lastDirection = Vector2.down; // direction the hero is facing
    private bool facingLeft = false;
    private float nextAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;      // top-down game: no gravity
        rb.freezeRotation = true;  // the hero must not spin when hitting walls
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;   // smooth movement between physics steps
        sr = GetComponent<SpriteRenderer>();

        sword = transform.Find("Sword");
        if (sword != null)
        {
            swordSr = sword.GetComponent<SpriteRenderer>();
            swordRest = sword.localPosition;
        }
    }

    void Update()
    {
        // Game paused (pause menu or a pop-up): the hero doesn't move or attack
        if (Time.timeScale == 0f) return;

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

        // Look left or right
        if (input.x != 0) facingLeft = input.x < 0;
        if (sr != null) sr.flipX = facingLeft;
        if (sword != null && !swinging) RestSword();

        // Attack with Space
        if (Input.GetKeyDown(KeyCode.Space)) Attack();
    }

    void FixedUpdate()
    {
        // Speed up and slow down smoothly instead of starting/stopping at once.
        // This also cancels pushes from collisions little by little.
        Vector2 targetVelocity = input * moveSpeed;
        float rate = input == Vector2.zero ? deceleration : acceleration;
        rb.velocity = Vector2.MoveTowards(rb.velocity, targetVelocity, rate * Time.fixedDeltaTime);
    }

    // Public so a mobile "Attack" button can call it
    public void Attack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        if (sword != null) StartCoroutine(Swing(lastDirection));

        // Hit zone in front of the hero
        Vector2 hitPoint = (Vector2)transform.position + lastDirection * attackDistance;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint, attackRange, monsterLayer);

        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (monster != null) monster.TakeDamage(attackDamage);
        }
    }

    // The sword turns around the hero, in an arc in front of him
    IEnumerator Swing(Vector2 direction)
    {
        swinging = true;
        swordSr.flipX = false;
        float aim = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Start on one side of the aim and finish on the other side
        float from = aim + swingAngle / 2f;
        float to = aim - swingAngle / 2f;
        if (facingLeft) { float temp = from; from = to; to = temp; }

        for (float t = 0f; t < swingTime; t += Time.deltaTime)
        {
            PlaceSword(Mathf.Lerp(from, to, t / swingTime));
            yield return null;
        }
        PlaceSword(to);
        yield return new WaitForSeconds(0.05f);   // tiny pause at the end of the swing
        swinging = false;
    }

    void PlaceSword(float angle)
    {
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        sword.localPosition = dir * swingReach;
        sword.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);   // the sword picture points up
    }

    // Sword back at the hero's side (left or right)
    void RestSword()
    {
        sword.localPosition = new Vector3(facingLeft ? -swordRest.x : swordRest.x, swordRest.y, swordRest.z);
        sword.localRotation = Quaternion.identity;
        swordSr.flipX = facingLeft;
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
