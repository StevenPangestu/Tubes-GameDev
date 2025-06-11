using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameController : MonoBehaviour
{
    private TextMeshProUGUI StageText;
    private TextMeshProUGUI GrenadeText;
    public Image GrenadeImage;
    public Image[] healthBar;
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;
    public int maxHealth = 5;
    private static int currentHealth;
    private static int grenadeOwned;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup restartCanvasGroup;
    [SerializeField] private CanvasGroup exitCanvasGroup;
    [SerializeField] private CanvasGroup darkOverlayCanvasGroup; // moved up here
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private GameObject darkOverlay;

    void Start()
    {
        restartButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        darkOverlay.SetActive(false);

        currentHealth = PlayerController.health;
        StageText = GameObject.Find("TextStage").GetComponent<TextMeshProUGUI>();
        GrenadeText = GameObject.Find("GrenadeText").GetComponent<TextMeshProUGUI>();
        grenadeOwned = PlayerController.grenadeOwned;

        UpdateStageText();
        UpdateHealth(currentHealth);
        UpdateGrenade(grenadeOwned);
    }

    void Update() { }

    public void showFailed()
    {
        darkOverlay.SetActive(true);

        // Enable buttons before fading in their canvas groups
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        StartCoroutine(FadeIn(darkOverlayCanvasGroup));
        StartCoroutine(FadeIn(restartCanvasGroup));
        StartCoroutine(FadeIn(exitCanvasGroup));

        restartButton.onClick.RemoveAllListeners(); // prevent duplicate listeners
        restartButton.onClick.AddListener(() =>
        {
            restartCanvasGroup.interactable = false;
            restartCanvasGroup.blocksRaycasts = false;
            darkOverlayCanvasGroup.interactable = false;
            darkOverlayCanvasGroup.blocksRaycasts = false;
            // Reset health and grenade count
            PlayerController.health = 5;
            PlayerController.grenadeOwned = 0;
            UpdateHealth(PlayerController.health);
            UpdateGrenade(PlayerController.grenadeOwned);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });

        exitButton.onClick.RemoveAllListeners(); // prevent duplicate listeners
        exitButton.onClick.AddListener(() =>
        {
            exitCanvasGroup.interactable = false;
            exitCanvasGroup.blocksRaycasts = false;
            darkOverlayCanvasGroup.interactable = false;
            darkOverlayCanvasGroup.blocksRaycasts = false;
            Application.Quit();
        });
    }

    public void UpdateStageText()
    {
        StageText.text = "Stage: " + (ChangeStage.StageIndex + 1);
    }

    public void UpdateHealth(int health)
    {
        for (int i = 0; i < healthBar.Length; i++)
        {
            if (healthBar[i] == null) continue;
            healthBar[i].sprite = i < health ? fullHealthSprite : emptyHealthSprite;
        }
    }

    public void UpdateGrenade(int grenadeCount)
    {
        if (grenadeCount == 0)
        {
            GrenadeText.text = "";
            GrenadeImage.enabled = false;
            grenadeOwned = 0;
        }
        else
        {
            grenadeOwned = grenadeCount;
            GrenadeImage.enabled = true;
            GrenadeText.text = grenadeOwned.ToString();
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
