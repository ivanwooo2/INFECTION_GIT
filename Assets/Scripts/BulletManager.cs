using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float BulletSpeed;
    [SerializeField] private int Damage;
    private PlayerHealth health;
    [SerializeField] public int difficulty = 0;
    private float RandomX;
    private float RandomY;
    private Vector3 RandomDirection;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    void Start()
    {
        //RandomX = Random.Range(-0.5f, 0.5f);
        //RandomY = Random.Range(-0.5f, 0.5f);
        Player = GameObject.FindWithTag("Player");
        playerHealth = Player.GetComponent<PlayerHealth>();
        playerMovement = Player.GetComponent<PlayerMovement>();
        direction = (Player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        health = Player.GetComponent<PlayerHealth>();
        //RandomDirection = new Vector3(direction.x + RandomX, direction.y + RandomY, 0).normalized;
        if (difficulty == 1)
        {
            //float angle1 = Mathf.Atan2(RandomDirection.y, RandomDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 5);
            RandomDirection = new Vector3(Mathf.Cos((angle - 5) * Mathf.Deg2Rad), Mathf.Sin((angle - 5) * Mathf.Deg2Rad), 0f);
        }
        if (difficulty == 2)
        {
            //float angle2 = Mathf.Atan2(RandomDirection.y, RandomDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 5);
            RandomDirection = new Vector3(Mathf.Cos((angle + 5) * Mathf.Deg2Rad), Mathf.Sin((angle + 5) * Mathf.Deg2Rad), 0f);
        }
        if (difficulty == 3)
        {
            //float angle2 = Mathf.Atan2(RandomDirection.y, RandomDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 10);
            RandomDirection = new Vector3(Mathf.Cos((angle - 10) * Mathf.Deg2Rad), Mathf.Sin((angle - 10) * Mathf.Deg2Rad), 0f);
        }
        if (difficulty == 4)
        {
            //float angle2 = Mathf.Atan2(RandomDirection.y, RandomDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 10);
            RandomDirection = new Vector3(Mathf.Cos((angle + 10) * Mathf.Deg2Rad), Mathf.Sin((angle + 10) * Mathf.Deg2Rad), 0f);
        }
    }
    void Update()
    {
        if (TimeManager.IsSkillPaused)
        {
            return;
        }
        if (difficulty == 0)
        {
            transform.position += direction * BulletSpeed * 1f * Time.deltaTime;
        }
        else if (difficulty == 1)
        {
            transform.position += RandomDirection * BulletSpeed * 1f * Time.deltaTime;
        }
        else if (difficulty == 2)
        {
            transform.position += RandomDirection * BulletSpeed * 1f * Time.deltaTime;
        }
        else if (difficulty == 3)
        {
            transform.position += RandomDirection * BulletSpeed * 1f * Time.deltaTime;
        }
        else if (difficulty == 4)
        {
            transform.position += RandomDirection * BulletSpeed * 1f * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerHealth.isInvincible && !playerMovement.isInvincible)
        {
            health.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
