﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public void AfterExplode()
    {
        gameObject.SetActive(false);
    }
}
