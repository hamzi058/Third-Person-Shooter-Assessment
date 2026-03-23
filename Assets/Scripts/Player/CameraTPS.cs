using UnityEngine;

public class CameraTPS : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0.6f, 2.2f, -4f);

    public float followSpeed = 12f;
    public float rotateSpeed = 12f;

    void LateUpdate()
    {
        if (!target) return;

        // ⭐ rotate offset according to player yaw
        Quaternion yawRot =
            Quaternion.Euler(0f, target.eulerAngles.y, 0f);

        Vector3 desiredPos =
            target.position + yawRot * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            followSpeed * Time.deltaTime
        );

        // ⭐ look slightly above player (natural framing)
        Vector3 lookPoint =
            target.position + Vector3.up * 1.5f;

        Quaternion lookRot =
            Quaternion.LookRotation(lookPoint - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRot,
            rotateSpeed * Time.deltaTime
        );
    }
}