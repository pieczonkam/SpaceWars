using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    private GameController gameController;
    private AudioManager audioManager;
    private ObjectPooler objectPooler;

    private float timeElapsed = 0.0f;
    private float afterGameTime = 2.0f;

    private bool gameOver = false;
    private bool gamePaused = false;

    [SerializeField] private TextMeshProUGUI player01Points;
    [SerializeField] private TextMeshProUGUI player01WeaponPower;
    [SerializeField] private TextMeshProUGUI player01HealthPoints;
    [SerializeField] private TextMeshProUGUI player02Points;
    [SerializeField] private TextMeshProUGUI player02WeaponPower;
    [SerializeField] private TextMeshProUGUI player02HealthPoints;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private GameObject player02Parameters;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject gamePausedMenu;
    [SerializeField] private GameObject levelTitleMenu;
    [SerializeField] private GameObject levelCompleteMenu;
    [SerializeField] private Button resumeGameBtn;
 
    private void Start()
    {
        gameController = GameController.instance;
        audioManager = AudioManager.instance;
        objectPooler = ObjectPooler.instance;

        if (gameController.twoPlayersMode) 
            player02Parameters.SetActive(true);
        else 
            player02Parameters.SetActive(false);

        SetParamsText();

        gameOverMenu.SetActive(false);
        gamePausedMenu.SetActive(false);
    }

    private void Update()
    {
        if (!gameOver)
        {
            SetParamsText();

            if (Input.GetKeyDown(KeyCode.Escape) && !levelCompleteMenu.activeSelf && levelTitleText.color.a == 0.0f)
            {
                if (gamePaused) 
                    Resume();
                else 
                    Pause();
            }
        }
        else
        {
            if (timeElapsed < afterGameTime) 
                timeElapsed += Time.deltaTime;
            else
            {
                audioManager.StopTheme();
                if (levelCompleteMenu.activeSelf)
                    levelCompleteMenu.SetActive(false);
                if (levelTitleMenu.activeSelf)
                    levelTitleMenu.SetActive(false);
                gameOverMenu.SetActive(true);
                Time.timeScale = 0.0f;
            }
        }

        if (gameController.twoPlayersMode && gameController.player01NumberOfLives <= 0 && gameController.player02NumberOfLives <= 0) 
            gameOver = true;
        else if (!gameController.twoPlayersMode && gameController.player01NumberOfLives <= 0) 
            gameOver = true;
    }

    private void Pause()
    {
        audioManager.PauseTheme();
        gamePaused = true;
        Time.timeScale = 0.0f;
        gamePausedMenu.SetActive(true);
    }

    public void Resume()
    {
        // These two lines reset button state, so that it isn't selected at the beggining of next pause
        resumeGameBtn.interactable = false;
        resumeGameBtn.interactable = true;

        audioManager.ResumeTheme();
        gamePaused = false;
        Time.timeScale = 1.0f;
        gamePausedMenu.SetActive(false);
    }

    public void PlayAgain()
    {
        if (gameController.currentLevel == gameController.lastLevel)
            gameController.ResetEnemies();

        gameController.ClearPlayers();
        gameController.currentLevel = 1;

        gameOver = false;
        Time.timeScale = 1.0f;
        audioManager.PlayTheme("NormalLevelsTheme");
        SceneManager.LoadScene(1);
        objectPooler.ClearPooledObjects();
    }

    public void MainMenu()
    {
        if (gameController.currentLevel == gameController.lastLevel)
            gameController.ResetEnemies();

        gameController.ClearPlayers();
        gameController.currentLevel = 0;

        gameOver = false;
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
        objectPooler.ClearPooledObjects();
    }

    public void ShowLevelCompleteMenu(bool show)
    {
        if (levelCompleteMenu.activeSelf != show)
            levelCompleteMenu.SetActive(show);
    }

    public void UpdateLevelCompleteText(string text)
    {
        levelCompleteText.text = text;
    }

    public void Quit()
    { 
        Application.Quit();
    }

    private void SetParamsText()
    {
        player01Points.text = gameController.player01NumberOfCoins.ToString();
        player01WeaponPower.text = gameController.player01WeaponUpgrade.ToString() + "/" + gameController.player01MaxWeaponLevel.ToString();
        player01HealthPoints.text = gameController.player01NumberOfLives.ToString();
        player02Points.text = gameController.player02NumberOfCoins.ToString();
        player02WeaponPower.text = gameController.player02WeaponUpgrade.ToString() + "/" + gameController.player02MaxWeaponLevel.ToString();
        player02HealthPoints.text = gameController.player02NumberOfLives.ToString();
    }
}
