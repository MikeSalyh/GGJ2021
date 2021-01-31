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
    private Card previousCard;

    public TextMeshProUGUI playersNameText, multiplierText, takeCardText, sockMeText;
    public CardVisualization luckySuit;
    public Button sockMe, takeCard;
    public CardVisualization card, peekCard, specialPurposeCard, cardBacking;
    public CanvasGroup gameArea;
    public GameObject tutorial;

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
        GameManager.instance.OnChangeLuckySuit += HandleChangeLuckySuit;
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
        card.SetFaceDown();
        peekCard.gameObject.SetActive(false);
    }

    void HandleChangeLuckySuit()
    {
        luckySuit.SetToCard(new Card(GameManager.instance.luckySuit, 1));
    }

    void HandleRoundOver()
    {
    }

    void HandleNewRound()
    {
        luckySuit.SetToCard(new Card(GameManager.instance.luckySuit, 1));
        multiplierText.text = (GameManager.instance.matchMultiplier.ToString() + "x") + " points if matching";
    }

    void HandleStartTurn(Player p)
    {
        playersNameText.text = p.data.name + "'s Turn";
        peekCard.gameObject.SetActive(false);
        card.gameObject.SetActive(true);
        card.SetFaceDown();
        takeCardText.text = "Take";
        sockMeText.text = "Sock Me";
        GameManager.instance.activePlayer.OnSock += HandleSockMe;
        GameManager.instance.activePlayer.OnTake += HandleTakeCard;
        GameManager.instance.activePlayer.OnPeek += HandlePeek;
        GameManager.instance.activePlayer.OnBust += HandleBust;
    }

    void HandleEndTurn(Player p)
    {
        playersNameText.text = "";
        GameManager.instance.activePlayer.OnSock -= HandleSockMe;
        GameManager.instance.activePlayer.OnTake -= HandleTakeCard;
        GameManager.instance.activePlayer.OnPeek -= HandlePeek;
        GameManager.instance.activePlayer.OnBust -= HandleBust;
    }

    void HandleGameOver()
    {
        gameArea.gameObject.SetActive(false);
    }

    public void ClickSockMe()
    {
        GameManager.instance.activePlayer.SockMe();
    }
    public void ClickTakeCard()
    {
        GameManager.instance.activePlayer.TakeCard();
    }

    void HandleBust(Card c)
    {
        sockMeText.text = "Bust!";
        takeCardText.text = "Take (+0)";
        card.transform.DOShakePosition(1f, 5f);
    }

    public void HandlePeek(Card c)
    {
        peekCard.SetToCard(c);
        peekCard.gameObject.SetActive(true);
    }

    public void HandleTakeCard(Card c)
    {
        peekCard.gameObject.SetActive(false);
        if (GameManager.instance.activePlayer.DeckSize > 1)
            card.SetFaceDown();
        else
            card.gameObject.SetActive(false);

        GameManager.instance.RequestDelay(1.5f);
        PlayerScoreCard destination = null;
        for (int i = 0; i < scoreCards.Length; i++)
        {
            if (scoreCards[i].playerRef == GameManager.instance.activePlayer)
            {
                destination = scoreCards[i];
                break;
            }
        }
        specialPurposeCard.CollectAs(c, card.transform, destination.rounds[GameManager.instance.currentRoundIndex].transform);
    }

    public void HandleSockMe(Card c)
    {
        if (!card.IsFaceDown)
        {
            specialPurposeCard.FallDown(previousCard, card.transform);
            card.SetToCard(c);
        }
        else
        {
            card.flipCard(c);
        }
        string takePhrase = GameManager.instance.activePlayer.CurrentCard.suit == GameManager.instance.luckySuit ? "Pair " : "";
        takeCardText.text = string.Format("Take {0}(+{1})", takePhrase, GameManager.instance.activePlayer.CurrentPot);
        peekCard.gameObject.SetActive(false);
        previousCard = c;
    }

    void Update()
    {
        if (GameManager.instance.activePlayer == null) {
            sockMe.interactable = false;
            takeCard.interactable = false;
        }
        else {
            sockMe.interactable = GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn;
            takeCard.interactable = (GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn || GameManager.instance.activePlayer.currentState == Player.PlayerState.LastCard || GameManager.instance.activePlayer.currentState == Player.PlayerState.Bust) && GameManager.instance.activePlayer.currentCardIndex >= 0;
        }
    }

    public void ToggleTutorial()
    {
        tutorial.gameObject.SetActive(!tutorial.gameObject.activeSelf);
    }

}
