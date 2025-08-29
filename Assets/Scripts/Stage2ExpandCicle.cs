using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2ExpandCicle : MonoBehaviour
{
    public Transform outerCircle;
    public Transform innerCircle;
    public GameObject expandCircle;

    public float expandDuration = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public int maxDepth = 2;
    public int currentDepth = 0;
    public Vector3 childProjectileScale;

    private Vector3 finalScale;
    private bool isExpanding;
    private Vector3 spawnPos;
    private Vector3 newSpawnPos;

    void Start()
    {
        if (currentDepth == 0)
        {
            finalScale = outerCircle.localScale;
        }
        innerCircle.localScale = Vector3.one * 0.1f;
        StartCoroutine(ExpandInnerCircle());
        LaunchProjectile();
    }

    IEnumerator ExpandInnerCircle()
    {
        isExpanding = true;
        float timer = 0;
        Vector3 initialScale = innerCircle.localScale;

        while (timer < expandDuration)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            float progress = timer / expandDuration;
            innerCircle.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        innerCircle.localScale = finalScale;

        isExpanding = false;
        Destroy(expandCircle);
    }

    void LaunchProjectile()
    {
        if (currentDepth <= 0)
        {
            spawnPos = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1.1f));
        }
        else if (currentDepth >= 1)
        {
            spawnPos = newSpawnPos;
        }
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        FireBottle firebottle = projectile.GetComponent<FireBottle>();

        if (currentDepth == 1)
        {
            childProjectileScale = projectile.transform.localScale;
            Vector3 projectileScale = childProjectileScale * 0.5f;
            projectile.transform.localScale = projectileScale;
        }
        else if (currentDepth == 2)
        {
            childProjectileScale = projectile.transform.localScale;
            Vector3 projectileScale = childProjectileScale * 0.2f;
            projectile.transform.localScale = projectileScale;
        }

        firebottle.Initialize(
            target: transform.position,
            travelTime: expandDuration
        );
        newSpawnPos = transform.position;
    }
}
