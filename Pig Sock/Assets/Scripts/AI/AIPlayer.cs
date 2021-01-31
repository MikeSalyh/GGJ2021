using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnNewGame += HandleNewGame;
        GameManager.instance.OnStartTurn += AddHandlers;
        GameManager.instance.OnEndTurn += RemoveHandlers;
    }

    public float averageTurnValue;
    public float nextCardEV;
    public float bustingRisk;

    void AddHandlers(Player p)
    {
        p.OnSock += CrunchData;
    }

    void RemoveHandlers(Player p)
    {
        p.OnSock -= CrunchData;
    }


    void HandleNewGame()
    {
        averageTurnValue = CalculateAverageTurnValue();
    }

    void CrunchData(Card c = null) {
        nextCardEV = CalculateNextCardExpectedValue(c);
    }

    public int CalculateTotalValue(Player p)
    {
        return 0;
    }

    public float CalculateAverageTurnValue()
    {
        float value = 0;
        for (int i = 1; i <= GameManager.instance.cardsPerDeck; i++)
            value += i;

        value *= (1 + (1f / Enum.GetNames(typeof(Card.Suit)).Length)); //add suit boost increase
        value /= GameManager.instance.cardsPerDeck; //take average
        return value; // set the average turn value here.
    }

    public float CalculateNextCardExpectedValue(Card currentCard)
    {
        float value = averageTurnValue;

        //factor in busting risk.
        int jokers = GameManager.instance.jokersPerDeck;

        if (!GameManager.instance.activePlayer.HasNextCard)
        {
            bustingRisk = 1;
            return 0;
        }else if (GameManager.instance.acesPeek && currentCard.value == 1)
        {
            //Aces peek
            bustingRisk = GameManager.instance.activePlayer.NextCard.type == Card.CardType.Joker ? 1 : 0;
            return GameManager.instance.activePlayer.NextCard.value; 
        }
        else
        {
            bustingRisk = (float)jokers / ((float)GameManager.instance.activePlayer.DeckSize - 1);
        }
        return value * (1f-bustingRisk);
    }
}
