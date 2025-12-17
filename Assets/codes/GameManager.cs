using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Global access point
    public static GameManager Instance { get; private set; }

    [Header("Kill Settings")]
    [SerializeField] private int killThreshold = 5;

    [Header("Debug")]
    [SerializeField] private int currentKillCount = 0;

    [Header("Scene Settings")]
    [SerializeField] private string endingSceneName = "GameEnding";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this when an enemy is killed.
    /// </summary>
    public void RegisterKill()
    {
        currentKillCount++;
        //Debug.Log($"Kill registered: {currentKillCount}/{killThreshold}");

        if (currentKillCount >= killThreshold)
        {
            GoToEnding();
        }
    }

    /// <summary>
    /// Reset kill count (optional – e.g., when restarting game).
    /// </summary>
    public void ResetGame()
    {
        currentKillCount = 0;
    }

    private void GoToEnding()
    {
        Debug.Log("Kill threshold reached. Loading ending scene...");
        ResetGame();
        SceneManager.LoadScene(endingSceneName);
    }

    public int CurrentKillCount => currentKillCount;
    public int KillThreshold => killThreshold;
}
