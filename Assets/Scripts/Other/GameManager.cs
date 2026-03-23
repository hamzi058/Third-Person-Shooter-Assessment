using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Start UI")]
    public GameObject tapToBeginPanel;
    public GameObject WinPanel;
    public GameObject LosePanel;

    [Header("Instruction Text")]
    public TextMeshProUGUI instructionText;

    [Header("Enemy Counter UI")]
    public TextMeshProUGUI enemyCounterText;

    [Header("Player Ref")]
    public PlayerMovement playerMovement;

    [Header("Enemies Count")]
    public int TotalEnemyCount;
    public int CurrentEnemyCount;

    bool gameStarted = false;
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // ⭐ start from zero → enemies will self register
        TotalEnemyCount = 0;
        CurrentEnemyCount = 0;
    }

    void Start()
    {
        if (tapToBeginPanel) tapToBeginPanel.SetActive(true);
        if (WinPanel) WinPanel.SetActive(false);
        if (LosePanel) LosePanel.SetActive(false);
        UpdateEnemyCounterUI();
        SetInstructionText();

        Time.timeScale = 0f;
    }

    void Update()
    {
        if (gameStarted) return;

        if (Input.GetMouseButtonDown(0))
            BeginGame();

        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
            BeginGame();
    }

    void BeginGame()
    {
        gameStarted = true;

        if (tapToBeginPanel)
            tapToBeginPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    void SetInstructionText()
    {
        if (!instructionText || !playerMovement) return;

        instructionText.text =
            playerMovement.controlMode ==
            PlayerMovement.ControlMode.PC
            ? "Left Mouse Click To Begin"
            : "Tap The Screen To Begin";
    }

    // ⭐ ENEMY SELF REGISTER
    public void RegisterEnemy()
    {
        TotalEnemyCount++;
        CurrentEnemyCount++;

        Debug.Log("TotalEnemyCount:" + TotalEnemyCount);

        UpdateEnemyCounterUI();
    }

    // ⭐ ENEMY DIED
    public void EnemyDied()
    {
        CurrentEnemyCount--;

        UpdateEnemyCounterUI();

        if (CurrentEnemyCount <= 0)
            WinGame();
    }

    void UpdateEnemyCounterUI()
    {
        if (enemyCounterText)
        {
            enemyCounterText.text =
                "Enemies : " +
                CurrentEnemyCount +
                " / " +
                TotalEnemyCount;
        }
    }

    // ⭐ WIN
    void WinGame()
    {
        StartCoroutine(SlowMotionWin());
    }

    System.Collections.IEnumerator SlowMotionWin()
    {
        float t = 1f;

        while (t > 0.2f)
        {
            t -= Time.unscaledDeltaTime * 2f;
            Time.timeScale = t;
            yield return null;
        }

        Time.timeScale = 0f;

        if (WinPanel)
            WinPanel.SetActive(true);
    }

    // ⭐ PLAYER DIED
    public void PlayerDied()
    {
        Time.timeScale = 0f;

        if (LosePanel)
            LosePanel.SetActive(true);
    }

    // ⭐ RESTART
    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }
}