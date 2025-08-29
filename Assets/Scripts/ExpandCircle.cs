using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandCircle : MonoBehaviour
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
    private Transform player;
    private Vector3 spawnPos;
    private Vector3 newSpawnPos;
    private ParabolaProjectile ParabolaProjectile;
    public GameObject stoneeffectPrefab;

    void Start()
    {
        ParabolaProjectile = projectilePrefab.GetComponent<ParabolaProjectile>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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
        GameObject stoneffect = Instantiate(stoneeffectPrefab,transform.position,Quaternion.identity);
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
        ParabolaProjectile parabola = projectile.GetComponent<ParabolaProjectile>();

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

        parabola.Initialize(
            target: transform.position,
            travelTime: expandDuration,
            player.gameObject,
            player.gameObject,
            this
        );
        newSpawnPos = transform.position;
    }

    public void SpawnChildCircles(Vector3 center)
    {
        if (currentDepth >= maxDepth) return;

        Vector3[] offsets = {
            new Vector3(1.5f, 1.5f, 0),
            new Vector3(-1.5f, 1.5f, 0),
            new Vector3(1.5f, -1.5f, 0),
            new Vector3(-1.5f, -1.5f, 0)
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 spawnPos = center + offset;
            GameObject circle = Instantiate(expandCircle, spawnPos, Quaternion.identity);
            ExpandCircle childCircle = circle.GetComponent<ExpandCircle>();

            if (childCircle != null)
            {
                childCircle.currentDepth = currentDepth + 1;
                childCircle.finalScale = finalScale * 0.5f;
                childCircle.expandDuration = expandDuration * 0.7f;
                childCircle.outerCircle.transform.localScale = finalScale * 0.5f;
            }
        }
    }
}
