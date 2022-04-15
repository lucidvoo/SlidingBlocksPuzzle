using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// manage pause menu and it's buttons
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Button menuButton;

    private bool isGamePaused = false;


    private void Start() => pauseMenu.SetActive(false);


    private void OnEnable()
    {
        Events.onBoardShuffled.AddListener(EnableMenuButton);
        Events.onBoardStartShuffling.AddListener(DisableMenuButton);
    }


    private void OnDisable()
    {
        Events.onBoardShuffled.RemoveListener(EnableMenuButton);
        Events.onBoardStartShuffling.RemoveListener(DisableMenuButton);
    }


    private void DisableMenuButton() => menuButton.interactable = false;


    private void EnableMenuButton() => menuButton.interactable = true;


    // Pause toggle button handler
    public void PauseToggle()
    {
        if (isGamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    
    // Pause button handler
    public void Pause()
    {
        if (isGamePaused)
        {
            return;
        }
        isGamePaused = true;
        Events.onGamePaused.Invoke();

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    // Resume button handler
    public void Resume()
    {
        if (!isGamePaused)
        {
            return;
        }
        isGamePaused = false;
        Events.onGameUnpaused.Invoke();

        pauseMenu.SetActive(false);

        Time.timeScale = 1f;
    }

    // "Home" button handler
    public void ToMenu()
    {
        // TODO:
        // save progress
        // restore timescale to 1
        // go to menu
    }

    // Restart button handler
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
