using UnityEngine;
using Unity.Netcode;
using ITKombat;

public class Player1 : NetworkBehaviour
{
    public HealthBar healthBar; 

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthBar.enabled = true; 
        }
        else
        {
            healthBar.enabled = false;
        }
    }
}
