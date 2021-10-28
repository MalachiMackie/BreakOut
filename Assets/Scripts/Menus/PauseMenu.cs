using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool _paused;
    
    private void Awake()
    {
        Debug.Log("Hello World");
        GameManager.Instance.GamePaused += OnGamePaused;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_paused && Input.GetKeyDown(KeyCode.Escape))
        {
            Unpause();
        }
    }

    private void OnGamePaused(object sender, EventArgs e)
    {
        _paused = true;
        gameObject.SetActive(true);
    }

    public void Quit()
    {
        Unpause();
        GameManager.Instance.Quit();
    }

    public void Unpause()
    {
        if (!_paused)
        {
            return;
        }
        _paused = false;
        gameObject.SetActive(false);
        GameManager.Instance.Unpause();
    }
}
