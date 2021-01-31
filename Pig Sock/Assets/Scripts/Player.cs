using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectedCard
{
    public Card card;
    public bool wasMatch;

    public CollectedCard(Card c, bool match)
    {
        this.card = c;
        this.wasMatch = match;
    }

    public CollectedCard() { }

    public int Value
    {
        get
        {
            if (card == null)
                return 0;
            if (wasMatch)
            {
                return card.value * GameManager.instance.matchMultiplier;
            } else{
                return card.value;
            }
        }
    }
}

//[System.Serializable]
public class Player
{
    public PlayerData data = new PlayerData();
    public List<Card> myHand;
    public int currentCardIndex = -1;
    public CollectedCard[] roundScores;

    public int Score
    {
        get
        {
            int output = 0;
            for (int i = 0; i < roundScores.Length; i++)
                output += roundScores[i].Value; 
            return output;
        }
    }
    public delegate void CardAction(Card c);
    public CardAction OnBust;
    public CardAction OnSock;
    public CardAction OnTake;
    public CardAction OnJackpot;
    public CardAction OnPeek;
    public GameManager.GameAction OnEndTurn;

    public enum PlayerState
    {
        Waiting,
        PlayerTurn,
        Reveal,
        Bust,
        Take,
        LastCard,
        Done
    }

    public PlayerState currentState;


    public int DeckSize
    {
        get
        {
            return myHand.Count - currentCardIndex;
        }
    }

    public Player()
    {
        data = new PlayerData();
        data.name = "SockFan" + UnityEngine.Random.Range(101, 999).ToString();

    }

    public void Init()
    {
        GameManager.instance.OnNewGame += HandleNewGame;
        //OnBust += EndTurn;
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
    }

    public void EndTurn(Card c = null)
    {
        roundScores[GameManager.instance.currentRoundIndex] = new CollectedCard(c, c.type != Card.CardType.Joker && c.suit == GameManager.instance.luckySuit);
        currentState = PlayerState.Done;
        if (OnEndTurn != null)
            OnEndTurn.Invoke();
    }

    public void SockMe()
    {
        currentCardIndex++;
        currentState = PlayerState.Reveal; //only matters for future corotuines.
        if (OnSock != null)
            OnSock.Invoke(CurrentCard);


        Debug.Log(DeckSize);

        if (CurrentCard.type == Card.CardType.Joker)
        {
            Bust();
        }
        else
        {
            if (HasNextCard && CurrentCard.value == 1 && GameManager.instance.acesPeek)
                Peek();

            if (DeckSize <= 1)
                currentState = PlayerState.LastCard;
            else
                currentState = PlayerState.PlayerTurn;
        }
    }

    public void Peek()
    {
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
        currentState = PlayerState.LastCard;
        if (OnJackpot != null)
            OnJackpot.Invoke(CurrentCard);
    }

    public void HandleNewGame()
    {
        roundScores = new CollectedCard[GameManager.instance.maxRounds];
        for (int i = 0; i < roundScores.Length; i++)
            roundScores[i] = new CollectedCard();
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
