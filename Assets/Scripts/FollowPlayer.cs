using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float xOffset = 7f;
    public float smoothSpeed = 5f;

    public Transform leftBarrier;
    public Transform rightBarrier;

    void Update()
    {
        if (player != null)
        {
            float targetX = player.transform.position.x + xOffset;

            // Clamp posisi X kamera agar tidak melewati batas kiri/kanan
            float minX = leftBarrier.position.x-8.2f;
            float maxX = rightBarrier.position.x-8.2f;

            float clampedX = Mathf.Clamp(targetX, minX, maxX);

            Vector3 targetPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
