using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [HideInInspector] public Player playerRef;
    public Button hitMe, take, peek;
    public TextMeshProUGUI scoreList, collectionValue, discardAction;
    public CardVisualization card, nextCard;

    public void Init(Player playerRef)
    {
        this.playerRef = playerRef;
        hitMe.onClick.AddListener(playerRef.HitMe);
        take.onClick.AddListener(playerRef.TakeCard);
        peek.onClick.AddListener(playerRef.Peek);
        playerRef.OnHit += DoUpdateCard;
        playerRef.OnTake += DoTakeCard;
        playerRef.OnPeek += DoPeek;
        playerRef.OnEndTurn += UpdateScoreList;
    }

    public void DoNewGameGraphics()
    {
        card.ShowBack();
        collectionValue.text = "Take ";
        UpdateScoreList();
        nextCard.gameObject.SetActive(false);
    }

    public void DoNewRoundGraphics()
    {
        nextCard.gameObject.SetActive(false);
        card.gameObject.SetActive(true);
        UpdateScoreList();
        card.ShowBack();
        collectionValue.text = "Take ";
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
        if (c.type == Card.CardType.Peek)
        {
            discardAction.text = "Collect";
        }
        else
        {
            discardAction.text = "Discard";
        }
        nextCard.gameObject.SetActive(false);
    }

    public void DoPeek(Card c)
    {
        nextCard.SetToCard(c);
        nextCard.gameObject.SetActive(true);
    }

    public void DoTakeCard(Card c)
    {
        nextCard.gameObject.SetActive(false);
        card.gameObject.SetActive(false);
        UpdateScoreList();
    }

    // Update is called once per frame
    void Update()
    {
        hitMe.interactable = playerRef.currentState == Player.PlayerState.PlayerTurn;
        take.interactable = (playerRef.currentState == Player.PlayerState.PlayerTurn || playerRef.currentState == Player.PlayerState.Jackpot) && playerRef.currentCardIndex >= 0 && playerRef.CurrentCard.type != Card.CardType.Peek;
        peek.interactable = playerRef.currentState == Player.PlayerState.PlayerTurn && playerRef.CanPeek;
    }
}
