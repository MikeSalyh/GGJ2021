using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundScore : MonoBehaviour
{
    public CardVisualization primaryCard, secondaryCard;

    private void Start()
    {
        ClearCard();    
    }

    public void SetCard(Card c, bool pair) {
        primaryCard.SetToCard(c);
        if (pair)
        {
            secondaryCard.SetToCard(c);
            secondaryCard.gameObject.SetActive(true);
        }
        else
        {
            secondaryCard.gameObject.SetActive(false);
        }
    }

    public void ClearCard()
    {
        primaryCard.SetFaceDown();
        secondaryCard.gameObject.SetActive(false);
    }
}
