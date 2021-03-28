using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private static HUDManager instance;
    public static HUDManager Instance { get { return instance; } }

    public GameObject mainUI;
    public GameObject ingameUI;

    public Transform startSignParent;
    [SerializeField]private List<GameObject> startSignList;

    public Transform LapInfoParent;
    [SerializeField] private Text lapNowText;
    [SerializeField] private Text lapTotalText;

    public Text velocityText;

    public List<Sprite> rankSprites;
    public Image rankUI;
    public Image fadeout;
    [SerializeField] private float fadeoutTime = 2;

    public GameObject winUI;
    public GameObject loseUI;
    public GameObject confettiGroup;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        startSignList = new List<GameObject>();
        foreach (Transform startSign in startSignParent)
        {
            startSignList.Add(startSign.gameObject);
        }

        lapNowText = LapInfoParent.Find("LapNow").GetComponent<Text>();
        lapTotalText = LapInfoParent.Find("LapTotal").GetComponent<Text>();
    }

    public IEnumerator StartRaceCountDown(Action onComplete)
    {
        int bgmIdx = 0;

        mainUI.SetActive(false);
        ingameUI.SetActive(true);

        yield return new WaitForSeconds(2f);

        startSignParent.gameObject.SetActive(true);
        foreach (GameObject startSign in startSignList)
        {
            startSign.SetActive(true);
            yield return new WaitForSeconds(1.2f);
            if (bgmIdx < 2)
            {
                SoundManager.Instance.PlaySound("Start_Red");
                bgmIdx++;
            }
            else if(bgmIdx == 2)
            {
                SoundManager.Instance.PlaySound("Start_Green");
                bgmIdx++;
            }
        }
        
        if (onComplete != null)
        {
            onComplete();
        }
        yield return new WaitForSeconds(0.5f);

        startSignParent.gameObject.SetActive(false);
    }

    public void GameOverWin()
    {
        ingameUI.SetActive(false);
        GameObject.Instantiate<GameObject>(confettiGroup, new Vector3(1.5f, 0.5f, -5f), Quaternion.Euler(0, 0, 0));
        winUI.SetActive(true);

        StartCoroutine(FadeOut());
    }
    public void GameOverLose()
    {
        ingameUI.SetActive(false);
        loseUI.SetActive(true);
        loseUI.GetComponent<Image>().sprite = rankSprites[RankingManager.Instance.playerRank - 1];
        
        StartCoroutine(FadeOut());
    }

    public void UpdateLapTotalUI(int lapTotal)
    {
        lapTotalText.text = lapTotal.ToString();
    }

    public void UpdatePlayerLapUI(int lapNow)
    {
        lapNowText.text = lapNow.ToString();
    }

    public void UpdateRankUI(int rank)
    {
        rankUI.sprite = rankSprites[rank - 1];
    }

    public void UpdateVelocityUI(float velocity)
    {
        velocityText.text = ((int)velocity).ToString();
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f);

        float t = 0;
        fadeout.gameObject.SetActive(true);
        Color color = fadeout.color;
        while (t < fadeoutTime)
        {
            t += Time.deltaTime;
            color.a += 1 / fadeoutTime * Time.deltaTime;
            fadeout.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
