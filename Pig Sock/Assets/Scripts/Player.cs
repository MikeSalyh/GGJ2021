using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
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
    public CardAction OnPeek;
    public GameManager.GameAction OnEndTurn;
    public int peeks = 0;

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
        OnBust += EndTurn;
        OnTake += EndTurn;
        OnJackpot += EndTurn;
    }

    public void GenerateNewHand()
    {
        currentCardIndex = -1;
        myHand = new List<Card>();
        if (GameManager.instance.shuffleMode == GameManager.ShuffleMode.Increasing)
        {
            for (int i = 0; i < GameManager.instance.cardsPerDeck; i++)
            {
                Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
                myHand.Add(new Card(randomSuit, i + 1));
            }
        }
        else if (GameManager.instance.shuffleMode == GameManager.ShuffleMode.RandomBetter)
        {
            myHand = GameManager.instance.fullDeck.OrderBy(x => UnityEngine.Random.value).Take(GameManager.instance.cardsPerDeck).OrderBy(x => x.value).ToList<Card>();
        }
        else
        {
            myHand = GameManager.instance.fullDeck.OrderBy(x => UnityEngine.Random.value).Take(GameManager.instance.cardsPerDeck).ToList<Card>();
        }

        //ADD SPECIAL CARDS:
        for (int i = 0; i < GameManager.instance.jokersPerDeck; i++)
        {
            Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
            int randomIndex = UnityEngine.Random.Range(1, myHand.Count);

            if (GameManager.instance.debug__putSpecialsAtEnd)
                randomIndex = myHand.Count;
            myHand.Insert(randomIndex, new Card(Card.CardType.Joker));
        }
        for (int i = 0; i < GameManager.instance.peeksPerDeck; i++)
        {
            Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
            int randomIndex = UnityEngine.Random.Range(1, myHand.Count);

            if (GameManager.instance.debug__putSpecialsAtEnd)
                randomIndex = myHand.Count;
            myHand.Insert(randomIndex, new Card(Card.CardType.Peek));
        }
    }

    public void EndTurn(Card c = null)
    {
        roundScores[GameManager.instance.currentRoundIndex] = CurrentPot;
        currentState = PlayerState.Waiting;
        if (OnEndTurn != null)
            OnEndTurn.Invoke();
    }

    public void HitMe()
    {
        if (currentCardIndex > 0 && CurrentCard.type == Card.CardType.Peek)
            peeks++;

        currentCardIndex++;
        currentState = PlayerState.Reveal; //only matters for future corotuines.
        if (OnHit != null)
            OnHit.Invoke(CurrentCard);

        if (CurrentCard.type == Card.CardType.Joker)
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

    public bool CanPeek
    {
        get { return peeks > 0 && HasNextCard; }
    }

    public void Peek()
    {
        peeks--;
        if (OnPeek != null)
            OnPeek.Invoke(NextCard);
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
        peeks = 0;
        roundScores = new int[GameManager.instance.maxRounds];
        for (int i = 0; i < roundScores.Length; i++)
            roundScores[i] = -1;
    }

    public Card CurrentCard
    {
        get {
            return myHand[currentCardIndex];
        }
    }

    public bool HasNextCard
    {
        get { return (currentCardIndex + 1 <= myHand.Count); }
    }

    public Card NextCard
    {
        get
        {
            return myHand[currentCardIndex + 1];
        }
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
