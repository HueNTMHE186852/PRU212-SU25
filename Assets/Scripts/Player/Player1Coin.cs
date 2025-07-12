using UnityEngine;
using UnityEngine.UI;

public class Player1Coin : MonoBehaviour
{
    public Text coinText; 
    private int coinCount = 0;

    void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString(); 
            Debug.Log("Update" + coinCount); 
        }
    }


    public int GetCoinCount()
    {
        return coinCount;
    }
}
