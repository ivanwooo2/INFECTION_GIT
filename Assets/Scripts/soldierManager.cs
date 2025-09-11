using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soldierManager : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private float LockSpeed;
    [SerializeField] private GameObject[] BulletPrefab;
    [SerializeField] private int BulletCount;
    [SerializeField] private GameObject Shootingpoint;
    [SerializeField] private float ShootInterval;
    [SerializeField] private GameObject GunPrefab;
    [SerializeField] public int difficulty = 1;
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;
    private float gotoframe = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        GunPrefab.transform.rotation = Quaternion.Euler(0, 0, angle);
        if (direction.x < 0)
        {
            GunPrefab.transform.rotation = Quaternion.Euler(180, 0, -angle);
        }
    }
    public IEnumerator GoToTarget(Vector2 targetPosition,Vector2 startposition, float Timer)
    {
        gotoframe = 0f;
        while(gotoframe < Timer)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            gotoframe += Time.deltaTime;
            this.transform.position = Vector3.Lerp(startposition, targetPosition,gotoframe/Timer);
            yield return null;
        }
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < BulletCount; i++)
        {
            while (TimeManager.IsSkillPaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(ShootInterval);
            GameObject Bullet = Instantiate(BulletPrefab[0], Shootingpoint.transform.position,Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.Play();
            if (difficulty >= 2)
            {
                GameObject Bullet1 = Instantiate(BulletPrefab[1], Shootingpoint.transform.position, Quaternion.identity);
                GameObject Bullet2 = Instantiate(BulletPrefab[2], Shootingpoint.transform.position, Quaternion.identity);
            }
            if (difficulty == 3)
            {
                GameObject Bullet3 = Instantiate(BulletPrefab[3], Shootingpoint.transform.position, Quaternion.identity);
                GameObject Bullet4 = Instantiate(BulletPrefab[4], Shootingpoint.transform.position, Quaternion.identity);
            }
        }
        yield return new WaitForSeconds(0.5f);
        if (difficulty == 3)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            GameObject Bullet5 = Instantiate(BulletPrefab[5], Shootingpoint.transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
