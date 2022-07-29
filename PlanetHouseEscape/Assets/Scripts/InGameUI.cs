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
/// - How does this script work?:
/// Well, in short, it works like any other barebones UI script. It just contains methods to call, which you select from the Inspector menu.
/// This particular script controls the logic of the InGameUI.
/// The InGameUI is what you see when you are actually playing the game, and not inside some main menu.
/// The InGameUI is mainly used to pause the game.
/// - When the game is paused, you can either adjust the mouse sensitivity, or you can go back to the main menu screen.
/// When you go back to the main menu screen, the player's progress is not saved anywhere.
/// Meaning, things like "how much progress has the player made in the game" are not saved in the disk, or in memory.
/// - It can also be used to show the game over screen.
/// The game over screen is shown when the game is over. Since this is a room escape puzzle game,
/// the game over screen is only shown when the player manages to escape the room. There's no player death or anything like that.
/// The game over screen just says "congratulations" and shows how much time has passed since the game scene has started.
/// This just shows how much time it took for the player to finish the game.
/// Therefore, if you wish to show the game over screen to the player, just call the <see cref="FinishGame"/> when the game over condition is met.
/// </summary>
public class InGameUI : MonoBehaviour
{
    public static bool isGamePaused = false;
    public static bool isGameOver = false;

    public Slider sliderMouseSensitivity;
    public TextMeshProUGUI txtMouseSensitivity;

    public GameObject screenPauseMenu;
    public GameObject screenGameOver;

    public TextMeshProUGUI txtGameOverBody; // writes out player's time to finish the game


    /// <summary>
    /// Callback method for <see cref="btnReturnToMainMenu"/>.
    /// Loads and starts the MainMenuScene. The actual game scene will be closed.
    /// </summary>
    public void OnClick_ButtonReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }

    /// <summary>
    /// Callback method for <see cref="sliderMouseSensitivity"/>.
    /// It is used to adjust <see cref="StaticVariables.PlayerRotationSensitivity"/>.
    /// </summary>
    public void OnSlider_ValueChanged()
    {
        int val = Convert.ToInt32(sliderMouseSensitivity.value);
        txtMouseSensitivity.text = "Mouse Sensitivity: " + val;
        StaticVariables.PlayerRotationSensitivity = val;
    }

    /// <summary>
    /// A public method which sets the game's state to finished.
    /// It also makes the game over screen visible.
    /// This method can be called when the game is meant to be finished.
    /// </summary>
    public void FinishGame()
    {
        isGameOver = true;
        ToggleMouse(true);
        screenGameOver.SetActive(true);
        TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        string timeStr = string.Format("{0:D2} minutes, {1:D2} seconds", timeSpan.Minutes, timeSpan.Seconds);

        txtGameOverBody.text = "Your time:" + Environment.NewLine + timeStr;
    }

    /// <summary>
    /// Toggles the activeness (ie, visibility) of the Pause Menu.
    /// </summary>
    /// <param name="show">True if the pause menu should be visible; false otherwise.</param>
    void TogglePauseMenu(bool show)
    {
        if (show)
        {
            screenPauseMenu.SetActive(true);
            Time.timeScale = 0;
            ToggleMouse(true);
        }
        else
        {
            screenPauseMenu.SetActive(false);
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
    void InitInGameUI()
    {
        // Init pause menu
        sliderMouseSensitivity.value = StaticVariables.PlayerRotationSensitivity;
        OnSlider_ValueChanged();

        isGamePaused = false;
        TogglePauseMenu(false);

        // Init game over menu
        screenGameOver.SetActive(false);
        isGameOver = false;
    }

    /// <summary>
    /// Unity's Update method. Update is called once per frame.
    /// In this case, it is used capture input 'M' and control the In Game UI.
    /// </summary>
    void Update()
    {
        if (isGameOver)
        {
            return;
        }

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
        InitInGameUI();
    }
}
