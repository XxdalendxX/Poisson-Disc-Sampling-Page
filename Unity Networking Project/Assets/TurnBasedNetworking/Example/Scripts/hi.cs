using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hi : MonoBehaviour
{
    [SerializeField]
    PlayingCards cards;

    bool isP1 = true;

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isP1)
            {
                if (cards.GetP1Sum() <= 21)
                {
                    cards.AddCardToPlayer1();
                }
            }

            else
            {
                if (cards.GetP2Sum() <= 21)
                {
                    cards.AddCardToPlayer2();
                }
            }
            isP1 = !isP1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            isP1 = !isP1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            cards.ResetCards();
        }

        if (isP1)
            turnText.text = "Player 1 Turn";
        else
            turnText.text = "Player 2 Turn";

        sum1Text.text = cards.GetP1Sum().ToString();
        sum2Text.text = cards.GetP2Sum().ToString();
    }
}
