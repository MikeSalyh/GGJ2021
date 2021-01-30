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
        foreach (PlayerGUI g in playersGUIs)
            g.gameObject.SetActive(false);

        if (playersGUIs.Length < GameManager.instance.numPlayers)
            throw new System.Exception("There arent enough GUI elements to support that many players");
        for (int i = 0; i < GameManager.instance.numPlayers; i++)
        {
            playersGUIs[i].gameObject.SetActive(true);
            playersGUIs[i].Init(GameManager.instance.players[i]);
            playersGUIs[i].DoNewGameGraphics();
        }
    }

    void DoUpdateScore()
    {
        for (int i = 0; i < GameManager.instance.numPlayers; i++)
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
