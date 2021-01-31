using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameGUI : MonoBehaviour
{
    private PlayerScoreCard[] scoreCards;
    public GameObject playerScoreRecapFab;
    public Transform playerScoreHolder;
    public CanvasGroup curtain;

    private void Awake()
    {
        curtain.alpha = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnNewGame += OnNewGame;
        scoreCards = new PlayerScoreCard[GameManager.instance.numPlayers];
        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            scoreCards[i] = GameObject.Instantiate(playerScoreRecapFab, playerScoreHolder).GetComponent<PlayerScoreCard>();
        }
    }

    void OnNewGame()
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
    }
}
