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

        damageLevelText.text = "" + stats.DamageLevel;
        speedLevelText.text = "" +  stats.MoveSpeedLevel;
        hpLevelText.text = "" + stats.HpLevel;
        mpLevelText.text = "" + stats.ManaLevel;
        coinsText.text = "" + stats.Coins;
    }
}
