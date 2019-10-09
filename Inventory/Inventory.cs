using System;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory {
    static Dictionary<string, int> _inventoryQuantities = new Dictionary<string, int>();


    static Inventory() {
        // Pre-fill inventory
        foreach ( var quantity in InventoryConfig.Instance.initialQuantities ) {
            _inventoryQuantities[quantity.itemId] = quantity.quantity;
        }
        // TODO load from prefs
    }

    public static int GetQuantity( string itemId ) {
        if ( _inventoryQuantities.TryGetValue( itemId, out int q ) )
            return q;
        return 0;
    }

    public static bool TryUseItems( string itemId, int quantity, out int remainingQuantity ) {
        remainingQuantity = GetQuantity( itemId );
        if ( remainingQuantity < quantity ) {
            return false;
        }

        remainingQuantity -= quantity;
        _inventoryQuantities[itemId] = remainingQuantity;
        return true;
    }

    [Serializable]
    public struct Quantity {
        public string itemId;
        public int quantity;
    }
}