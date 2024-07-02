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
/// - How to use this script?:
/// Well, in short, it works like any other barebones UI script. It just contains methods to call, which you select from the Inspector menu.
/// This particular script controls the logic of the MainMenuUI.
/// The main menu UI consists of 3 parts:
/// - 1) The main menu part: This is what the player sees when they first start the game.
/// This part of the menu contains buttons like "Start Game", "Controls", "Hints", "Exit Game".
/// "Start Game" literally starts the game by loading the game scene.
/// "Exit Game" closes the application (how about that?).
/// "Controls" opens the controls menu, by hiding the visibility of every other UI element.
/// "Hints" opens the hints menu, by hiding the visibility of every other UI element.
/// - 2) Controls menu part: This is where the player can see how to control themselves once the game actually starts.
/// It also contains a slider to change the mouse sensitivity.
/// Finally, there's a "Go Back" button which navigates back to the main menu part.
/// - 3) Hints menu part: This is where the player see a text which contains some hints as to how to beat the game.
/// It contains a lot of spoilers.
/// It also contains the same "Go Back" button which navigates back to the main menu part.
/// -
/// - This script also contains a reference to the mainCamera of the scene.
/// Because the UI is fancy, and it rotates around slowly as the planets crash onto one another.
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

    public Button btnIncreaseQuality;
    public Button btnDecreaseQuality;
    public Button btnDefaultQuality;

    public TextMeshProUGUI txtChosenQuality;

    public GameObject mainCamera;

    [Range(-180f, 180f)]
    public float camRotPerSecond = 6f;

    static bool isLoadingForTheFirstTime = true;

    // Spawn planets
    public Vector3 planetSpawnPosMinVec;
    public Vector3 planetSpawnPosMaxVec;

    /// <summary>
    /// Callback method for <see cref="btnStartGame"/>.
    /// Loads and starts the actual game scene.
    /// </summary>
    public void OnClick_ButtonStartGame()
    {
        StaticVariables.PlayerRotationSensitivity = sliderMouseSensitivity.value;
        Time.timeScale = 1;
        InGameUI.isGameOver = false;
        InGameUI.isGamePaused = false;
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
#if (UNITY_WEBGL)
        Application.OpenURL("about:blank");
#endif
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
    /// It is used to adjust <see cref="StaticVariables.PlayerRotationSensitivity"/>.
    /// </summary>
    public void OnSlider_ValueChanged()
    {
        int val = Convert.ToInt32(sliderMouseSensitivity.value);
        txtMouseSensitivity.text = "Mouse Sensitivity: " + val;
        StaticVariables.PlayerRotationSensitivity = val;
    }

    /// <summary>
    /// Event to increase the Quality setting.
    /// </summary>
    public void OnClick_ButtonIncreaseQuality() 
    {
        QualitySettings.IncreaseLevel(true);
        UpdateQualitySettingWidgets();
    }

    /// <summary>
    /// Event to decrease the Quality setting.
    /// </summary>
    public void OnClick_ButtonDecreaseQuality() 
    {
        QualitySettings.DecreaseLevel(true);
        UpdateQualitySettingWidgets();
    }

    /// <summary>
    /// Event for the Default Quality selection button.
    /// </summary>
    public void OnClick_ButtonDefaultQuality() 
    {
        SetDefaultQuality();
    }

    /// <summary>
    /// Updates the Quality Setting related widgets.
    /// </summary>
    void UpdateQualitySettingWidgets()
    {
        int index = QualitySettings.GetQualityLevel();

        btnDecreaseQuality.interactable = (index != 0);
        btnIncreaseQuality.interactable = (index != (QualitySettings.names.Length - 1));

        txtChosenQuality.text = QualitySettings.names[index];
    }

    /// <summary>
    /// Sets the Quality Setting to the default one described in <see cref="StaticVariables.QualitySetting"/>.
    /// </summary>
    void SetDefaultQuality()
    {
        QualitySettings.SetQualityLevel(StaticVariables.DefaultQualitySetting, true);

        UpdateQualitySettingWidgets();
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

        sliderMouseSensitivity.value = StaticVariables.PlayerRotationSensitivity;

        if (isLoadingForTheFirstTime)
        {
            SetDefaultQuality();
            isLoadingForTheFirstTime = false;
        }
        else
        {
            UpdateQualitySettingWidgets();
        }

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
