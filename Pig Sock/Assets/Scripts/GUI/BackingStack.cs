using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackingStack : MonoBehaviour
{
    public GameObject backingFab;
    public GameObject[] backings;

    // Start is called before the first frame update
    void Start()
    {
        backings = new GameObject[GameManager.instance.cardsPerDeck + GameManager.instance.jokersPerDeck];
        for (int i = 0; i < backings.Length; i++)
        {
            GameObject g = GameObject.Instantiate(backingFab, transform);
            g.transform.localPosition = new Vector3((backings.Length - i+1) * -3, 0, 0);
            backings[i] = g;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.activePlayer != null && GameManager.instance.activePlayer.currentState != Player.PlayerState.Done)
        {
            for (int i = 0; i < backings.Length; i++)
            {
                backings[backings.Length - i - 1].gameObject.SetActive(GameManager.instance.activePlayer.DeckSize - 1 > i);
            }
        }
    }
}
