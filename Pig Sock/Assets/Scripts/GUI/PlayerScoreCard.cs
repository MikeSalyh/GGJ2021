using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerScoreCard : MonoBehaviour
{
    public GameObject cardPrefab;
    public TextMeshProUGUI nametag;
    public TextMeshProUGUI scoreNumber;
    public Image background;

    public Transform cardHolder;
    public RoundScore[] rounds;
    private Player playerRef;

    public void SetUp(Player p)
    {
        playerRef = p;
        nametag.text = p.data.name;
        scoreNumber.text = p.Score.ToString();
        foreach (Transform child in cardHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        rounds = new RoundScore[GameManager.instance.maxRounds];
        for (int i = 0; i < GameManager.instance.maxRounds; i++)
            rounds[i] = GameObject.Instantiate(cardPrefab, cardHolder).GetComponent<RoundScore>();
    }

    private void Update()
    {
        if (playerRef != null)
        {
            //calling this every frame isn't efficient.
            scoreNumber.text = playerRef.Score > 0 ? playerRef.Score.ToString() : "";
            background.enabled = (playerRef.currentState == Player.PlayerState.PlayerTurn);
            for (int i = 0; i < rounds.Length; i++)
            {
                rounds[i].SetCard(playerRef.roundScores[i].card, playerRef.roundScores[i].wasMatch);
            }
        }
    }
}
