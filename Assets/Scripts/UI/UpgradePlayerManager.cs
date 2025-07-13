using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePlayerManager : MonoBehaviour
{
    [Header("Text Displays")]
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI speedLevelText;
    public TextMeshProUGUI hpLevelText;
    public TextMeshProUGUI mpLevelText;
    public TextMeshProUGUI coinsText;

    [Header("Upgrade Buttons")]
    public Button damageButton;
    public Button speedButton;
    public Button hpButton;
    public Button mpButton;
    public Button exitButton;

    [Header("Panels")]
    public GameObject ChooseCharacterPanel;
    public GameObject UpgradePanel;
    void Start()
    {
        damageButton.onClick.AddListener(() => Upgrade("Damage"));
        speedButton.onClick.AddListener(() => Upgrade("MoveSpeed"));
        hpButton.onClick.AddListener(() => Upgrade("MaxHP"));
        mpButton.onClick.AddListener(() => Upgrade("MaxMP"));
        exitButton.onClick.AddListener(() => CloseUpgrade());

        UpdateUI();
    }
    public void CloseUpgrade()
    {
        if (ChooseCharacterPanel != null)
        {
            UpgradePanel.SetActive(false);
            ChooseCharacterPanel.SetActive(true);
        }
    }
    void Upgrade(string stat)
    {
        if (SharedPlayerStats.GameStats.sharedStats.TryUpgrade(stat))
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        var stats = SharedPlayerStats.GameStats.sharedStats;

        damageButton.interactable = stats.DamageLevel < 10;
        speedButton.interactable = stats.MoveSpeedLevel < 10;
        hpButton.interactable = stats.HpLevel < 10;
        mpButton.interactable = stats.ManaLevel < 10;

        // Update text
        damageLevelText.text = stats.DamageLevel >= 10 ? "MAX" : stats.DamageLevel.ToString();
        speedLevelText.text = stats.MoveSpeedLevel >= 10 ? "MAX" : stats.MoveSpeedLevel.ToString();
        hpLevelText.text = stats.HpLevel >= 10 ? "MAX" : stats.HpLevel.ToString();
        mpLevelText.text = stats.ManaLevel >= 10 ? "MAX" : stats.ManaLevel.ToString();
        coinsText.text = stats.Coins.ToString();
    }

}
