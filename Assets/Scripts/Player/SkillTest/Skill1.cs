using ITKombat;
using UnityEngine;

[CreateAssetMenu]
public class Skill1 : Skills 
{   
    public override void Activate(GameObject parent)
    {
        // Logic skill di taruh disini
        // Contoh
        Debug.Log("Skill 1 Aktif");
    }
    public override void BeginCooldown(GameObject parent)
    {
        //Logic cooldown skill di taruh disini
        Debug.Log("Skill 1 Cooldown");
    }
}
