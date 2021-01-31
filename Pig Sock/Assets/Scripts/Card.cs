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

    public CardType type;
    public enum CardType
    {
        Normal,
        Joker,
        Peek
    }

    public Card(Suit suit, int value)
    {
        this.suit = suit;
        this.value = value;
    }

    public Card(CardType type)
    {
        switch (type) {
            case CardType.Joker:
                this.type = CardType.Joker;
                break;
            case CardType.Peek:
                this.type = CardType.Peek;
                break;
            case CardType.Normal:
                throw new System.Exception("Cannot construct normal cards this way");
        }
    }
}
