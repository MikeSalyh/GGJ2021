using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour
{
    private PlayerScoreCard[] scoreCards;
    public GameObject playerScoreRecapFab;
    public Transform playerScoreHolder;
    public CanvasGroup curtain;

    public TextMeshProUGUI playersNameText, multiplierText, takeCardText;
    public SuitGraphic luckySuit;
    public Button sockMe, takeCard;
    public CardVisualization card, peekCard;

    private void Awake()
    {
        curtain.alpha = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        scoreCards = new PlayerScoreCard[GameManager.instance.numPlayers];
        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            scoreCards[i] = GameObject.Instantiate(playerScoreRecapFab, playerScoreHolder).GetComponent<PlayerScoreCard>();
        }
        GameManager.instance.OnNewGame += HandleNewGame;
        GameManager.instance.OnRoundOver += HandleRoundOver;
        GameManager.instance.OnNewRound += HandleNewRound;
        GameManager.instance.OnGameOver += HandleGameOver;
        GameManager.instance.OnStartTurn += HandleStartTurn;
        GameManager.instance.OnEndTurn += HandleEndTurn;
        sockMe.onClick.AddListener(ClickSockMe);
        takeCard.onClick.AddListener(ClickTakeCard);
    }

    void HandleNewGame()
    {
        StartCoroutine(COnNewGame());
    }

    private IEnumerator COnNewGame()
    {
        yield return new WaitForEndOfFrame();
        curtain.DOFade(0f, 0.25f);
        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            scoreCards[i].SetUp(GameManager.instance.players[i]);
        }
        card.ShowBack();
        takeCardText.text = "Take";
        peekCard.gameObject.SetActive(false);
    }

    public void DoUpdateCard(Card c)
    {
        card.SetToCard(c);
        string takePhrase = GameManager.instance.activePlayer.CurrentCard.suit == GameManager.instance.luckySuit ? "Pair " : "";
        takeCardText.text = string.Format("Take {0}(+{1})", takePhrase, GameManager.instance.activePlayer.CurrentPot);
        peekCard.gameObject.SetActive(false);
    }

    void HandleRoundOver()
    {
    }

    void HandleNewRound()
    {
        luckySuit.SetSuit(GameManager.instance.luckySuit);
        multiplierText.text = (GameManager.instance.matchMultiplier.ToString() + "x") + " points if matching";
    }

    void HandleStartTurn(Player p)
    {
        playersNameText.text = p.myName + "'s Turn";
        peekCard.gameObject.SetActive(false);
        card.gameObject.SetActive(true);
        card.ShowBack();
        takeCardText.text = "Take";
        GameManager.instance.activePlayer.OnSock += DoUpdateCard;
        GameManager.instance.activePlayer.OnTake += DoTakeCard;
        GameManager.instance.activePlayer.OnPeek += DoPeek;
    }

    void HandleEndTurn(Player p)
    {
        playersNameText.text = "";
        GameManager.instance.activePlayer.OnSock -= DoUpdateCard;
        GameManager.instance.activePlayer.OnTake -= DoTakeCard;
        GameManager.instance.activePlayer.OnPeek -= DoPeek;
    }

    void HandleGameOver()
    {
    }

    public void ClickSockMe()
    {
        GameManager.instance.activePlayer.SockMe();
    }
    public void ClickTakeCard()
    {
        GameManager.instance.activePlayer.TakeCard();
    }

    public void DoPeek(Card c)
    {
        peekCard.SetToCard(c);
        peekCard.gameObject.SetActive(true);
    }

    public void DoTakeCard(Card c)
    {
        peekCard.gameObject.SetActive(false);
        card.gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.instance.activePlayer == null) {
            sockMe.interactable = false;
            takeCard.interactable = false;
        }
        else {
            sockMe.interactable = GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn;
            takeCard.interactable = (GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn || GameManager.instance.activePlayer.currentState == Player.PlayerState.Jackpot) && GameManager.instance.activePlayer.currentCardIndex >= 0;
        }
    }

}
