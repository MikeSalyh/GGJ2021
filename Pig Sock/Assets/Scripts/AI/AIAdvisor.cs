using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAdvisor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnNewGame += HandleNewGame;
        GameManager.instance.OnStartTurn += HandleNewTurn;
        GameManager.instance.OnEndTurn += HandleEndTurn;
    }

    public float averageTurnValue;
    public float nextCardEV;
    public float bustingRisk;
    public float scoreDelta; //A large score delta means the player is behind and they need to play aggressively. A negative means they're ahead and could play conservative.
    public float goodDeal;

    public enum Mood
    {
        Cowardly,
        Careful,
        Neutral,
        Aggressive,
        Reckless
    }
    public Mood mood;

    public enum Recommendation
    {
        None,
        SockMe,
        Take
    }

    public Recommendation recommendation;

    void HandleNewTurn(Player activePlayer)
    {
        scoreDelta = ComputeScoreDelta(activePlayer);
        activePlayer.OnSock += CrunchData;
    }

    float ComputeScoreDelta(Player activePlayer)
    {
        float scoreToBeat = -1;
        Player bestOpponent = null;
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {
            float hypotheticalScore = GetPlayerHypotheticalScore(GameManager.instance.players[i]);
            //Debug.Log(GameManager.instance.players[i].data.name + " " + hypotheticalScore);

            if (GameManager.instance.players[i] != activePlayer && hypotheticalScore > scoreToBeat)
            {
                bestOpponent = GameManager.instance.players[i];
                scoreToBeat = hypotheticalScore;
            }
        }

        //solo case
        if (bestOpponent == null)
        {
            Debug.Log("There was no opponent."); //This is the case where an AI plays itself.
            scoreToBeat = 1;
        }

        Debug.Log("The best opponents hypothetical score was " + scoreToBeat);

        //Now that we know the score to beat, compare with self:
        float myHypotheticalScore = GetPlayerHypotheticalScore(activePlayer);
        Debug.Log("My hypothetical score was " + myHypotheticalScore);

        return scoreToBeat - myHypotheticalScore;

    }

    float GetPlayerHypotheticalScore(Player p)
    {
        float output = 0;
        for (int i = 0; i < p.roundScores.Length; i++)
        {
            if (!p.roundScores[i].Played)
            {
                output += averageTurnValue;
            }
            else
            {
                output += p.roundScores[i].Value;
            }
        }
        return output;
    }

    void HandleEndTurn(Player activePlayer)
    {
        activePlayer.OnSock -= CrunchData;
    }


    void HandleNewGame()
    {
        averageTurnValue = CalculateAverageTurnValue();
    }

    void CrunchData(Card c = null) {
        nextCardEV = CalculateNextCardExpectedValue(c);
        mood = SetMood(scoreDelta);
        goodDeal = GetGoodDeal(mood);

        if (c.type == Card.CardType.Joker)
            recommendation = Recommendation.Take;
        else
            recommendation = GameManager.instance.activePlayer.CurrentPot >= goodDeal ? Recommendation.Take : Recommendation.SockMe;
    }

    Mood SetMood(float scoreDelta)
    {
        if (scoreDelta > 15)
            return Mood.Reckless;
        if (scoreDelta > 7.5f)
            return Mood.Aggressive;
        if (scoreDelta < -15)
            return Mood.Cowardly;
        if (scoreDelta < -7.5f)
            return Mood.Careful;
        return Mood.Neutral;
    }

    float GetGoodDeal(Mood mood)
    {
        float output = 0;
        switch (mood) {
            case Mood.Cowardly:
                output = averageTurnValue * 0.5f;
                break;
            case Mood.Careful:
                output = averageTurnValue * 0.75f;
                break;
            case Mood.Neutral:
                output = averageTurnValue;
                break;
            case Mood.Aggressive:
                output = averageTurnValue * 1.5f;
                break;
            case Mood.Reckless:
                output = averageTurnValue * 2f;
                break;
            default:
                output = averageTurnValue;
                break;
        }
        return output * (1 - bustingRisk);
    }

    public float CalculateTotalPlayerValue(Player p)
    {
        float value = 0;
        for (int i = 0; i < p.roundScores.Length; i++)
        {
            if (p.roundScores[i].card == null)
            {
                value += averageTurnValue;
                //Uses approx for unplayed turns
            }
            else
            {
                value += p.roundScores[i].Value;
            }
        }
        return value;
    }

    public float CalculateAverageTurnValue()
    {
        float value = 0;
        for (int i = 1; i <= GameManager.instance.cardsPerDeck; i++)
            value += i;

        value *= (1 + (1f / Enum.GetNames(typeof(Card.Suit)).Length)); //add suit boost increase
        value /= GameManager.instance.cardsPerDeck; //take average
        return value; // set the average turn value here.
    }

    public float CalculateNextCardExpectedValue(Card currentCard)
    {
        float value = averageTurnValue;

        //factor in busting risk.
        int jokers = GameManager.instance.jokersPerDeck;

        if (!GameManager.instance.activePlayer.HasNextCard)
        {
            bustingRisk = 1;
            return 0;
        }else if (GameManager.instance.acesPeek && currentCard.value == 1)
        {
            //Aces peek
            bustingRisk = GameManager.instance.activePlayer.NextCard.type == Card.CardType.Joker ? 1 : 0;
            return GameManager.instance.activePlayer.NextCard.value; 
        }
        else
        {
            bustingRisk = (float)jokers / ((float)GameManager.instance.activePlayer.DeckSize - 1);
        }
        return value * (1f-bustingRisk);
    }
}
