using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHPTutorial : MonoBehaviour
{
    [SerializeField] private Collider2D damageCollider;
    private BossManagerTutorial BossManagerTutorial;
    void Start()
    {
        BossManagerTutorial = FindAnyObjectByType<BossManagerTutorial>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BossManagerTutorial.OnBossTriggerEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BossManagerTutorial.OnBossTriggerExit(other);
    }
}
