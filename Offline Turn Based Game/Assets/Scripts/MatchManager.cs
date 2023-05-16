using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    ScreenManager screenManager;
    GameManager gameManager;

    private void Awake()
    {
        screenManager = FindObjectOfType<ScreenManager>();
        gameManager = FindObjectOfType<GameManager>();
        //gameManager.UpdateCellState();
    }




}
