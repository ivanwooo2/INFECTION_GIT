using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMissileExplore : MonoBehaviour
{
    [SerializeField] private int Damage;
    private PlayerHealth health;
    void Start()
    {
        health = FindAnyObjectByType<PlayerHealth>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            health.TakeDamage(Damage);
        }
    }
}
