using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiffcultData", menuName = "Diffculty/Diffcult Data")]
public class DiffcultData : ScriptableObject
{
    public bool isUnlocked;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite BackGround;
}
