using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public enum Suit
    {
        Yellow,
        Red,
        Blue,
        Green
    }

    public Suit suit;
    public int value;

    public CardType type;
    public enum CardType
    {
        Normal,
        Joker
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
            case CardType.Normal:
                throw new System.Exception("Cannot construct normal cards this way");
        }
    }

    public string Name
    {
        get
        {
            //WIP
            if (type == CardType.Joker)
                return "Joker";

            string output = "";
            switch (value) {
                case 13:
                    output+= "King";
                    break;
                case 12:
                    output += "Queen";
                    break;
                case 11:
                    output += "Jack";
                    break;
                case 1:
                    output += "Ace";
                    break;
                default:
                    output += value.ToString();
                    break;
            }
            output += " of ";
            output += suit.ToString();
            return output;
        }
    }

    public string FlavorText
    {
        //WIP
        get
        {
            return "It's a special card that everybody likes. Thats why this is placeholder text.";
        }
    }
}
