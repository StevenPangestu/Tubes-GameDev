using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public void UpdateHealth(int health)
    {
        if (healthBar == null || healthBar.Length == 0)
        {
            Debug.LogError("healthBar not initialized!");
            return;
        }

       
        for (int i = 0; i < healthBar.Length; i++)
        {
            if (healthBar[i] == null)
            {
                Debug.LogError($"healthBar[{i}] is null!");
                continue;
            }

            if (i < health)
                healthBar[i].sprite = fullHealthSprite;
            else
                healthBar[i].sprite = emptyHealthSprite;
        }
    }

    public void UpdateGrenade(int grenadeCount)
    {
        if (grenadeCount == 0)
        {
            GrenadeText.text = "";
            GrenadeImage.enabled = false;
            grenadeOwned = 0;
            return;
        }
        grenadeOwned = grenadeCount;
        GrenadeImage.enabled = true;
        //GrenadeImage.sprite = Resources.Load<Sprite>("GrenadeIcon"); // Assuming you have a grenade icon in Resources folder
    
        GrenadeText.text = grenadeOwned.ToString();
    }
    void Start()
    {
        //get health from playercontroller
        currentHealth = PlayerController.health;
        StageText = GameObject.Find("TextStage").GetComponent<TextMeshProUGUI>();
        GrenadeText = GameObject.Find("GrenadeText").GetComponent<TextMeshProUGUI>();
        grenadeOwned = PlayerController.grenadeOwned;
        Debug.Log(grenadeOwned);
        UpdateStageText();
        UpdateHealth(currentHealth);
        UpdateGrenade(grenadeOwned);
    }

    void Update()
    {

    }
    //get variable from ChangeStage.cs
    public void UpdateStageText()
    {
        StageText.text = "Stage: " + (ChangeStage.StageIndex + 1);
    }
}
