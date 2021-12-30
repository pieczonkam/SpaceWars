using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameController gameController;
    private AudioManager audioManager;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject modeMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject creditsMenu;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        audioManager = FindObjectOfType<AudioManager>();

        mainMenu.SetActive(true);
        modeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void Play(bool twoPlayersMode)
    {
        gameController.twoPlayersMode = twoPlayersMode;
        audioManager.PlayTheme("NormalLevelsTheme");
        SceneManager.LoadScene(1);
        gameController.currentLevel = 1;
    }

    public void ShowModeSelection()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(true);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        optionsMenu.SetActive(true);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ShowControls()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenu.SetActive(false);
        modeMenu.SetActive(false);
        optionsMenu.SetActive(false);
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
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }
}
