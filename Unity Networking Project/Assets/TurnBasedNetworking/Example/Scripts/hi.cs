using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class hi : MonoBehaviour
{
    [SerializeField]
    PlayingCards cards;

    bool isP1 = true;

    public TMP_Text text;

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
            text.text = "Player 1 Turn";
        else
            text.text = "Player 2 Turn";
    }
}
