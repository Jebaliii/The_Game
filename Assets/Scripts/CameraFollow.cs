using UnityEngine;

// The camera follows the hero. Put this script on the Main Camera.
public class CameraFollow : MonoBehaviour
{
    public Transform target;        // drag the Player here
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 wanted = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, wanted, smoothSpeed * Time.deltaTime);
    }
}
