using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TutorialCheckManager : MonoBehaviour
{
    public static TutorialCheckManager instance;

    public bool TutorialDone = false;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && TutorialDone == false)
        {
            TutorialDone = true;
        }
        else if (Input.GetKeyDown(KeyCode.T) && TutorialDone == true)
        {
            TutorialDone = false;
        }
    }
}
