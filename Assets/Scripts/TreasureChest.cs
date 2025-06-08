using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public Sprite closedSprite;  
    public Sprite openFullSprite; 
    public Sprite openEmptySprite;

    private SpriteRenderer spriteRenderer;
    private bool isOpened = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSprite;
    }

    void OnMouseDown() 
    {
        if (!isOpened)
        {
            spriteRenderer.sprite = openFullSprite;
            isOpened = true;

            Invoke("SetToEmpty", 2f);
        }
    }

    void SetToEmpty()
    {
        spriteRenderer.sprite = openEmptySprite;
    }
}
