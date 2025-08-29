using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;

    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private SpriteRenderer Spriterenderer;
    [SerializeField] private Sprite[] sprites;

    private Transform _transform;
    [SerializeField] private Vector3 rotation;

    void Start()
    {
        int randomSprite = Random.Range(0,sprites.Length);
        Spriterenderer = GetComponent<SpriteRenderer>();
        _transform = transform;
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
        Spriterenderer.sprite = sprites[randomSprite];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerHealth.isInvincible  && !playerMovement.isInvincible)
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
        _transform.Rotate(rotation * Time.deltaTime);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
