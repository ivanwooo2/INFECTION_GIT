using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiffcultySelection : MonoBehaviour
{
    private MenuNavigationSystem navigationSystem;

    [SerializeField] private bool isReturn;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    private int selectedIndex = -1;
    private bool isDiffcultSelected = false;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip confiem, back;

    [Header("データ")]
    public DiffcultData[] diffcultDatas;

    [Header("UI")]
    public Button[] diffcultButtons;
    public Button[] diffcultBackGround;
    public Button nextSceneButton;

    private TutorialCheckManager tutorialCheckManager;

    void Start()
    {
        tutorialCheckManager = FindObjectOfType<TutorialCheckManager>();
        transition = FindAnyObjectByType<Animator>();
        Cursor.lockState = CursorLockMode.None;
        InitializeButtons();
        if (nextSceneButton != null)
        {
            nextSceneButton.interactable = false;
        }
        foreach (Button bgButton in diffcultBackGround)
        {
            if (bgButton != null && bgButton.GetComponent<IgnoreNavigation>() == null)
            {
                bgButton.gameObject.AddComponent<IgnoreNavigation>();
            }
        }
        navigationSystem = gameObject.AddComponent<MenuNavigationSystem>();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < diffcultButtons.Length; i++)
        {
            int index = i;
            diffcultButtons[i].onClick.AddListener(() => ShowLevelName(index));

            UpdateButtonAppearance(index);
        }
    }

    private void UpdateButtonAppearance(int index)
    {
        if (index < diffcultDatas.Length)
        {
            Image buttonBackGround = diffcultBackGround[index].GetComponent<Image>();
            Image buttonImage = diffcultButtons[index].GetComponent<Image>();
            buttonImage.sprite = diffcultDatas[index].isUnlocked ?
                diffcultDatas[index].unlockedSprite :
                diffcultDatas[index].lockedSprite;
            buttonBackGround.sprite = diffcultDatas[index].BackGround;
        }
    }

    public void ShowLevelName(int diffcultIndex)
    {

        if (selectedIndex >= 0 && selectedIndex < diffcultButtons.Length)
        {
            diffcultButtons[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            diffcultBackGround[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        AudioSource.clip = confiem;
        AudioSource.Play();
        diffcultButtons[diffcultIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        diffcultBackGround[diffcultIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        selectedIndex = diffcultIndex;

        bool isUnlocked = diffcultDatas[diffcultIndex].isUnlocked;
        isDiffcultSelected = isUnlocked;

        if (nextSceneButton != null)
        {
            nextSceneButton.interactable = isUnlocked;
            nextSceneButton.GetComponent<Image>().color = isUnlocked ?
                new Color(1f, 1f, 1f, 1f) :
                new Color(1f, 1f, 1f, 0.1f);
        }
    }

    public void NextStageScene()
    {
        if (isDiffcultSelected && selectedIndex >= 0)
        {
            AudioSource.clip = confiem;
            AudioSource.Play();
            PlayerPrefs.SetInt("SelectedLevelIndex", selectedIndex);
            PlayerPrefs.Save();

            isReturn = false;
            StartCoroutine(Scenemove());
        }
    }

    public void TitleScene()
    {
        AudioSource.clip = back;
        AudioSource.Play();
        isReturn = true;
        StartCoroutine(Scenemove());
    }

    IEnumerator Scenemove()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        if (tutorialCheckManager.TutorialDone == false && selectedIndex == 0)
        {
            SceneManager.LoadScene("tutorial");
            SelectionSceneBGMManager BGMmanager = FindObjectOfType<SelectionSceneBGMManager>();
            if (BGMmanager != null)
            {
                Destroy(BGMmanager.gameObject);
            }
        }
        else if (isReturn == true)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (isReturn == false)
        {
            SceneManager.LoadScene("CharacterSelectScene");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            NextStageScene();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            TitleScene();
        }
    }
}
