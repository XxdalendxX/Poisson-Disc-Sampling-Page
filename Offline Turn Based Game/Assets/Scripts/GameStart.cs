using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    GameManager gameManager;
    Player player1;
    Player player2;

    GameBoard gameBoard;

    private void Awake()
    {
       
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameBoard = FindObjectOfType<GameBoard>();
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

        gameBoard.SetActive(true);
        gameManager.UpdateCellState();
        this.gameObject.SetActive(false);
    }

    public void ChooseO()
    {
        player2.IsFirst();
        player1.SetTurnState();
        player2.SetTurnState();
        player1.SetText();
        player2.SetText();

        gameBoard.SetActive(true);
        gameManager.UpdateCellState();
        this.gameObject.SetActive(false);
    }
}
