using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public static CardController instance = null;


    [SerializeField]
    private GameObject CardPrefab;

    [SerializeField]
    private Transform CardInstantiatePos;

    [SerializeField]
    private Transform CardParent;

    [SerializeField]GameObject[] card;

    public int CardCount;

    private int upgradeCard;

    [SerializeField]
    Sprite[] cardSprites;
    [SerializeField]
    Sprite[] cardUpSprites;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        card = new GameObject[10];
    }

    private void Start()
    {
        //StartCoroutine(DrawCardCoroutine());
    }

    private IEnumerator DrawCardCoroutine()
    {
        while (!GameManager.Instance.isGameInPlay)
            yield return null;
        while(GameManager.Instance.isGameInPlay)
        {
            DrawCard();
            yield return new WaitForSeconds(2.0f);
        }
    }

    public void DrawCard()
    {

        if (CardCount < 10)
        {
            card[CardCount] = Instantiate(CardPrefab, CardInstantiatePos.position, Quaternion.Euler(new Vector3(0, 180, 0)), CardParent);
            card[CardCount].GetComponent<Card>().CardMoving(CardCount);

            int random = Random.Range(0, 100);
            int ranType = 0;

            if (random < 19)
                ranType = 0;
            else if (random < 29)
                ranType = 1;
            else if (random < 48)
                ranType = 2;
            else if (random < 67)
                ranType = 3;
            else if (random < 72)
                ranType = 4;
            else if (random < 91)
                ranType = 5;
            else
                ranType = 6;


            card[CardCount].GetComponent<Card>().cardType = ranType;
            card[CardCount].GetComponent<Image>().sprite = cardSprites[card[CardCount].GetComponent<Card>().cardType];


            CardCount++;

            if (CardCount >= 2)
            {
                for (int i = 2; i < CardCount; i++)
                {
                    if (card[i-2].GetComponent<Card>().isUpgrade) continue;

                    if ((card[i].GetComponent<Card>().cardType == card[i - 1].GetComponent<Card>().cardType && card[i].GetComponent<Card>().cardType == card[i - 2].GetComponent<Card>().cardType))
                    {

                        upgradeCard = i;
                        StartCoroutine(CardUpgrade());
                    }
                }
            }
        }
    }

    public void DeleteCard(int cardNum)
    {
        for (int i = cardNum; i < CardCount - 1; i++)
        {
            card[i] = card[i + 1];
            card[i].GetComponent<Card>().CardMoving(i);
        }
        CardCount--;

        if (CardCount >= 2)
        {
            for (int i = 2; i < CardCount; i++)
            {
                if (card[i].GetComponent<Card>().cardType == card[i - 1].GetComponent<Card>().cardType && card[i].GetComponent<Card>().cardType == card[i - 2].GetComponent<Card>().cardType)
                {
                    upgradeCard = i;
                    StartCoroutine(CardUpgrade());
                    break;
                }
            }
        }
    }

    IEnumerator CardUpgrade()
    {

        card[upgradeCard].GetComponent<Card>().CardUpgrade(card[upgradeCard - 2]);
        card[upgradeCard-1].GetComponent<Card>().CardUpgrade(card[upgradeCard - 2]);
        card[upgradeCard-2].GetComponent<Card>().isUpgrade = true;

        SoundManager.Instance.PlaySound("Card_Upgrade");
        yield return new WaitForSeconds(.3f);

        for (int i = upgradeCard - 1; i < CardCount - 2; i++)
        {
            card[i] = card[i + 2];
            card[i].GetComponent<Card>().CardMoving(i);
        }

        card[upgradeCard - 2].GetComponent<Image>().sprite = cardUpSprites[card[upgradeCard - 2].GetComponent<Card>().cardType];

        CardCount -= 2;
    }

}
