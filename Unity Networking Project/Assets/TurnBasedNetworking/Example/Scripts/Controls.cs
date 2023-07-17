using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedSystem;
using Mirror;

public class Controls : NetworkBehaviour
{
    TBPlayer player;

    [SerializeField]
    PlayingCards cards;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || player.TB_GetTurnState() == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
             if (cards.GetP1Sum() <= 21)
             {
                 cards.AddCardToPlayer1();
             }

            else
            {
                if (cards.GetP2Sum() <= 21)
                {
                    cards.AddCardToPlayer2();
                }
            }
            player.RpcTB_ToggleTurn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.RpcTB_ToggleTurn();
        }
    }
}
