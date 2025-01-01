using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace ITKombat
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [SerializeField] private Button createPublicButton;
        [SerializeField] private Button createPrivateButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;

        private void Awake()
        {
            createPublicButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.CreateLobby(lobbyNameInputField.text, false);
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });
            createPrivateButton.onClick.AddListener(() => {
                LobbyRoomManager.Instance.CreateLobby(lobbyNameInputField.text, true);
                NewSoundManager.Instance.PlaySound2D("Button_Click");
            });
        }
    }
}
