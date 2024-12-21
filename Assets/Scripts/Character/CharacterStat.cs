using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    [CreateAssetMenu(fileName = "Create Characters", menuName = "Characters/CreateCharacters", order =1)]
    public class CharacterStat : ScriptableObject
    {
        public string characterName;
        public int characterBaseAtk;
        public int characterBaseDef;
        public int characterBaseInt;
        public int characterGroupId;
        [TextArea (10,30)] public string character_description;
    }
}
