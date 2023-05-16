using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Cell[] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = GetComponentsInChildren<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool state)
    {
        this.gameObject.SetActive(state);
    }

    public Cell GetCell(int i)
    {
        return cells[i];
    }
}
