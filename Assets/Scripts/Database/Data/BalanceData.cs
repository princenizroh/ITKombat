using UnityEngine;
using System;
namespace ITKombat
{
    [Serializable]
    public class BalanceData
    {
        [field: Tooltip("The amount of KTM the player has.")]
        [field: SerializeField] public string balance_id { get; private set; }
        [field: SerializeField] public int KTM { get; private set; }
        [field: SerializeField] public int danus { get; private set; }
        [field: SerializeField] public string player_id{ get; private set; } 

        public BalanceData(string _balance_id, int _KTM, int _danus, string _player_id)
        {
            this.balance_id = _balance_id;
            this.KTM = _KTM;
            this.danus = _danus;
            this.player_id = _player_id;
        }
    }
}
