using UnityEngine;

using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    private bool keyPressed = false;
    
    private void Update()
    {
        if (Input.anyKeyDown && !keyPressed)
        {
            keyPressed = true; 
            SceneManager.LoadScene(0); 
        }
    }
}
