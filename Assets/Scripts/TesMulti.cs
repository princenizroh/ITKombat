using UnityEngine;
using Unity.Services.Multiplay;
using Newtonsoft.Json;
namespace ITKombat
{
    public class TesMulti : MonoBehaviour
    {
        void Start()
        {
            string json = "{ \"ServerName\": \"TestServer\", \"MaxPlayers\": 10 }";
            ServerConfig config = LoadServerConfig(json);
           
        }
        public static ServerConfig LoadServerConfig(string json)
        {   
            Debug.Log("JSON Content: " + json);
            return JsonConvert.DeserializeObject<ServerConfig>(json);
        }
    }
}
