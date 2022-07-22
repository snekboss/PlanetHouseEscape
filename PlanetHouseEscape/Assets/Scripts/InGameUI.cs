using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A class which designates the attached game object as the InGameUI.
/// This script contains the logic of the InGameUI.
/// </summary>
public class InGameUI : MonoBehaviour
{
    public static bool isGamePaused = false;
    public static bool isGameOver = false;

    public Button btnReturnToMainMenu;

    public Slider sliderMouseSensitivity;
    public TextMeshProUGUI txtMouseSensitivity;

    public GameObject panelPauseMenu;
    public GameObject panelGameOver;


    /// <summary>
    /// Callback method for <see cref="btnReturnToMainMenu"/>.
    /// Loads and starts the MainMenuScene. The actual game scene will be closed.
    /// </summary>
    public void OnClick_ButtonReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Callback method for <see cref="sliderMouseSensitivity"/>.
    /// It is used to adjust <see cref="StaticVariables.MouseSensitivity"/>.
    /// </summary>
    public void OnSlider_ValueChanged()
    {
        int val = Convert.ToInt32(sliderMouseSensitivity.value);
        txtMouseSensitivity.text = "Mouse Sensitivity: " + val;
        StaticVariables.MouseSensitivity = val;
    }

    /// <summary>
    /// Toggles the activeness (ie, visibility) of the Pause Menu.
    /// </summary>
    /// <param name="show">True if the pause menu should be visible; false otherwise.</param>
    void TogglePauseMenu(bool show)
    {
        if (show)
        {
            panelPauseMenu.SetActive(true);
            Time.timeScale = 0;
            ToggleMouse(true);
        }
        else
        {
            panelPauseMenu.SetActive(false);
            Time.timeScale = 1;
            ToggleMouse(false);
        }
    }

    /// <summary>
    /// Toggles the visibility of the mouse cursor.
    /// </summary>
    /// <param name="show">True if the mouse cursor should be visible; false otherwise.</param>
    void ToggleMouse(bool show)
    {
        if (show)
        {
            //UnityEngine.Cursor.lockState = CursorLockMode.Confined;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            //UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    /// <summary>
    /// Initializes the In Game UI.
    /// </summary>
    void InitPauseMenu()
    {
        sliderMouseSensitivity.value = StaticVariables.MouseSensitivity;
        OnSlider_ValueChanged();

        TogglePauseMenu(false);
    }

    /// <summary>
    /// Unity's Update method. Update is called once per frame.
    /// In this case, it is used capture input 'M' and control the In Game UI.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isGamePaused = !isGamePaused;
            TogglePauseMenu(isGamePaused);
        }
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// In this case, it is used to initialzie the In Game UI.
    /// </summary>
    void Start()
    {
        InitPauseMenu();
    }
}
