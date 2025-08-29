using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MainMenuController : MonoBehaviour
{
    private MenuNavigationSystem navigationSystem;

    private int selectedIndex;
    [SerializeField] GameObject mainmenu;
    [SerializeField] private GameObject Panel;

    [SerializeField] GameObject settingPanel;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle fullscreenToggle;

    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip confiem,back,StartButton;

    public bool isTutorialSwitcherOpen = false;
    void Start()
    {
        TimeManager gameManager = FindObjectOfType<TimeManager>();
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
        }
        Time.timeScale = 1;
        transition.SetBool("Start", false);
        transition = FindAnyObjectByType<Animator>();
        Cursor.lockState = CursorLockMode.None;
        navigationSystem = gameObject.AddComponent<MenuNavigationSystem>();
    }

    void LoadSetting()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
    public void PlayButton()
    {
        AudioSource.clip = confiem;
        AudioSource.Play();
        TimeManager gameManager = FindObjectOfType<TimeManager>();
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
        }
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("diffcultySelectScene");
    }

    public void Setting()
    {
        mainmenu.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        mainmenu.SetActive(true);
        settingPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void onSetVolume(float vol)
    {
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("MasterVolume", vol);
    }

    public void onSetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            PlayButton();
        }
    }
}
