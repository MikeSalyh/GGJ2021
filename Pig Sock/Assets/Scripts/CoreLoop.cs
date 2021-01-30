using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreLoop : MonoBehaviour
{
    public int maxRounds = 5;
    public int numCardsPerDeck = 13;

    public enum GameState
    {
        NewRound,
        PlayerTurn,
        Reveal,
        Bust,
        GameOver
    }

    public GameState CurrentState = GameState.GameOver;

    [Header("Debug")]
    public int currentRound;
    public int[] roundScores;

    //DELEGATES
    public delegate void CardAction(Card c = null);
    public CardAction OnBust;
    public CardAction OnHit;
    public CardAction OnTake;

    public delegate void GameAction();
    public GameAction OnNewGame;
    public GameAction OnGameOver;
    public GameAction OnNewRound;

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
        roundScores = new int[maxRounds];
        for (int i = 0; i < roundScores.Length; i++)
            roundScores[i] = -1;
        currentRound = 0;

        if (OnNewGame != null)
            OnNewGame.Invoke();
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
