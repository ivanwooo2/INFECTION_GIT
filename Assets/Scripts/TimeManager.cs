using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] public static float totalTime = 300f;
    public static float currentTime;
    public bool isGameOver = false;
    [SerializeField] private TMP_Text timerText;

    private GameObject player;
    private PlayerHealth playerHealth;
    public bool isTutorial = false;

    public static bool IsSkillPaused { get; private set; } = false;
    private static float pauseEndTime = 0f;
    private static float pauseDuration = 0f;

    public float CurrentTime => currentTime;
    public static float LastRemainingTime { get; private set; }
    public static float LastTotalTime { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        currentTime = totalTime;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (IsSkillPaused && Time.realtimeSinceStartup >= pauseEndTime)
        {
            IsSkillPaused = false;
            var projectileManager = FindObjectOfType<ProjectileManagerRandom>();
            if (projectileManager != null)
            {
                projectileManager.SkillCleanupProjectiles();
            }
            var stage2projectileManager = FindObjectOfType<Stage2ProjectileManager>();
            if (stage2projectileManager != null)
            {
                stage2projectileManager.SkillCleanupProjectiles();
            }
        }

        if (IsSkillPaused) return;

        if (!isGameOver && SceneManager.GetActiveScene().name != "ResultScene" && isTutorial == false)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                LoadResultScene();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadResultScene()
    {
        isGameOver = true;
        LastRemainingTime = currentTime;
        LastTotalTime = totalTime;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("LastLevel", currentLevelIndex);
        var projectileManager = FindObjectOfType<ProjectileManagerRandom>();
        if (projectileManager != null)
        {
            projectileManager.CleanupProjectiles();
        }
        var stage2projectileManager = FindObjectOfType<Stage2ProjectileManager>();
        if (stage2projectileManager != null)
        {
            stage2projectileManager.CleanupProjectiles();
        }

        BossController bossController = FindObjectOfType<BossController>();
        Stage2BossController stage2BossController = FindObjectOfType<Stage2BossController>();
        if (bossController != null && bossController.IsBossDead)
        {
            SceneManager.LoadScene("ResultScene");
            return;
        }

        if (stage2BossController != null && stage2BossController.IsBossDead)
        {
            SceneManager.LoadScene("ResultScene");
            return;
        }

        if (playerHealth.currentHealth > 0 && currentTime > 0)
        {
            SceneManager.LoadScene("ResultScene");
        }
        else if (currentTime <= 0)
        {
            SceneManager.LoadScene("GameOverScene");
        }
        else
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public static void TriggerSkillPause(float duration)
    {
        IsSkillPaused = true;
        pauseDuration = duration;
        pauseEndTime = Time.realtimeSinceStartup + duration;
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        isGameOver = false;
    }
}
