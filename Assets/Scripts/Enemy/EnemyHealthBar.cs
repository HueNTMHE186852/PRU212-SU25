using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image _thanhMau;
    public void updateHeathBar(float currentHealth, float maxHealth)
    {
        _thanhMau.fillAmount = currentHealth / maxHealth;
    }
}
