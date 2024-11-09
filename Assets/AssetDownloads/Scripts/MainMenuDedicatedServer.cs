using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDedicatedServer : MonoBehaviour {


    private void Start() {
#if DEDICATED_SERVER
        Debug.Log("DEDICATED_SERVER 6.8");
        Loader.Load(Loader.Scene.LobbyScene);
#endif
    }

}