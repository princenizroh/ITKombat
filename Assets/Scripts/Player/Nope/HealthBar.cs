using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using ITKombat;

public class HealthBar : NetworkBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public int maxHealth = 100;
    public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public float lerpSpeed = 5f;
    public RoundTracker roundTracker; // skript RoundTracker
    public SidePlayer playerSide; // Use the fully qualified name

    private void Start() 
    {
        Debug.Log($"Owner: {NetworkObject.OwnerClientId}, Is Owner: {IsOwner}");
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        UpdateHealthBar();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Q) && IsOwner)
        {
            Debug.Log("Q pressed and player is owner.");
            Debug.Log("Darah berkurang 10");
            TakeDamage(10);
        }
        else if (!IsOwner)
        {
            Debug.Log("Not the owner of the object.");
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, lerpSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Attempting to take {damage} damage. Current health: {health.Value}");

        if (IsServer)
        {
            health.Value -= damage;
            if (health.Value <= 0)
            {
                health.Value = 0;
                Debug.Log("Anda mati!");
                roundTracker.RecordRoundWinner(playerSide.playerSide == SidePlayer.Side.Left ? RoundTracker.Side.Right : RoundTracker.Side.Left);

                if (roundTracker.FinalRound())
                {
                    roundTracker.ShowEndGameButton(); 
                }
                else
                {
                    roundTracker.StartNextRound();
                }
            }
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        healthSlider.value = health.Value;
    }

    public override void OnNetworkSpawn()
    {
        health.OnValueChanged += (oldValue, newValue) => {
            UpdateHealthBar();
        };
    }
}
