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
        selectors[0].nameInput.text = "SockFan" + UnityEngine.Random.Range(101, 999).ToString();
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
        List<PlayerData> output = new List<PlayerData>();
        for (int i = 0; i < selectors.Length; i++)
        {
            //Avoid empty names
            if (string.IsNullOrEmpty(selectors[i].data.name))
            {
                string prefix = selectors[i].data.type == PlayerData.Type.CPU ? "Bot " : "Player ";
                selectors[i].name = prefix + (i + 1).ToString();
            }

            if (selectors[i].data.type != PlayerData.Type.Off)
            {
                selectors[i].data.name = selectors[i].nameInput.text;
                output.Add(selectors[i].data);
            }
        }

        MetagameManager.instance.playerData = output.ToArray();
        MetagameManager.instance.GoToGameplay();
        AudioManager.instance.Play(AudioManager.instance.gameStart);
    }

    public void PlayClick()
    {
        AudioManager.instance.Play(AudioManager.instance.generalMenuSelect);
    }

    //STRETCH GOAL: The ability to tab
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        Debug.Log("Tabbin");
    //        int currentFocus = 0;
    //        for (int i = 0; i < selectors.Length; i++)
    //        {
    //            if (selectors[i].nameInput.isFocused)
    //            {
    //                currentFocus = i;
    //                break;
    //            }
    //        }
    //        currentFocus++;
    //        if (currentFocus >= selectors.Length)
    //        {
    //            currentFocus = 0;
    //        }
    //    }
    //}
}
