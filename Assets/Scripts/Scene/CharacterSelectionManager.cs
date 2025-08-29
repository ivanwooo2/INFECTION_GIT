using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CharacterSelectionManager : MonoBehaviour
{
    private MenuNavigationSystem navigationSystem;

    [SerializeField] private bool isReturn;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    private int selectedIndex = -1;
    private bool isCharacterSelected = false;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip confiem, back;

    [Header("角色資料")]
    public CharacterData[] characterDatas;

    [Header("UI 參考")]
    public Button[] characterButtons;
    public Button[] characterBackGround;
    public Image[] SkillIcon;
    public TextMeshProUGUI CRdescriptionText;
    public TextMeshProUGUI SkilldescriptionText;
    public Button nextStageButton;

    private void Start()
    {
        Time.timeScale = 1;
        transition = FindAnyObjectByType<Animator>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InitializeButtons();
        if (nextStageButton != null)
        {
            nextStageButton.interactable = false;
        }
        foreach (Button bgButton in characterBackGround)
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
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => ShowCharacterDescription(index));

            UpdateButtonAppearance(index);
        }
    }

    private void UpdateButtonAppearance(int index)
    {
        if (index < characterDatas.Length)
        {
            Image Skill = SkillIcon[index].GetComponent<Image>();
            Image buttonBackGround = characterBackGround[index].GetComponent<Image>();
            Image buttonImage = characterButtons[index].GetComponent<Image>();
            buttonImage.sprite = characterDatas[index].isUnlocked ?
                characterDatas[index].unlockedSprite :
                characterDatas[index].lockedSprite;
            buttonBackGround.sprite = characterDatas[index].backGround;
            Skill.sprite = characterDatas[index].SkillIcon;
        }
    }

    public void ShowCharacterDescription(int characterIndex)
    {
        if (characterIndex < characterDatas.Length)
        {
            CRdescriptionText.text = characterDatas[characterIndex].isUnlocked ?
                $"<b>{characterDatas[characterIndex].characterName}</b>\n\n{characterDatas[characterIndex].CRdescription}" :
                "キャラクターがロック解除されていません";

            SkilldescriptionText.text = characterDatas[characterIndex].isUnlocked ?
                $"{characterDatas[characterIndex].Skilldescription}" :
                "キャラクターがロック解除されていません";

            if (selectedIndex >= 0 && selectedIndex < characterButtons.Length)
            {
                characterButtons[selectedIndex].GetComponent<Image>().color = new Color(1f,1f,1f,0.5f);
                characterBackGround[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
                SkillIcon[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }
            AudioSource.clip = confiem;
            AudioSource.Play();
            characterButtons[characterIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f,1f);
            characterBackGround[characterIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            SkillIcon[characterIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            selectedIndex = characterIndex;

            bool isUnlocked = characterDatas[characterIndex].isUnlocked;
            isCharacterSelected = isUnlocked;

            if (nextStageButton != null)
            {
                nextStageButton.interactable = isUnlocked;
                nextStageButton.GetComponent<Image>().color = isUnlocked ?
                    new Color(1f, 1f, 1f, 1f) :
                    new Color(1f, 1f, 1f, 0.1f);
            }
        }
    }

    public void NextStageScene()
    {
        if (isCharacterSelected && selectedIndex >= 0)
        {
            AudioSource.clip = confiem;
            AudioSource.Play();
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedIndex);
            PlayerPrefs.SetString("SelectedCharacterName", characterDatas[selectedIndex].characterName);
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
        if (isReturn == true)
        {
            SceneManager.LoadScene("diffcultySelectScene");
        }
        else if (isReturn == false)
        {
            SceneManager.LoadScene("LevelSelectScene");
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            NextStageScene();
        }
        else if(Input.GetButtonDown("Cancel"))
        {
            TitleScene();
        }
    }
}
