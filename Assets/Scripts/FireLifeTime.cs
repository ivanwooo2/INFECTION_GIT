using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLifeTime : MonoBehaviour
{
    [SerializeField] private float LifeTime;
    [SerializeField] private int damage;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private ParticleSystem _particleSystem;
    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();
        _particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, LifeTime);
    }

    void Update()
    {
        while (TimeManager.IsSkillPaused)
        {
            _particleSystem.Pause();
        }
        _particleSystem.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
