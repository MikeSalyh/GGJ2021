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
    public int maxCardsPerHand = 13;
    public int numCardsPerSuit = 13;
    public int matchMultiplier = 2;

    public enum GameState
    {
        NewRound,
        PlayerTurn,
        Reveal,
        Bust,
        GameOver
    }


    [Header("Debug")]
    public int currentRoundIndex = -1;
    public int currentCardIndex = -1;
    public int[] roundScores;
    public GameState CurrentState = GameState.GameOver;

    //DELEGATES
    public delegate void CardAction(Card c = null);
    public CardAction OnBust;
    public CardAction OnHit;
    public CardAction OnTake;

    public delegate void GameAction();
    public GameAction OnNewGame;
    public GameAction OnGameOver;
    public GameAction OnNewRound;

    [SerializeField] private Card[] fullDeck;
    [SerializeField] private Card jokerCard = new Card(Card.Suit.Clubs, int.MaxValue) { isJoker = true };
    [SerializeField] private Card[] myHand;

    public Card CurrentCard
    {
        get { return myHand[currentCardIndex]; } 
    }



    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        NewRound();
    }

    protected void NewRound()
    {
        currentRoundIndex++;
        GenerateNewHand();
        if (OnNewRound != null)
            OnNewRound.Invoke();
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
        myHand = new Card[maxCardsPerHand];
        if (useSequentialValuesInHand)
        {
            for (int i = 0; i < myHand.Length; i++)
            {
                Card.Suit randomSuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length);
                myHand[i] = new Card(randomSuit, i+1);
            }
        }
        else
        {
            myHand = fullDeck.OrderBy(x => UnityEngine.Random.value).Take(maxCardsPerHand).OrderBy(x => x.value).ToArray();
        }
    }

    public void ResetGame()
    {
        NewGame();
    }

    public void HitMe()
    {
        if (OnHit != null)
            OnHit.Invoke();
    }

    public void TakeCard()
    {
        if (OnTake != null)
            OnTake.Invoke();
    }

    protected void Bust()
    {
        if (OnBust != null)
            OnBust.Invoke();
    }
}
