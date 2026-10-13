using UnityEngine;

// PUZZLE 1: push the rock onto this plate -> a bridge appears over the river.
// Needs a Collider 2D with "Is Trigger" ticked.
// The rock must have the Tag "Rock".
public class PressurePlate : MonoBehaviour
{
    public GameObject bridge;      // the bridge sprite (hidden at the start)
    public GameObject waterBlock;  // the water piece that blocks the way (removed when solved)
    public int scoreReward = 200;

    private bool activated = false;

    void Start()
    {
        if (bridge != null) bridge.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Rock"))
        {
            activated = true;

            // Lock the rock on the plate
            other.transform.position = transform.position;
            if (other.attachedRigidbody != null)
                other.attachedRigidbody.bodyType = RigidbodyType2D.Static;

            if (bridge != null) bridge.SetActive(true);
            if (waterBlock != null) waterBlock.SetActive(false);

            GameState.score += scoreReward;
        }
    }
}
