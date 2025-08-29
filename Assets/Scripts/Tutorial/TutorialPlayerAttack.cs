using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerAttack : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 direction;
    private BossManagerTutorial BossManagerTutorial;
    private SpriteRenderer SpriteRenderer;
    private PlayerMovementTutorial PlayerMovementTutorial;
    [SerializeField] private Sprite NormalAttack;
    [SerializeField] private Sprite SkillAttack;
    void Start()
    {
        PlayerMovementTutorial = FindAnyObjectByType<PlayerMovementTutorial>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BossManagerTutorial = FindAnyObjectByType<BossManagerTutorial>();
        if (BossManagerTutorial != null)
        {
            direction = BossManagerTutorial.currentBoss.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }

        if (PlayerMovementTutorial.isSkilling == true)
        {
            SpriteRenderer.sprite = SkillAttack; 
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
}
