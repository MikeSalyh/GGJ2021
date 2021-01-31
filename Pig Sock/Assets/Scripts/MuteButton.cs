using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MuteButton : MonoBehaviour
{
    public TextMeshProUGUI label;

    // Update is called once per frame
    void Update()
    {
        label.text = AudioManager.instance.isMuted ? "Unmute" : "Mute";
    }

    public void Click()
    {
        AudioManager.instance.Play(AudioManager.instance.generalMenuSelect);
        AudioManager.instance.ToggleMute();
    }
}
