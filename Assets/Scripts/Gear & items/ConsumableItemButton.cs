using UnityEngine;

public class ConsumableItemButton : MonoBehaviour
{
    public ITKombat.NewGearManager.ConsumableListAll consumableData;

    // Optionally, you can add a method to debug the stored data
    public void PrintData()
    {
        if (consumableData != null)
        {
            Debug.Log($"Consumable ID: {consumableData.consumableId}, Quantitiy: {consumableData.consumableQuantity}, Value: {consumableData.consumableValue}");
        }
        else
        {
            Debug.LogError("Consumable data is null!");
        }
    }
}

