using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarningProjectileController : MonoBehaviour
{
    [SerializeField] private GameObject warningIndicator;
    [SerializeField] private GameObject[] Projectiles;
    [SerializeField] private GameObject[] warningReds;
    [SerializeField] private float flashInterval = 0.2f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private float redInterval;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float projectileSpacing = 1f;

    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float originprojectileSpeed;
    [SerializeField] private float aliveTime = 3f;
    [SerializeField] private int damage = 1;

    private SpriteRenderer warningRenderer;
    private bool isActivated = false;
    [SerializeField] private float downwardRotation;
    [SerializeField] private float leftwardRotation;

    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    public AudioSource bow;
    public AudioClip sfx1,sfx2;

    private Rigidbody2D[] projectileRigidbodies;
    private Vector3[] initialScales;

    private Vector2 GoingDir;
    private bool isGoing;

    void Awake()
    {
        originprojectileSpeed = projectileSpeed;
        warningRenderer = warningIndicator.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();

        initialScales = new Vector3[warningReds.Length];
        for (int i = 0; i < warningReds.Length; i++)
        {
            initialScales[i] = warningReds[i].transform.localScale;
        }

        projectileRigidbodies = new Rigidbody2D[Projectiles.Length];
        for (int i = 0; i < Projectiles.Length; i++)
        {
            projectileRigidbodies[i] = Projectiles[i].GetComponent<Rigidbody2D>();
        }
    }

    void Start()
    {
        bow.clip = sfx1;
        bow.Play();
        foreach (var proj in Projectiles)
        {
            proj.SetActive(false);
        }

        InitializePosition();
        StartCoroutine(ActivationSequence());
    }

    void InitializePosition()
    {
        Vector2 spawnPos = GetEdgeSpawnPosition();
        transform.position = spawnPos;

        ArrangeProjectiles();
        ArrangeWarningReds();
    }

    void ArrangeWarningReds()
    {
        Vector2 direction = GetLaunchDirection();
        int count = warningReds.Length;
        float spacing = projectileSpacing;

        if (direction == Vector2.down)
        {
            for (int i = 0; i < count; i++)
            {
                int offsetIndex = i - (count - 1) / 2;
                Vector3 position = new Vector3(offsetIndex * spacing, 0, 0);
                warningReds[i].transform.localPosition = position;
            }
        }
        else if (direction == Vector2.left || direction == Vector2.right)
        {
            for (int i = 0; i < count; i++)
            {
                int offsetIndex = i - (count - 1) / 2;
                Vector3 position = new Vector3(0, offsetIndex * spacing, 0);
                warningReds[i].transform.localPosition = position;
            }
        }
    }

    void ArrangeProjectiles()
    {
        Vector2 direction = GetLaunchDirection();
        int count = Projectiles.Length;
        float spacing = projectileSpacing;

        if (direction == Vector2.down)
        {
            for (int i = 0; i < count; i++)
            {
                int offsetIndex = i - (count - 1) / 2;
                Vector3 position = new Vector3(offsetIndex * spacing, 0, 0);
                Projectiles[i].transform.localPosition = position;
            }
        }
        else if (direction == Vector2.left || direction == Vector2.right)
        {
            for (int i = 0; i < count; i++)
            {
                int offsetIndex = i - (count - 1) / 2;
                Vector3 position = new Vector3(0, offsetIndex * spacing, 0);
                Projectiles[i].transform.localPosition = position;
            }
        }
    }

    IEnumerator ActivationSequence()
    {
        float totalWarningTime = flashInterval * 2 * flashCount;

        StartCoroutine(ScaleAllWarningReds(totalWarningTime));

        for (int i = 0; i < flashCount; i++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            warningRenderer.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
            warningRenderer.color = new Color(0, 0, 0, 0);
            yield return new WaitForSeconds(flashInterval);
        }

        isActivated = true;
        warningIndicator.SetActive(false);

        foreach (var red in warningReds)
        {
            red.SetActive(false);
        }

        foreach (var proj in Projectiles)
        {
            proj.SetActive(true);
        }

        LaunchProjectile();
    }

    IEnumerator ScaleAllWarningReds(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            float progress = elapsedTime / duration;

            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            for (int i = 0; i < warningReds.Length; i++)
            {
                if (warningReds[i] != null && warningReds[i].activeSelf)
                {
                    float newYScale = Mathf.Lerp(initialScales[i].y, 0, easedProgress);
                    warningReds[i].transform.localScale = new Vector3(
                        initialScales[i].x,
                        newYScale,
                        initialScales[i].z
                    );
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < warningReds.Length; i++)
        {
            if (warningReds[i] != null)
            {
                warningReds[i].transform.localScale = new Vector3(
                    initialScales[i].x,
                    0,
                    initialScales[i].z
                );
            }
        }
    }

    void LaunchProjectile()
    {
        Vector2 direction = GetLaunchDirection();
        GoingDir = direction;
        bow.clip = sfx2;
        bow.Play();

        for (int i = 0; i < Projectiles.Length; i++)
        {
            if (direction == Vector2.down)
            {
                Projectiles[i].transform.rotation = Quaternion.Euler(0, 0, downwardRotation);
            }
            else if (direction == Vector2.left)
            {
                Projectiles[i].transform.rotation = Quaternion.Euler(0, 0, leftwardRotation);
            }
            else if (direction == Vector2.right)
            {
                Projectiles[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (projectileRigidbodies[i] != null)
            {
                StartCoroutine(Fire(i));
            }
        }
    }

    Vector2 GetLaunchDirection()
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x < 0.1f) return Vector2.right;
        if (viewportPos.x > 0.9f) return Vector2.left;
        if (viewportPos.y > 0.8f) return Vector2.down;
        return Vector2.zero;
    }

    Vector2 GetEdgeSpawnPosition()
    {
        int side = Random.Range(0, 3);
        Vector2 viewportPos = Vector2.zero;

        switch (side)
        {
            case 0:
                viewportPos = new Vector2(0.05f, Random.Range(0.2f, 0.8f));
                break;
            case 1:
                viewportPos = new Vector2(0.95f, Random.Range(0.2f, 0.8f));
                break;
            case 2:
                viewportPos = new Vector2(Random.Range(0.2f, 0.8f), 0.85f);
                break;
        }

        if (side == 2)
        {
            foreach (var red in warningReds)
            {
                if (red != null)
                {
                    red.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }
        }

        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && other.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    IEnumerator Fire(int number)
    {
        while (true)
        {
            while (TimeManager.IsSkillPaused)
            {
                projectileRigidbodies[number].velocity = GoingDir * 0;
                yield return null;
            }
            if (isGoing) yield return null;
            isGoing = true;
            projectileRigidbodies[number].velocity = GoingDir * projectileSpeed;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
