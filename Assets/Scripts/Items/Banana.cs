using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    [Tooltip("바나나의 사라지는 시간"), Range(0.5f, 5f)] public float disappearTime;

    private void Start()
    {
        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        float t = 0;
        float targetTime = disappearTime;
        while (t < targetTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
