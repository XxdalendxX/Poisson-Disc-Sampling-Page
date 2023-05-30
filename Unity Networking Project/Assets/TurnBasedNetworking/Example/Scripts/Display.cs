using System.Collections;
using System.Collections.Generic;
using TMPro;
using TurnBasedSystem;
using UnityEngine;

public class Display : MonoBehaviour
{
    [SerializeField]
    GameManager manager;

    public TMP_Text turnText;
    public TMP_Text sum1Text;
    public TMP_Text sum2Text;

    // Update is called once per frame
    void Update()
    {
          
         
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            manager.cards.ResetCards();
        }
        
       turnText.text = "Player " + manager.TB_GetPlayerTurn() + " Turn!";

        sum1Text.text = manager.cards.GetP1Sum().ToString();
        sum2Text.text = manager.cards.GetP2Sum().ToString();
    }
}
