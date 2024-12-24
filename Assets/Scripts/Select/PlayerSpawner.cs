using UnityEngine;

namespace ITKombat
{
    public class PlayerSpawner : MonoBehaviour
    {
        private void Start()
        {
            // Instansiasi karakter yang dipilih dari GameManager
            Instantiate(GameManagerSelect.instance.currentCharacter.prefab, transform.position, Quaternion.identity);
            
            // Panggil method untuk assign komponen animator
            // AssignCharacterComponents(characterInstance);
        }

        // private void AssignCharacterComponents(GameObject characterInstance)
        // {
        //     // Ambil komponen PlayerMovement_2 dari karakter yang sudah diinstansiasi
        //     PlayerMovement_2 playerMovement = characterInstance.GetComponent<PlayerMovement_2>();

            

        //     // Pastikan anim terhubung dengan Animator setelah instansiasi
        //     // if (playerMovement != null && playerMovement.anim == null)
        //     // {
        //     //     playerMovement.anim = characterInstance.GetComponent<Animator>();
        //     // }
        // }
    }
}
