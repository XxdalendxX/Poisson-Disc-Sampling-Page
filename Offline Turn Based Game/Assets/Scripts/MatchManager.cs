using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    ScreenManager screenManager;

    private void Awake()
    {
        screenManager = FindObjectOfType<ScreenManager>();
    }


}
