using UnityEngine;

namespace ITKombat
{
    public class DedicatedServer : MonoBehaviour
    {
        private void Start()
        {
            #if DEDICATED_SERVER
                Debug.Log("DEDICATED_SERVER 6.8");
                Loader.Load(Loader.Scene.WaitingUnranked);
            #endif
        }

    }
}
