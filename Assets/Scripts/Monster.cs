using System.Collections;
using UnityEngine;

// Simple monster: chases the hero when close, flashes red when hit, dies at 0 health.
// When hit by the sword it is pushed back (knockback) and stunned for a moment,
// so it can't hurt the hero right away and the hero has time for a second hit.
// Put this script on each monster.
[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
    public int health = 2;              // number of sword hits to kill it
    public float speed = 1.5f;          // moving speed
    public float chaseDistance = 4f;    // starts chasing when the hero is this close
    public int scoreValue = 50;         // points given when killed

    [Header("When hit")]
    public float stunTime = 0.8f;       // how long the monster stops after a hit
    public float knockbackSpeed = 8f;   // how fast it is pushed back
    public float knockbackTime = 0.12f; // how long it is pushed back

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float stunnedUntil = 0f;
    private float knockbackUntil = 0f;
    private Vector2 knockDirection;

    // PlayerHealth checks this: a stunned monster doesn't hurt the hero
    public bool IsStunned { get { return Time.time < stunnedUntil; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        sr = GetComponent<SpriteRenderer>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        // Cancel pushes from collisions, otherwise the monster slides away forever
        rb.velocity = Vector2.zero;

        if (player == null) return;

        // Stunned: only slide back a little, don't chase
        if (IsStunned)
        {
            if (Time.time < knockbackUntil)
                rb.MovePosition(rb.position + knockDirection * knockbackSpeed * Time.fixedDeltaTime);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance < chaseDistance)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            GameState.score += scoreValue;
            Destroy(gameObject);
            return;
        }

        // Knockback away from the hero + stun
        if (player != null)
            knockDirection = (rb.position - (Vector2)player.position).normalized;
        knockbackUntil = Time.time + knockbackTime;
        stunnedUntil = Time.time + stunTime;
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (sr != null) sr.color = original;
    }
}
