using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public LobbySelector[] selectors;
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        selectors[0].SetType(PlayerData.Type.Human);
        selectors[1].SetType(PlayerData.Type.CPU);
        for (int i = 2; i < selectors.Length; i++)
        {
            selectors[i].SetType(PlayerData.Type.Off);
        }
        startButton.onClick.AddListener(StartGame);
    }

    // Update is called once per frame
    void Update()
    {
        bool allOff = true;
        for (int i = 0; i < selectors.Length; i++)
        {
            if (selectors[i].data.type != PlayerData.Type.Off)
            {
                allOff = false;
                break;
            }
        }
        startButton.interactable = !allOff;
    }

    public void StartGame()
    {
        for (int i = 0; i < selectors.Length; i++)
        {
            //Avoid empty names
            if (string.IsNullOrEmpty(selectors[i].data.name))
            {
                string prefix = selectors[i].data.type == PlayerData.Type.CPU ? "Bot " : "Player ";
                selectors[i].name = prefix + (i + 1).ToString();
            }

            MetagameManager.instance.GoToGameplay();
        }
    }
}
