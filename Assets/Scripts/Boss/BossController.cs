using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] private GameObject projectileManager;

    [SerializeField] private GameObject playerDamageEffect;

    private TimeManager GameManager;
    [SerializeField] public GameObject BossPreFab;
    private Animator BossAnimator;
    private GameObject Boss;
    [SerializeField] private GameObject LastBossPrefab;
    [SerializeField] private GameObject Player1Attack;
    [SerializeField] private GameObject Player2Attack;
    [SerializeField] public float minInterval;
    [SerializeField] public float maxInterval;
    [SerializeField] public float moveSpeed;
    private float originSpeed;
    [SerializeField] public float stayDuration;

    [SerializeField] public Vector2 initialSpawnViewport;
    [SerializeField] public Vector2 initialTargetViewport;

    [SerializeField] public float verticalMoveDistance;
    [SerializeField] public float horizontalMoveDistance;

    [SerializeField] private Image[] healthBars;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private float maxHealth = 300;
    [SerializeField] private float currentHealth;
    private bool isPlayerInDamageArea;
    [SerializeField] public float GameDamage;
    [SerializeField] public float Player1damage;
    [SerializeField] public float Player2damage;
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
    public AudioSource arc,arc2;
    public AudioClip attack1,attack2,DestroySE;
    private int PlayerIndex;

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

    private bool isPausedBySkill = false;
    private bool isMoving;
    void Start()
    {
        PlayerIndex = PlayerPrefs.GetInt("SelectedCharacterIndex");
        originSpeed = moveSpeed;
        transition = Crossfade.GetComponent<Animator>();
        transition.SetBool("Start", false);
        originDamageInterval = damageInterval;
        activeDamageInterval = originDamageInterval;
        GameManager = GetComponent<TimeManager>();
        if (PlayerIndex == 0)
        {
            GameDamage = Player1damage;
            originalDamage = Player1damage;
            doubleDamage = Player1damage * damageMult;
        }
        else if (PlayerIndex == 1)
        {
            originalDamage = Player2damage;
            GameDamage = Player2damage;
        }
        timeManager = TimeManager.Instance;
        InitializeHealthSystem();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = playerTransform.GetComponent<PlayerHealth>();
        StartCoroutine(InitialEntrance());
        StartCoroutine(WeakPointSpawnLoop());
    }

    void InitializeHealthSystem()
    {
        currentHealth = 0;
        healthCanvas.gameObject.SetActive(true);
        UpdateHealthDisplay();
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

        yield return new WaitForSeconds(stayDuration);

        Vector3 exitPos = startPos + Vector3.up * 2f;
        yield return StartCoroutine(MoveTo(exitPos));

        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }
        yield return new WaitUntil(() => isMoving == false);
        Destroy(currentBoss);
        isOperating = false;
        StartCoroutine(RandomSpawnLoop());
    }

    IEnumerator RandomSpawnLoop()
    {
        while (true)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            if (!isOperating) StartCoroutine(SpawnFromRandomSide());
        }
    }

    IEnumerator SpawnFromRandomSide()
    {
        isOperating = true;

        int side = Random.Range(0, 3);

        Vector3 spawnPos = GetSideSpawnPosition(side);
        Vector3 targetPos = GetSideTargetPosition(side);
        Vector3 exitPos = spawnPos + GetExitDirection(side) * 2f;

        currentBoss = Instantiate(BossPreFab);
        Boss = currentBoss.transform.Find("Boss").gameObject;
        BossAnimator = currentBoss.GetComponent<Animator>();
        currentBoss.transform.position = spawnPos;

        yield return StartCoroutine(MoveTo(targetPos));

        yield return new WaitForSeconds(stayDuration);

        yield return StartCoroutine(MoveTo(exitPos));

        if (currentWeakPoint != null)
        {
            Destroy(currentWeakPoint);
            isWeakPointActive = false;
        }
        yield return new WaitUntil(() => isMoving == false);
        Destroy(currentBoss);
        isOperating = false;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        if (timeManager.isGameOver == true)
        {
            yield return null;
        }
        else
        {
            isMoving = true;
            Vector3 startPos = currentBoss.transform.position;
            float progress = 0;

            while (progress < 1)
            {
                while (TimeManager.IsSkillPaused)
                {
                    yield return null;
                }
                progress += Time.deltaTime * moveSpeed;
                currentBoss.transform.position = Vector3.Lerp(startPos, target, progress);
                yield return null;
            }
            isMoving = false;
        }
    }

    Vector3 ViewportToWorld(Vector2 viewportPos)
    {
        return Camera.main.ViewportToWorldPoint(new Vector3(viewportPos.x, viewportPos.y, 10));
    }

    Vector3 GetSideSpawnPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 1.45f)),
            1 => ViewportToWorld(new Vector2(-0.45f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(1.45f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetSideTargetPosition(int side)
    {
        return side switch
        {
            0 => ViewportToWorld(new Vector2(verticalMoveDistance, 1f)),
            1 => ViewportToWorld(new Vector2(0f, horizontalMoveDistance)),
            2 => ViewportToWorld(new Vector2(1f, horizontalMoveDistance)),
            _ => Vector3.zero
        };
    }

    Vector3 GetExitDirection(int side)
    {
        return side switch
        {
            0 => Vector3.up,
            1 => Vector3.left,
            2 => Vector3.right,
            _ => Vector3.zero
        };
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

    void UpdateHealthDisplay()
    {
        int filledBars = Mathf.FloorToInt((float)currentHealth / maxHealth * healthBars.Length);
        filledBars = Mathf.Clamp(filledBars, 0, healthBars.Length);

        for (int i = 0; i < healthBars.Length; i++)
        {
            healthBars[i].gameObject.SetActive(i < filledBars);
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
        StopCoroutine(RandomSpawnLoop());
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

    IEnumerator ApplyDamage()
    {
        while (isPlayerInDamageArea && currentHealth < maxHealth)
        {
            if (currentBoss != null)
            {
                if (PlayerIndex == 0)
                {
                    GameObject Player1attack = Instantiate(Player1Attack, playerTransform.position, Quaternion.identity);
                    Vector3 direction = currentBoss.transform.position - playerTransform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Player1attack.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                }
                if (PlayerIndex == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        yield return new WaitForSeconds(0.03f);
                        GameObject Player1attack = Instantiate(Player1Attack, playerTransform.position, Quaternion.identity);
                        Vector3 direction = currentBoss.transform.position - playerTransform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        Player1attack.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                    }
                }
                arc.clip = attack1;
                arc.Play();
                GameObject EffectClone = Instantiate(playerDamageEffect,currentBoss.transform);
                EffectClone.transform.rotation = Quaternion.Euler(0, 0, EffectrandomZ);
                Boss.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                currentHealth += GameDamage;
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

    void SpawnClosestWeakPoint()
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
        StartCoroutine(RemoveWeakPointAfterDelay());
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

    public void ChrSkill1(float duration)
    {
        if (duration > 0)
        {
            i = duration;
            GameDamage = doubleDamage;
            activeDamageInterval = SkilldamageInterval;
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
            }
        }
    }

    private void PauseBoss()
    {
        isPausedBySkill = true;
        StopCoroutine(RandomSpawnLoop());
        moveSpeed = 0;
    }

    private void ResumeBoss()
    {
        isPausedBySkill = false;
        moveSpeed = originSpeed;
        StartCoroutine(RandomSpawnLoop());
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
            GameDamage = originalDamage;
            activeDamageInterval = originDamageInterval;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            currentHealth = 10000f;
        }

        randomx = Random.Range(-3, 3);
        randomy = Random.Range(-3, 3);
        EffectrandomZ = Random.Range(-360, 360);

        if (!phase2Triggered && currentHealth >= maxHealth * 0.5f)
        {
            phase2Triggered = true;
            isPhase2 = true;

            if (projectileManager != null)
            {
                var manager = projectileManager.GetComponent<ProjectileManagerRandom>();
                if (manager != null)
                {
                    manager.ActivatePhase2();
                }
            }
        }

        if (TimeManager.IsSkillPaused)
        {
            if (!isPausedBySkill)
            {
                PauseBoss();
            }
            return;
        }
        else if (isPausedBySkill)
        {
            ResumeBoss();
        }
    }


}
