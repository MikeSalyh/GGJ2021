﻿using System.Collections;
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
    public CardVisualization luckySuit, specialPurposeLucky;
    public Button sockMe, takeCard;
    public CardVisualization card, peekCard, specialPurposeCard, cardBacking;
    public CanvasGroup cardDeckArea, controlsArea;
    public GameObject tutorial, gameplayArea;
    public Transform cardCenter;
    public AIAdvisor ai;
    public FinaleManager finale;
    public GameObject botWaitingPrompt;

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
        HandleChangeLuckySuit();
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
        luckySuit.SetToCard(new Card(GameManager.instance.luckySuit, 1));        
    }

    void HandleChangeLuckySuit()
    {
        multiplierText.text = GameManager.instance.luckySuit.ToString() + " socks are worth double points!";
        luckySuit.flipCard(new Card(GameManager.instance.luckySuit, 1), true);
    }

    void HandleRoundOver()
    {
    }

    void HandleNewRound()
    {
    }

    void HandleStartTurn(Player p)
    {
        playersNameText.text = p.data.name + "'s Turn";
        peekCard.gameObject.SetActive(false);
        card.gameObject.SetActive(true);
        card.SetFaceDown();
        takeCardText.text = "Bank";
        sockMeText.text = "Sock Me";

        GameManager.instance.activePlayer.OnSock += HandleSockMe;
        GameManager.instance.activePlayer.OnTake += HandleTakeCard;
        GameManager.instance.activePlayer.OnPeek += HandlePeek;
        GameManager.instance.activePlayer.OnBust += HandleBust;

        //Fading & animating in
        cardDeckArea.DOKill();
        cardDeckArea.transform.position = cardCenter.transform.position + new Vector3(0f, -560f, 0f); 
        cardDeckArea.transform.DOLocalMoveY(12, 0.5f).SetEase(Ease.OutExpo);
        playersNameText.DOKill();
        playersNameText.alpha = 0f;
        playersNameText.DOFade(1f, 0.25f);
        controlsArea.DOKill();
        controlsArea.alpha = 0f;
        if (GameManager.instance.activePlayer.data.type == PlayerData.Type.Human)
        {
            controlsArea.DOFade(1f, 0.75f).SetDelay(0.5f).SetEase(Ease.OutQuad);
            controlsArea.interactable = true;
        }
        else
        {
            controlsArea.alpha = 0;
            controlsArea.interactable = false;
        }
        AudioManager.instance.Play(AudioManager.instance.deckEnter);

        if (GameManager.instance.activePlayer.data.type == PlayerData.Type.CPU)
            StartCoroutine(BotTakeAction());
    }

    void HandleEndTurn(Player p)
    {
        playersNameText.text = "";
        Debug.Log(GameManager.instance.activePlayer.ToString());
        GameManager.instance.activePlayer.OnSock -= HandleSockMe;
        GameManager.instance.activePlayer.OnTake -= HandleTakeCard;
        GameManager.instance.activePlayer.OnPeek -= HandlePeek;
        GameManager.instance.activePlayer.OnBust -= HandleBust;
    }

    void HandleGameOver()
    {
        cardDeckArea.gameObject.SetActive(false);
        gameplayArea.gameObject.SetActive(false);
        AudioManager.instance.Play(AudioManager.instance.gameEnd);
        Invoke("ShowFinale", 1f);
    }

    void ShowFinale()
    {
        finale.gameObject.SetActive(true);
        finale.GetComponent<CanvasGroup>().alpha = 0;
        finale.GetComponent<CanvasGroup>().DOFade(1f, 1f);
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
        takeCardText.text = "Okay";
        card.transform.DOShakePosition(1f, 8f);
        AudioManager.instance.Play(AudioManager.instance.bust);
    }

    public void HandlePeek(Card c)
    {
        peekCard.SetToCard(c);
        peekCard.gameObject.SetActive(true);
        AudioManager.instance.Play(AudioManager.instance.peek);
    }

    public void HandleTakeCard(Card c)
    {
        StartCoroutine(CTakeCard(c));
    }

    private IEnumerator CTakeCard(Card c){
        peekCard.gameObject.SetActive(false);
        controlsArea.DOKill();
        controlsArea.DOFade(0f, 0.25f);
        playersNameText.DOKill();
        playersNameText.DOFade(0f, 0.25f);
        if (GameManager.instance.activePlayer.DeckSize > 1)
            card.SetFaceDown();
        else
            card.gameObject.SetActive(false);

        PlayerScoreCard destination = null;
        for (int i = 0; i < scoreCards.Length; i++)
        {
            if (scoreCards[i].playerRef == GameManager.instance.activePlayer)
            {
                destination = scoreCards[i];
                break;
            }
        }

        AudioManager.instance.Play(AudioManager.instance.takeSock);
        int bestRound = GameManager.instance.currentRoundIndex >= GameManager.instance.maxRounds ? GameManager.instance.maxRounds - 1 : GameManager.instance.currentRoundIndex;
        if (c.type == Card.CardType.Normal)
        {
            specialPurposeCard.CollectAs(c, card.transform, destination.rounds[bestRound].transform);
        }
        if (c.suit == GameManager.instance.luckySuit && c.type == Card.CardType.Normal)
        {
            specialPurposeLucky.CollectAs(c, luckySuit.transform, destination.rounds[bestRound].transform);
        }

        int newCardValue = c.suit == GameManager.instance.luckySuit ? c.value * GameManager.instance.matchMultiplier : c.value;
        if (newCardValue > GameManager.instance.jackpotThreshhold)
        {
            //Do jackpot stuff here
            AudioManager.instance.Play(AudioManager.instance.jackpot);
        }

        GameManager.instance.RequestDelay(1.6f);
        yield return new WaitForSeconds(1f);

        //Fading
        cardDeckArea.transform.DOKill();
        cardDeckArea.transform.DOLocalMoveY(570f, 0.5f).SetEase(Ease.InExpo);
        AudioManager.instance.Play(AudioManager.instance.deckExit);
    }

    public void HandleSockMe(Card c)
    {
        if (!card.IsFaceDown)
        {
            specialPurposeCard.FallDown(previousCard, card.transform);
            card.SetToCard(c);
            if(GameManager.instance.activePlayer.DeckSize < GameManager.instance.cardsPerDeck + GameManager.instance.jokersPerDeck)
                AudioManager.instance.Play(AudioManager.instance.sockMeBtn);
        }
        else
        {
            card.SetToCard(c);
        }
        string takePhrase = GameManager.instance.activePlayer.CurrentCard.suit == GameManager.instance.luckySuit ? "Pair " : "";
        takeCardText.text = string.Format("Bank {0}(+{1})", takePhrase, GameManager.instance.activePlayer.CurrentPot);
        peekCard.gameObject.SetActive(false);
        previousCard = c;
    }


    IEnumerator BotTakeAction()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (GameManager.instance.activePlayer.data.type != PlayerData.Type.CPU)
            {
                yield break;
            }
            else
            {

                if (ai.recommendation == AIAdvisor.Recommendation.Take)
                {
                    if (GameManager.instance.activePlayer.CurrentCard.type == Card.CardType.Joker)
                        yield return new WaitForSeconds(1.5f);
                    else
                        yield return new WaitForSeconds(0.75f);

                    takeCard.onClick.Invoke();
                    yield break;
                }
                else
                {
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.35f));
                    sockMe.onClick.Invoke();
                }
            }
        }
    }


    void Update()
    {
        if(GameManager.instance.activePlayer != null)
            botWaitingPrompt.gameObject.SetActive(GameManager.instance.activePlayer.data.type == PlayerData.Type.CPU);
        if (GameManager.instance.activePlayer == null || controlsArea.alpha < 0.5f) {
            sockMe.interactable = false;
            takeCard.interactable = false;
        }
        else{
            sockMe.interactable = GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn;
            takeCard.interactable = (GameManager.instance.activePlayer.currentState == Player.PlayerState.PlayerTurn || GameManager.instance.activePlayer.currentState == Player.PlayerState.LastCard || GameManager.instance.activePlayer.currentState == Player.PlayerState.Bust) && GameManager.instance.activePlayer.currentCardIndex >= 0;
        }
    }

    public void ToggleTutorial()
    {
        tutorial.gameObject.SetActive(!tutorial.gameObject.activeSelf);
    }

    public void PlayClick()
    {
        AudioManager.instance.Play(AudioManager.instance.generalMenuSelect);
    }
}
