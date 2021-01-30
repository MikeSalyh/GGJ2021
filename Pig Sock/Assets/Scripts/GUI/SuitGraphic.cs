using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuitGraphic : MonoBehaviour
{
    public Sprite[] suitImages;
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>(); 
    }

    public void SetSuit(Card.Suit suit)
    {
        img.sprite = suitImages[(int)suit];
    }
}
