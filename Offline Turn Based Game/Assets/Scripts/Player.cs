using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool first;
    bool isTurn;
    string text;

    //Setters
    public void SetText()
    {
        if (first)
        {
            text = "X";
        }
        else
        {
            text = "O";
        }
    }
    public void SetTurnState()
    {
        if (isTurn == false)
            isTurn = true;
        else
            isTurn = false;
    }
    public void IsFirst()
    {
        first = true;
    }

    public string GetText()
    {
        return text;
    }
    public bool GetFirst()
    {
        return first;
    }
    public bool GetTurnState()
    {
        return isTurn;
    }
}
