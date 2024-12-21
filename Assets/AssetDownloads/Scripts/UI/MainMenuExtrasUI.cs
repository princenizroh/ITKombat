using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuExtrasUI : MonoBehaviour {


    [SerializeField] private Button youTubeButton;


    private void Awake() {
        youTubeButton.onClick.AddListener(() => {
            Application.OpenURL("https://youtu.be/7glCsF9fv3s");
        });
    }

}