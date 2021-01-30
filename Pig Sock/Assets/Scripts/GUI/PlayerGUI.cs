using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [HideInInspector] public Player playerRef;
    public Button hitMe, take;
    public TextMeshProUGUI scoreList, collectionValue;
    public CardVisualization card;

    public void Init(Player playerRef)
    {
        this.playerRef = playerRef;
        hitMe.onClick.AddListener(playerRef.HitMe);
        take.onClick.AddListener(playerRef.TakeCard);
        playerRef.OnHit += DoUpdateCard;
        playerRef.OnTake += DoTakeCard;
        //playerRef.OnBust += DoUpdateCard;
    }

    public void DoNewGameGraphics()
    {
        UpdateScoreList();
        card.ShowBack();
        collectionValue.text = "Take Card";
    }

    public void DoNewRoundGraphics()
    {
        card.gameObject.SetActive(true);
        UpdateScoreList();
        card.ShowBack();
        collectionValue.text = "Take Card";
    }

    public void UpdateScoreList()
    {
        scoreList.text = "";
        int finalScore = 0;
        if (playerRef != null && playerRef.roundScores != null)
        {
            for (int i = 0; i < playerRef.roundScores.Length; i++)
            {
                scoreList.text += "\n";
                if (GameManager.instance.currentRoundIndex == i) scoreList.text += "<color=yellow>";
                scoreList.text += playerRef.roundScores[i] >= 0 ? playerRef.roundScores[i].ToString() : "?";
                if (GameManager.instance.currentRoundIndex == i) scoreList.text += "</color>";
                finalScore += playerRef.roundScores[i];
            }
        }

        if (GameManager.instance.CurrentState == GameManager.GameState.GameOver)
        {
            scoreList.text += "\n--\n";
            scoreList.text += "<color=green>" + finalScore.ToString() + "</color>";
        }
    }

    public void DoUpdateCard(Card c)
    {
        card.SetToCard(c);
        collectionValue.text = "Take Card   " + string.Format("(+{0})", playerRef.CurrentPot);
    }

    public void DoTakeCard(Card c)
    {
        card.gameObject.SetActive(false);
        UpdateScoreList();
    }

    // Update is called once per frame
    void Update()
    {
        hitMe.interactable = playerRef.currentState == Player.PlayerState.PlayerTurn;
        take.interactable = (playerRef.currentState == Player.PlayerState.PlayerTurn || playerRef.currentState == Player.PlayerState.Jackpot) && playerRef.currentCardIndex >= 0;
    }
}
