using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player object is assigned
        if (player != null)
        {
            // Move the camera to follow the player
            transform.position = new Vector3(player.transform.position.x + 7, player.transform.position.y + 2.3f, transform.position.z);
        }
    }
}
