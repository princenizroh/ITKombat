using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class LoginHistoryData
    {
        [field: Tooltip("The login history of the player.")]
        [field: SerializeField] public string login_history_id { get; set; }
        [field: SerializeField] public string login_date { get; set; }
        [field: SerializeField] public string logout_date { get; set; }
        [field: SerializeField] public string player_id { get; set; }
    }
}
