using UnityEngine;

namespace ITKombat
{
    public class SceneSpawner : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Instantiate(GameManagerScene.instance.currentScene.prefab, transform.position, Quaternion.identity);
        }

    }
}
