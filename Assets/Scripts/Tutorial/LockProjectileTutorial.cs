using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockProjectileTutorial : MonoBehaviour
{
    public float speed = 15f;
    private Vector3 direction;
    private PlayerHealth playerHealth;
    private PlayerMovementTutorial PlayerMovementTutorial;
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;
    public AudioSource bow1;
    public AudioClip sfx1;
    void Start()
    {
        bow1.clip = sfx1;
        bow1.Play();
    }

    public void Initialize(Vector3 dir, GameObject playerObj, GameObject playerObj2)
    {
        Destroy(gameObject, lifetime);
        direction = dir;
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        PlayerMovementTutorial = playerObj.GetComponent<PlayerMovementTutorial>();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHealth.isInvincible && !PlayerMovementTutorial.isInvincible)
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
