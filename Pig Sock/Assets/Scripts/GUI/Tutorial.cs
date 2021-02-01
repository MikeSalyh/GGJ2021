using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public GameObject slide1, slide2;

    private void OnEnable()
    {
        slide1.gameObject.SetActive(true);
        slide2.gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().DOFade(1f, 0.25f);
    }

    public void GoToSlide2()
    {
        slide1.gameObject.SetActive(false);
        slide2.gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
