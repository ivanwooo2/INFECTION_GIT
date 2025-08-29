using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPathProjectile : MonoBehaviour
{
    [Header("阶段设置")]
    public float phase1Duration = 5f;
    public float phase2Duration = 1f;
    public float baseSpeed = 3f;
    public float directionChangeInterval = 1f;

    [Header("画面限制")]
    [Range(0.05f, 0.2f)]
    public float screenMargin = 0.1f;

    private Transform _player;
    private Camera _mainCamera;
    private Vector2 _currentDirection;
    private bool _isPhase2;
    private float _phaseTimer;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _mainCamera = Camera.main;
        StartCoroutine(BehaviorRoutine());
    }

    IEnumerator BehaviorRoutine()
    {
        StartCoroutine(Phase1Movement());
        yield return new WaitForSeconds(phase1Duration);

        _isPhase2 = true;
        Vector2 toPlayer = (_player.position - transform.position).normalized;
        _currentDirection = toPlayer;
        UpdateRotation();

        Debug.Log($"冲刺方向: {_currentDirection}, 玩家位置: {_player.position}");

        float timer = 0;
        while (timer < phase2Duration)
        {
            transform.Translate(_currentDirection * baseSpeed * 2 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator Phase1Movement()
    {
        while (!_isPhase2)
        {
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
            Vector2 centerBias = (Vector2.one * 0.5f - (Vector2)viewportPos).normalized;

            if (viewportPos.x < 0.1f) centerBias.x = 1;
            if (viewportPos.x > 0.9f) centerBias.x = -1;
            if (viewportPos.y < 0.1f) centerBias.y = 1;
            if (viewportPos.y > 0.9f) centerBias.y = -1;

            _currentDirection = Vector2.Lerp(
                Random.insideUnitCircle.normalized,
                centerBias.normalized,
                0.8f
            ).normalized;

            UpdateRotation();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    void Update()
    {
        if (!_isPhase2)
        {
            transform.Translate(_currentDirection * baseSpeed * Time.deltaTime);
            ClampPosition();
        }
    }

    void ClampPosition()
    {
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(transform.position);
        viewportPos.x = Mathf.Clamp(viewportPos.x, screenMargin, 1 - screenMargin);
        viewportPos.y = Mathf.Clamp(viewportPos.y, screenMargin, 1 - screenMargin);
        transform.position = _mainCamera.ViewportToWorldPoint(viewportPos);
    }

    void UpdateRotation()
    {
        float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_player.position, 0.5f);
    }
}