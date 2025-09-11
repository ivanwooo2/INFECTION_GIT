using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 500;
    [SerializeField] public int currentHealth;
    [SerializeField] private bool hasPersisted = false;

    [SerializeField] private Image healthBackground; 
    [SerializeField] private Image healthFill;
    [SerializeField] private float invincibleDuration;
    [SerializeField] private GameObject PlayerDamageBackGround;
    [SerializeField] private GameObject playerDamageEffect;
    [SerializeField] private GameObject projectileManager;
    [SerializeField] private GameObject Stage2projectileManager;
    [SerializeField] private ParticleSystem PlayerParticle;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;
    [SerializeField] GameObject PlayerDestroyEffect;
    [SerializeField] private float randomX;
    [SerializeField] private float randomY;
    private float randomRotateZ;
    private SpriteRenderer spriteRenderer;
    public AudioSource arc;
    public AudioClip DestroySE;

    public bool isInvincible = false;

    public bool isSkillActive = false;
    private float originalScale;

    private TimeManager timeManager;

    public AudioSource Player;
    public AudioClip Damaged;

    void Start()
    {
        timeManager = TimeManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Awake()
    {
        originalScale = transform.localScale.x;
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void ToggleSkill(bool state, float scaleMultiplier = 1f)
    {
        isSkillActive = state;
        Vector3 newScale = state ?
            new Vector3(originalScale * scaleMultiplier, originalScale * scaleMultiplier, 1) :
            new Vector3(originalScale, originalScale, 1);
        transform.localScale = newScale;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        GameObject DamageEffect = Instantiate(playerDamageEffect,transform);
        DamageEffect.transform.rotation = Quaternion.Euler(0, 0, randomRotateZ);
        PlayerParticle.Play();
        Player.clip = Damaged;
        Player.Play();
        StartCoroutine(InvincibleRoutine());

        int finalDamage = isSkillActive ? damage * 2 : damage;
        if (!hasPersisted && currentHealth - finalDamage <= 0)
        {
            currentHealth = 1;
            hasPersisted = true;
            return;
        }
        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
    }

    IEnumerator InvincibleRoutine()
    {
        isInvincible = true;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        PlayerDamageBackGround.SetActive(true);
        yield return new WaitForSeconds(invincibleDuration);
        PlayerDamageBackGround.SetActive(false);
        GetComponent<SpriteRenderer>().color = Color.white;
        isInvincible = false;
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthFill.fillAmount = fillAmount;
        }
    }

    private void Die()
    {

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Debug.Log("Player Died!");
        StartCoroutine(PlayerDie());
    }

    IEnumerator PlayerDie()
    {
        timeManager.isGameOver = true;
        isInvincible = true;
        if (projectileManager != null)
        {
            var manager = projectileManager.GetComponent<ProjectileManagerRandom>();
            if (manager != null)
            {
                manager.CleanupProjectiles();
            }
        }

        if (Stage2projectileManager != null)
        {
            var manager = Stage2projectileManager.GetComponent<Stage2ProjectileManager>();
            if (manager != null)
            {
                manager.CleanupProjectiles();
            }
        }
        Collider2D coli = GetComponent<CircleCollider2D>();
        coli.enabled = false;
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = Vector2.zero;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.9f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        GameObject PlayerDestroy = Instantiate(PlayerDestroyEffect,transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x+randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0); gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        PlayerDestroy = Instantiate(PlayerDestroyEffect, transform);
        PlayerDestroy.transform.position = new Vector3(transform.position.x + randomX, transform.position.y + randomY, 0);
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
        arc.clip = DestroySE;
        arc.Play();
        yield return new WaitForSeconds(0.3f);
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        timeManager.LoadResultScene();
    }
    void Update()
    {
        randomX = Random.Range(-0.5f, 0.5f);
        randomY = Random.Range(-0.5f, 0.5f);
        randomRotateZ = Random.Range(-360, 360);
    }
}
