using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public float xOffset = 7f;
    public float smoothSpeed = 5f;

    void Update()
    {
        if (player != null)
        {
            float targetX = player.transform.position.x + xOffset;
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
