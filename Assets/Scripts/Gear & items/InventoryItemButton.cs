using UnityEngine;

public class InventoryItemButton : MonoBehaviour
{
    public ITKombat.NewGearManager.InventoryListAll itemData;

    // Optionally, you can add a method to debug the stored data
    public void PrintData()
    {
        if (itemData != null)
        {
            Debug.Log($"Item ID: {itemData.item_id}, Level: {itemData.item_level}, Ascend: {itemData.item_ascend}");
        }
        else
        {
            Debug.LogError("Item data is null!");
        }
    }
}
