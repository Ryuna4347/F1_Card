using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [Range(0f, 2f)]
    [Tooltip("목표 주위로 랜덤한 도착점을 만들 수 있도록 한다.")]public float randomPathRange = 0.5f;

    [SerializeField]
    GameObject waypointHolder = null;

    CarMovement carController = null;

    List<GameObject> path = null;
    WaypointManager waypointManager;
    [SerializeField]private int currentWaypointIndex;
    [SerializeField]private Transform currentWaypoint;
    [SerializeField]private Vector3 currentTargetPos;
    private bool needNextTarget = true;
    int totalWayPoints;

    private int lapNow = 0;

    public bool isPlayer = false;
    private bool passedStartLine = false; //시작 전 체크포인트 접촉 방지
    [SerializeField]private bool isAccelPressed = false;
    [SerializeField] private bool isBoostUsed = false;
    [SerializeField] private bool isBoostMaxUsed = false;
    private Transform currentBrakeWaypoint;
    private bool isBraking;
    [SerializeField]private string turnDirection;

    public GameObject missilePrefab;
    public GameObject oilPrefab;
    public GameObject bananaPrefab;

    Transform player;

    public List<AudioClip> soundList;
    private AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarMovement>();

        if (waypointHolder == null)
        {
            Debug.LogWarning("경로를 찾을 수 없습니다.");
            return;
        }
        waypointManager = waypointHolder.transform.GetChild(Random.Range(0,waypointHolder.transform.childCount)).GetComponent<WaypointManager>();
        path = waypointManager.wayPoints;
        currentWaypointIndex = 0;
        totalWayPoints = path.Count;

        currentWaypoint = path[0].transform; //처음 목표
        currentTargetPos = currentWaypoint.position;
        currentTargetPos += currentWaypoint.right * Random.Range(-randomPathRange, randomPathRange);

        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameInPlay) //게임 시작 전 혹은 종료 후 움직임 봉쇄
        {
            isAccelPressed = false;
            return ;
        }
        if (carController.isUnavailable) return;

        CalculateDirection();

        float dist = Vector2.Distance(transform.position, currentTargetPos);

        isAccelPressed = true;
        if (dist < 3)
        {
            if(!isBraking) //1회용 브레이킹(각 점에서 계속 브레이크가 걸리는 것을 방지하기 위해 1번만 브레이크하도록 한다.)
            {
                Waypoint targetNode = currentWaypoint.gameObject.GetComponent<Waypoint>();

                if(currentBrakeWaypoint != currentWaypoint) //현재 도착하고자 하는 곳과 최근 브레이크 한 곳이 달라야한다. 같으면 중복이므로 브레이크를 밟지 않는다.
                {
                    currentBrakeWaypoint = currentWaypoint;
                    StartCoroutine(CountBrake(targetNode.brakeTime));
                }
            }

            currentWaypointIndex++;
            if (currentWaypointIndex >= totalWayPoints)
            {
                currentWaypointIndex = 0;
            }
            currentWaypoint = path[currentWaypointIndex].transform;

            currentTargetPos = currentWaypoint.position;
            currentTargetPos += currentWaypoint.right * Random.Range(-randomPathRange, randomPathRange);
        }

        if (isBraking) //브레이크가 필요한 경우 엑셀 중지
        {
            isAccelPressed = false;
        }
    }

    private void FixedUpdate()
    {
        if (!carController.isUnavailable)
        {
            carController.UpdateMovement(turnDirection, 0, isAccelPressed, isBoostUsed, isBoostMaxUsed);
        }
    }

    #region 차량 조작 관련

    /// <summary>
    /// 영구 최대 속력 변화
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMaxVelocity(float value)
    {
        PlaySound("Speed_Up");
        carController.maxspeed += value;
    }
    /// <summary>
    /// 영구 가속 변화
    /// </summary>
    /// <param name="value"></param>
    public void ChangeAcceleration(float value)
    {
        PlaySound("Speed_Up");
        carController.accel += value;
    }

    private IEnumerator CountBrake(float brakeTime)
    {
        float time = 0;

        isBraking = true;
        while (time < brakeTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        isBraking = false;
        isAccelPressed = true;
    }

    /// <summary>
    /// 최대 가속도 증가
    /// </summary>
    /// <param name="boostTime"></param>
    /// <returns></returns>
    private IEnumerator CountBoost(float boostTime)
    {
        float time = 0;

        PlaySound("Booster");
        isBoostUsed = true;
        while (time < boostTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        isBoostUsed = false;
    }

    /// <summary>
    /// 최대 속도 증가
    /// </summary>kairakuno horikku
    /// <param name="boostTime"></param>
    /// <returns></returns>
    private IEnumerator CountBoostMax(float boostTime)
    {
        float time = 0;

        PlaySound("Booster");
        isBoostMaxUsed = true;
        while (time < boostTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        isBoostMaxUsed = false;
    }

    /// <summary>
    /// 일정시간 무적
    /// </summary>
    /// <param name="invinTime"></param>
    /// <returns></returns>
    private IEnumerator CountInvinsible(float invinTime)
    {
        float time = 0;

        carController.isInvincible = true;
        while (time < invinTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        carController.isInvincible = false;
    }

    private IEnumerator CountGiant(float giantTime)
    {
        float time = 0;

        PlaySound("Giant");
        gameObject.transform.localScale = new Vector3(2, 2, 1);
        while (time < giantTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator CountBanana(float bananaTime)
    {
        float time = 0;

        carController.isCollidedBanana = true;
        GetComponent<Animator>().SetTrigger("Banana");
        while (time < bananaTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        carController.isCollidedBanana = false;
    }
    #endregion

    #region 설치/생성형 아이템 관련

    public void Shoot(bool reinforce = true)
    {
        if (reinforce)
        {
            GameObject missile = GameObject.Instantiate<GameObject>(missilePrefab, transform.position + (transform.up + transform.right * 0.5f).normalized, Quaternion.Euler(0,0,transform.rotation.eulerAngles.z - 20));
            Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            missile = GameObject.Instantiate<GameObject>(missilePrefab, transform.position + transform.up, transform.rotation);
            Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            missile = GameObject.Instantiate<GameObject>(missilePrefab, transform.position + (transform.up - transform.right * 0.5f).normalized, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 20));
            Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }
        else
        {
            GameObject missile = GameObject.Instantiate<GameObject>(missilePrefab, transform.position + transform.up, transform.rotation);
            Physics2D.IgnoreCollision(missile.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }
    }
    public void InstallOil(bool reinforce = true)
    {
        if (reinforce)
        {
            GameObject oil = GameObject.Instantiate<GameObject>(oilPrefab, transform.position - transform.up, transform.rotation);
            oil.transform.localScale = new Vector2(1.75f, 1.75f);
        }
        else
        {
            GameObject oil = GameObject.Instantiate<GameObject>(oilPrefab, transform.position - transform.up, transform.rotation);
        }
    }
    public void InstallBanana(bool reinforce = true)
    {
        if (reinforce)
        {
            GameObject banana = GameObject.Instantiate<GameObject>(bananaPrefab, transform.position - (transform.up + transform.right).normalized, transform.rotation);
            Physics2D.IgnoreCollision(banana.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            banana = GameObject.Instantiate<GameObject>(bananaPrefab, transform.position - transform.up, transform.rotation);
            Physics2D.IgnoreCollision(banana.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            banana = GameObject.Instantiate<GameObject>(bananaPrefab, transform.position - (transform.up - transform.right).normalized, transform.rotation);
            Physics2D.IgnoreCollision(banana.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }
        else
        {
            GameObject banana = GameObject.Instantiate<GameObject>(bananaPrefab, transform.position - transform.up, transform.rotation);
            Physics2D.IgnoreCollision(banana.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
        }
    }

    #endregion

    private void CalculateDirection()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(currentTargetPos); //자신 기준 다음 목표로의 로컬 좌표 벡터가 구해진다.

        turnDirection = null;

        //determine the turn direction
        if (relativeVector.x > 0.5f)
        {
            turnDirection = "Right";
        }
        else if (relativeVector.x < -0.5f)
        {
            turnDirection = "Left";
        }
        else if (relativeVector.y < 0)
        {
            turnDirection = "Left";
            StartCoroutine(CountBrake(0.1f));
        }
    }

    public void PlaySound(string name)
    {
        AudioClip selected = soundList.Find(x => x.name.Contains(name));
        sound.clip = selected;
        sound.Play();
    }
    #region ai 랜덤 카드 생성 관련

    private void DrawRandomCard()
    {
        if (isPlayer) return;

        int randomCard = Random.Range(0, 100);
        bool isUpgrade = (Random.Range(0, 100) < 5);

        if (randomCard < 24)
        {
            StartCoroutine("CountBoost", 2f);
            StartCoroutine("CountBoostMax", 2f);
            if (isUpgrade)
                StartCoroutine("CountInvinsible", 2f);
        }
        else if (randomCard < 48)
        {
            InstallBanana(isUpgrade);
        }
        else if (randomCard < 52)
        {
            Shoot(isUpgrade);
        }
        else if (randomCard < 76)
        {
            InstallOil(isUpgrade);
        }
        else
        {
            StartCoroutine("CountGiant", 1f);
            if (isUpgrade)
                StartCoroutine("CountInvinsible", 1f);
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Vector3 collidedObjVector = transform.InverseTransformPoint(collision.gameObject.transform.position); //충돌한 오브젝트로의 로컬 좌표 벡터가 구해진다.

            if (collidedObjVector.y > 0 && !isBraking) //자신이 뒤에서 박은 경우 일시적으로 속도 감소(연속 호출 방지를 위해 현재 브레이크 중인 경우 실행하지 않는다.)
            {
                StartCoroutine(CountBrake(0.05f));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (passedStartLine && collision.gameObject.CompareTag("CheckPoint"))
        {
            RankingManager.Instance.SetCurrentPosition(gameObject, lapNow, collision.gameObject.transform.GetSiblingIndex(), isPlayer);
        }
        else if(collision.gameObject.CompareTag("StartLine"))
        {
            passedStartLine = true;
            lapNow++;
            if (lapNow > 1 && isPlayer)
                SoundManager.Instance.PlaySound("Lap_Complete");
            if(!GameManager.Instance.CheckComplete(lapNow))
            {
                HUDManager.Instance.UpdatePlayerLapUI(lapNow);
            }
        }
        else if(collision.gameObject.CompareTag("Banana"))
        {
            PlaySound("Banana");
            StartCoroutine(CountBanana(2.0f));
            collision.gameObject.SetActive(false);
        }
        else if(passedStartLine && collision.gameObject.CompareTag("DrawLine"))
        {
            if (isPlayer)
                CardController.instance.DrawCard();
            else
            {
                bool drawCard = Random.Range(0, 100) < 15;
                if (drawCard)
                    DrawRandomCard();
            }
        }
    }
}
