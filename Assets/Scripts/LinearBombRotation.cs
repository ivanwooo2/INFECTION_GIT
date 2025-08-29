using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBombRotation : LinearBomber
{
    private Transform _transform;
    [SerializeField] private Vector3 rotation;

    void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        if (TimeManager.IsSkillPaused)
        {
            return;
        }
        _transform.Rotate(rotation * Time.deltaTime);
    }
}
