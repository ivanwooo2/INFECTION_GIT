using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1AttackManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 direction;
    private BossController BossController;
    private SpriteRenderer SpriteRenderer;
    private PlayerMovement PlayerMovement;
    [SerializeField] private Sprite NormalAttack;
    [SerializeField] private Sprite SkillAttack;
    [SerializeField] private Sprite Player2Attack;
    private int PlayerIndex;
    void Start()
    {
        PlayerIndex = PlayerPrefs.GetInt("SelectedCharacterIndex");
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BossController = FindAnyObjectByType<BossController>();
        PlayerMovement = FindAnyObjectByType<PlayerMovement>();
        if (BossController != null)
        {
            direction = BossController.currentBoss.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (PlayerMovement.isSkilling == true && PlayerIndex == 0)
        {
            SpriteRenderer.sprite = SkillAttack;
            transform.localScale = new Vector3(0.3f, 0.3f, 1);
        }
        else if (PlayerMovement.isSkilling == false && PlayerIndex == 0)
        {
            SpriteRenderer.sprite = NormalAttack;
        }
        else if (PlayerIndex == 1)
        {
            SpriteRenderer.sprite = Player2Attack;
            transform.localScale = new Vector3(0.1f, 0.1f, 1);
        }
    }

    void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
