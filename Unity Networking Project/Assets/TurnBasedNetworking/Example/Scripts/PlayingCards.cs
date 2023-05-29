using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;


public class PlayingCards : NetworkBehaviour
{
    public GameObject cardPrefab;

    public int[] cardPool = new int[11];

    public List<int> player1Cards;

    public List<int> player2Cards;

    void Start()
    {
        for(int i = 0; i < 11; i++)
        {
            int card = i + 1;
            cardPool[i] = card;
        }
    }

    public void AddCardToPlayer1()
    {
        int num = Random.Range(0, cardPool.Length);
        int card = cardPool[num];
        player1Cards.Add(card);
    }

    public void AddCardToPlayer2()
    {
        int num = Random.Range(0, cardPool.Length);
        int card = cardPool[num];
        player2Cards.Add(card);
    }

    public int GetP1Sum()
    {
        int sum = 0;
        foreach(int card in player1Cards)
        {
            sum += card;
        }
        return sum;
    }

    public int GetP2Sum()
    {
        int sum = 0;
        foreach (int card in player2Cards)
        {
            sum += card;
        }
        return sum;
    }

    public void ResetCards()
    {
        int P1Count = player1Cards.Count;
        int P2Count = player2Cards.Count;

        for (int i = 0; i < player1Cards.Count; i++)
        {
            cardPool[i] = player1Cards[i];
            player1Cards.RemoveAt(i);
        }

        for (int i = 1; i < player2Cards.Count; i++)
        {
            cardPool[i] = player2Cards[i];
            player2Cards.RemoveAt(i);
        }
    }
}
