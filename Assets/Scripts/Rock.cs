using UnityEngine;

// The rock of puzzle 1. It can be pushed right, but never left of where it started,
// otherwise it gets stuck in the corner and the puzzle can't be finished.
// Put this script on the Rock (it also needs a Rigidbody 2D with "Freeze Position Y").
public class Rock : MonoBehaviour
{
    private Rigidbody2D rb;
    private float startX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startX = rb.position.x;
    }

    void FixedUpdate()
    {
        // Pushed left of the start: put it back and stop it
        if (rb.position.x < startX)
        {
            rb.position = new Vector2(startX, rb.position.y);
            rb.velocity = Vector2.zero;
        }
    }
}
