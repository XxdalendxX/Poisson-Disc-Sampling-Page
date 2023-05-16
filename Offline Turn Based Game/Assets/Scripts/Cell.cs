using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Button button;
    [SerializeField] GameManager gameManager;
    int state = 0;

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        button = GetComponentInChildren<Button>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Claim()
    {
        state += gameManager.CheckTurnState();
    }

    public int CellState()
    {
        return state;
    }
}
