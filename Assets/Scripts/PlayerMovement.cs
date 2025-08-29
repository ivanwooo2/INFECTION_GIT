using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playermoveSpeed;
    [SerializeField] private float SlowMoveSpeed;
    [SerializeField] private float playerSkillmoveSpeed;
    [SerializeField] private float originalMoveSpeed;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private Sprite origin1, origin2;
    [SerializeField] private Sprite Skill1, Skill2;
    [SerializeField] private Sprite PlayerSkin1, PlayerSkin2;
    private Vector2 playerPosition;

    [SerializeField] private float activeMoveSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTime;
    private float dashLengthCounter;
    private SpriteRenderer spriteRenderer;

    [Header("Cooldown UI")]
    [SerializeField] private Image dashCooldownImage;
    [SerializeField] private Image skillCooldownImage;
    [SerializeField] private TMP_Text dashCooldownText;
    [SerializeField] private TMP_Text skillCooldownText;
    [SerializeField] private GameObject SkillusingImage;
    [SerializeField] private GameObject SkillCooldownBG;

    [SerializeField] private float skill2FreezeDuration = 6f;
    [SerializeField] private float skill2Cooldown = 16f;
    private float skillCooldownRemaining;

    [SerializeField] private float skill1Duration = 5f;
    [SerializeField] private float skill1Cooldown = 10f;
    [SerializeField] private float scaleMultiplier = 0.5f;
    private bool isSkill1Ready = true;
    private bool isSkill2Ready = true;
    public bool isSkilling;
    private PlayerHealth playerHealth;
    private BossController bossController;
    private Stage2BossController stage2BossController;

    [SerializeField] private TrailRenderer playerTR;

    public AudioSource Player;
    public AudioClip dash, skill1, skill2;

    public bool isInvincible = false;
    public int PlayerIndex;

    void Start()
    {
        originalMoveSpeed = playermoveSpeed;
        playerHealth = GetComponent<PlayerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        activeMoveSpeed = playermoveSpeed;
        bossController = FindObjectOfType<BossController>();
        stage2BossController = FindObjectOfType<Stage2BossController>();
        PlayerIndex = PlayerPrefs.GetInt("SelectedCharacterIndex");
        if (PlayerIndex == 0)
        {
            spriteRenderer.sprite = PlayerSkin1;
        }
        else if (PlayerIndex == 1)
        {
            spriteRenderer.sprite = PlayerSkin2;
        }
    }

    void Update()
    {

        if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
        {
            playerRB.velocity = Vector2.zero;
            return;
        }

        UpdateCooldownUI();
        playerPosition.x = Input.GetAxisRaw("Horizontal");
        playerPosition.y = Input.GetAxisRaw("Vertical");

        playerPosition.Normalize();

        playerRB.velocity = playerPosition * activeMoveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Dash"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            if (dashCooldownTime <= 0 && dashLengthCounter <= 0)
            {
                Player.clip = dash;
                Player.Play();
                spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                isInvincible = true;
                if (playerTR != null)
                {
                    playerTR.emitting = true;
                }
                dashLengthCounter = dashLength;
                activeMoveSpeed = dashSpeed;

            }
        }

        if (Input.GetButtonDown("Slow"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            activeMoveSpeed = SlowMoveSpeed;
        }

        if (Input.GetButtonUp("Slow"))
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            activeMoveSpeed = originalMoveSpeed;
        }

        if (dashLengthCounter > 0)
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            dashLengthCounter -= Time.deltaTime;

            if (dashLengthCounter <= 0)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1); ;
                dashCooldownTime = dashCooldown;
                if (playerTR != null)
                {
                    playerTR.emitting = false;
                }
                isInvincible = false;
                if (!isSkilling)
                {
                    activeMoveSpeed = originalMoveSpeed;
                }
                else
                {
                    activeMoveSpeed = playerSkillmoveSpeed;
                }
            }
        }

        if (dashCooldownTime > 0)
        {
            dashCooldownTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q) && isSkill1Ready || Input.GetButtonDown("Skill") && isSkill1Ready)
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            if (PlayerIndex == 0)
            {
                Player.clip = skill1;
                Player.Play();
                StartCoroutine(ActivateSkill1());
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && isSkill2Ready || Input.GetButtonDown("Skill") && isSkill2Ready)
        {
            if (TimeManager.Instance != null && TimeManager.Instance.isGameOver)
            {
                playerRB.velocity = Vector2.zero;
                return;
            }
            if (PlayerIndex == 1)
            {
                Player.clip = skill1;
                Player.Play();
                StartCoroutine(ActivateSkill2());
            }
        }
    }

    private void UpdateCooldownUI()
    {
        if (dashCooldownImage != null)
        {
            float dashFill = Mathf.Clamp01(1 - (dashCooldownTime / dashCooldown));
            dashCooldownImage.fillAmount = dashFill;

            if (dashCooldownText != null)
            {
                dashCooldownText.text = dashCooldownTime > 0 ?
                    Mathf.Ceil(dashCooldownTime).ToString() : "";
            }
        }

        if (skillCooldownImage != null)
        {
            float skillFill = Mathf.Clamp01(1 - (skillCooldownRemaining / skill1Cooldown));
            skillCooldownImage.fillAmount = skillFill;

            if (skillCooldownText != null)
            {
                skillCooldownText.text = skillCooldownRemaining > 0 ?
                    Mathf.Ceil(skillCooldownRemaining).ToString() : "";
            }
        }
    }

    IEnumerator ActivateSkill1()
    {
        isSkill1Ready = false;
        isSkilling = true;
        playerHealth.ToggleSkill(true, scaleMultiplier);
        spriteRenderer.sprite = Skill1;
        activeMoveSpeed = playerSkillmoveSpeed;
        SkillusingImage.SetActive(true);
        SkillCooldownBG.SetActive(false);
        if (bossController != null)
        {
            bossController.ChrSkill1(skill1Duration);
        }
        else if (stage2BossController != null)
        {
            stage2BossController.ChrSkill1(skill1Duration);
        }
        yield return new WaitForSeconds(skill1Duration);
        SkillusingImage.SetActive(false);
        SkillCooldownBG.SetActive(true);
        Player.clip = skill2;
        Player.Play();
        isSkilling = false;
        spriteRenderer.sprite = origin1;
        activeMoveSpeed = originalMoveSpeed;
        playerHealth.ToggleSkill(false);
        skillCooldownRemaining = skill1Cooldown;
        while (skillCooldownRemaining > 0)
        {
            skillCooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        isSkill1Ready = true;
        skillCooldownRemaining = 0;
    }

    IEnumerator ActivateSkill2()
    {
        isSkill2Ready = false;
        isSkilling = true;
        TimeManager.TriggerSkillPause(skill2FreezeDuration);
        yield return new WaitForSeconds(skill2FreezeDuration);
        skillCooldownRemaining = skill2Cooldown;
        while (skillCooldownRemaining > 0)
        {
            skillCooldownRemaining -= Time.deltaTime;
            yield return null;
        }
        isSkill2Ready = true;
        isSkilling = false;
    }
}
