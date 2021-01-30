using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoreLoopGraphics : MonoBehaviour
{
    public Button reset, hitMe, take, nextRound;
    public TextMeshProUGUI scoreList, cardsRemainingText, multiplierText, collectionValue;
    public CardVisualization card;
    public SuitGraphic luckySuit;
    public Player player1;

    private void Start()
    {
        player1 = GameManager.instance.player;
        reset.onClick.AddListener(GameManager.instance.ResetGame);
        hitMe.onClick.AddListener(player1.HitMe);
        take.onClick.AddListener(player1.TakeCard);
        nextRound.onClick.AddListener(GameManager.instance.StartNewRound);
        GameManager.instance.OnNewGame += DoNewGameGraphics;
        GameManager.instance.OnRoundOver += UpdateScoreList;
        GameManager.instance.OnNewRound += DoNewRoundGraphics;
        player1.OnHit += DoUpdateCard;
        player1.OnTake += DoTakeCard;
    }

    // Update is called once per frame
    void Update()
    {
        hitMe.interactable = player1.currentState == Player.PlayerState.PlayerTurn;
        take.interactable = (player1.currentState == Player.PlayerState.PlayerTurn || player1.currentState == Player.PlayerState.Jackpot) && player1.currentCardIndex >= 0;
        nextRound.interactable = GameManager.instance.currentRoundIndex < GameManager.instance.maxRounds - 1 && (player1.currentState == Player.PlayerState.Bust || player1.currentState == Player.PlayerState.Take);
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
        luckySuit.SetSuit(GameManager.instance.luckySuit);
        collectionValue.gameObject.SetActive(false);
        cardsRemainingText.text = (GameManager.instance.cardsPerDeck + GameManager.instance.jokersPerDeck - player1.currentCardIndex - 1).ToString();
    }

    void UpdateScoreList()
    {
        scoreList.text = "ROUND SCORES:";
        for (int i = 0; i < player1.roundScores.Length; i++)
        {
            scoreList.text += "\n";
            scoreList.text += player1.roundScores[i] >= 0 ? player1.roundScores[i].ToString() : "?";
        }
        multiplierText.text = GameManager.instance.matchMultiplier.ToString() + "x";
    }

    void DoUpdateCard(Card c)
    {
        cardsRemainingText.text = (GameManager.instance.cardsPerDeck + GameManager.instance.jokersPerDeck - player1.currentCardIndex - 1).ToString();
        card.SetToCard(c);
        collectionValue.gameObject.SetActive(true);
        //bool isMatch <-- WIP Need matching logic.
        collectionValue.text = string.Format("(+{0})", player1.CurrentPot);
    }

    void DoTakeCard(Card c)
    {
        card.gameObject.SetActive(false);
    }
}
