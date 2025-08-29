using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScene : MonoBehaviour
{
    private MenuNavigationSystem navigationSystem;

    [SerializeField] private bool isReturn;
    [SerializeField] private TMP_Text resultTimeText;

    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip confiem, back;

    private void Start()
    {
        transition = FindAnyObjectByType<Animator>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (TimeManager.Instance != null)
        {
            float remainingTime = TimeManager.Instance.CurrentTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            resultTimeText.text = $"{minutes:00}:{seconds:00}";

            Destroy(TimeManager.Instance.gameObject);
        }
        navigationSystem = gameObject.AddComponent<MenuNavigationSystem>();
    }

    public void RestartGame()
    {
        AudioSource.clip = confiem;
        AudioSource.Play();
        if (TimeManager.Instance != null)
        {
            Destroy(TimeManager.Instance.gameObject);
            TimeManager.Instance.ResetTimer();
        }
        isReturn = false;
        StartCoroutine(SceneMove());
    }

    public void Title()
    {
        AudioSource.clip = back;
        AudioSource.Play();
        isReturn = true;
        StartCoroutine(SceneMove());
    }

    IEnumerator SceneMove()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        if (isReturn == true)
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (isReturn == false)
        {
            int lastLevel = PlayerPrefs.GetInt("LastLevel");
            SceneManager.LoadScene(lastLevel);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            RestartGame();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            Title();
        }
    }
}
