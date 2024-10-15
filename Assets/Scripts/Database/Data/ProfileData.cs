using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class ProfileData
    {
        [field: Tooltip("The profile of the player.")]
        public int playerExp;
        public int playerLevel;
        public string playerRank;
        public int sk2pm;

        public ProfileData(int _playerExp, int _playerLevel, string _playerRank, int _sk2pm)
        {
            this.playerExp = _playerExp;
            this.playerLevel = _playerLevel;
            this.playerRank = _playerRank;
            this.sk2pm = _sk2pm;
        }
    }
}
