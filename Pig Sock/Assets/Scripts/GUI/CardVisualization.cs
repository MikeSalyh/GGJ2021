using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
class SuitSprites
{
    public Card.Suit suit;
    public Sprite[] sprites;
}

public class CardVisualization : MonoBehaviour
{
    private Image image;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite jokerSprite;
    [SerializeField] private SuitSprites[] sprites;
    public GameObject specialGraphics;
    private Vector3 defaultScale;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        defaultScale = transform.localScale;
    }

    public void SetToCard(Card c)
    {
        if (c == null)
            SetFaceDown();
        else
            GetComponent<Image>().sprite = GetSprite(c);
    }

    public void flipCard(Card c, bool showSpecialGraphicsAfterFlip = false, float time = 0.35f)
    {
        if(gameObject.activeSelf)
            StartCoroutine(cFlipCard(c, showSpecialGraphicsAfterFlip, time));
    }
    IEnumerator cFlipCard(Card c, bool showSpecialGraphicsAfterFlip, float time)
    {
        SetFaceDown(!showSpecialGraphicsAfterFlip);
        transform.localScale = defaultScale;
        transform.eulerAngles = Vector3.zero;
        transform.DOLocalRotate(new Vector3(0, -90f, 0), time / 2);
        transform.DOLocalMoveZ(5f, time / 2f);
        yield return new WaitForSeconds(time / 2);
        if (specialGraphics != null)
        {
            specialGraphics.gameObject.SetActive(showSpecialGraphicsAfterFlip);
        }
        transform.eulerAngles = new Vector3(0, 90f, 0);
        transform.DOLocalMoveZ(-5f, time / 2f);
        SetToCard(c);
        transform.DOLocalRotate(Vector3.zero, time / 2);
    }

    public void CollectAs(Card c, Transform refTransform, Transform targetTransform, float time = 0.75f)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(cCollectAs(c, refTransform, targetTransform, time));
    }
    IEnumerator cCollectAs(Card c, Transform refTransform, Transform targetTransform, float time)
    {
        SetToCard(c);
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = defaultScale;
        transform.position = refTransform.position;
        transform.DOKill();
        transform.DOLocalMoveZ(-100, time / 2f).SetEase(Ease.OutBack);
        transform.DOLocalRotate(new Vector3(-3f, 0f, 0f), time / 2f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(time / 2f);
        transform.DOMove(targetTransform.position, time / 2f).SetEase(Ease.InSine);
        transform.DOScale(Vector3.one * 0.05f, time/2f).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(time / 2f);
        gameObject.SetActive(false);
    }


    public void FallDown(Card c, Transform refTransform, float time = 0.35f)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(cFall(c, refTransform, time));
    }

    IEnumerator cFall(Card c, Transform refTransform, float time)
    {
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        transform.position = refTransform.position;
        SetToCard(c);
        transform.DOKill();
        transform.DOMoveY(refTransform.position.y + 30, time).SetEase(Ease.OutSine);
        transform.DOMoveX(refTransform.position.x + 75, time * 2);
        transform.DOLocalRotate(new Vector3(Random.Range(5f, 15f), Random.Range(45f, 60f), Random.Range(5f, 15f)), time);
        yield return new WaitForSeconds(time / 2f);
        transform.DOMoveY(refTransform.position.y - 120f, time).SetEase(Ease.InSine);

        yield break;
    }

    public void SetFaceDown(bool showSpecialGraphics = false)
    {
        image.sprite = backSprite;
        if (specialGraphics != null)
            specialGraphics.gameObject.SetActive(showSpecialGraphics);
    }

    public bool IsFaceDown
    {
        get { return image.sprite == backSprite; }
    }

    private Sprite GetSprite(Card c)
    {
        if (c.type == Card.CardType.Joker)
        {
            return jokerSprite;
        }
        else
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].suit == c.suit)
                {
                    return sprites[i].sprites[c.value - 1];
                }
            }
        }
        Debug.LogWarning("No sprite found!");
        return backSprite;
    }
}
