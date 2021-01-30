using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoreLoopGraphics : MonoBehaviour
{
    public Button reset, nextRound;
    public TextMeshProUGUI multiplierText;
    public SuitGraphic luckySuit;
    public PlayerGUI[] playersGUIs;

    private void Start()
    {
        reset.onClick.AddListener(GameManager.instance.ResetGame);
        nextRound.onClick.AddListener(GameManager.instance.StartNewRound);
        GameManager.instance.OnNewGame += DoNewGameGraphics;
        GameManager.instance.OnRoundOver += DoUpdateScore;
        GameManager.instance.OnNewRound += DoNewRoundGraphics;
        GameManager.instance.OnGameOver += DoUpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
        nextRound.interactable = GameManager.instance.currentRoundIndex < GameManager.instance.maxRounds - 1 && GameManager.instance.AllActionsDone;
    }

    void DoNewGameGraphics()
    {
        for (int i = 0; i < playersGUIs.Length; i++)
        {
            Debug.Log(GameManager.instance.players[i]);
            playersGUIs[i].Init(GameManager.instance.players[i]);
            playersGUIs[i].DoNewGameGraphics();
        }
    }

    void DoUpdateScore()
    {
        for (int i = 0; i < playersGUIs.Length; i++)
            playersGUIs[i].UpdateScoreList();
    }

    void DoNewRoundGraphics()
    {
        luckySuit.SetSuit(GameManager.instance.luckySuit);
        multiplierText.text = (GameManager.instance.matchMultiplier.ToString() + "x") + " points if matching";
        for (int i = 0; i < playersGUIs.Length; i++)
            playersGUIs[i].DoNewRoundGraphics();
    }
}
