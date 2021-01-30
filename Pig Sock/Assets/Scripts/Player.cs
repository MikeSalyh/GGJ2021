using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public List<Card> myHand;
    public int currentCardIndex = -1;
    public int[] roundScores;
    public delegate void CardAction(Card c);
    public CardAction OnBust;
    public CardAction OnHit;
    public CardAction OnTake;
    public CardAction OnJackpot;

    public enum PlayerState
    {
        Waiting,
        PlayerTurn,
        Reveal,
        Bust,
        Take,
        Jackpot
    }
    public PlayerState currentState;

    public void Init()
    {
        GameManager.instance.OnNewGame += HandleNewGame;
        GameManager.instance.OnRoundOver += HandleRoundEnd;
    }

    public void GenerateNewHand()
    {
        currentCardIndex = -1;
        myHand = new List<Card>();
        if (GameManager.instance.useSequentialValuesInHand)
        {
            for (int i = 0; i < GameManager.instance.cardsPerDeck; i++)
            {
                Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
                myHand.Add(new Card(randomSuit, i + 1));
            }
        }
        else
        {
            myHand = GameManager.instance.fullDeck.OrderBy(x => UnityEngine.Random.value).Take(GameManager.instance.cardsPerDeck).OrderBy(x => x.value).ToList<Card>();
        }
        for (int i = 0; i < GameManager.instance.jokersPerDeck; i++)
        {
            Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
            int randomIndex = UnityEngine.Random.Range(1, myHand.Count);

            if (GameManager.instance.debug__putJokersAtEnd)
                randomIndex = myHand.Count;
            myHand.Insert(randomIndex, new Card(randomSuit, 0, true));
        }
    }

    public void HitMe()
    {
        currentCardIndex++;
        currentState = PlayerState.Reveal; //only matters for future corotuines.
        if (OnHit != null)
            OnHit.Invoke(CurrentCard);

        if (CurrentCard.isJoker)
        {
            Bust();
        }
        else
        {
            if (currentCardIndex + 1 >= GameManager.instance.cardsPerDeck)
                currentState = PlayerState.Jackpot;
            else
                currentState = PlayerState.PlayerTurn;
        }
    }

    public void TakeCard()
    {
        currentState = PlayerState.Take;
        if (OnTake != null)
            OnTake.Invoke(CurrentCard);
    }

    protected void Bust()
    {
        currentState = PlayerState.Bust;
        if (OnBust != null)
            OnBust.Invoke(CurrentCard);
    }

    protected void Jackpot()
    {
        currentState = PlayerState.Jackpot;
        if (OnJackpot != null)
            OnJackpot.Invoke(CurrentCard);
    }


    public void HandleNewGame()
    {
        roundScores = new int[GameManager.instance.maxRounds];
        for (int i = 0; i < roundScores.Length; i++)
            roundScores[i] = -1;
    }

    public void HandleRoundEnd()
    {
        roundScores[GameManager.instance.currentRoundIndex] = CurrentPot;
    }

    public Card CurrentCard
    {
        get { return myHand[currentCardIndex]; }
    }

    public int CurrentPot
    {
        get
        {
            if (GameManager.instance.useAddativeValues)
            {
                int output = 0;
                for (int i = 0; i <= CurrentCard.value; i++)
                    output += i;
                if (CurrentCard.suit == GameManager.instance.luckySuit)
                    output *= 2;
                return output;
            }
            else
            {
                if (CurrentCard.suit == GameManager.instance.luckySuit)
                    return CurrentCard.value * GameManager.instance.matchMultiplier;
                else
                    return CurrentCard.value;
            }
        }
    }
}
