﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MetagameManager : MonoBehaviour
{
    public static MetagameManager instance;
    public CanvasGroup dimPlane;
    public PlayerData[] playerData;

    public enum GameState
    {
        Init,
        Menu,
        Lobby,
        Gameplay,
        Finale,
        Loading
    }

    private GameState currentState;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (dimPlane.alpha > 0f)
                dimPlane.alpha = 0f;

            AudioManager.instance.PlayMusic(AudioManager.instance.music1); //WIP, if 2 songs
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void GoToGameplay()
    {
        if (currentState != GameState.Loading)
            StartCoroutine(GoToGameplayCoroutine());
    }

    private IEnumerator GoToGameplayCoroutine()
    {
        SwitchState(GameState.Loading);
        dimPlane.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Gameplay");
        SwitchState(GameState.Gameplay);
        dimPlane.DOFade(0f, 0.5f);
    }

    public void GoToMenu()
    {
        if (currentState != GameState.Loading)
            StartCoroutine(GoToMenuCoroutine());
    }

    private IEnumerator GoToMenuCoroutine()
    {
        SwitchState(GameState.Loading);
        dimPlane.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Menu");
        SwitchState(GameState.Menu);
        dimPlane.DOFade(0f, 0.5f);
    }

    public void GoToLobby()
    {
        if (currentState != GameState.Loading)
            StartCoroutine(GoToLobbyCoroutine());
    }

    private IEnumerator GoToLobbyCoroutine()
    {
        SwitchState(GameState.Loading);
        dimPlane.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Lobby");
        SwitchState(GameState.Lobby);
        dimPlane.DOFade(0f, 0.5f);
    }


    public void GoToFinale()
    {
        if (currentState != GameState.Loading)
            StartCoroutine(GoToFinaleCoroutine());
    }

    private IEnumerator GoToFinaleCoroutine()
    {
        SwitchState(GameState.Loading);
        dimPlane.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Finale");
        SwitchState(GameState.Finale);
        dimPlane.DOFade(0f, 0.5f);
    }


    private void SwitchState(GameState value)
    {
        if (currentState == value)  //If it's already in this state, do nothing.
            return;

        currentState = value;
        switch (value)
        {
            case GameState.Menu:
                break;
            case GameState.Gameplay:
                break;
            case GameState.Lobby:
                playerData = new PlayerData[0];
                break;
            case GameState.Finale:
                break;
        }
    }
}
