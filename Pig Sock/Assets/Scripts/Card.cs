using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Suit
    {
        Diamonds,
        Hearts,
        Spades,
        Clubs
    }

    public Suit suit;
    public int value;
    public bool isJoker = false;
}
