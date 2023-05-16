using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    GameManager gameManager;
    Player player1;
    Player player2;

    ScreenManager screenManager;

    private void Awake()
    {
      screenManager = FindObjectOfType<ScreenManager>();
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        player1 = gameManager.GetPlayers(0);
        player2 = gameManager.GetPlayers(1);
    }

    public void ChooseX()
    {

        player1.IsFirst();
        player1.SetTurnState();
        player2.SetTurnState();
        player1.SetText();
        player2.SetText();

        //gameManager.UpdateCellState();
        screenManager.UpdateScreenState();
    }

    public void ChooseO()
    {
        player2.IsFirst();
        player1.SetTurnState();
        player2.SetTurnState();
        player1.SetText();
        player2.SetText();

        //gameManager.UpdateCellState();
        screenManager.UpdateScreenState();
    }
}
