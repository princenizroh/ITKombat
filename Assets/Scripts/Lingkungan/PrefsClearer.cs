using UnityEngine;

namespace ITKombat{
    public class PlayerPrefsClearer : MonoBehaviour
{
    private void Start()
    {
        ClearAllPlayerPrefs();
    }

    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs data has been deleted.");
    }
}
}

