using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnLaser : MonoBehaviour
{
    [Header("參數設定")]
    public float trackingDuration = 3f;
    public float attackDelay = 1f;
    public GameObject projectilePrefab;

    [Header("追蹤延遲設定")]
    public bool useTrackingDelay = true;
    public float trackingDelay = 0.4f;
    public float smoothness = 5f;
    public float laserExtension = 10f;

    [Header("材質設定")]
    public Material laserMaterial;
    public Material PauselaserMaterial;

    private Transform[] spawnPoints;
    private Transform player;
    private Camera mainCamera;
    private List<LineRenderer> laserLines = new List<LineRenderer>();
    private Vector3[] targetPositions;
    private Vector3 LastPlayerposition;
    public bool IsComplete { get; private set; }

    public void Initialize(Transform[] points)
    {
        spawnPoints = points;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("沒有指定有效的激光生成點!");
            IsComplete = true;
            return;
        }

        targetPositions = new Vector3[spawnPoints.Length];
        for (int i = 0; i < targetPositions.Length; i++)
        {
            targetPositions[i] = player.position;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            GameObject laserObj = new GameObject($"LaserLine_{i}");
            laserObj.transform.SetParent(transform);
            LineRenderer lr = laserObj.AddComponent<LineRenderer>();

            lr.positionCount = 2;
            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.textureScale = new Vector2(7f, 1.5f);

            if (laserMaterial != null)
            {
                lr.material = laserMaterial;
                lr.startColor = new Color(1, 1, 0, 0.4f);
                lr.endColor = new Color(1, 1, 0, 0.4f);
            }
            else
            {
                Material loadedMat = Resources.Load<Material>("LaserMaterial");
                if (loadedMat != null)
                {
                    lr.material = loadedMat;
                }
                else
                {
                    lr.material = new Material(Shader.Find("Sprites/Default"));
                    lr.startColor = Color.red;
                    lr.endColor = Color.red;
                    Debug.LogWarning("未找到 LaserMaterial，使用默認材質");
                }
            }
            laserLines.Add(lr);

        }

        StartCoroutine(LockOnRoutine());
    }

    void Update()
    {
        
    }

    IEnumerator LockOnRoutine()
    {
        IsComplete = false;

        float timer = 0;
        while (timer < trackingDuration)
        {
            while (TimeManager.IsSkillPaused)
            {
                foreach (LineRenderer lr in laserLines)
                {
                    lr.material = PauselaserMaterial;
                }
                yield return null;
            }
            UpdateLaserPositions();
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackDelay);
        FireProjectiles();

        foreach (LineRenderer lr in laserLines)
        {
            if (lr != null) Destroy(lr.gameObject);
        }

        laserLines.Clear();
        IsComplete = true;
    }

    void UpdateLaserPositions()
    {
        int lineCount = Mathf.Min(laserLines.Count, spawnPoints.Length);

        for (int i = 0; i < lineCount; i++)
        {
            if (TimeManager.IsSkillPaused)
            {
                return;
            }
            if (laserLines[i] == null || spawnPoints[i] == null) continue;

            Vector3 currentPosition = player.position;
            LastPlayerposition = currentPosition;
            if (useTrackingDelay)
            {
                if (TimeManager.IsSkillPaused)
                {
                    return;
                }
                targetPositions[i] = Vector3.Lerp(
                    targetPositions[i],
                    currentPosition,
                    smoothness * Time.deltaTime
                );
            }
            else
            {
                targetPositions[i] = currentPosition;
            }

            Vector3 laserEndPoint = CalculateLaserEndPoint(spawnPoints[i].position, targetPositions[i]);
            laserLines[i].SetPosition(0, spawnPoints[i].position);
            laserLines[i].SetPosition(1, laserEndPoint);
        }
    }

    Vector3 CalculateLaserEndPoint(Vector3 startPoint, Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - startPoint).normalized;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(targetPoint);

        if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            return targetPoint;
        }

        Vector3 endPoint = targetPoint + direction * laserExtension;

        Vector3 viewportEnd = mainCamera.WorldToViewportPoint(endPoint);

        int safetyCounter = 0;
        while ((viewportEnd.x >= 0 && viewportEnd.x <= 1 &&
               viewportEnd.y >= 0 && viewportEnd.y <= 1) &&
               safetyCounter < 10)
        {
            endPoint += direction * laserExtension;
            viewportEnd = mainCamera.WorldToViewportPoint(endPoint);
            safetyCounter++;
        }
        return endPoint;
    }


    void FireProjectiles()
    {
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            Vector3 direction = (LastPlayerposition - point.position).normalized;
            GameObject projectile = Instantiate(
                projectilePrefab,
                point.position,
                Quaternion.LookRotation(direction)
            );
            projectile.GetComponent<LockProjectile>().Initialize(direction, player.gameObject, player.gameObject);
        }
    }
}