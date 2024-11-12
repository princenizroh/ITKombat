using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public Image actionButtonImage; // Drag & drop tombol UI yang ingin diubah di inspector
    public Sprite attackSprite;     // Sprite untuk default tombol Attack
    public Sprite doorSprite;       // Sprite untuk tombol Door
    public Sprite shopSprite;       // Sprite untuk tombol Shop
    public Sprite topUpSprite;      // Sprite untuk tombol Top Up

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pintu"))
        {
            actionButtonImage.sprite = doorSprite;  // Ganti gambar tombol menjadi Door
        }
        else if (collision.CompareTag("ShopItem"))
        {
            actionButtonImage.sprite = shopSprite;  // Ganti gambar tombol menjadi Shop
        }
        else if (collision.CompareTag("TopUp"))
        {
            actionButtonImage.sprite = topUpSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pintu") || collision.CompareTag("ShopItem") || collision.CompareTag("TopUp"))
        {
            actionButtonImage.sprite = attackSprite;  // Kembali ke gambar default Attack
        }
    }
}
