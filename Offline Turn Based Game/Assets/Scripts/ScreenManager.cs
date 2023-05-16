using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] Screen[] screens;

    private void Start()
    {
        screens = GetComponentsInChildren<Screen>();
    }

    public void NewGame()
    {
        for (int i = 0; i < screens.Length; ++i)
        {
            screens[i]gameObject.SetActive(false);
        }
    }

    public void UpdateScreenState()
    {
       
    }
}
