using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TurnBasedSystem;

public class GameManager : TBManager
{
    [HideInInspector]
     public PlayingCards cards;

    private new void Start()
    {
        cards = GetComponent<PlayingCards>();
    }
}
