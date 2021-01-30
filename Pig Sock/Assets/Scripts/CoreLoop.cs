using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoreLoop : MonoBehaviour
{
    [Header("Game Options")]
    public bool useSequentialValuesInHand = true;
    public int maxRounds = 5;
    public int cardsPerDeck = 13;
    public int numCardsPerSuit = 13;
    public int matchMultiplier = 2;
    public int jokersPerDeck = 1;

    public enum GameState
    {
        NewRound,
        PlayerTurn,
        Reveal,
        Bust,
        Take,
        Jackpot,
        GameOver
    }


    [Header("Debug")]
    public int currentRoundIndex = -1;
    public int currentCardIndex = -1;
    public int[] roundScores;
    public GameState CurrentState = GameState.GameOver;
    public Card.Suit luckySuit;

    //DELEGATES
    public delegate void CardAction(Card c);
    public CardAction OnBust;
    public CardAction OnHit;
    public CardAction OnTake;
    public CardAction OnJackpot;

    public delegate void GameAction();
    public GameAction OnNewGame;
    public GameAction OnGameOver;
    public GameAction OnNewRound;
    public GameAction OnRoundOver;

    [SerializeField] private Card[] fullDeck;
    [SerializeField] private List<Card> myHand;

    public bool debug__putJokersAtEnd = false;

    public Card CurrentCard
    {
        get { return myHand[currentCardIndex]; } 
    }

    public int CurrentPot
    {
        get {
            if (CurrentCard.suit == luckySuit)
                return CurrentCard.value * matchMultiplier;
            else
                return CurrentCard.value;
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    protected void NewGame()
    {
        GenerateFullDeck();
        roundScores = new int[maxRounds];
        for (int i = 0; i < roundScores.Length; i++)
            roundScores[i] = -1;
        currentRoundIndex = -1;

        if (OnNewGame != null)
            OnNewGame.Invoke();

        StartNewRound();
    }

    public void StartNewRound()
    {
        currentRoundIndex++;
        CurrentState = GameState.NewRound;
        GenerateNewHand();
        luckySuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length); //randomize the lucky suit.

        //This may become a coroutine after animation is added
        if (OnNewRound != null)
            OnNewRound.Invoke();

        //This may become a coroutine after animation is added
        CurrentState = GameState.PlayerTurn;
    }

    protected void EndRound()
    {
        roundScores[currentRoundIndex] = CurrentPot;
        if (OnRoundOver != null)
            OnRoundOver.Invoke();

        if (currentRoundIndex == maxRounds)
        {
            DoGameOver();
        }
    }

    private void GenerateFullDeck()
    {
        int numSuits = Enum.GetNames(typeof(Card.Suit)).Length;
        fullDeck = new Card[numCardsPerSuit * numSuits];
        int counter = 0;
        for (int i = 0; i < numSuits; i++) {
            for (int j = 1; j <= numCardsPerSuit; j++)
            {
                fullDeck[counter] = new Card((Card.Suit)i, j);
                counter++;
            }
        }
    }

    private void GenerateNewHand()
    {
        currentCardIndex = -1;
        myHand = new List<Card>();
        if (useSequentialValuesInHand)
        {
            for (int i = 0; i < cardsPerDeck; i++)
            {
                Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
                myHand.Add(new Card(randomSuit, i+1));
            }
        }
        else
        {
            myHand = fullDeck.OrderBy(x => UnityEngine.Random.value).Take(cardsPerDeck).OrderBy(x => x.value).ToList<Card>();
        }
        for (int i = 0; i < jokersPerDeck; i++)
        {
            Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
            int randomIndex = UnityEngine.Random.Range(1, myHand.Count);

            if (debug__putJokersAtEnd)
                randomIndex = myHand.Count;
            myHand.Insert(randomIndex, new Card(randomSuit, 0, true));
        }
    }

    public void ResetGame()
    {
        NewGame();
    }

    public void HitMe()
    {
        currentCardIndex++;
        CurrentState = GameState.Reveal; //only matters for future corotuines.
        if (OnHit != null)
            OnHit.Invoke(CurrentCard);

        if (CurrentCard.isJoker)
        {
            Bust();
        }
        else
        {
            if (currentCardIndex + 1 >= cardsPerDeck)
                CurrentState = GameState.Jackpot;
            else
                CurrentState = GameState.PlayerTurn;
        }
    }

    public void TakeCard()
    {
        CurrentState = GameState.Take;
        if (OnTake != null)
            OnTake.Invoke(CurrentCard);

        EndRound();
    }

    protected void Bust()
    {
        CurrentState = GameState.Bust;
        if (OnBust != null)
            OnBust.Invoke(CurrentCard);

        EndRound();
    }

    protected void Jackpot()
    {
        CurrentState = GameState.Jackpot;
        if (OnJackpot != null)
            OnJackpot.Invoke(CurrentCard);

        EndRound();
    }

    protected void DoGameOver()
    {
        CurrentState = GameState.GameOver;
        if (OnGameOver != null)
            OnGameOver.Invoke();
    }
}
