using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hi : MonoBehaviour
{
    [SerializeField]
    PlayingCards cards;

    public TMP_Text turnText;
    public TMP_Text sum2Text;
    public TMP_Text sum1Text;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /* this is for reference purposes
         * 
         *   if (Input.GetKeyDown(KeyCode.LeftShift))
         *   {
         *       cards.ResetCards();
         *   }
         *   
         *   if (isP1)
         *       turnText.text = "Player 1 Turn";
         *   else
         *       turnText.text = "Player 2 Turn";
        */

        sum1Text.text = cards.GetP1Sum().ToString();
        sum2Text.text = cards.GetP2Sum().ToString();
    }
}
