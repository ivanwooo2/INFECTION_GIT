using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPathController : MonoBehaviour
{
    public float lockDuration = 3f;
    private Transform player;
    private bool isLocked = false;
    private Vector3 targetPosition;
    [SerializeField] private int damage;

    void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPosition = new Vector3(transform.position.x, player.position.y, transform.position.z);
        StartCoroutine(LockAndAttack());
    }

    System.Collections.IEnumerator LockAndAttack()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed*2);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isLocked = true;

        CaptureAndSplit();
    }

    void CaptureAndSplit()
    {
        StartCoroutine(CaptureScreen());
    }

    private IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();

        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found!");
            yield break;
        }

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);

        try
        {
            Camera.main.targetTexture = rt;
            Camera.main.Render();

            Texture2D screenshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

            RenderTexture.active = rt;
            Rect safeReadRect = new Rect(0, 0, rt.width, rt.height);

            screenshot.ReadPixels(safeReadRect, 0, 0);
            screenshot.Apply();

            screenshot = VerticalFlipTexture(screenshot);

            ApplySplitEffect(screenshot);
        }
        finally
        {
            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
        }
    }

    Texture2D VerticalFlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        int width = original.width;
        int height = original.height;

        for (int y = 0; y < height; y++)
        {
            Color[] origPixels = original.GetPixels(0, y, width, 1);

            int flippedY = height - y - 1;

            flipped.SetPixels(0, flippedY, width, 1, origPixels);
        }
        flipped.Apply();
        Destroy(original);
        return flipped;
    }

    private void ApplySplitEffect(Texture2D screenshot)
    {
        Sprite fullSprite = Sprite.Create(
            screenshot,
            new Rect(0, 0, screenshot.width, screenshot.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit: 100
        );

        CreateHalfSprite(fullSprite, true);
        CreateHalfSprite(fullSprite, false);
    }

    private void CreateHalfSprite(Sprite source, bool isUpper)
    {
        Rect uvRect = new Rect(0, isUpper ? 0.5f : 0, 1, 0.5f);

        Sprite halfSprite = Sprite.Create(
            source.texture,
            new Rect(
                source.rect.x,
                source.rect.y + (isUpper ? source.rect.height / 2 : 0),
                source.rect.width,
                source.rect.height / 2
            ),
            new Vector2(0.5f, isUpper ? 1 : 0),
            source.pixelsPerUnit
        );

        GameObject halfObj = new GameObject(isUpper ? "UpperHalf" : "LowerHalf");
        SpriteRenderer sr = halfObj.AddComponent<SpriteRenderer>();
        sr.sprite = halfSprite;

        Vector3 startPos = Camera.main.transform.position + Vector3.forward * 10;
        halfObj.transform.position = startPos;
        StartCoroutine(SplitAnimation(halfObj.transform, isUpper));
    }

    private IEnumerator SplitAnimation(Transform target, bool isUpper)
    {
        float duration = 1f;
        float speed = 6f;
        Vector3 direction = isUpper ? Vector3.down : Vector3.up;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            target.Translate(direction * speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
        Color color = sr.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * 2;
            sr.color = color;
            yield return null;
        }
        Destroy(target.gameObject);
        ApplyDamage();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    private void ApplyDamage()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        Invoke("DisableCollider", 0.1f);
    }

    private void DisableCollider()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
}
