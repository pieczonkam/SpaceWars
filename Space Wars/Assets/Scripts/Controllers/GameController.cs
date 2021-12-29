using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static private GameController instance = null;

    private float timeElapsed = 0.0f;
    private int lastLevel = 10;
    private float extraOnLevelClearDelay = 0.0f;

    [HideInInspector] public int currentLevel = 0;
    [HideInInspector] public bool twoPlayersMode = false;
    public int player01MaxWeaponLevel = 0;
    public int player01NumberOfLives = 0;
    public int player01WeaponUpgrade = 0;
    public int player01NumberOfCoins = 0;
    public int player02MaxWeaponLevel = 0;
    public int player02NumberOfLives = 0;
    public int player02WeaponUpgrade = 0;
    public int player02NumberOfCoins = 0;
    [SerializeField] private float onLevelClearDelay = 2.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    private void Update()
    {
        if (currentLevel != 0)
        {
            extraOnLevelClearDelay = currentLevel == 10 ? 3.0f : 0.0f;

            GameObject enemiesContainer = GameObject.Find("Enemies");
            if (enemiesContainer != null)
            {
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                Boss boss = FindObjectOfType<Boss>();
                if (enemies != null && enemies.Length == 0 && boss == null)
                {
                    InGameMenu inGameMenu = GameObject.Find("InGameMenu").GetComponent<InGameMenu>();

                    if (timeElapsed < onLevelClearDelay + extraOnLevelClearDelay)
                    {
                        if (currentLevel == lastLevel)
                            FindObjectOfType<AudioManager>().StopTheme();
                        SetPlayersImmortal(true);
                        if (inGameMenu != null)
                        {
                            inGameMenu.ShowLevelCompleteMenu(true);
                            if (currentLevel == lastLevel)
                                inGameMenu.UpdateLevelCompleteText("LEVEL COMPLETE\nYOU WIN!\n\nFINAL SCORE: " + (player01NumberOfCoins + player02NumberOfCoins).ToString());
                        }
                                
                        timeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        SetPlayersImmortal(false);
                        if (inGameMenu != null)
                            inGameMenu.ShowLevelCompleteMenu(false);
                        timeElapsed = 0.0f;
                        currentLevel++;
                        if (currentLevel <= lastLevel)
                        {
                            SetPlayersPosition();
                            SwitchPlayersLighting();
                            if (currentLevel == lastLevel)
                                FindObjectOfType<AudioManager>().PlayTheme("BossLevelTheme");
                            SceneManager.LoadScene(currentLevel);
                        }
                        else
                        {
                            ClearPlayers();
                            FindObjectOfType<AudioManager>().PlayTheme("MainMenuTheme");
                            SceneManager.LoadScene(0);
                        }
                    }
                }
            }
        }

        //Debug.Log(Time.timeScale + " at " + SceneManager.GetActiveScene().name);
        Debug.Log("FPS: " + (1.0f / Time.smoothDeltaTime).ToString() + " at " + SceneManager.GetActiveScene().name);
    }

    private void SetPlayersImmortal(bool state)
    {
        GameObject player01 = GameObject.Find("Player01");
        GameObject player02 = GameObject.Find("Player02");
        Player player01Script = null;
        Player player02Script = null;

        if (player01 != null)
            player01Script = player01.GetComponent<Player>();
        if (player02 != null)
            player02Script = player02.GetComponent<Player>();

        if (player01Script != null)
            player01Script.immortal = state;
        if (player02Script != null)
            player02Script.immortal = state;
    }

    private void SwitchPlayersLighting()
    {
        if (twoPlayersMode)
        {
            switch (currentLevel)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 10:
                    if (player01NumberOfLives > 0)
                        GameObject.Find("Player01").GetComponent<Player>().SwitchLighting(false);
                    if (player02NumberOfLives > 0)
                        GameObject.Find("Player02").GetComponent<Player>().SwitchLighting(false);
                    break;
                case 7:
                case 8:
                case 9:
                    if (player01NumberOfLives > 0)
                        GameObject.Find("Player01").GetComponent<Player>().SwitchLighting(true);
                    if (player02NumberOfLives > 0)
                        GameObject.Find("Player02").GetComponent<Player>().SwitchLighting(true);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (currentLevel)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 10:
                    if (player01NumberOfLives > 0)
                        GameObject.Find("Player01").GetComponent<Player>().SwitchLighting(false);
                    break;
                case 7:
                case 8:
                case 9:
                    if (player01NumberOfLives > 0)
                        GameObject.Find("Player01").GetComponent<Player>().SwitchLighting(true);
                    break;
                default:
                    break;
            }
        }
    }

    private void SetPlayersPosition()
    {
        if (twoPlayersMode)
        {
            switch (currentLevel)
            {
                case 2:
                case 3:
                case 4:
                case 7:
                case 8:
                case 9:
                case 10:
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(-3.0f, -4.0f);
                    GameObject.Find("Player02").GetComponent<Player>().SetPosition(3.0f, -4.0f);
                    break;
                case 5:
                case 6:
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(-1.0f, 0.0f);
                    GameObject.Find("Player02").GetComponent<Player>().SetPosition(1.0f, 0.0f);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (currentLevel)
            {
                case 2:
                case 3:
                case 4:
                case 7:
                case 8:
                case 9:
                case 10:
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(0.0f, -4.0f);
                    break;
                case 5:
                case 6:
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(0.0f, 0.0f);
                    break;
                default:
                    break;
            }
        }
    }

    private void ClearPlayers()
    {
        Player[] players = Resources.FindObjectsOfTypeAll<Player>();
        foreach (Player player in players) Destroy(player.gameObject);
    }
}
