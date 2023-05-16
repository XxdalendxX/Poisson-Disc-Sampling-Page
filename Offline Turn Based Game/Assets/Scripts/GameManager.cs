using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player[] players;
    GameBoard board;
    [SerializeField] int[] cellState = new int[9];

    private void Awake()
    {
        players = GetComponents<Player>();
        board = FindObjectOfType<GameBoard>();
    }

    private void Start()
    {
        
    }

    public Player GetPlayers(int i)
    {
        return players[i];
    }

    public int CheckTurnState()
    {
        if (players[0].GetTurnState() == true)
            return 1;
        else
            return 2;
    }

    public void UpdateCellState()
    {
        for (int i = 0; i < 9; i++)
        {
            Cell cell = board.GetCell(i);
            cellState[i] = cell.CellState();
        }
    }
}
