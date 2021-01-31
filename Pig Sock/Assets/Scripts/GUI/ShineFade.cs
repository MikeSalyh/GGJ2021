using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ShineFade : MonoBehaviour
{
    public float time = 0.5f;
    public Ease ease = Ease.OutSine;

    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<Image>().DOFade(1f, 0);
        GetComponent<Image>().DOFade(0f, time).SetEase(ease);
    }

}
