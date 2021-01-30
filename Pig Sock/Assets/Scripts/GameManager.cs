using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Options")]
    public bool useSequentialValuesInHand = true;
    public bool useAddativeValues = false;

    [Header("Variables")]
    public int numPlayers = 1;
    public int maxRounds = 5;
    public int cardsPerDeck = 13;
    public int numCardsPerSuit = 13;
    public int matchMultiplier = 2;
    public int jokersPerDeck = 1;

    
    public enum GameState
    {
        Playing,
        RoundOver,
        GameOver
    }

    [Header("Debug")]
    public int currentRoundIndex = -1;
    public GameState CurrentState = GameState.GameOver;
    public Card.Suit luckySuit;
    public Player player;

    //DELEGATES
    public delegate void GameAction();
    public GameAction OnNewGame;
    public GameAction OnGameOver;
    public GameAction OnNewRound;
    public GameAction OnRoundOver;

    public Card[] fullDeck;
    public bool debug__putJokersAtEnd = false;


    private void Awake()
    {
        instance = this;
        player.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        NewGame();
    }

    protected void NewGame()
    {
        player.OnBust += EndRound;
        player.OnTake += EndRound;
        player.OnJackpot += EndRound;

        GenerateFullDeck();
        currentRoundIndex = -1;
        if (OnNewGame != null)
            OnNewGame.Invoke();

        StartNewRound();
    }

    public void StartNewRound()
    {
        currentRoundIndex++;
        player.GenerateNewHand();
        luckySuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length); //randomize the lucky suit.

        //This may become a coroutine after animation is added
        if (OnNewRound != null)
            OnNewRound.Invoke();

        //This may become a coroutine after animation is added
        player.currentState = Player.PlayerState.PlayerTurn;
        CurrentState = GameState.Playing;
    }

    protected void EndRound(Card c = null)
    {
        CurrentState = GameState.RoundOver;
        if (OnRoundOver != null)
            OnRoundOver.Invoke();

        if (currentRoundIndex + 1 >= maxRounds)
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

    

    public void ResetGame()
    {
        NewGame();
    }

    
    protected void DoGameOver()
    {
        CurrentState = GameState.GameOver;
        if (OnGameOver != null)
            OnGameOver.Invoke();
    }
}
