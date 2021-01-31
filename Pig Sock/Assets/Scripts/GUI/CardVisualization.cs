using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class SuitSprites
{
    public Card.Suit suit;
    public Sprite[] sprites;
}

public class CardVisualization : MonoBehaviour
{
    private Image image;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite jokerSprite;
    [SerializeField] private SuitSprites[] sprites;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();   
    }

    public void SetToCard(Card c)
    {
        if (c == null) ShowBack();
        else image.sprite = GetSprite(c);
    }

    public void ShowBack()
    {
        image.sprite = backSprite;
    }

    private Sprite GetSprite(Card c)
    {
        if (c.type == Card.CardType.Joker)
        {
            return jokerSprite;
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].suit == c.suit)
                {
                    return sprites[i].sprites[c.value - 1];
                }
            }
        }
        Debug.LogWarning("No sprite found!");
        return backSprite;
    }
}
