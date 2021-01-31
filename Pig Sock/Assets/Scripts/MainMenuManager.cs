using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject tutorial;
    public Button tutorialBtn, startButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowTutorial()
    {
        tutorial.SetActive(true);
    }

    public void StartGame()
    {
        MetagameManager.instance.GoToLobby();
    }
}
