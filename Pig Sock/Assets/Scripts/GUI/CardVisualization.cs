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

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetToCard(Card c)
    {
        if (c == null) SetFaceDown();
        else image.sprite = GetSprite(c);
    }

    public void flipCard(Card c, float time = 0.5f)
    {
        StartCoroutine(cFlipCard(c, time));
    }
    IEnumerator cFlipCard(Card c, float time)
    {
        SetFaceDown();
        transform.eulerAngles = Vector3.zero;
        transform.DOLocalRotate(new Vector3(0, 90f, 0), time / 2);
        yield return new WaitForSeconds(time / 2);
        transform.eulerAngles = new Vector3(0, -90f, 0);
        SetToCard(c);
        transform.DOLocalRotate(Vector3.zero, time / 2);
    }

    public void FallDown(Card c, Transform refTransform, float time = 0.35f)
    {
        gameObject.SetActive(true);
        StartCoroutine(cFall(c, refTransform, time));
    }

    IEnumerator cFall(Card c, Transform refTransform, float time)
    {
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

    public void SetFaceDown()
    {
        image.sprite = backSprite;
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
