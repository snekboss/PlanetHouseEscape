using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A class which designates the attached game object as the MainMenuUI.
/// This script contains the logic of the Main Menu UI.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public Slider sliderMouseSensitivity;
    public TextMeshProUGUI txtMouseSensitivity;

    public GameObject screenMainMenu;
    public GameObject screenControls;
    public GameObject screenHints;

    public Button btnStartGame;
    public Button btnControls;
    public Button btnHints;
    public Button btnExitGame;
    public Button btnGoBack;

    public GameObject mainCamera;

    [Range(-180f, 180f)]
    public float camRotPerSecond = 6f;

    // Spawn planets
    public Vector3 planetSpawnPosMinVec;
    public Vector3 planetSpawnPosMaxVec;

    /// <summary>
    /// Callback method for <see cref="btnStartGame"/>.
    /// Loads and starts the actual game scene.
    /// </summary>
    public void OnClick_ButtonStartGame()
    {
        StaticVariables.MouseSensitivity = sliderMouseSensitivity.value;
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Callback method for <see cref="btnControls"/>.
    /// Shows the UI of the Controls menu.
    /// </summary>
    public void OnClick_ButtonControls()
    {
        screenMainMenu.SetActive(false);
        screenControls.SetActive(true);
        screenHints.SetActive(false);

        btnGoBack.gameObject.SetActive(true);
    }

    /// <summary>
    /// Callback method for <see cref="btnHints"/>.
    /// Shows the UI of the Hints menu.
    /// </summary>
    public void OnClick_ButtonHints()
    {
        screenMainMenu.SetActive(false);
        screenControls.SetActive(false);
        screenHints.SetActive(true);

        btnGoBack.gameObject.SetActive(true);
    }

    /// <summary>
    /// Callback method for <see cref="btnExitGame"/>.
    /// Closes the game.
    /// </summary>
    public void OnClick_ButtonExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Callback method for <see cref="btnGoBack"/>.
    /// It is used to go back to the Main Menu screen from several different menu screens.
    /// </summary>
    public void OnClick_ButtonGoBack()
    {
        screenMainMenu.SetActive(true);
        screenControls.SetActive(false);
        screenHints.SetActive(false);

        btnGoBack.gameObject.SetActive(false);

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
    /// Initializes the Main Menu UI.
    /// </summary>
    void InitMainMenuUI()
    {
        screenMainMenu.SetActive(true);
        screenControls.SetActive(false);
        screenHints.SetActive(false);

        btnGoBack.gameObject.SetActive(false);

        sliderMouseSensitivity.value = StaticVariables.MouseSensitivity;

        OnSlider_ValueChanged();
    }

    /// <summary>
    /// Spawns planets at random positions, as part of the Main Menu UI animation.
    /// </summary>
    void InitSpawnPlanetsAtRandomPositions()
    {
        foreach (var kvp in Planet.PlanetPrefabs)
        {
            GameObject planetPrefab = kvp.Value;
            GameObject planetGO = Instantiate(planetPrefab);

            float randPosX = UnityEngine.Random.Range(planetSpawnPosMinVec.x, planetSpawnPosMaxVec.x);
            float randPosY = UnityEngine.Random.Range(planetSpawnPosMinVec.y, planetSpawnPosMaxVec.y);
            float randPosZ = UnityEngine.Random.Range(planetSpawnPosMinVec.z, planetSpawnPosMaxVec.z);
            planetGO.transform.position = new Vector3(randPosX, randPosY, randPosZ);
            planetGO.transform.parent = null;

            planetGO.AddComponent<MainMenuPlanet>();
        }
    }

    /// <summary>
    /// Unity's Update method. Update is called once per frame.
    /// In this case, it is used to rotate the camera in the Main Menu UI.
    /// </summary>
    void Update()
    {
        mainCamera.transform.Rotate(Vector3.up, camRotPerSecond * Time.deltaTime);
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update.
    /// In this case, it is used to initialzie the Main Menu UI.
    /// </summary>
    void Start()
    {
        InitMainMenuUI();
        InitSpawnPlanetsAtRandomPositions();
    }
}
