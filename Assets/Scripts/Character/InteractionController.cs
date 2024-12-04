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
    public Image actionButtonImage; // Drag & drop tombol UI yang ingin diubah di inspector
    public Sprite defaultSprite;    // Sprite default untuk tombol
    public InteractionSprite[] interactionSprites; // Array pasangan tag dan sprite

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cari sprite berdasarkan tag yang ditemukan
        foreach (var interaction in interactionSprites)
        {
            if (collision.CompareTag(interaction.tagName))
            {
                actionButtonImage.sprite = interaction.sprite; // Ganti gambar tombol
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Kembalikan ke sprite default jika keluar dari collider
        foreach (var interaction in interactionSprites)
        {
            if (collision.CompareTag(interaction.tagName))
            {
                actionButtonImage.sprite = defaultSprite; // Kembali ke gambar default
                return;
            }
        }
    }
}
