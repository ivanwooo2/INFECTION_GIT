using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMissileExplore : MonoBehaviour
{
    [SerializeField] private int Damage;
    private PlayerHealth health;
    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    void Start()
    {
        health = FindAnyObjectByType<PlayerHealth>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            health.TakeDamage(Damage);
        }
    }
}
