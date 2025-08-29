using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManagerRandom : MonoBehaviour
{
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public float cooldown = 2.1f;
        public float weight = 1;

        [Header("鎖定彈幕專用參數")]
        public GameObject lockOnLaserPrefab;
        public Transform[] laserSpawnPoints;

        [Header("方向變換彈幕參數")]
        public GameObject directionChangeProjectilePrefab;
        public float directionChangeSpeed = 5f;
        public float phase1Duration = 5f;
        public float phase2Duration = 1f;

        [HideInInspector]
        public bool isReady = true;
    }

    private Dictionary<AttackPattern, float> originalCooldowns = new Dictionary<AttackPattern, float>();
    private bool isPhase2Active = false;
    public GameObject linearBombPrefab;
    public GameObject warningProjectilePrefab;
    [SerializeField] private GameObject expandCirclePrefab;

    public AttackPattern[] patterns = new AttackPattern[3];

    private bool isGlobalCooldown = false;
    private List<AttackPattern> availablePatterns = new List<AttackPattern>();
    private List<GameObject> activeProjectiles = new List<GameObject>();
    private bool isStopped = false;

    private bool wasPaused = false;

    void Start()
    {
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

    public void RegisterFragment(GameObject fragment)
    {
        if (!activeProjectiles.Contains(fragment))
        {
            activeProjectiles.Add(fragment);
        }
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
            case "Trebuchet":
                yield return StartCoroutine(Pattern1Logic());
                break;
            case "Kunai":
                yield return StartCoroutine(Pattern2Logic());
                break;
            case "arrow":
                yield return StartCoroutine(Pattern3Logic());
                break;
            case "sword":
                yield return StartCoroutine(Pattern4Logic(pattern));
                break;
            case "Crossbow":
                yield return StartCoroutine(LockOnAttackLogic(pattern));
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

    IEnumerator Pattern1Logic()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0.2f, 0)).x,
            Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0)).x),
            Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0, 0.2f)).y,
            Camera.main.ViewportToWorldPoint(new Vector2(0, 0.8f)).y)
        );

        GameObject circle = Instantiate(expandCirclePrefab, spawnPos, Quaternion.identity);
        activeProjectiles.Add(circle);

        ExpandCircle expandComp = circle.GetComponent<ExpandCircle>();
        ExpandCircleInanse expandCompInanse = circle.GetComponent<ExpandCircleInanse>();

        yield return new WaitForSeconds(1f);
        //yield return new WaitUntil(() => circle == null);

        if (expandComp != null)
        {
            expandComp.currentDepth = 0;
        }

        if (expandCompInanse != null)
        {
            expandCompInanse.currentDepth = 0;
        }
    }

    IEnumerator Pattern2Logic()
    {
        bool isLeft = Random.value > 0.5f;
        Vector2 spawnPos = GetSpawnPosition(isLeft);

        GameObject bomb = Instantiate(linearBombPrefab, spawnPos, Quaternion.identity);
        activeProjectiles.Add(bomb);
        bomb.GetComponent<LinearBomber>().InitializeDirection(isLeft);

        //yield return new WaitUntil(() => bomb == null);
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

    IEnumerator Pattern3Logic()
    {
        GameObject projectile = Instantiate(warningProjectilePrefab,Vector3.zero,Quaternion.identity);
        activeProjectiles.Add(projectile);
        //yield return new WaitUntil(() => projectile == null);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Pattern4Logic(AttackPattern pattern)
    {
        Camera cam = Camera.main;
        Vector2[] edges = {
        new Vector2(0.5f, 1.1f),
        new Vector2(0.5f, -0.1f),
        new Vector2(-0.1f, 0.5f),
        new Vector2(1.1f, 0.5f)
    };
        Vector2 spawnPos = cam.ViewportToWorldPoint(edges[Random.Range(0, 4)]);

        GameObject projectile = Instantiate(
            pattern.directionChangeProjectilePrefab,
            spawnPos,
            Quaternion.identity
        );
        activeProjectiles.Add(projectile);

        float timer = 0;
        Vector2 currentDir = (Vector2.zero - spawnPos).normalized;

        while (timer < pattern.phase1Duration)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            if (Mathf.FloorToInt(timer) != Mathf.FloorToInt(timer - Time.deltaTime))
            {
                Vector2 toCenter = ((Vector2)cam.transform.position - (Vector2)projectile.transform.position).normalized;

                float biasStrength = 0.5f;
                Vector2 biasedDirection = Vector2.Lerp(
                    currentDir,
                    toCenter + Random.insideUnitCircle * 0.3f,
                    biasStrength
                ).normalized;

                Vector2 viewportPos = cam.WorldToViewportPoint(projectile.transform.position);
                Vector2 boundaryCorrection = Vector2.zero;

                if (viewportPos.x < 0.15f) boundaryCorrection.x += 0.7f;
                if (viewportPos.x > 0.7f) boundaryCorrection.x -= 0.7f;
                if (viewportPos.y < 0.15f) boundaryCorrection.y += 0.7f;
                if (viewportPos.y > 0.7f) boundaryCorrection.y -= 0.7f;

                currentDir = (biasedDirection + boundaryCorrection).normalized;
            }

            projectile.transform.position += (Vector3)currentDir * pattern.directionChangeSpeed * Time.deltaTime;

            float angle = Mathf.Atan2(currentDir.y, currentDir.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            timer += Time.deltaTime;
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            yield return new WaitForSeconds(0.55f);
            Vector2 targetDir = ((Vector2)player.transform.position - (Vector2)projectile.transform.position).normalized;
            float chaseTimer = 0;
            while (chaseTimer < pattern.phase2Duration)
            {
                while (TimeManager.IsSkillPaused)
                {
                    yield return null;
                }
                projectile.transform.position += (Vector3)targetDir * pattern.directionChangeSpeed * 1.5f * Time.deltaTime;

                float chaseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, chaseAngle - 90);

                chaseTimer += Time.deltaTime;
                yield return null;
            }
        }

        Destroy(projectile , 2f);
    }
    IEnumerator LockOnAttackLogic(AttackPattern pattern)
    {
        GameObject laserObj = Instantiate(
            pattern.lockOnLaserPrefab,
            Vector3.zero,
            Quaternion.identity
        );
        activeProjectiles.Add(laserObj);
        LockOnLaser laserComp = laserObj.GetComponent<LockOnLaser>();
        laserComp.Initialize(pattern.laserSpawnPoints);

        yield return new WaitUntil(() => laserComp.IsComplete);
        yield return new WaitForSeconds(1f);
        Destroy(laserObj);
    }
}