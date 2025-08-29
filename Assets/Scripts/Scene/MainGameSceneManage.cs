using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameSceneManage : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] bool isPausing = false;
    [SerializeField] GameObject BGM;
    [SerializeField] GameObject SE;
    [SerializeField] AudioSource BGMsource;
    [SerializeField] AudioSource SEsource;
    [SerializeField] GameObject Crossfade;
    [SerializeField] Animator transition;
    public bool isTutorialPause = false;
    void Start()
    {
        transition = Crossfade.GetComponent<Animator>();
        BGMsource = BGM.GetComponent<AudioSource>();
        SEsource = SE.GetComponent<AudioSource>();
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isTutorialPause)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isPausing || Input.GetButtonDown("Pause") && !isPausing)
            {
                BGMsource.Pause();
                SEsource.Pause();
                Cursor.visible = true;
                isPausing = true;
                pausePanel.SetActive(true);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isPausing || Input.GetButtonDown("Pause") && isPausing)
            {
                BGMsource.Play();
                SEsource.Play();
                Cursor.visible = false;
                isPausing = false;
                pausePanel.SetActive(false);
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Restart()
    {
        TimeManager gameManager = FindObjectOfType<TimeManager>();
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
        }

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ResetTimer();
        }
        transition.SetBool("Start", false);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Title()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
