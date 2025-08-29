using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaProjectileInanse : MonoBehaviour
{
    public float height = 3f;
    private Vector3 startPosition;
    public Vector3 targetPosition;
    private float duration;
    private float timer;
    private bool canDamage = false;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    [SerializeField] private int damage;
    private GameObject SoundEffect;
    public AudioSource stone;
    public AudioClip sfx1, sfx2;
    private Transform _transform;
    private ExpandCircleInanse parentCircle;
    [SerializeField] private Vector3 rotation;

    void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        _transform = transform;
        SoundEffect = GameObject.FindGameObjectWithTag("SoundEffectPlayer");
        stone = SoundEffect.GetComponent<AudioSource>();
        stone.clip = sfx1;
        stone.Play();
    }
    public void Initialize(Vector3 target, float travelTime, GameObject playerObj, GameObject playerObj2, ExpandCircleInanse circle)
    {
        startPosition = transform.position;
        targetPosition = target;
        duration = travelTime;
        parentCircle = circle;

        StartCoroutine(EnableDamageOnArrival());

        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            playerMovement = playerObj.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (timer > duration) return;
        _transform.Rotate(rotation * Time.deltaTime);
        timer += Time.deltaTime;
        float normalizedTime = timer / duration;

        Vector3 horizontalPos = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
        float verticalPos = height * Mathf.Sin(normalizedTime * Mathf.PI);
        transform.position = horizontalPos + Vector3.up * verticalPos;

        if (normalizedTime >= 1f)
        {
            stone.clip = sfx2;
            stone.Play();
            transform.position = targetPosition;

            if (parentCircle != null)
            {
                parentCircle.SpawnChildCircles(targetPosition);
            }

            Destroy(gameObject, 0.1f);
        }
    }

    IEnumerator EnableDamageOnArrival()
    {
        yield return new WaitForSeconds(duration - 0.1f);
        canDamage = true;
        GetComponent<Collider2D>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, targetPosition);
    }
}
