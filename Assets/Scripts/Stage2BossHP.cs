using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2BossHP : MonoBehaviour
{
    [SerializeField] private Collider2D damageCollider;
    private Stage2BossController controller;
    void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
        }
        controller = FindObjectOfType<Stage2BossController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        controller.OnBossTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        controller.OnBossTriggerExit(other);
    }
}
