using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSceneBGMManager : MonoBehaviour
{
    public static SelectionSceneBGMManager Instance;

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
    }
}
