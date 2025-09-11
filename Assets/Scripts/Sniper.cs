using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour
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

    [SerializeField] private GameObject SniperAim;
    [SerializeField] private AudioClip audioClip1;
    private GameObject Player;
    private Camera mainCamera;
    private Vector3 targetPositions;
    private Vector3 LastPlayerposition;
    private LineRenderer lineRenderer;
    private AudioSource audioSource;

    public bool IsComplete { get; private set; }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
        targetPositions = Player.transform.position;
        audioSource.clip = audioClip1;
        audioSource.Play();
        StartCoroutine(LockOnRoutine());
    }

    IEnumerator LockOnRoutine()
    {
        IsComplete = false;

        float timer = 0;
        GameObject sniperaim = Instantiate(SniperAim,targetPositions,Quaternion.identity);
        while (timer < trackingDuration)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            UpdateLaserPositions();
            sniperaim.transform.position = targetPositions;
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackDelay);
        FireProjectiles();
        Destroy(sniperaim);
        Destroy(gameObject);
        IsComplete = true;
    }
    void UpdateLaserPositions()
    {
        if (TimeManager.IsSkillPaused)
        {
            return;
        }
        Vector3 currentPosition = Player.transform.position;
        LastPlayerposition = currentPosition;

        if (useTrackingDelay)
        {
            if (TimeManager.IsSkillPaused)
            {
                return;
            }
            targetPositions = Vector3.Lerp(
                targetPositions,
                currentPosition,
                smoothness * Time.deltaTime
            );
        }
        else
        {
            targetPositions = currentPosition;
        }

        Vector3 laserEndPoint = CalculateLaserEndPoint(transform.position, targetPositions);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, laserEndPoint);
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
        Vector3 direction = (LastPlayerposition - transform.position).normalized;
        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.LookRotation(direction)
        );
        projectile.GetComponent<SnipedBullet>().Initialize(direction,Player);
    }
}
