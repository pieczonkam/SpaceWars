using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance = null;

    private AudioManager audioManager;

    private float timeElapsed = 0.0f;
    private float extraOnLevelClearDelay = 0.0f;

    [HideInInspector] public int lastLevel = 10;
    [HideInInspector] public int currentLevel = 0;
    [HideInInspector] public bool twoPlayersMode = false;
    [HideInInspector] public int player01MaxWeaponLevel = 0;
    [HideInInspector] public int player01NumberOfLives = 0;
    [HideInInspector] public int player01WeaponUpgrade = 0;
    [HideInInspector] public int player01NumberOfCoins = 0;
    [HideInInspector] public int player02MaxWeaponLevel = 0;
    [HideInInspector] public int player02NumberOfLives = 0;
    [HideInInspector] public int player02WeaponUpgrade = 0;
    [HideInInspector] public int player02NumberOfCoins = 0;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float onLevelClearDelay = 2.0f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        ResetEnemies();
    }

    private void Update()
    {
        if (currentLevel != 0)
        {
            extraOnLevelClearDelay = currentLevel == 10 ? 3.0f : 0.0f;
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            Boss boss = FindObjectOfType<Boss>();

            if (enemies != null && enemies.Length == 0 && boss == null)
            {
                InGameMenu inGameMenu = FindObjectOfType<InGameMenu>();

                if (timeElapsed < onLevelClearDelay + extraOnLevelClearDelay)
                {
                    if (currentLevel == lastLevel)
                        audioManager.StopTheme();
                    SetPlayersImmortal(true);
                    if (inGameMenu != null)
                    {
                        inGameMenu.ShowLevelCompleteMenu(true);
                        if (currentLevel == lastLevel)
                            inGameMenu.UpdateLevelCompleteText("BOSS DEFEATED\nYOU WIN!\n\nFINAL SCORE: " + (player01NumberOfCoins + player02NumberOfCoins).ToString());
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
                            audioManager.PlayTheme("BossLevelTheme");
                        SceneManager.LoadScene(currentLevel);
                    }
                    else
                    {
                        currentLevel = 0;
                        ResetEnemies();
                        ClearPlayers();
                        audioManager.PlayTheme("MainMenuTheme");
                        SceneManager.LoadScene(0);
                    }
                }
            }
        }

        Debug.Log("FPS: " + (1.0f / Time.smoothDeltaTime).ToString() + " at " + SceneManager.GetActiveScene().name);
    }

    private void SetPlayersImmortal(bool state)
    {
        GameObject player01 = GameObject.Find("Player01");
        GameObject player02 = GameObject.Find("Player02");

        if (player01 != null)
            player01.GetComponent<Player>().immortal = state;
        if (twoPlayersMode && player02 != null)
            player02.GetComponent<Player>().immortal = state;
    }

    private void SwitchPlayersLighting()
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
                if (twoPlayersMode && player02NumberOfLives > 0)
                    GameObject.Find("Player02").GetComponent<Player>().SwitchLighting(false);
                break;
            case 7:
            case 8:
            case 9:
                if (player01NumberOfLives > 0)
                    GameObject.Find("Player01").GetComponent<Player>().SwitchLighting(true);
                if (twoPlayersMode && player02NumberOfLives > 0)
                    GameObject.Find("Player02").GetComponent<Player>().SwitchLighting(true);
                break;
            default:
                break;
        }
    }

    private void SetPlayersPosition()
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
                if (twoPlayersMode)
                {
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(-3.0f, -4.0f);
                    GameObject.Find("Player02").GetComponent<Player>().SetPosition(3.0f, -4.0f);
                }
                else
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(0.0f, -4.0f);
                break;
            case 5:
            case 6:
                if (twoPlayersMode)
                {
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(-1.0f, 0.0f);
                    GameObject.Find("Player02").GetComponent<Player>().SetPosition(1.0f, 0.0f);
                }
                else
                    GameObject.Find("Player01").GetComponent<Player>().SetPosition(0.0f, 0.0f);
                break;
            default:
                break;
        }
    }

    public void ClearPlayers()
    {
        Player[] players = Resources.FindObjectsOfTypeAll<Player>();
        foreach (Player player in players) 
            Destroy(player.gameObject);
    }

    public void ResetEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.destroyOffScreen = false;
            enemyScript.movementLoop = true;
            enemyScript.velocity = 0.0f;
            enemyScript.velocityDirection = new Vector2(1.0f, 0.0f);
        }
    }
}
