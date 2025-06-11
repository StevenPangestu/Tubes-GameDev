using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
public class StartScene : MonoBehaviour
{
    public TMP_Text text;
    public Button playButton;
    public Button exitButton;

    void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            text.text = "Loading...";
            SceneManager.LoadScene(0);
        });

        exitButton.onClick.AddListener(() =>
        {
            text.text = "Exiting...";
            EditorApplication.isPlaying = false;
            Application.Quit();
        });
    }
}
