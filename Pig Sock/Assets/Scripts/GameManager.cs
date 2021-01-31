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
    public bool acesPeek = true;

    [Header("Variables")]
    public int numPlayers = 1;
    public int maxRounds = 5;
    public int cardsPerDeck = 13;
    public int numCardsPerSuit = 13;
    public int matchMultiplier = 2;
    public int jokersPerDeck = 1;
    public readonly float gameEndDelay = 0.5f;

    
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
    public GameAction OnChangeLuckySuit;

    public delegate void PlayerAction(Player p);
    public PlayerAction OnStartTurn;
    public PlayerAction OnEndTurn;


    public Card[] fullDeck;
    public bool debug__putSpecialsAtEnd = false;

    private Queue<Player> activePlayers;
    public Player activePlayer;

    private float requestedDelay;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (MetagameManager.instance.playerData != null && MetagameManager.instance.playerData.Length > 0)
        {
            Debug.Log("Init from data");
            players = new Player[MetagameManager.instance.playerData.Length];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Player();
                players[i].data = MetagameManager.instance.playerData[i];
            }
            numPlayers = players.Length;
        }
        else
        {
            Debug.Log("Init from fallback");
            players = new Player[numPlayers];
            PlayerData[] dataToMetagame = new PlayerData[numPlayers];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Player();
                dataToMetagame[i] = players[i].data;
            }
            MetagameManager.instance.playerData = dataToMetagame;
        }

        Invoke("NewGame", 0.1f); // A little janky; don't start the new game immediately, so other classes can init.
    }

    protected void NewGame()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Init();
            players[i].OnEndTurn += TryEndRound;
        }
        
        GenerateFullDeck();
        currentRoundIndex = -1;
        CurrentState = GameState.Playing;

        if (OnNewGame != null)
            OnNewGame.Invoke();

        OnRoundOver += StartNewRound;
        ChangeLuckySuit();
        StartNewRound();
    }

    public void ChangeLuckySuit()
    {
        //WIP - should it always be different?
        Debug.Log("Changing lucky suit");
        luckySuit = (Card.Suit)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Card.Suit)).Length); //randomize the lucky suit.
        if (OnChangeLuckySuit != null)
            OnChangeLuckySuit.Invoke();
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

        //This may become a coroutine after animation is added
        if (OnNewRound != null)
            OnNewRound.Invoke();

        //This may become a coroutine after animation is added
        for (int i = 0; i < players.Length; i++)
            players[i].currentState = Player.PlayerState.Waiting;

        CurrentState = GameState.Playing;
    }

    public void RequestDelay(float seconds)
    {
        requestedDelay = seconds;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            if (requestedDelay > 0f)
            {
                requestedDelay -= Time.deltaTime;
            }
            else
            {
                if (activePlayers.Count > 0 && activePlayers.Peek().currentState == Player.PlayerState.Waiting)
                {
                    activePlayer = activePlayers.Peek();
                    activePlayer.currentState = Player.PlayerState.PlayerTurn;
                    if (OnStartTurn != null)
                        OnStartTurn.Invoke(activePlayer);
                    activePlayer.SockMe();
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
    }

    protected void TryEndRound()
    {
        if (AllActionsDone)
        {
            CurrentState = GameState.RoundOver;
            if (OnRoundOver != null)
                OnRoundOver.Invoke();

            if (currentRoundIndex >= maxRounds)
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
        Debug.Log("The game is over!");
        CurrentState = GameState.GameOver;

        //Save out the relevant end of game data.
        for (int i = 0; i < players.Length; i++)
        {
            players[i].data.finalScore = players[i].Score;
            int bestVal = -99;
            Card bestCard = null;
            for (int j = 0; j < players[i].roundScores.Length; j++) {
                int cardVal = players[i].roundScores[j].card.value;
                if (players[i].roundScores[j].wasMatch) cardVal *= 2;
                if (cardVal > bestVal)
                {
                    bestVal = cardVal;
                    bestCard = players[i].roundScores[j].card;
                }
            }
            players[i].data.bestCard = bestCard;
        }

        if (OnGameOver != null)
            OnGameOver.Invoke();

        Invoke("GoToFinale", gameEndDelay);
    }

    void GoToFinale()
    {
        MetagameManager.instance.GoToFinale();
    }

    public void QuitToMenu()
    {
        MetagameManager.instance.GoToMenu();
    }
}
