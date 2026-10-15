using System.Collections;
using UnityEngine;

// Simple monster: walks around the map, chases the hero when close, flashes red when hit, dies at 0 health.
// When hit by the sword it is pushed back (knockback) and stunned for a moment,
// so it can't hurt the hero right away and the hero has time for a second hit.
// It also wobbles like jelly and looks at the hero.
// Put this script on each monster.
[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
    public int health = 2;              // number of sword hits to kill it
    public float speed = 1.5f;          // moving speed
    public float acceleration = 10f;    // how fast it reaches its speed (smooth start/stop)
    public float chaseDistance = 4f;    // starts chasing when the hero is this close
    public int scoreValue = 50;         // points given when killed

    [Header("When hit")]
    public float stunTime = 0.8f;       // how long the monster stops after a hit
    public float knockbackSpeed = 8f;   // how fast it is pushed back
    public float knockbackTime = 0.12f; // how long it is pushed back

    [Header("Walk around (when the hero is far)")]
    public float wanderSpeed = 0.9f;        // slower than when chasing
    public float wanderChangeTime = 2f;     // about how long it walks before changing direction

    [Header("Jelly wobble")]
    public float wobbleAmount = 0.06f;  // how much it squashes (0.06 = 6%)
    public float wobbleSpeed = 6f;      // how fast it wobbles (faster when chasing)

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector3 startScale;
    private float wobbleTime;
    private bool chasing = false;
    private float stunnedUntil = 0f;
    private float knockbackUntil = 0f;
    private Vector2 knockDirection;
    private Vector2 wanderDirection;
    private float nextWanderChange = 0f;

    // PlayerHealth checks this: a stunned monster doesn't hurt the hero
    public bool IsStunned { get { return Time.time < stunnedUntil; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;   // smooth movement between physics steps
        sr = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;

        // Difficulty chosen in the main menu (Medium = the values of the Inspector)
        if (GameState.difficulty == GameState.Easy)
        {
            speed *= 0.7f;
            wanderSpeed *= 0.7f;
            chaseDistance *= 0.75f;
            health = 1;                 // one sword hit is enough
        }
        else if (GameState.difficulty == GameState.Hard)
        {
            speed *= 1.4f;
            wanderSpeed *= 1.3f;
            chaseDistance *= 1.4f;
            health += 1;                // one more sword hit
        }
        wobbleTime = Random.value * 10f;   // each monster wobbles a bit differently

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // Squash and stretch: wider when flatter, taller when thinner
        wobbleTime += Time.deltaTime * (chasing ? wobbleSpeed * 2f : wobbleSpeed);
        float w = Mathf.Sin(wobbleTime) * wobbleAmount;
        transform.localScale = new Vector3(startScale.x * (1f + w), startScale.y * (1f - w), startScale.z);

        // Look at the hero when chasing, otherwise look where it walks
        if (sr == null) return;
        if (chasing && player != null) sr.flipX = player.position.x < transform.position.x;
        else if (Mathf.Abs(rb.velocity.x) > 0.05f) sr.flipX = rb.velocity.x < 0;
    }

    void FixedUpdate()
    {
        chasing = false;

        // Stunned: only slide back a little, don't chase
        if (IsStunned)
        {
            rb.velocity = Time.time < knockbackUntil ? knockDirection * knockbackSpeed : Vector2.zero;
            return;
        }

        Vector2 targetVelocity;
        if (player != null && Vector2.Distance(rb.position, player.position) < chaseDistance)
        {
            // Hero close: chase him
            chasing = true;
            targetVelocity = ((Vector2)player.position - rb.position).normalized * speed;
        }
        else
        {
            // Hero far: walk around, changing direction from time to time
            if (Time.time > nextWanderChange) PickWanderDirection(Random.insideUnitCircle);
            targetVelocity = wanderDirection * wanderSpeed;
        }

        // Change speed smoothly. This also cancels pushes from collisions little by little.
        rb.velocity = Vector2.MoveTowards(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
    }

    // A new random direction (sometimes it just stops for a moment)
    void PickWanderDirection(Vector2 direction)
    {
        wanderDirection = Random.value < 0.2f ? Vector2.zero : direction.normalized;
        nextWanderChange = Time.time + Random.Range(0.5f, 1.5f) * wanderChangeTime;
    }

    // Hit a tree, a wall or another monster: walk away from it
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (chasing || collision.collider.CompareTag("Player")) return;
        Vector2 away = rb.position - collision.GetContact(0).point;
        PickWanderDirection(away.normalized + Random.insideUnitCircle * 0.7f);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            GameState.score += scoreValue;
            SoundFX.PlayMonsterKilled();
            CameraFollow.Shake(0.12f, 0.15f);
            Destroy(gameObject);
            return;
        }

        CameraFollow.Shake(0.06f, 0.1f);

        // Knockback away from the hero + stun
        if (player != null)
            knockDirection = (rb.position - (Vector2)player.position).normalized;
        knockbackUntil = Time.time + knockbackTime;
        stunnedUntil = Time.time + stunTime;
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = Color.white;
    }
}
