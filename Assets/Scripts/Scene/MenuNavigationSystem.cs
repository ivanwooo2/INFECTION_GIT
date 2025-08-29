using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuNavigationSystem : MonoBehaviour
{
    [Header("Navigation Settings")]
    public float navigationDelay = 0.2f;

    private List<Selectable> navigableItems = new List<Selectable>();
    private int currentIndex = 0;
    private float lastNavigationTime = 0f;
    private bool isUsingController = false;

    private void Start()
    {
        List<Selectable> allItems = new List<Selectable>();
        foreach (Selectable selectable in GetComponentsInChildren<Selectable>(true))
        {
            if (selectable.interactable && selectable.GetComponent<IgnoreNavigation>() == null)
            {
                allItems.Add(selectable);
            }
        }

        navigableItems = allItems;

        if (navigableItems.Count > 0)
        {
            
        }
    }

    private void Update()
    {
        if (navigableItems.Count == 0) return;

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            isUsingController = true;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ||
                 Input.GetMouseButtonDown(2) || Input.mousePosition != Input.mousePosition)
        {
            isUsingController = false;
        }

        if (!isUsingController) return;

        if (Time.time - lastNavigationTime < navigationDelay) return;

        HandleNavigationInput();
    }

    private void HandleNavigationInput()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0.5f)
        {
            Navigate(-1);
        }
        else if (vertical < -0.5f)
        {
            Navigate(1);
        }
        else if (horizontal > 0.5f)
        {
            Navigate(1);
        }
        else if (horizontal < -0.5f)
        {
            Navigate(-1);
        }
    }

    private void Navigate(int direction)
    {
        int newIndex = currentIndex + direction;

        if (newIndex < 0)
        {
            newIndex = navigableItems.Count - 1;
        }
        else if (newIndex >= navigableItems.Count)
        {
            newIndex = 0;
        }

        currentIndex = newIndex;

        TriggerSelectionEvent();

        lastNavigationTime = Time.time;
    }

    private void TriggerSelectionEvent()
    {
        Button currentButton = navigableItems[currentIndex] as Button;
        if (currentButton != null)
        {
            currentButton.onClick.Invoke();
        }
    }

    private void SubmitCurrentSelection()
    {
        Button currentButton = navigableItems[currentIndex] as Button;
        if (currentButton != null)
        {
            currentButton.onClick.Invoke();
        }
    }
}