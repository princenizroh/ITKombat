using UnityEngine;
using System;

namespace ITKombat
{
    [Serializable]
    public class PlayerData
    {
        [field: Tooltip("The player's data.")]
        public string username;
        public string password;
        public string email;
        // [field: SerializeField] public int registration_date { get; private set; }
        //
        public PlayerData(string _username, string _password, string _email)
        {
            this.username = _username;
            this.password = _password;
            this.email = _email;
            // this.registration_date = _registration_date;
        }
    }


}
