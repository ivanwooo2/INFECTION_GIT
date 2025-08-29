using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragGrendeRotation : MonoBehaviour
{
    private Transform _transform;
    private fragGrenade fraggrenade;
    void Start()
    {
        _transform = transform;
        fraggrenade = GetComponentInParent<fragGrenade>();
        if (fraggrenade != null )
        {
            if (fraggrenade.moveDirection == Vector2.right)
            {
                transform.Rotate(0, 0, -180);
            }
            else if (fraggrenade.moveDirection == Vector2.left)
            {
                transform.Rotate(0,0,0);
            }
        }
    }

    void Update()
    {
        
    }
}
