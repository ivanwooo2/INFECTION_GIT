using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultImageController : MonoBehaviour
{
    [System.Serializable]
    public class TimeBasedImage
    {
        [Range(0, 1)] public float minPercentage;
        public Sprite displaySprite;
    }

    [SerializeField] private Image targetImage;
    [SerializeField] private TimeBasedImage[] timeImages;

    [SerializeField] private Sprite defaultSprite;

    private void Start()
    {
        float remainingTime = TimeManager.LastRemainingTime;
        float totalTime = TimeManager.LastTotalTime;
        float timePercentage = totalTime > 0 ? remainingTime / totalTime : 0;
        Sprite selectedSprite = defaultSprite;

        System.Array.Sort(timeImages, (a, b) => b.minPercentage.CompareTo(a.minPercentage));

        foreach (var timeImage in timeImages)
        {
            if (timePercentage >= timeImage.minPercentage)
            {
                selectedSprite = timeImage.displaySprite;
                break;
            }
        }

        targetImage.sprite = selectedSprite;
        targetImage.preserveAspect = true;

        if (TimeManager.Instance != null)
        {
            Destroy(TimeManager.Instance.gameObject);
        }
    }
}
