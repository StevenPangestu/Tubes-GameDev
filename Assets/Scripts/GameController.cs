using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI StageText;
    [SerializeField] private TextMeshProUGUI StatusText;
    [SerializeField] private TextMeshProUGUI GrenadeText;

    public Image GrenadeImage;
    public Image[] healthBar;
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;
    public int maxHealth = 5;

    private static int currentHealth;
    private static int grenadeOwned;

    private BossScript boss;
    public GameObject portal;

    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup restartCanvasGroup;
    [SerializeField] private CanvasGroup exitCanvasGroup;
    [SerializeField] private CanvasGroup darkOverlayCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private GameObject darkOverlay;

    private void Start()
    {
        EnemyController.enemiesKilled = 0;
        ResetUIState();
        AudioManager audioManager = FindObjectOfType<AudioManager>();

        if (ChangeStage.StageIndex == 0)
        {
            audioManager.PlayBGM(audioManager.background1);
        }
        else
        {
            audioManager.PlayBGM(audioManager.background2);
        }
        currentHealth = PlayerController.health;
        grenadeOwned = PlayerController.grenadeOwned;

        UpdateHealth(currentHealth);
        UpdateGrenade(grenadeOwned);

        GameObject bossObj = GameObject.FindWithTag("Boss");
        if (bossObj != null)
        {
            boss = bossObj.GetComponent<BossScript>();
            if (boss == null)
                Debug.LogError("BossScript component not found on Boss object.");
        }
        else
        {
            Debug.LogWarning("Boss object not found in scene.");
        }
    }

    private void Update()
    {
        UpdateStageText();

        if (boss != null && boss.IsDead())
        {
            portal.SetActive(true);
        }

        if (BossScript.bossDefeated == 2)
        {
            showWin();
        }
    }

    public void showFailed()
    {
        if (darkOverlay) darkOverlay.SetActive(true);
        if (StageText) StageText.text = "";
        if (StatusText)
        {
            StatusText.gameObject.SetActive(true);
            StatusText.text = "You Failed!";
        }

        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        StartCoroutine(FadeIn(darkOverlayCanvasGroup));
        StartCoroutine(FadeIn(restartCanvasGroup));
        StartCoroutine(FadeIn(exitCanvasGroup));

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() =>
        {
            ResetAndRestart();
        });

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {

            SceneManager.LoadScene(2, LoadSceneMode.Single);

        });
        //set the player set active to false


    }

    public void showWin()
    {
        if (darkOverlay) darkOverlay.SetActive(true);
        if (StageText) StageText.text = "";
        if (StatusText)
        {
            StatusText.gameObject.SetActive(true);
            StatusText.text = "You WIN!";
        }

        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        StartCoroutine(FadeIn(darkOverlayCanvasGroup));
        StartCoroutine(FadeIn(restartCanvasGroup));
        StartCoroutine(FadeIn(exitCanvasGroup));

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(() =>
        {
            ResetAndRestart();
        });

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() =>
        {

            SceneManager.LoadScene(2, LoadSceneMode.Single);

        });

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.gameObject.SetActive(false);
        }
    }

    private void ResetUIState()
    {
        if (portal) portal.SetActive(false);
        if (restartButton) restartButton.gameObject.SetActive(false);
        if (exitButton) exitButton.gameObject.SetActive(false);
        if (darkOverlay) darkOverlay.SetActive(false);
        if (StatusText) StatusText.gameObject.SetActive(false);

        if (restartCanvasGroup)
        {
            restartCanvasGroup.alpha = 0;
            restartCanvasGroup.interactable = false;
            restartCanvasGroup.blocksRaycasts = false;
        }

        if (exitCanvasGroup)
        {
            exitCanvasGroup.alpha = 0;
            exitCanvasGroup.interactable = false;
            exitCanvasGroup.blocksRaycasts = false;
        }

        if (darkOverlayCanvasGroup)
        {
            darkOverlayCanvasGroup.alpha = 0;
            darkOverlayCanvasGroup.interactable = false;
            darkOverlayCanvasGroup.blocksRaycasts = false;
        }
    }

    private void ResetAndRestart()
    {
        BossScript.bossDefeated = 0;

        PlayerController.health = maxHealth;
        PlayerController.grenadeOwned = 0;
        UpdateHealth(PlayerController.health);
        UpdateGrenade(PlayerController.grenadeOwned);
        ChangeStage.StageIndex = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void UpdateStageText()
    {
        if (StageText != null)
        {
            Debug.Log("Now stage index: " + ChangeStage.StageIndex);
            StageText.text = "Stage: " + (ChangeStage.StageIndex + 1);
        }
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
        if (GrenadeText == null || GrenadeImage == null)
        {
            Debug.LogError("GrenadeText or GrenadeImage is not assigned.");
            return;
        }

        if (grenadeCount <= 0)
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
