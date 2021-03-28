using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButton : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
