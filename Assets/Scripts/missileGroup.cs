using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileGroup : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int Damage;
    private PlayerHealth PlayerHealth;
    [SerializeField] private GameObject hitAnimation;
    void Start()
    {
        PlayerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (TimeManager.IsSkillPaused)
        {
            return;
        }
        transform.position += Vector3.down * moveSpeed * 1.5f * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth.TakeDamage(Damage);
            GameObject boom = Instantiate(hitAnimation, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
