using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public string StageName;
    public bool isUnlocked;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite BackGround;
}
