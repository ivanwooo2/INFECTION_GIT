using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFollow : MonoBehaviour
{
    private GameObject Player;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        Vector3 direction = Player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
