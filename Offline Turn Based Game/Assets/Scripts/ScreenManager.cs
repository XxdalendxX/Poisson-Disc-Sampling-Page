using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] Screen[] screens;
    int state;

    private void Start()
    {
        screens = GetComponentsInChildren<Screen>();
        NewGame();
    }

    public void NewGame()
    {
        for (int i = 1; i < screens.Length; ++i)
        {
            screens[i].gameObject.SetActive(false);
        }
        state = 0;
    }

    public void UpdateScreenState()
    {
        screens[state].gameObject.SetActive(false);
        state++;
        screens[state].gameObject.SetActive(true);
    }
}
