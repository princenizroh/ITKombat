using TMPro;
using UnityEngine;

namespace ITKombat
{
    public class PlayerDataInit : MonoBehaviour
    {

        public PlayerScriptableObject playerdata;
        public TMP_Text playerName;
        public TMP_Text playerUkt;
        public TMP_Text playerDanus;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            playerName.text = playerdata.playerName;
            playerUkt.text = playerdata.playerUkt.ToString();
            playerDanus.text = playerdata.playerDanus.ToString();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
