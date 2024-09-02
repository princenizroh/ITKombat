using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public GameObject Objek;
    private bool isObjekInside = false;

    public Button yourButton; // Drag your button here in the inspector

    private void Start()
    {
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Objek)
        {
            isObjekInside = true;
            Debug.Log("Objek berada di dalam collider.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Objek)
        {
            isObjekInside = false;
            Debug.Log("Objek keluar dari collider.");
        }
    }

    private void OnButtonClick()
    {
        if (isObjekInside)
        {
            Debug.Log("Tombol diklik dan objek berada di dalam collider. Memuat scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("Objek tidak berada di dalam collider. Tidak dapat memuat scene.");
        }
    }
}
