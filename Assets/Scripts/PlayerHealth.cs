using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Hearts of the hero. Touching a monster removes 1 heart.
// Put this script on the Player (with PlayerController).
public class PlayerHealth : MonoBehaviour
{
    public float invincibleTime = 1f;   // after a hit, the hero can't be hit again for 1 second

    private float invincibleUntil = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Called every frame while touching something
    void OnCollisionStay2D(Collision2D collision)
    {
        Monster monster = collision.collider.GetComponent<Monster>();
        if (monster != null && !monster.IsStunned)   // a stunned monster doesn't hurt
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;                       // already dead: ignore more hits
        if (Time.time < invincibleUntil) return;
        invincibleUntil = Time.time + invincibleTime;

        GameState.hearts = Mathf.Max(0, GameState.hearts - amount);   // never below 0
        StartCoroutine(Blink());

        if (GameState.hearts == 0)
        {
            isDead = true;
            SceneManager.LoadScene("GameOver");   // GameOver must be in the Scene List
        }
    }

    private bool isDead = false;

    // The hero blinks while invincible
    IEnumerator Blink()
    {
        if (sr == null) yield break;
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
