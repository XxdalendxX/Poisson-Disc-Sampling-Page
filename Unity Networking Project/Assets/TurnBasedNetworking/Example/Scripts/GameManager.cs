using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TurnBasedSystem;
using UnityEditor.Build;

public class GameManager : TBManager
{
    [HideInInspector]
     public PlayingCards cards;
    [HideInInspector]
    public Display display;

    public override void OnStartClient()
    {
        base.OnStartClient();

        cards = GetComponent<PlayingCards>();
        display = GetComponent<Display>();
        TB_StartGame();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        cards = GetComponent<PlayingCards>();
        display = GetComponent<Display>();
        TB_StartGame();
    }

    public override void TB_StartGame()
    {
        base.TB_StartGame();

        display.ChangeTurnText(TB_GetPlayerTurn());
        display.ChangePlayer1SumText(cards.GetP1Sum());
        display.ChangePlayer2SumText(cards.GetP2Sum());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            cards.ResetCards();
        }
    }
}
