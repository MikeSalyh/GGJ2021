using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoreLoopGraphics : MonoBehaviour
{
    public CoreLoop gameManager;
    public Button reset, hitMe, take, nextRound;
    public TextMeshProUGUI scoreList, cardsRemainingText, multiplierText, collectionValue;
    public CardVisualization card;
    public SuitGraphic luckySuit;

    private void Start()
    {
        reset.onClick.AddListener(gameManager.ResetGame);
        hitMe.onClick.AddListener(gameManager.HitMe);
        take.onClick.AddListener(gameManager.TakeCard);
        nextRound.onClick.AddListener(gameManager.StartNewRound);
        gameManager.OnNewGame += DoNewGameGraphics;
        gameManager.OnRoundOver += UpdateScoreList;
        gameManager.OnNewRound += DoNewRoundGraphics;
        gameManager.OnHit += DoUpdateCard;
        gameManager.OnTake += DoTakeCard;
    }

    // Update is called once per frame
    void Update()
    {
        hitMe.interactable = gameManager.CurrentState == CoreLoop.GameState.PlayerTurn;
        take.interactable = (gameManager.CurrentState == CoreLoop.GameState.PlayerTurn || gameManager.CurrentState == CoreLoop.GameState.Jackpot) && gameManager.currentCardIndex >= 0;
        nextRound.interactable = gameManager.currentRoundIndex < gameManager.maxRounds - 1 && (gameManager.CurrentState == CoreLoop.GameState.Bust || gameManager.CurrentState == CoreLoop.GameState.Take);
    }

    void DoNewGameGraphics()
    {
        UpdateScoreList();
        card.ShowBack();
        collectionValue.gameObject.SetActive(false);
    }

    void DoNewRoundGraphics()
    {
        card.gameObject.SetActive(true);
        UpdateScoreList();
        card.ShowBack();
        luckySuit.SetSuit(gameManager.luckySuit);
        collectionValue.gameObject.SetActive(false);
        cardsRemainingText.text = (gameManager.cardsPerDeck + gameManager.jokersPerDeck - gameManager.currentCardIndex - 1).ToString();
    }

    void UpdateScoreList()
    {
        scoreList.text = "ROUND SCORES:";
        for (int i = 0; i < gameManager.roundScores.Length; i++)
        {
            scoreList.text += "\n";
            scoreList.text += gameManager.roundScores[i] >= 0 ? gameManager.roundScores[i].ToString() : "?";
        }
        multiplierText.text = gameManager.matchMultiplier.ToString() + "x";
    }

    void DoUpdateCard(Card c)
    {
        cardsRemainingText.text = (gameManager.cardsPerDeck + gameManager.jokersPerDeck - gameManager.currentCardIndex - 1).ToString();
        card.SetToCard(c);
        collectionValue.gameObject.SetActive(true);
        //bool isMatch <-- WIP Need matching logic.
        collectionValue.text = string.Format("(+{0})", gameManager.CurrentPot);
    }

    void DoTakeCard(Card c)
    {
        card.gameObject.SetActive(false);
    }
}
