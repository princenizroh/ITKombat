using UnityEngine;
using System.Collections.Generic;
namespace ITKombat
{
    public class GameDataSource : MonoBehaviour
    {
        public static GameDataSource Instance { get; private set; }

        [Header("Character classes")]
        [Tooltip("All CharacterClass data should be slotted in here")]
        [SerializeField]
        private CharacterClass[] m_CharacterData;

        Dictionary<CharacterTypeEnum, CharacterClass> m_CharacterDataMap;

        
    }
}
