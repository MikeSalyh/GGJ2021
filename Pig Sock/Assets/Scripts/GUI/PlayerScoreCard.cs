using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerScoreCard : MonoBehaviour
{
    public GameObject cardPrefab;
    public TextMeshProUGUI nametag;
    public TextMeshProUGUI scoreNumber;

    public Transform cardHolder;
    public RoundScore[] rounds;

    public void SetUp(Player p)
    {
        nametag.text = p.myName;
        scoreNumber.text = p.Score.ToString();
        foreach (Transform child in cardHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        rounds = new RoundScore[GameManager.instance.maxRounds];
        for (int i = 0; i < GameManager.instance.maxRounds; i++)
            rounds[i] = GameObject.Instantiate(cardPrefab, cardHolder).GetComponent<RoundScore>();
    }
}
