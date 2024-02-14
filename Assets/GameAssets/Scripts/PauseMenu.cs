using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;
    public GameObject HUD;
    private bool gamePaused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            PauseGame();
        }

        if (gamePaused && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.C))
        {
            ContinueGame();
        }
    }

    private void PauseGame()
    {
        PausePanel.SetActive(true);
        HUD.SetActive(false);
        gamePaused = true;
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        PausePanel.SetActive(false);
        HUD.SetActive(true);
        gamePaused = false;
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
