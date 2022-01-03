using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private GameController gameController;
    private AudioManager audioManager;
    private ObjectPooler objectPooler;
    private List<Resolution> resolutions;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject modeMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private Toggle fullscreen;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject warningText;

    private void Start()
    {
        gameController = GameController.instance;
        audioManager = AudioManager.instance;
        objectPooler = ObjectPooler.instance;

        fullscreen.isOn = Screen.fullScreen;

        float volume;
        volume = PlayerPrefs.GetFloat("Volume", 1000.0f);
        if (Mathf.Approximately(volume, 1000.0f))
            audioMixer.GetFloat("Volume", out volume);
        else
            audioMixer.SetFloat("Volume", volume);
        volumeSlider.value = volume;

        graphicsDropdown.value = QualitySettings.GetQualityLevel();

        resolutions = new List<Resolution>();
        Resolution[] allResolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int resWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int resHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
        int currentResolutionIdx = 0;

        for (int i = 0; i < allResolutions.Length; ++i)
        {
            float aspectRatio = (float)allResolutions[i].width / allResolutions[i].height;

            if (resolutions.FindIndex(res => res.width == allResolutions[i].width && res.height == allResolutions[i].height) == -1 && Mathf.Approximately(aspectRatio, 16.0f / 9.0f))
                resolutions.Add(allResolutions[i]);
        }
        for (int i = 0; i < resolutions.Count; ++i)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == resWidth && resolutions[i].height == resHeight)
                currentResolutionIdx = i;
        }

        resolutionDropdown.enabled = true;
        resolutionDropdown.ClearOptions();
        if (resolutions.Count > 0)
        {
            warningText.SetActive(false);
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIdx;
            resolutionDropdown.RefreshShownValue();
            SetResolution(currentResolutionIdx);
        }
        else
        {
            warningText.SetActive(true);
            resolutionDropdown.enabled = false;
        }

        mainMenu.SetActive(true);
        modeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);

        audioManager.PlayTheme("MainMenuTheme");
    }

    public void Play(bool twoPlayersMode)
    {
        gameController.twoPlayersMode = twoPlayersMode;
        audioManager.PlayTheme("NormalLevelsTheme");
        SceneManager.LoadScene(1);
        objectPooler.ClearPooledObjects();
        gameController.currentLevel = 1;
    }

    public void ShowModeSelection()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(true);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        settingsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ShowControls()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        modeMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetQuality(int idx)
    {
        QualitySettings.SetQualityLevel(idx);
    }
   
    public void SetFullscreen(bool state)
    {
        Screen.fullScreen = state;
    }

    public void SetResolution(int idx)
    {
        Screen.SetResolution(resolutions[idx].width, resolutions[idx].height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionWidth", resolutions[idx].width);
        PlayerPrefs.SetInt("ResolutionHeight", resolutions[idx].height);
    }
}
