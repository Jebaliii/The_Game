using UnityEngine;

// Makes one water "wave" line slide gently left and right, forever.
// Put it on each small wave sprite. No collider needed.
public class WaterWaves : MonoBehaviour
{
    public float distance = 0.25f;  // how far it slides to each side (4 pixels)
    public float speed = 1f;        // how fast it slides

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Mathf.Sin goes smoothly between -1 and 1, so the line goes left, right, left...
        // Adding startPos.x makes each wave move a bit differently from the others.
        float x = startPos.x + Mathf.Sin(Time.time * speed + startPos.x) * distance;

        // Snap to whole pixels (16 per unit) so the line stays sharp in pixel art
        x = Mathf.Round(x * 16f) / 16f;
        transform.localPosition = new Vector3(x, startPos.y, startPos.z);
    }
}
