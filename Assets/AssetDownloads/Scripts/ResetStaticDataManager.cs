using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour {


    private void Awake() {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
        CharacterSelectReady.ResetStaticData();
    }

}