using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title : MonoBehaviour
{
    [SerializeField]
    Text[] text;
    [SerializeField]
    Image[] img;

    public void GameStartButton()
    {
        StartCoroutine(GameStart());
    }

    public IEnumerator GameStart()
    {
        for(int i=0;i<text.Length;i++)
        {
            text[i].DOFade(0,2);
        }

        for(int i=0;i<img.Length;i++)
        {
            img[i].DOFade(0, 2);
        }

        Camera.main.GetComponent<Camera>().DOOrthoSize(5, 2);

        yield return new WaitForSeconds(2);

        GameManager.Instance.StartGame();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
