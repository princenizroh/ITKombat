using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InteractionSprite
{
    public string tagName;     // Nama tag untuk mendeteksi interaksi
    public Sprite sprite;      // Sprite yang akan digunakan
}

public class InteractionController : MonoBehaviour
{
    public Image actionButtonImage; 
    public Sprite defaultSprite; 
    public InteractionSprite[] interactionSprites; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var interaction in interactionSprites)
        {
            if (collision.CompareTag(interaction.tagName))
            {
                actionButtonImage.sprite = interaction.sprite; 
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var interaction in interactionSprites)
        {
            if (collision.CompareTag(interaction.tagName))
            {
                actionButtonImage.sprite = defaultSprite; 
                return;
            }
        }
    }
}
