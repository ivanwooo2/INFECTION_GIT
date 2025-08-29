using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProjetileManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private PlayerMovementTutorial PlayerMovementTutorial;

        [Header("玛﹚紆辊盡ノ把计")]
        public GameObject lockOnLaserPrefab;
        public Transform[] laserSpawnPoints;

        [Header("よ跑传紆辊把计")]
        public GameObject directionChangeProjectilePrefab;
        public float directionChangeSpeed = 5f;
        public float phase1Duration = 5f;
        public float phase2Duration = 1f;

    private List<GameObject> activeProjectiles = new List<GameObject>();
    private bool isStopped = false;
    void Start()
    {
       PlayerMovementTutorial = Player.GetComponent<PlayerMovementTutorial>();
    }

    void Update()
    {
        activeProjectiles.RemoveAll(item => item == null);
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

    public void spawnSword()
    {
        StartCoroutine(Pattern4Logic());
    }

    public void spawnLaser()
    {
            StartCoroutine(LockOnAttackLogic());
    }

    IEnumerator Pattern4Logic()
    {
        Camera cam = Camera.main;
        Vector2[] edges = {
        new Vector2(-0.3f, 0.5f),
        new Vector2(1f, 0.5f)
    };
        Vector2 spawnPos = cam.ViewportToWorldPoint(edges[Random.Range(0, 2)]);

        GameObject projectile = Instantiate(
            directionChangeProjectilePrefab,
            spawnPos,
            Quaternion.identity
        );
        activeProjectiles.Add(projectile);

        float timer = 0;
        Vector2 currentDir = (Vector2.zero - spawnPos).normalized;

        while (timer < phase1Duration)
        {
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

            projectile.transform.position += (Vector3)currentDir * directionChangeSpeed * Time.deltaTime;

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
            while (chaseTimer < phase2Duration)
            {
                projectile.transform.position += (Vector3)targetDir * directionChangeSpeed * 1.5f * Time.deltaTime;

                float chaseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, chaseAngle - 90);

                chaseTimer += Time.deltaTime;
                yield return null;
            }
        }

        Destroy(projectile, 2f);
    }
    IEnumerator LockOnAttackLogic()
    {
        while (PlayerMovementTutorial.dashed == false)
        {
            GameObject laserObj = Instantiate(
                lockOnLaserPrefab,
                Vector3.zero,
                Quaternion.identity
            );
            activeProjectiles.Add(laserObj);
            LockOnLaserTutorial laserComp = laserObj.GetComponent<LockOnLaserTutorial>();
            laserComp.Initialize(laserSpawnPoints);

            yield return new WaitUntil(() => laserComp.IsComplete);
            yield return new WaitForSeconds(1f);
            Destroy(laserObj);
        }
    }
}
