using UnityEngine;
using UnityEngine.UI;

public class Player1Coin : MonoBehaviour
{
    [Header("UI Text")]
    public Text totalCoinText;
    public Text sessionCoinText;

    [Header("Map")]
    public MapLevel currentLevel = MapLevel.Forest;

    private int sessionCoin = 0;

    void Start()
    {
        currentLevel = (MapLevel)PlayerPrefs.GetInt("SelectedMap", 0);
        sessionCoin = 0;
        UpdateCoinUI();
    }

    public void AddCoinOnCollect()
    {
        int amount = GetCoinAmountByLevel(currentLevel);
        sessionCoin += amount;
        UpdateCoinUI();
        Debug.Log($"+{amount} coin (Level: {currentLevel}) - Session total: {sessionCoin}");
    }

    public void AddCoin(int coin)
    {
        SharedPlayerStats.GameStats.sharedStats.Coins += coin;
    }

    public void CommitSessionToTotal()
    {
        int total = SharedPlayerStats.GameStats.sharedStats.Coins;
        total += sessionCoin;
        PlayerPrefs.SetInt("TotalCoins", total);
        PlayerPrefs.Save();

        Debug.Log($"🎉 Win! Gained {sessionCoin} coin → Total now: {total}");

        sessionCoin = 0;
        UpdateCoinUI();
    }

    public void ResetSessionCoins()
    {
        Debug.Log("💀 Game failed → Reset session coin");
        sessionCoin = 0;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        int total = SharedPlayerStats.GameStats.sharedStats.Coins;

        if (totalCoinText != null)
            totalCoinText.text = $"{total}";

        if (sessionCoinText != null)
            sessionCoinText.text = $"{sessionCoin}";
    }

    private int GetCoinAmountByLevel(MapLevel level)
    {
        switch (level)
        {
            case MapLevel.Forest: return 15;
            case MapLevel.Maya: return 20;
            case MapLevel.Dark: return 25;
            case MapLevel.Endless: return 30;
            default: return 10;
        }
    }

    public int GetTotalCoin()
    {
        return SharedPlayerStats.GameStats.sharedStats.Coins;
    }

    public int GetSessionCoin()
    {
        return sessionCoin;
    }

    public enum MapLevel
    {
        Forest,
        Maya,
        Dark,
        Endless
    }

    public static string GetSceneName(MapLevel level)
    {
        switch (level)
        {
            case MapLevel.Forest: return "ForestMap";
            case MapLevel.Maya: return "MayaScene";
            case MapLevel.Dark: return "DarkScene";
            case MapLevel.Endless: return "1-Endless";
            default: return "GameScene";
        }
    }
}
