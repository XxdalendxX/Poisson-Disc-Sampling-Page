using System.Collections;
using System.Collections.Generic;
using TMPro;
using TurnBasedSystem;
using UnityEngine;

public class Display : MonoBehaviour
{
    public TMP_Text turnText;
    public TMP_Text sum1Text;
    public TMP_Text sum2Text;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
    }

    public void ChangeTurnText(int playerNo)
    {
        turnText.text = "Player " + playerNo + "'s Turn!";
    }

    public void ChangePlayer1SumText(int sum)
    {
        turnText.text = "Player " + sum + "'s Turn!";
    }

    public void ChangePlayer2SumText(int sum)
    {
        turnText.text = "Player " + sum + "'s Turn!";
    }
}
