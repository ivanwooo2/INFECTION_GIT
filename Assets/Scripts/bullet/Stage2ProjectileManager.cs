using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Stage2ProjectileManager : MonoBehaviour
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public float cooldown = 2.1f;
        public float weight = 1;

        [Header("missileGroup")]
        public GameObject missilePrefab;
        public GameObject[] missileSpawnPoints;
        public int missileInterval;
        public float missileSpawnSpeed;

        [Header("aimMissile")]
        public GameObject aimMissilePrefab;
        public int aimMissileNumber;
        public float aimMissileinterval;

        [Header("narudo")]
        public GameObject FireBottlePrefab;
        public int FireBottleNumber;

        [Header("grenade")]
        public GameObject fraggrenadePrefab;

        [Header("sniper")]
        public GameObject SniperRifle;
        public Transform Sniperpoint;

        [HideInInspector]
        public bool isReady = true;
    }
    private Dictionary<AttackPattern, float> originalCooldowns = new Dictionary<AttackPattern, float>();
    private bool isPhase2Active = false;

    public AttackPattern[] patterns = new AttackPattern[3];

    private bool isGlobalCooldown = false;
    private List<AttackPattern> availablePatterns = new List<AttackPattern>();
    private List<GameObject> activeProjectiles = new List<GameObject>();
    private bool isStopped = false;
    private Stage2BossController Stage2BossController;
    private Vector3 aimMissilespawnOffset;

    private bool wasPaused = false;

    private AudioSource audioSource;
    [SerializeField] private AudioClip MissileRain;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AttackScheduler());
    }

    void Update()
    {
        if (TimeManager.IsSkillPaused)
        {
            if (!wasPaused)
            {
                PauseAllProjectiles();
                wasPaused = true;
            }
            return;
        }
        else if (wasPaused)
        {
            ResumeAllProjectiles();
            wasPaused = false;
        }
        activeProjectiles.RemoveAll(item => item == null);
    }

    private void PauseAllProjectiles()
    {
        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var proj in projectiles)
        {
            if (proj != null)
            {
                var rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;

                var moveScript = proj.GetComponent<MonoBehaviour>();
                if (moveScript != null) moveScript.enabled = false;
            }
        }
        foreach (var proj in activeProjectiles)
        {
            if (proj != null)
            {
                var rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = false;

                var moveScript = proj.GetComponent<MonoBehaviour>();
                if (moveScript != null) moveScript.enabled = false;
            }
        }
    }

    private void ResumeAllProjectiles()
    {
        foreach (var proj in activeProjectiles)
        {
            if (proj != null)
            {
                var rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = true;

                var moveScript = proj.GetComponent<MonoBehaviour>();
                if (moveScript != null) moveScript.enabled = true;
            }
        }
    }

    public void CleanupProjectiles()
    {
        StopAllCoroutines();

        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var proj in projectiles)
        {
            Destroy(proj);
        }

        foreach (var proj in activeProjectiles)
        {
            if (proj != null)
            {
                Destroy(proj);
            }
        }
        activeProjectiles.Clear();

        isStopped = true;
    }

    public void SkillCleanupProjectiles()
    {
        var projectiles = GameObject.FindGameObjectsWithTag("EnemyProjectile");
        foreach (var proj in projectiles)
        {
            Destroy(proj);
        }

        foreach (var proj in activeProjectiles)
        {
            if (proj != null)
            {
                Destroy(proj);
            }
        }
        activeProjectiles.Clear();
    }

    IEnumerator AttackScheduler()
    {
        yield return new WaitForSeconds(3);
        while (!isStopped)
        {
            while (true)
            {
                while (TimeManager.IsSkillPaused)
                {
                    yield return null;
                }
                yield return new WaitUntil(() => !isGlobalCooldown);
                UpdateAvailablePatterns();

                if (availablePatterns.Count > 0)
                {
                    AttackPattern selected = SelectByWeight();
                    isGlobalCooldown = true;

                    StartCoroutine(ExecuteAttack(selected));

                    yield return new WaitForSeconds(selected.cooldown);
                    isGlobalCooldown = false;
                }
                yield return null;
            }
        }
    }

    void UpdateAvailablePatterns()
    {
        availablePatterns.Clear();
        foreach (AttackPattern pattern in patterns)
        {
            if (pattern.isReady) availablePatterns.Add(pattern);
        }
    }

    AttackPattern SelectByWeight()
    {
        float totalWeight = 0;
        foreach (AttackPattern p in availablePatterns)
        {
            totalWeight += p.weight;
        }

        float randomPoint = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (AttackPattern p in availablePatterns)
        {
            currentWeight += p.weight;
            if (randomPoint < currentWeight)
            {
                return p;
            }
        }
        return availablePatterns[0];
    }

    IEnumerator ExecuteAttack(AttackPattern pattern)
    {
        pattern.isReady = false;
        Debug.Log($"AttackStart: {pattern.name}");

        switch (pattern.name)
        {
            case "missileGroup":
                yield return StartCoroutine(Pattern1Logic(pattern));
                break;
            case "aimMissile":
                yield return StartCoroutine(Pattern2Logic(pattern));
                break;
            case "narudo":
                yield return StartCoroutine(Pattern3Logic(pattern));
                break;
            case "grenade":
                yield return StartCoroutine(Pattern4logic(pattern));
                break;
            case "sniper":
                yield return StartCoroutine(Pattern5logic(pattern));
                break;
        }

        StartCoroutine(CooldownTimer(pattern));
        yield return null;
    }

    IEnumerator CooldownTimer(AttackPattern pattern)
    {
        yield return new WaitForSeconds(pattern.cooldown);
        pattern.isReady = true;
    }

    public void ActivatePhase2()
    {
        if (isPhase2Active) return;

        isPhase2Active = true;

        foreach (AttackPattern pattern in patterns)
        {
            if (!originalCooldowns.ContainsKey(pattern))
            {
                originalCooldowns.Add(pattern, pattern.cooldown);
            }

            pattern.cooldown /= 2f;
        }
    }

    IEnumerator Pattern1Logic(AttackPattern pattern)
    {
        audioSource.clip = MissileRain;
        audioSource.Play();
        int Count = 0;
        for (int i = 0; i < pattern.missileSpawnPoints.Length;i++)
        {
            Count = i;
        }
        for (int i = 0; i < pattern.missileInterval;i++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(pattern.missileSpawnSpeed);
            int RandomCounts = Random.Range(0, Count);
            GameObject missile = Instantiate(pattern.missilePrefab, pattern.missileSpawnPoints[RandomCounts].transform.position, Quaternion.identity);
            activeProjectiles.Add(missile);
        }
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Pattern2Logic(AttackPattern pattern)
    {
        Stage2BossController = FindAnyObjectByType<Stage2BossController>();
        for (int i = 0; i < pattern.aimMissileNumber; i++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(pattern.aimMissileinterval);

            if (Stage2BossController.currentBoss != null)
            {
                aimMissilespawnOffset = new Vector3(Stage2BossController.currentBoss.transform.position.x, Stage2BossController.currentBoss.transform.position.y + 1, 0);
            }

            if (Stage2BossController.currentBoss == null)
            {
                yield return null;
            }
            else
            {
                GameObject aimMissile = Instantiate(pattern.aimMissilePrefab, aimMissilespawnOffset, Quaternion.identity);
                activeProjectiles.Add(aimMissile);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator Pattern3Logic(AttackPattern pattern)
    {
        for (int i = 0; i < pattern.FireBottleNumber; i++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }

            Vector2 spawnPos = new Vector2(
                Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0.2f, 0)).x,
                Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0)).x),
                Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0, 0.2f)).y,
                Camera.main.ViewportToWorldPoint(new Vector2(0, 0.8f)).y)
            );
            GameObject fireBottle = Instantiate(pattern.FireBottlePrefab, spawnPos, Quaternion.identity);
            activeProjectiles.Add(fireBottle);

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Pattern4logic(AttackPattern pattern)
    {
        while (TimeManager.IsSkillPaused)
        {
            yield return null;
        }
        bool isLeft = Random.value > 0.5f;
        Vector2 spawnPos = GetSpawnPosition(isLeft);

        GameObject fragGrenade = Instantiate(pattern.fraggrenadePrefab,spawnPos, Quaternion.identity);
        activeProjectiles.Add(fragGrenade);
        fragGrenade.GetComponent<fragGrenade>().InitializeDirection(isLeft);

        yield return new WaitForSeconds(1f);
    }

    Vector2 GetSpawnPosition(bool isLeft)
    {
        float yPos = Random.Range(0.2f, 0.8f);
        Vector2 viewportPos = new Vector2(
            isLeft ? -0.1f : 1.1f,
            yPos
        );
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }

    IEnumerator Pattern5logic(AttackPattern pattern)
    {
        while (TimeManager.IsSkillPaused)
        {
            yield return null;
        }
        GameObject SniperR = Instantiate(pattern.SniperRifle,pattern.Sniperpoint.transform.position, Quaternion.identity);
        activeProjectiles.Add(SniperR);
        Sniper sniper = SniperR.GetComponent<Sniper>();

        yield return new WaitUntil(() => sniper.IsComplete);
        yield return new WaitForSeconds(1f);
        Destroy(sniper);
    }
}
