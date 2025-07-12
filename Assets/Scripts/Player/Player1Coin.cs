using UnityEngine;
using UnityEngine.UI;

public class Player1Coin : MonoBehaviour
{
    public Text coinText;

    void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        SharedPlayerStats.GameStats.sharedStats.Coins += amount;
        UpdateCoinUI();
    }

    public void SetCoins(int amount) // Optional: to force set from saved data
    {
        SharedPlayerStats.GameStats.sharedStats.Coins = amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            int current = SharedPlayerStats.GameStats.sharedStats.Coins;
            coinText.text = current.ToString();
            Debug.Log("Updated Coins: " + current);
        }
    }

    public int GetCoinCount()
    {
        return SharedPlayerStats.GameStats.sharedStats.Coins;
    }
}
