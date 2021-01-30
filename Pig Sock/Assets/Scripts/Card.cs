﻿using System.Collections;
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

    public Card(Suit suit, int value, bool isJoker = false)
    {
        this.suit = suit;
        this.value = value;
        this.isJoker = isJoker;
    }
}
