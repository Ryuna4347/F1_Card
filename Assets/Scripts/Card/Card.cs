using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public Vector3 CardPos;

    Button m_Btn;

    int cardNumber;

    public int cardType = 1;
    public bool isUpgrade;
    void Start()
    {
        CardRotate();
        m_Btn = this.transform.GetComponent<Button>();
        m_Btn.onClick.AddListener(ReturnCardNum);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, CardPos, Time.deltaTime * 10);
    }

    public void CardMoving(int cardNum)
    {
        CardPos = new Vector3((cardNum * 190)+15, 20);

        cardNumber = cardNum;
    }

    public void CardRotate()
    {
        transform.DORotate(new Vector3(0,0,0),1f);
    }

    public void CardUse()
    {
        transform.DOLocalMoveY(-200,.3f);

        if(cardType == 0)
        {
            if (!isUpgrade)
            {
                GameManager.Instance.playerCar.GetComponent<PathFinder>().StartCoroutine("CountBoost", 2.5f);
                GameManager.Instance.playerCar.GetComponent<PathFinder>().StartCoroutine("CountBoostMax", 2.5f);
            }
            if (isUpgrade)
                GameManager.Instance.playerCar.GetComponent<PathFinder>().StartCoroutine("CountInvinsible", 4f);
        }
        else if(cardType == 1)
        {
            if(!isUpgrade)
                GameManager.Instance.playerCar.GetComponent<PathFinder>().ChangeAcceleration(0.65f); 
            else
                GameManager.Instance.playerCar.GetComponent<PathFinder>().ChangeAcceleration(2.3f);
        }
        else if (cardType == 2)
        {
            if (!isUpgrade)
                GameManager.Instance.playerCar.GetComponent<PathFinder>().ChangeMaxVelocity(0.65f);
            else
                GameManager.Instance.playerCar.GetComponent<PathFinder>().ChangeMaxVelocity(2.3f);
        }
        else if (cardType == 3)
        {
            GameManager.Instance.playerCar.GetComponent<PathFinder>().InstallBanana(isUpgrade);
        }
        else if (cardType == 4)
        {
            GameManager.Instance.playerCar.GetComponent<PathFinder>().Shoot(isUpgrade);
        }
        else if (cardType == 5)
        {
            GameManager.Instance.playerCar.GetComponent<PathFinder>().InstallOil(isUpgrade);
        }
        else if (cardType == 6)
        {
            GameManager.Instance.playerCar.GetComponent<PathFinder>().StartCoroutine("CountGiant", 3f);
            if (isUpgrade)
                GameManager.Instance.playerCar.GetComponent<PathFinder>().StartCoroutine("CountInvinsible", 4f);
        }
        SoundManager.Instance.PlaySound("Use_Card");

        Destroy(gameObject, .35f);
    }

    public void ReturnCardNum()
    {
        CardController.instance.DeleteCard(cardNumber);
    }

    public void CardUpgrade(GameObject upgradeCard)
    {
        transform.DOMove(upgradeCard.transform.position, .3f);
        Destroy(gameObject, .35f);
    }
}
