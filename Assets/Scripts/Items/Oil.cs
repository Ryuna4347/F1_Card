using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [Tooltip("일반 오일의 사라지는 시간"), Range(0.5f, 5f)] public float normalTime;
    [Tooltip("강화 오일의 사라지는 시간"), Range(0.5f, 5f)] public float reinforceTime;

    private void Start()
    {
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        float t = 0;
        float targetTime = normalTime;
        while(t < targetTime)
        {
            if (targetTime != reinforceTime && transform.localScale.x > 1) //instantiate 시 동시에 scale 변화가 안되서 우선 normalTime으로 실행하고 중간에 강화 체크하면서 변경
                targetTime = reinforceTime;
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
