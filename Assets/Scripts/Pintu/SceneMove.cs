using UnityEngine;

namespace ITKombat{
    public class FinishPoint : MonoBehaviour
{
    public void PindahScene(string targetSceneName)
    {
        SceneController.instance.LoadSceneByName(targetSceneName);
    }
}
}

