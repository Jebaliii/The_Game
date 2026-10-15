using UnityEngine;

// The camera follows the hero smoothly. Put this script on the Main Camera.
// Other scripts can shake it: CameraFollow.Shake(0.1f, 0.15f);
public class CameraFollow : MonoBehaviour
{
    public Transform target;        // drag the Player here
    public float smoothTime = 0.15f; // time to catch up with the hero (bigger = softer)

    private Vector3 followPosition;  // where the camera would be without the shake
    private Vector3 velocity;        // used by SmoothDamp

    private static float shakeAmount = 0f;
    private static float shakeUntil = 0f;

    void Start()
    {
        followPosition = transform.position;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 wanted = new Vector3(target.position.x, target.position.y, transform.position.z);
        followPosition = Vector3.SmoothDamp(followPosition, wanted, ref velocity, smoothTime);

        // Small random move while shaking
        Vector3 shake = Vector3.zero;
        if (Time.time < shakeUntil) shake = (Vector3)(Random.insideUnitCircle * shakeAmount);
        transform.position = followPosition + shake;
    }

    public static void Shake(float amount, float time)
    {
        if (!Settings.ScreenShake) return;   // turned off in the Settings
        shakeAmount = amount;
        shakeUntil = Time.time + time;
    }
}
