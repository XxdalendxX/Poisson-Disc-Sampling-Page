using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginGame : MonoBehaviour
{
    [SerializeField] Player player1;
    [SerializeField] Player player2;
    public void ChooseX()
    {
        if (player1.playerNumber == "1")
        {
            player1.first = true;
            player1.isTurn = true;
            player2.first = false;
            player2.isTurn = false;
        }
        else 
        {
            player1.first = false;
            player1.isTurn = false;
            player2.first = true;
            player2.isTurn = true;
        }
        this.gameObject.SetActive(false);
    }

    public void ChooseO()
    {
        if (player1.playerNumber == "1")
        {
            player1.first = false;
            player1.isTurn = false;
            player2.first = true;
            player2.isTurn = true;
        }
        else
        {
            player1.first = true;
            player1.isTurn = true;
            player2.first = false;
            player2.isTurn = false;
        }

        this.gameObject.SetActive(false);
    }
}
