using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialCheckerTextBoxManager : MonoBehaviour
{
    private TutorialCheckManager tutorialCheckManager;
    private TMP_Text TutorialCheck;
    void Start()
    {
        tutorialCheckManager = FindObjectOfType<TutorialCheckManager>();
        TutorialCheck = GetComponent<TMP_Text>();
    }

    public void Switch()
    {
        if (tutorialCheckManager.TutorialDone == true)
        {
            tutorialCheckManager.TutorialDone = false;
        }
        else if (tutorialCheckManager.TutorialDone == false)
        {
            tutorialCheckManager.TutorialDone = true;
        }
    }

    void Update()
    {
        if (tutorialCheckManager.TutorialDone == true)
        {
            TutorialCheck.text = "Tutorial Status:Done";
        }
        else if (tutorialCheckManager.TutorialDone == false)
        {
            TutorialCheck.text = "Tutorial Status:unDone";
        }
    }
}
