using UnityEngine;

namespace ITKombat
{
    public class AISpawner : MonoBehaviour
    {
        private void Start()
        {
            // Instansiasi karakter yang dipilih dari GameManager
            Instantiate(GameManagerSelect.instance.currentAICharacter.prefab, transform.position, Quaternion.identity);
        }
    }
}
