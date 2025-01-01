using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Create Characters", menuName = "Characters/CreateCharacters", order =1)]
    public class CharacterStat : ScriptableObject
    {
        public string characterName;
        public string characterClass;
        public string characterTier;        
        public int characterBaseAtk;
        public int characterBaseDef;
        public int characterBaseInt;
        public int characterGroupId;
        public Sprite Skill1;
        public Sprite Skill2;
        public Sprite Skill3;
        public Sprite characterClassImage;
        public Sprite characterTierImage;
        [TextArea (10,30)] public string character_description;
    }
}
