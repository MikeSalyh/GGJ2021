using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoreLoopGraphics : MonoBehaviour
{
    public Button reset, hitMe, take;
    public CoreLoop gameManager;
    public TextMeshProUGUI scoreList;

    private void Start()
    {
        reset.onClick.AddListener(gameManager.ResetGame);
        hitMe.onClick.AddListener(gameManager.HitMe);
        take.onClick.AddListener(gameManager.TakeCard);
        gameManager.OnNewGame += UpdateScoreList;
        gameManager.OnNewRound += UpdateScoreList;
    }

    // Update is called once per frame
    void Update()
    {
        hitMe.enabled = gameManager.CurrentState == CoreLoop.GameState.PlayerTurn;
        take.enabled = gameManager.CurrentState == CoreLoop.GameState.PlayerTurn;
    }

    void UpdateScoreList()
    {
        scoreList.text = "ROUND SCORES:";
        for (int i = 0; i < gameManager.roundScores.Length; i++)
        {
            scoreList.text += "\n";
            scoreList.text += gameManager.roundScores[i] >= 0 ? gameManager.roundScores[i].ToString() : "?";
        }
    }
}
