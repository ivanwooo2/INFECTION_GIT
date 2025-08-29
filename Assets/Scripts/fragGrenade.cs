using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragGrenade : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float minMoveDistance = 3f;
    public float maxMoveDistance = 8f;

    public float explosionRadius = 1.5f;
    public GameObject fragmentPrefab;
    public GameObject bombeffectPrefab;
    public int fragmentCount = 5;
    public float fragmentSpeed = 6f;
    private SpriteRenderer SpriteRendererChild;
    private GameObject frag;

    public Vector2 moveDirection;
    private float moveDistance;
    private Vector2 spawnPosition;
    public bool hasExploded = false;

    public int attackPhases = 2;
    public float phaseInterval = 0.3f;
    public float[] phaseAngles = { 0f, 90f };

    public AudioSource kuna;
    public AudioClip sfx1;

    private ProjectileManagerRandom projectileManager;
    void Start()
    {
        SpriteRendererChild = GetComponentInChildren<SpriteRenderer>();
        projectileManager = FindObjectOfType<ProjectileManagerRandom>();
        moveDirection = GetInitialDirection();
        moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        spawnPosition = transform.position;
        frag = transform.Find("Frag").gameObject;
    }

    Vector2 GetInitialDirection()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.x < 0.5f ? Vector2.right : Vector2.left;
    }

    void Update()
    {
        if (hasExploded) return;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(spawnPosition, transform.position) >= moveDistance)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;
        GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(MultiPhaseExplosion());

        frag.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
    }

    IEnumerator MultiPhaseExplosion()
    {
        for (int phase = 0; phase < attackPhases; phase++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            kuna.clip = sfx1;
            kuna.Play();
            SpawnPhaseFragments(phase);
            yield return new WaitForSeconds(phaseInterval);
        }
        Destroy(gameObject);
    }

    void SpawnPhaseFragments(int phaseIndex)
    {
        float baseAngle = phaseAngles[phaseIndex % phaseAngles.Length];
        float angleStep = 360f / fragmentCount;

        for (int i = 0; i < fragmentCount; i++)
        {
            float angle = baseAngle + i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject fragment = Instantiate(
                fragmentPrefab,
                transform.position,
                Quaternion.identity
            );

            GameObject bombeffect = Instantiate(
            bombeffectPrefab,
            transform.position,
            Quaternion.identity
            );

            if (projectileManager != null)
            {
                projectileManager.RegisterFragment(fragment);
            }

            Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
            rb.velocity = dir * fragmentSpeed;
        }
    }

    public void InitializeDirection(bool isLeftSide)
    {
        moveDirection = isLeftSide ? Vector2.right : Vector2.left;
        spawnPosition = transform.position;

        Debug.DrawLine(spawnPosition,
            spawnPosition + moveDirection * moveDistance,
            Color.red, 2f);
    }
}
