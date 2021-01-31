using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource sfxSrc;
    public AudioSource musicSrc;

    [Header("Music")]
    public AudioClip music1;
    [Range(0,1)]
    public float musicVolume = 0.5f;

    [Header("SFX")]
    public AudioClip[] sockMeBtn;
    public AudioClip[] takeSock;
    public AudioClip[] generalMenuSelect;
    public AudioClip[] gameStart;
    public AudioClip[] gameEnd;
    public AudioClip[] peek;
    public AudioClip[] bust;
    public AudioClip[] jackpot;
    public AudioClip[] deckExit, deckEnter;

    [HideInInspector] public bool isMuted;
    public void ToggleMute()
    {
        isMuted = !isMuted;
        sfxSrc.volume = isMuted ? 0 : 1;
        musicSrc.volume = isMuted ? 0 : 1;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public bool Play(AudioClip[] sounds)
    {
        if (sounds != null && sounds.Length > 0)
        {
            AudioClip randClip = sounds[Random.Range(0, sounds.Length)];
            if(randClip != null)
                sfxSrc.PlayOneShot(randClip);
            return randClip != null;
        }
        else
        {
            return false;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        //WIP - doesn't have xfading

        musicSrc.volume = musicVolume;
        if (clip != null && musicSrc.clip != clip)
        {
            musicSrc.clip = clip;
            musicSrc.Play();
        }
    }
}
