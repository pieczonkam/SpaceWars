using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    private GameController gameController;
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
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        if (gameController.twoPlayersMode) player02Parameters.SetActive(true);
        else player02Parameters.SetActive(false);

        player01Points.text = gameController.player01NumberOfCoins.ToString();
        player01WeaponPower.text = gameController.player01WeaponUpgrade.ToString() + "/" + gameController.player01MaxWeaponLevel.ToString();
        player01HealthPoints.text = gameController.player01NumberOfLives.ToString();
        player02Points.text = gameController.player02NumberOfCoins.ToString();
        player02WeaponPower.text = gameController.player02WeaponUpgrade.ToString() + "/" + gameController.player02MaxWeaponLevel.ToString();
        player02HealthPoints.text = gameController.player02NumberOfLives.ToString();

        gameOverMenu.SetActive(false);
        gamePausedMenu.SetActive(false);
    }

    private void Update()
    {
        if (!gameOver)
        {
            player01Points.text = gameController.player01NumberOfCoins.ToString();
            player01WeaponPower.text = gameController.player01WeaponUpgrade.ToString() + "/" + gameController.player01MaxWeaponLevel.ToString();
            player01HealthPoints.text = gameController.player01NumberOfLives.ToString();
            player02Points.text = gameController.player02NumberOfCoins.ToString();
            player02WeaponPower.text = gameController.player02WeaponUpgrade.ToString() + "/" + gameController.player02MaxWeaponLevel.ToString();
            player02HealthPoints.text = gameController.player02NumberOfLives.ToString();

            if (Input.GetKeyDown(KeyCode.Escape) && !levelCompleteMenu.activeSelf && levelTitleText.color.a == 0.0f)
            {
                if (gamePaused) Resume();
                else Pause();
            }
        }
        else
        {
            if (timeElapsed < afterGameTime) timeElapsed += Time.deltaTime;
            else
            {
                FindObjectOfType<AudioManager>().PauseTheme();
                if (levelCompleteMenu.activeSelf)
                    levelCompleteMenu.SetActive(false);
                if (levelTitleMenu.activeSelf)
                    levelTitleMenu.SetActive(false);
                gameOverMenu.SetActive(true);
                Time.timeScale = 0.0f;
            }
        }

        if (gameController.twoPlayersMode && gameController.player01NumberOfLives <= 0 && gameController.player02NumberOfLives <= 0) gameOver = true;
        else if (!gameController.twoPlayersMode && gameController.player01NumberOfLives <= 0) gameOver = true;
    }

    private void Pause()
    {
        FindObjectOfType<AudioManager>().PauseTheme();
        gamePaused = true;
        Time.timeScale = 0.0f;
        gamePausedMenu.SetActive(true);
    }

    public void Resume()
    {
        resumeGameBtn.interactable = false;
        resumeGameBtn.interactable = true;

        FindObjectOfType<AudioManager>().ResumeTheme();
        gamePaused = false;
        Time.timeScale = 1.0f;
        gamePausedMenu.SetActive(false);
    }

    public void PlayAgain()
    {
        ClearPlayers();
        gameController.currentLevel = 1;

        gameOver = false;
        Time.timeScale = 1.0f;
        FindObjectOfType<AudioManager>().PlayTheme("NormalLevelsTheme");
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        ClearPlayers();
        gameController.currentLevel = 0;

        gameOver = false;
        Time.timeScale = 1.0f;
        FindObjectOfType<AudioManager>().PlayTheme("MainMenuTheme");
        SceneManager.LoadScene(0);
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

    private void ClearPlayers()
    {
        Player[] players = Resources.FindObjectsOfTypeAll<Player>();
        foreach (Player player in players) Destroy(player.gameObject);
    }
}
