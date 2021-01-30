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
    void Start()
    {
        image = GetComponent<Image>();   
    }

    public void SetToCard(Card c)
    {
        image.sprite = GetSprite(c);
    }

    private Sprite GetSprite(Card c)
    {
        if (c.isJoker)
        {
            return jokerSprite;
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].suit == c.suit)
                {
                    return sprites[i].sprites[c.value];
                }
            }
        }
        Debug.LogWarning("No sprite found!");
        return backSprite;
    }
}
