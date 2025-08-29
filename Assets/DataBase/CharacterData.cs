using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/New Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    [TextArea(3, 10)] public string Skilldescription;
    [TextArea(3, 10)] public string CRdescription;
    public bool isUnlocked;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite backGround;
    public Sprite SkillIcon;
}
