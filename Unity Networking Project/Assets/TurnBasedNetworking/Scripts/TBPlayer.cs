using Mirror;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using Unity.VisualScripting;
using UnityEngine;

namespace TurnBasedSystem
{
    /// <summary>
    /// this would carry all the nessesary information for the player to be used as a template for the other player scripts.
    /// </summary>
    public class TBPlayer : NetworkBehaviour
    {
        [SerializeField] TBManager TB_Manager;
        [SerializeField] string TB_ManagerLocation;
        
        bool TB_isTurn = false;
        int TB_wins = 0;

        [ClientRpc]
        public void RpcTB_ToggleTurn()
        {
            TB_isTurn = !TB_isTurn;
        }

        public bool TB_GetTurnState()
        {
            return TB_isTurn;
        }

        public int TB_GetPlayerWins()
        {
            return TB_wins;
        }

        [ClientRpc]
        public void RpcTB_PlayerWon()
        {
            TB_wins++;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            TB_Manager = GameObject.Find(TB_ManagerLocation).GetComponent<TBManager>();

            TB_Manager.RpcInsertPlayer(this);

            Debug.Log("Client connected to the server");
        }

        public override void OnStopClient()
        {
            TB_Manager.RpcRemovePlayer(this);

            //NetworkServer.Destroy(this.gameObject);

            //TB_Manager.ClearPlayerList();
            
            base.OnStopClient();

            Debug.Log("Client has left the server");
        }
    }
}
