using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossManagerTutorial : MonoBehaviour
{
    [SerializeField] private GameObject projectileManager;

    [SerializeField] private GameObject playerDamageEffect;

    private TimeManager TimeManager;
    [SerializeField] public GameObject BossPreFab;
    private Animator BossAnimator;
    private GameObject Boss;
    [SerializeField] private GameObject LastBossPrefab;
    [SerializeField] private GameObject Player1Attack;
    [SerializeField] public float minInterval;
    [SerializeField] public float maxInterval;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float stayDuration;

    [SerializeField] public Vector2 initialSpawnViewport;
    [SerializeField] public Vector2 initialTargetViewport;

    [SerializeField] public float verticalMoveDistance;
    [SerializeField] public float horizontalMoveDistance;

    [SerializeField] private Image[] healthBars;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private float maxHealth = 300;
    [SerializeField] public float currentHealth;
    private bool isPlayerInDamageArea;
    [SerializeField] public float damage;
    private float originalDamage;
    private float doubleDamage;
    [SerializeField] private float damageMult;
    [SerializeField] private float damageInterval;
    [SerializeField] private float SkilldamageInterval;
    [SerializeField] private float originDamageInterval;
    [SerializeField] private float activeDamageInterval;
    private float i;

    [SerializeField] private TMP_Text healthText;

    [SerializeField] private GameObject weakPointPrefab;
    [SerializeField] private float weakPointDuration = 5f;
    [SerializeField] private int weakPointDamage = 50;
    [SerializeField] private float dashCheckDistance = 3f;
    [SerializeField] private Vector2[] weakPointOffsets;

    private TimeManager timeManager;

    private GameObject currentWeakPoint;
    private bool isWeakPointActive;
    private Transform playerTransform;
    public AudioSource arc, arc2;
    public AudioClip attack1, attack2, DestroySE;

    public GameObject currentBoss;

    private GameObject LastBoss;
    private PlayerHealth playerHealth;
    private bool isOperating;

    [SerializeField] GameObject Crossfade;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;
    [SerializeField] GameObject BossDestroyPrefab;
    [SerializeField] int randomx;
    [SerializeField] int randomy;
    [SerializeField] float EffectrandomZ;
    private bool isPhase2 = false;
    private bool phase2Triggered = false;
    public bool IsBossDead { get; private set; } = false;
    public bool WeakPointHited = false;
    void Start()
    {
        transition = Crossfade.GetComponent<Animator>();
        transition.SetBool("Start", false);
        originDamageInterval = damageInterval;
        activeDamageInterval = originDamageInterval;
        TimeManager = GetComponent<TimeManager>();
        originalDamage = damage;
        doubleDamage = damage * damageMult;
        timeManager = TimeManager.Instance;
        InitializeHealthSystem();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
    }

    void InitializeHealthSystem()
    {
        currentHealth = 0;
        healthCanvas.gameObject.SetActive(true);
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        int filledBars = Mathf.FloorToInt((float)currentHealth / maxHealth * healthBars.Length);
        filledBars = Mathf.Clamp(filledBars, 0, healthBars.Length);

        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].gameObject.SetActive(i < filledBars);
        }

    }

    public void SpawnBoss()
    {
        StartCoroutine(InitialEntrance());
    }

    IEnumerator InitialEntrance()
    {
        isOperating = true;
        currentBoss = Instantiate(BossPreFab);
        Boss = currentBoss.transform.Find("Boss").gameObject;
        BossAnimator = currentBoss.GetComponent<Animator>();
        UpdateHealthDisplay();
        Vector3 startPos = ViewportToWorld(initialSpawnViewport);
        Vector3 targetPos = ViewportToWorld(initialTargetViewport);

        currentBoss.transform.position = startPos;
        yield return StartCoroutine(MoveTo(targetPos));
        yield return null;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        if (timeManager.isGameOver == true)
        {
            yield return null;
        }
        else
        {
            Vector3 startPos = currentBoss.transform.position;
            float progress = 0;

            while (progress < 1)
            {
                progress += Time.deltaTime * moveSpeed;
                currentBoss.transform.position = Vector3.Lerp(startPos, target, progress);
                yield return null;
            }
        }
    }

    Vector3 ViewportToWorld(Vector2 viewportPos)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 10));
    }

    public void OnBossTriggerEnter(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BossAnimator.SetBool("isHitting", true);
            isPlayerInDamageArea = true;
            StartCoroutine(ApplyDamage());
        }
    }

    public void OnBossTriggerExit(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BossAnimator.SetBool("isHitting", false);
            isPlayerInDamageArea = false;
        }
    }

    IEnumerator ApplyDamage()
    {
        while (isPlayerInDamageArea && currentHealth < maxHealth)
        {
            if (currentBoss != null)
            {
                GameObject Player1attack = Instantiate(Player1Attack, playerTransform.position, Quaternion.identity);
                Vector3 direction = currentBoss.transform.position - playerTransform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Player1attack.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                arc.clip = attack1;
                arc.Play();
                GameObject EffectClone = Instantiate(playerDamageEffect, currentBoss.transform);
                EffectClone.transform.rotation = Quaternion.Euler(0, 0, EffectrandomZ);
                Boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                currentHealth += damage;
                UpdateHealthDisplay();
                yield return new WaitForSeconds(activeDamageInterval / 2);
                Boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                if (currentHealth >= maxHealth)
                {
                    StartCoroutine(DestroyBoss());
                }

                yield return new WaitForSeconds(activeDamageInterval / 2);
            }

        }
    }

    IEnumerator DestroyBoss()
    {
        IsBossDead = true;
        timeManager.isGameOver = true;
        playerHealth.isInvincible = true;
        if (projectileManager != null)
        {
            var manager = projectileManager.GetComponent<ProjectileManagerRandom>();
            if (manager != null)
            {
                manager.CleanupProjectiles();
            }
        }
        if (currentBoss != null)
        {
            Destroy(currentBoss);
        }
        LastBoss = Instantiate(LastBossPrefab);
        LastBoss.transform.position = new Vector3(0, 0, 0);
        GameObject LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.9f);
        arc.clip = DestroySE;
        arc.Play();
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        yield return new WaitForSeconds(0.3f);
        LastBoss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
        arc.clip = DestroySE;
        arc.Play();
        LastBossDestroyEffect = Instantiate(BossDestroyPrefab);
        LastBossDestroyEffect.transform.position = new Vector3(randomx, randomy, 0);
        playerHealth.isInvincible = true;
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        timeManager.LoadResultScene();
        yield return null;
    }

    public void ChrSkill1(float duration)
    {
        if (duration > 0)
        {
            i = duration;
            damage = doubleDamage;
            activeDamageInterval = SkilldamageInterval;
        }
    }

    void DealWeakPointDamage()
    {
        currentHealth = Mathf.Clamp(currentHealth + weakPointDamage, 0, maxHealth);

        UpdateHealthDisplay();

        if (currentHealth >= maxHealth)
        {
            StartCoroutine(DestroyBoss());
        }

        // AudioManager.Instance.Play("WeakPointHit");
    }

    public void RandomSpawnWeakPoit()
    {
        StartCoroutine(WeakPointSpawnLoop());
    }

    IEnumerator WeakPointSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 13f));
            if (!isWeakPointActive && currentBoss != null)
            {
                SpawnClosestWeakPoint();
            }
        }
    }

    public void SpawnClosestWeakPoint()
    {
        if (currentBoss == null) return;

        Vector2 closestOffset = Vector2.zero;
        float minDistance = Mathf.Infinity;

        foreach (Vector2 offset in weakPointOffsets)
        {
            Vector3 worldPos = currentBoss.transform.position + new Vector3(offset.x, offset.y, 0);
            float distance = Vector3.Distance(playerTransform.position, worldPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestOffset = offset;
            }
        }

        currentWeakPoint = Instantiate(weakPointPrefab, currentBoss.transform);
        currentWeakPoint.transform.localPosition = closestOffset;
        isWeakPointActive = true;
        if (WeakPointHited)
        {
            StartCoroutine(RemoveWeakPointAfterDelay());
        }
    }

    IEnumerator RemoveWeakPointAfterDelay()
    {
        yield return new WaitForSeconds(weakPointDuration);
        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }
    }

    void TryAttackWeakPoint()
    {
        if (isWeakPointActive && currentWeakPoint != null)
        {
            float distance = Vector3.Distance(
                playerTransform.position,
                currentWeakPoint.transform.position
            );

            if (distance <= dashCheckDistance)
            {
                arc2.clip = attack2;
                arc2.Play();
                GameObject WeakPointEffect = Instantiate(BossDestroyPrefab);
                WeakPointEffect.transform.position = currentWeakPoint.transform.position;
                DealWeakPointDamage();
                Destroy(currentWeakPoint);
                isWeakPointActive = false;
                WeakPointHited = true;
            }
        }
    }

    void Update()
    {
        float percentage = (currentHealth * 100 / maxHealth);
        healthText.SetText($"{Mathf.RoundToInt(percentage)} %");

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Weakpoint"))
        {
            TryAttackWeakPoint();
        }

        if (i > 0)
        {
            i -= Time.deltaTime;
        }

        if (i <= 0)
        {
            damage = originalDamage;
            activeDamageInterval = originDamageInterval;
        }

        if (currentHealth >= 1000)
        {
            currentHealth = 999;
        }

        randomx = Random.Range(-3, 3);
        randomy = Random.Range(-3, 3);
        EffectrandomZ = Random.Range(-360, 360);
    }
}
