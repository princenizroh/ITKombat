using ITKombat;
using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class Skill3 : Skills
{
    public float vanishDuration = 4f; // Durasi menghilang dalam detik
    public override void Activate(GameObject parent)
    {
        // Mulai efek menghilang pada karakter
        parent.GetComponent<MonoBehaviour>().StartCoroutine(VanishEffect(parent));
        Debug.Log("Skill 3 Aktif - Karakter menghilang sementara");
    }

    private IEnumerator VanishEffect(GameObject parent)
    {
        // Menghilangkan sprite agar karakter tidak terlihat
        SpriteRenderer spriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Menyembunyikan sprite
        }

        // Mendaftarkan event untuk mengakhiri skill jika terkena damage
        PlayerState playerState = parent.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.OnTakeDamage += CancelVanish; // Mendaftarkan event cancel vanish
        }

        // Menunggu sampai durasi menghilang selesai
        yield return new WaitForSeconds(vanishDuration);

        // Mengakhiri vanish secara otomatis setelah durasi berakhir
        EndVanish(parent);
    }

    private void EndVanish(GameObject parent)
    {
        // Menampilkan kembali sprite karakter
        SpriteRenderer spriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true; // Menampilkan kembali sprite
        }

        // Menghapus event cancel vanish
        PlayerState playerState = parent.GetComponent<PlayerState>();
        if (playerState != null)
        {
            playerState.OnTakeDamage -= CancelVanish; // Menghapus event
        }

        Debug.Log("Skill 3 selesai - Karakter muncul kembali");
    }

    private void CancelVanish(GameObject parent)
    {
        // Mengakhiri vanish ketika terkena damage
        EndVanish(parent);
        Debug.Log("Skill 3 dibatalkan karena terkena damage");
    }


    public override void BeginCooldown(GameObject parent)
    {
        //Logic cooldown skill di taruh disini
        Debug.Log("Skill 3 Cooldown");
    }
}
