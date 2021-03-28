using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [Tooltip("차량 이동 경로가 속해있는 부모 루트 오브젝트")] public GameObject routePointParent;
    private List<Transform> routePointList;

    public bool isGameInPlay = false;
    [SerializeField] private int lapTotal = 4;

    public GameObject playerCar;
    public Image fadeout;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        playerCar = GameObject.Find("PlayerCar");
    }

    public void StartGame()
    {
        HUDManager.Instance.UpdateLapTotalUI(lapTotal);
        StartCoroutine(HUDManager.Instance.StartRaceCountDown(() =>
        {
                //when the count down finishes, flag the game as 'in play'
                isGameInPlay = true;
        }));
    }

    public bool CheckComplete(int lap)
    {
        if (lap > lapTotal)
        {
            isGameInPlay = false;
            if (RankingManager.Instance.playerRank == 1)
            {
                HUDManager.Instance.GameOverWin();
                SoundManager.Instance.PlayOnceBGM("Win");
            }
            else
            {
                HUDManager.Instance.GameOverLose();
                SoundManager.Instance.PlayOnceBGM("Retire");
            }
            return true;
        }
        return false;
    }

    public void GameOver()
    {
        isGameInPlay = false;
        HUDManager.Instance.GameOverLose();
        SoundManager.Instance.PlayBGM("Retire");
    }
}
