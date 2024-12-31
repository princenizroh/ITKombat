using UnityEngine;

namespace ITKombat
{
    public class Gacha : MonoBehaviour
    {
        private GameFirebase gameFirebase;
        public GameObject gachaScene;
        public GameObject gachaResult;
        private string gearName;
        
        // // Start is called once before the first execution of Update after the MonoBehaviour is created
        // void Start()
        // {
        
        // }

        // // Update is called once per frame
        // void Update()
        // {
        
        // }

        public void doAGacha(string player_id, string type_value) {

            if (type_value == "danus") {
                gameFirebase.ChangeValueInteger(player_id, "balances", "sk2pm", -200);

                gameFirebase.AddGearRandom(player_id);

            } else if (type_value == "sk2pm") {
                gameFirebase.ChangeValueInteger(player_id, "balances", "sk2pm", -20);

                gameFirebase.AddGearRandom(player_id);
            }

        }


    }
}
