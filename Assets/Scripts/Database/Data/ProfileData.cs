using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class ProfileData
    {
        [field: Tooltip("The profile of the player.")]
        [field: SerializeField] public string profile_id { get; set; }
        [field: SerializeField] public string username { get; set; }
        [field: SerializeField] public int player_exp { get; set; }
        [field: SerializeField] public int player_level { get; set; }
        [field: SerializeField] public int player_rank_sk2pm { get; set; }
        [field: SerializeField] public int sk2pm { get; set; }
        [field: SerializeField] public string player_id { get; set; }
    }
}
