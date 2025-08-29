using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{

    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public float minInterval = 3f;
        public float maxInterval = 5f;
        public float cooldown = 10f;
        public bool isOnCooldown = false;
    }

    [SerializeField] private GameObject expandCirclePrefab;

    public AttackPattern[] attackPatterns = new AttackPattern[3];

    public GameObject linearBombPrefab;
    public GameObject warningProjectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PatternScheduler());
    }

    IEnumerator PatternScheduler()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            List<int> availablePatterns = new List<int>();
            for (int i = 0; i < attackPatterns.Length; i++)
            {
                if (!attackPatterns[i].isOnCooldown)
                    availablePatterns.Add(i);
            }

            if (availablePatterns.Count > 0)
            {
                int selected = availablePatterns[Random.Range(0, availablePatterns.Count)];
                StartCoroutine(TriggerPattern(selected));
            }
        }
    }

    IEnumerator TriggerPattern(int patternIndex)
    {
        attackPatterns[patternIndex].isOnCooldown = true;

        switch (patternIndex)
        {
            case 0:
                StartCoroutine(TriggerPattern1());
                break;
            case 1:
                StartCoroutine(TriggerPattern2());
                break;
            case 2:
                StartCoroutine(TriggerPattern3());
                break;
        }

        yield return new WaitForSeconds(attackPatterns[patternIndex].cooldown);
        
        attackPatterns[patternIndex].isOnCooldown = false;
    }

    IEnumerator TriggerPattern1() 
    {
        Vector2 spawnPos = new Vector2(
    Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0.2f, 0)).x,
                 Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0)).x),
    Random.Range(Camera.main.ViewportToWorldPoint(new Vector2(0, 0.2f)).y,
                 Camera.main.ViewportToWorldPoint(new Vector2(0, 0.8f)).y)
);

        GameObject circle = Instantiate(expandCirclePrefab, spawnPos, Quaternion.identity);
        yield return new WaitUntil(() => circle == null);
    }
    IEnumerator TriggerPattern2() 
    {
        bool isLeft = Random.value > 0.5f;
        Vector2 spawnPos = GetSpawnPosition(isLeft);

        GameObject bomb = Instantiate(linearBombPrefab, spawnPos, Quaternion.identity);
        bomb.GetComponent<LinearBomber>().InitializeDirection(isLeft);

        yield return new WaitUntil(() => bomb == null);
    }

    Vector2 GetSpawnPosition(bool isLeft)
    {
        float yPos = Random.Range(0.2f, 0.8f); 
        Vector2 viewportPos = new Vector2(
            isLeft ? -0.1f : 1.1f, 
            yPos
        );
        return Camera.main.ViewportToWorldPoint(viewportPos);
    }
    IEnumerator TriggerPattern3() 
    {
        GameObject projectile = Instantiate(
           warningProjectilePrefab,
           Vector3.zero, 
           Quaternion.identity
       );

        yield return new WaitUntil(() => projectile == null);
    }

}
