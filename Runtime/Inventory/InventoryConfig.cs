using Utilikit;
using UnityEngine;

public class InventoryConfig : StaticScriptableObject<InventoryConfig> {
    public string inventoryPrefsPrefix = "Inventory_";

    public Inventory.Quantity[] initialQuantities;
}