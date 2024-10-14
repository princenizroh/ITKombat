using UnityEngine;
using UnityEngine.UI;

namespace ITKombat
{
    public class AuthUI : MonoBehaviour
    {
        [SerializeField] private Button authenticateButton;

        private void Awake()
        {
            authenticateButton.onClick.AddListener(() =>
            {
                LobbyRoomManager.Instance.Authenticate(LobbyRoomManager.Instance.GetFirebaseUser());
                Hide();
            });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
