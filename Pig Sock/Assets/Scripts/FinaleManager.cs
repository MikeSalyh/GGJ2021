using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FinaleManager : MonoBehaviour
{
    public PlayerData winner;
    public TextMeshProUGUI winnerNameLabel, bestCardLabel;
    public CardVisualization card;

    // Start is called before the first frame update
    void Start()
    {
        //Create fake data if needed
        if (MetagameManager.instance.playerData.Length == 0)
        {
            MetagameManager.instance.playerData = new PlayerData[1];
            MetagameManager.instance.playerData[0] = new PlayerData("Example", PlayerData.Type.Human);
            MetagameManager.instance.playerData[0].bestCard = new Card(Card.Suit.Clubs, 10);
            MetagameManager.instance.playerData[0].finalScore = 30;
        }

        int bestScore = -99;
        for (int i = 0; i < MetagameManager.instance.playerData.Length; i++)
        {
            if (MetagameManager.instance.playerData[i].finalScore > bestScore)
            {
                bestScore = MetagameManager.instance.playerData[i].finalScore;
                winner = MetagameManager.instance.playerData[i];
            }
        }

        //card.SetToCard(winner.bestCard);
        winnerNameLabel.text = winner.name; 
        //string.Format("{0} - {1} Ponts", winner.name, winner.finalScore);
        //bestCardLabel.text = string.Format("{0}'s Best Card: <b><color=yellow>{1}</color></b><br><i>{2}</i>", winner.name, winner.bestCard.Name, winner.bestCard.FlavorText);
    }

    public void QuitToMenu()
    {
        MetagameManager.instance.GoToMenu();
    }

    public void Rematch()
    {
        MetagameManager.instance.GoToGameplay();
    }

    public void PlayClick()
    {
        AudioManager.instance.Play(AudioManager.instance.generalMenuSelect);
    }
}
