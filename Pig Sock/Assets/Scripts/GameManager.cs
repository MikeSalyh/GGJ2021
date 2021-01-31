using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum ShuffleMode
    {
        Increasing,
        RandomBetter,
        FullRandom
    }

    [Header("Game Options")]
    public ShuffleMode shuffleMode;
    public bool useAddativeValues = false;
    public bool highestScoreGoesFirst = true;

    [Header("Variables")]
    public int numPlayers = 1;
    public int maxRounds = 5;
    public int cardsPerDeck = 13;
    public int numCardsPerSuit = 13;
    public int matchMultiplier = 2;
    public int jokersPerDeck = 1;
    public int peeksPerDeck = 1;

    
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
    public Player[] players;

    //DELEGATES
    public delegate void GameAction();
    public GameAction OnNewGame;
    public GameAction OnGameOver;
    public GameAction OnNewRound;
    public GameAction OnRoundOver;

    public delegate void PlayerAction(Player p);
    public PlayerAction OnStartTurn;
    public PlayerAction OnEndTurn;


    public Card[] fullDeck;
    public bool debug__putSpecialsAtEnd = false;

    private Queue<Player> activePlayers;
    public Player activePlayer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("NewGame", 0.1f); // A little janky; don't start the new game immediately, so other classes can init.
    }

    protected void NewGame()
    {
        Debug.Log("New game!");
        CurrentState = GameState.Playing;
        players = new Player[numPlayers];
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new Player();
            players[i].Init();
            players[i].OnEndTurn += TryEndRound;
        }
        
        GenerateFullDeck();
        currentRoundIndex = -1;
        if (OnNewGame != null)
            OnNewGame.Invoke();

        StartNewRound();
    }

    public bool AllActionsDone
    {
        get
        {
            for (int i = 0; i < players.Length; i++) {
                if(players[i].currentState != Player.PlayerState.Done)
                    return false;
            }
            return true;
        }
    }

    public void StartNewRound()
    {
        if (highestScoreGoesFirst)
            activePlayers = new Queue<Player>(players.OrderBy(x => x.Score));
        else
            activePlayers = new Queue<Player>(players);

        currentRoundIndex++;
        for(int i = 0; i < players.Length; i++)
            players[i].GenerateNewHand();

        luckySuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length); //randomize the lucky suit.

        //This may become a coroutine after animation is added
        if (OnNewRound != null)
            OnNewRound.Invoke();

        //This may become a coroutine after animation is added
        for (int i = 0; i < players.Length; i++)
            players[i].currentState = Player.PlayerState.Waiting;

        CurrentState = GameState.Playing;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            if (activePlayers.Count > 0 && activePlayers.Peek().currentState == Player.PlayerState.Waiting)
            {
                activePlayer = activePlayers.Peek();
                activePlayer.currentState = Player.PlayerState.PlayerTurn;
                if (OnStartTurn != null)
                    OnStartTurn.Invoke(activePlayer);
            }
            else if (activePlayers.Count > 0 && activePlayers.Peek().currentState == Player.PlayerState.Done)
            {
                activePlayers.Dequeue();
                if (OnEndTurn != null)
                    OnEndTurn.Invoke(activePlayer);
                activePlayer = null;
            }
        }
    }

    protected void TryEndRound()
    {
        if (AllActionsDone)
        {
            CurrentState = GameState.RoundOver;
            if (OnRoundOver != null)
                OnRoundOver.Invoke();

            if (currentRoundIndex + 1 >= maxRounds)
            {
                DoGameOver();
            }
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
