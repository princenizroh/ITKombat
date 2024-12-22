using UnityEngine;
using UnityEngine.UI;
namespace ITKombat
{
    public class SelectCharacterMultiplayerUI : MonoBehaviour
    {
        [SerializeField] private Button readyButton;

        private void Awake()
        {
            readyButton.onClick.AddListener(() =>
            {
                CharacterSelectReadyMultiplayer.Instance.SetPlayerReady();
            });
        }
    }
}
