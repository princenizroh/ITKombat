using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class PlayerData
    {
        [field: Tooltip("The player's data.")]
        [field: SerializeField] public string player_id { get; private set; }
        [field: SerializeField] public string username { get; private set; }
        [field: SerializeField] public string email{ get; private set; }
        // [field: SerializeField] public int registration_date { get; private set; }
        //
        public PlayerData(string _player_id, string _username, string _email)
        {
            this.player_id = _player_id;
            this.username = _username;
            this.email = _email;
            // this.registration_date = _registration_date;
        }
    }


}
