using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Player1Attack : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 direction;
    private Stage2BossController BossController;
    private SpriteRenderer SpriteRenderer;
    private PlayerMovement PlayerMovement;
    [SerializeField] private Sprite NormalAttack;
    [SerializeField] private Sprite SkillAttack;
    private int PlayerIndex;
    void Start()
    {
        PlayerIndex = PlayerPrefs.GetInt("SelectedCharacterIndex");
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BossController = FindAnyObjectByType<Stage2BossController>();
        PlayerMovement = FindAnyObjectByType<PlayerMovement>();
        if (BossController != null)
        {
            direction = (BossController.currentBoss.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (PlayerMovement.isSkilling == true && PlayerIndex == 0)
        {
            SpriteRenderer.sprite = SkillAttack;
            transform.localScale = new Vector3(0.3f, 0.3f, 1);
        }
        else
        {
            SpriteRenderer.sprite = NormalAttack;
        }
    }

    void Update()
    {
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
