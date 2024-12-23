using UnityEngine;

namespace ITKombat
{
    public class CharacterSelectVisual : MonoBehaviour
    {
        [SerializeField] private Transform playerModelParent;

        private GameObject currentModel;

        public void SetPlayerPrefab(GameObject prefab)
        {
            // Hapus model sebelumnya jika ada
            if (currentModel != null)
            {
                Destroy(currentModel);
                Debug.Log("Destroy current model");
            }

            Debug.Log("Instantiate new model");

            // Instansiasi prefab baru
            currentModel = Instantiate(prefab, playerModelParent);
            Debug.Log("Instantiate prefab");
            currentModel.transform.localPosition = Vector3.zero;
            Debug.Log("Set local position");
            currentModel.transform.localRotation = Quaternion.identity;
            Debug.Log("Set local rotation");
        }

    }
}
