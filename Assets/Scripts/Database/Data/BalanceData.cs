using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class BalanceData
    {
        [field: Tooltip("The amount of KTM the player has.")]
        public int KTM;
        public int danus;


        public BalanceData(int _KTM, int _danus)
        {
            this.KTM = _KTM;
            this.danus = _danus;
        }
    }
}
