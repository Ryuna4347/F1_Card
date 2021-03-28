using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RankingManager : MonoBehaviour
{
    private static RankingManager instance;
    public static RankingManager Instance { get { return instance; } }

    private GameManager gameManager;
    private List<GameObject> path = null;
    public GameObject waypointHolder;
    WaypointManager waypointManager;

    private Dictionary<int, List<GameObject>> currentPosition; //현재
    public int playerRank;

    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        gameManager = GetComponent<GameManager>();
        waypointManager = waypointHolder.GetComponent<WaypointManager>();
        path = waypointManager.wayPoints;
        currentPosition = new Dictionary<int, List<GameObject>>();
    }

    /// <summary>
    /// 플레이어의 앞에 몇 대의 차가 존재하는지 알아보고 랭크를 갱신한다.
    /// </summary>
    private void UpdateRanking()
    {
        int i = 0;
        GameObject player = GameManager.Instance.playerCar;

        currentPosition=currentPosition.OrderBy(x => x.Key).Reverse().ToDictionary(x=>x.Key, x=>x.Value);
        
        foreach (List<GameObject> cars in currentPosition.Values)
        {
            if(cars.Contains(player))
            {
                i += cars.IndexOf(player) + 1; //index는 0부터 시작하므로 1을 늘려야한다.
                break;
            }
            else
            {
                i += cars.Count;
            }
        }
        playerRank = i;
        HUDManager.Instance.UpdateRankUI(playerRank);
    }

    public void SetCurrentPosition(GameObject carObj, int lapNow, int checkPointNum, bool isPlayer = false)
    {
        int totalCheckPointNum = lapNow * (path.Count - 1) + checkPointNum;
        if (totalCheckPointNum > path.Count - 1) //처음이 1lap이어서 0이 아니라 좌측의 값이 시작이다. 시작지점 이전의 totalCheckPointNum은 존재하지 않으므로 패스
        {
            currentPosition[totalCheckPointNum - 1].Remove(carObj); //이전 체크포인트 관련 값 삭제
            if (currentPosition[totalCheckPointNum - 1].Count == 0) //빈 체크포인트 값은 딕셔너리 자리만 차지한다.
            {
                currentPosition.Remove(totalCheckPointNum-1);
            }
        }

        if (!currentPosition.ContainsKey(totalCheckPointNum))
        {
            currentPosition.Add(totalCheckPointNum, new List<GameObject>());
        }
        currentPosition[totalCheckPointNum].Add(carObj);

        if (isPlayer)
        {
            UpdateRanking();
        }
    }
}
