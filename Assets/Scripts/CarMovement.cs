using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    //#############################################################
    //  에디터에서 조절 가능한 변수들
    //#############################################################

    [Range(1f, 30f)]
    [Tooltip("차량 가속도")]public float accel = 15;

    [Range(1.01f, 3f)]
    [Tooltip("부스트 시 가속도 증가비율")]public float boostAccelRatio = 1.5f;

    [Range(1.0f, 10f)]
    [Tooltip("최대 속도")]public float maxspeed = 6;

    [Range(1.01f, 2f)]
    [Tooltip("부스트 시 가속도 증가비율")] public float boostMaxSpeedRatio = 1.5f;

    [Range(1, 4)]
    [Tooltip("핸들 조작 속도")]public float turnpower = 2;

    [Range(0.5f, 3)]
    [Tooltip("마찰력")]public float friction = 1.5f;
    
    [SerializeField]
    internal bool isUnavailable = false; // 서 있는가?
    private bool isOnGrass = false; //풀 위에 있으면 마찰력 증가
    [SerializeField]private bool isOnOil = false; //기름 위에 있으면 최대 속력 감소

    //#############################################################
    //  내부 전용 변수
    //#############################################################
    float currentTurnPower;
    float currentAccel;
    public Vector2 curspeed;
    Rigidbody2D rb;
    private bool isTurning;
    private bool isTurningL;
    private bool isTurningR;
    private bool isAccelerating;
    [SerializeField]private bool isBoosted; //가속력 증가
    [SerializeField]private bool isBoostedMax; //최대속도 증가
    internal bool isAIControlled; //컴퓨터가 조종하는가?(카드 자동 사용에 사용될 수 있음)

    public bool isInvincible = false;
    public bool isCollidedBanana = false;

    private float currentSpeedPerc;

    // Start is called before the first frame update
    void Start()
    {
        currentTurnPower = turnpower;
        currentAccel = accel;

        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!isUnavailable)
        {
            if (isAccelerating)
            {
                rb.AddForce(transform.up * accel * (isBoosted ? boostAccelRatio : 1));
                rb.drag = friction * (isOnGrass? 1.25f : 1f); //풀 위에서는 마찰력이 더 심하다.
            }
            else // 가속하지 않을 때의 마찰을 늘려서 속도가 빨리 줄어들게 한다.
            {
                rb.drag = friction * 2;
            }

            float maxSpeedNow = maxspeed * (isBoostedMax ? boostMaxSpeedRatio : 1) * ((isCollidedBanana || isOnOil) ? 0.5f : 1f);

            currentSpeedPerc = curspeed.magnitude / maxSpeedNow;

            curspeed = (Vector2)rb.velocity; //속도 제한
            if (curspeed.magnitude > maxSpeedNow)
                rb.velocity = curspeed.normalized * maxSpeedNow;

            if(gameObject == GameManager.Instance.playerCar)
                HUDManager.Instance.UpdateVelocityUI(rb.velocity.magnitude * 20);
        }
    }

    /// <summary>
    /// PathFinder에서 방향, 가속, 브레이크에 대한 정보를 받아서 적용한다.
    /// </summary>
    public void UpdateMovement(string turnDirection, float axisX, bool isAccelPressed, bool isBoostUsed, bool isBoostMaxUsed)
    {
        if (!isCollidedBanana)
        {
            isAccelerating = isAccelPressed;
            isBoosted = isBoostUsed;
            isBoostedMax = isBoostMaxUsed;

            RotateTransform(turnDirection);
        }
    }

    private void RotateTransform(string turnDirection)
    {
        float turnCalculation = currentSpeedPerc * currentTurnPower + currentSpeedPerc;

        if (turnDirection == "Left")
        {
            transform.Rotate(Vector3.forward * turnCalculation);
        }
        else if (turnDirection == "Right")
        {
            transform.Rotate(Vector3.forward * -turnCalculation);
        }
    }

    public IEnumerator GetDamaged()
    {
        if (isInvincible) yield break;

        rb.velocity = new Vector2(0, 0);
        GetComponent<Collider2D>().enabled = false;
        rb.isKinematic = false;
        isUnavailable = true;

        transform.GetChild(1).gameObject.SetActive(true); //애니메이션 재생

        SpriteRenderer sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color c = sprite.color;

        float t = 0;
        float disappearTime = 2.0f;
        while(t < disappearTime)
        {
            t += Time.deltaTime;
            c.a -= 1 / disappearTime * Time.deltaTime;
            sprite.color = c;
            yield return null;
        }

        if(GetComponent<PathFinder>().isPlayer)
        {
            Camera.main.transform.SetParent(null);
            GameManager.Instance.GameOver();
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Grass"))
        {
            isOnGrass = true;
        }
        else if (collision.gameObject.CompareTag("Oil"))
        {
            isOnOil = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Grass"))
        {
            isOnGrass = false;
        }
        else if (collision.gameObject.CompareTag("Oil"))
        {
            isOnOil = false;
        }
    }
}