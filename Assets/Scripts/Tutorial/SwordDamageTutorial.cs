using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamageTutorial : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerMovementTutorial playerMovementTutorial;
    private GameObject player;
    [SerializeField] private int damage;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovementTutorial = player.GetComponent<PlayerMovementTutorial>();
    }
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHealth.isInvincible && !playerMovementTutorial.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
