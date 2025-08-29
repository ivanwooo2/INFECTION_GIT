using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine;

public class AimMissile : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private int phaseDuration;
    [SerializeField] private GameObject explore;
    private GameObject Player;
    private Vector3 direction;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        StartCoroutine(Follow());
    }

    void Update()
    {
        
    }

    IEnumerator Follow()
    {
        float aimTime = 0;
        while (aimTime < phaseDuration)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            aimTime += Time.deltaTime;
            direction = (Player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 45);
            transform.position += direction * moveSpeed * 1f * Time.deltaTime;
            if (Vector2.Distance(transform.position,Player.transform.position) <= 0.4f )
            {
                GameObject Boom = Instantiate(explore, transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
                Destroy(gameObject);
            }
            yield return null;
        }
        GameObject BoomAnimation = Instantiate(explore, transform.position,Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
