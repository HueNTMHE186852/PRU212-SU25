using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player1Coin : MonoBehaviour
{
    public Text totalCoinText;
    public Text sessionCoinText;

    private int sessionCoin = 0;

    void Start()
    {
        sessionCoin = 0;
        UpdateCoinUI();
        Debug.Log("🎮 Current Scene: " + SceneManager.GetActiveScene().name);
    }

    public void AddCoinOnCollect()
    {
        int amount = GetCoinAmountByScene();
        sessionCoin += amount;
        UpdateCoinUI();

        Debug.Log($"+{amount} coin (Scene: {SceneManager.GetActiveScene().name}) - Session total: {sessionCoin}");
    }

    private int GetCoinAmountByScene()
    {
        string scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Level1": return 15;
            case "Level2": return 20;
            case "Level3": return 25;
            default: return 10;
        }
    }

    public void AddCoin(int coin)
    {
        SharedPlayerStats.GameStats.sharedStats.Coins += coin;
    }

    public void CommitSessionToTotal()
    {
        AddCoin(sessionCoin);
        PlayerPrefs.SetInt("TotalCoins", SharedPlayerStats.GameStats.sharedStats.Coins);
        PlayerPrefs.Save();

        Debug.Log($"🎉 Win! Gained {sessionCoin} coin → Total now: {SharedPlayerStats.GameStats.sharedStats.Coins}");
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

    public int GetSessionCoin()
    {
        return sessionCoin;
    }

    public int GetTotalCoin()
    {
        return SharedPlayerStats.GameStats.sharedStats.Coins;
    }

}
