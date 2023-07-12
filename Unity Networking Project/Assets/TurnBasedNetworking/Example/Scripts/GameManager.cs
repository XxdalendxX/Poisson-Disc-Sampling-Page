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



    bool ongoingGame = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        cards = GetComponent<PlayingCards>();
        display = GetComponent<Display>();
        
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        cards = GetComponent<PlayingCards>();
        display = GetComponent<Display>();
        
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
        if (!ongoingGame && TB_players.Count >= 2)
        {
            TB_StartGame();
            ongoingGame = true;
            display.gameObject.SetActive(true);
            Debug.Log("Starting game");
        }
        else if (ongoingGame && TB_players.Count < 2)
        {
            ongoingGame = false;
            display.gameObject.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            cards.ResetCards();
        }
    }
}
