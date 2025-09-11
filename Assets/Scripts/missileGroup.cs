using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missileGroup : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int Damage;
    private PlayerHealth PlayerHealth;
    [SerializeField] private GameObject hitAnimation;
    [SerializeField] private GameObject SEplayer;
    private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    private GameObject player;
    private PlayerMovement playerMovement;
    void Start()
    {
        PlayerHealth = FindAnyObjectByType<PlayerHealth>();
        SEplayer = GameObject.FindGameObjectWithTag("SoundEffectPlayer");
        audioSource = SEplayer.GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
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
        if (collision.CompareTag("Player") && !PlayerHealth.isInvincible && !playerMovement.isInvincible)
        {
            PlayerHealth.TakeDamage(Damage);
            GameObject boom = Instantiate(hitAnimation, transform.position, Quaternion.identity);
            audioSource.clip = clip;
            audioSource.Play();
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
