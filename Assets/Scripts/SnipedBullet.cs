using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipedBullet : MonoBehaviour
{
    [SerializeField] private float Speed;
    private Vector3 direction;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;
    [SerializeField] private AudioClip audioClip1;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = audioClip1;
        audioSource.Play();
    }

    public void Initialize(Vector3 dir, GameObject playerObj)
    {
        Destroy(gameObject, lifetime);
        direction = dir;
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerMovement = playerObj.GetComponent<PlayerMovement>();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (TimeManager.IsSkillPaused)
        {
            return;
        }
        transform.position += direction * Speed * Time.deltaTime;
    }
}
