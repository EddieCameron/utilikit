using System;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory {
    static Dictionary<string, int> _inventoryQuantities = new Dictionary<string, int>();

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
    private static void Initialize() {
        RestoreDefaultContents();
    }

    public static int GetQuantity( string itemId ) {
        if ( _inventoryQuantities.TryGetValue( itemId, out int q ) )
            return q;
        return 0;
    }

    public static void AddItems( string itemId, int quantity = 1 ) {
        if ( quantity <= 0 )
            throw new ArgumentException( "Invalid number of items to add" );

        if ( _inventoryQuantities.TryGetValue( itemId, out int existingAmount ) ) {
            existingAmount += quantity;
            _inventoryQuantities[itemId] = existingAmount;
        }
        else {
            _inventoryQuantities[itemId] = quantity;
        }
    }

    public static bool TryTakeItems( string itemId, int quantity ) {
        return TryTakeItems( itemId, quantity, out int _ );
    }

    public static bool TryTakeItems( string itemId, int quantity, out int remainingQuantity ) {
        if ( quantity <= 0 )
            throw new ArgumentException( "Invalid number of items to use" );

        remainingQuantity = GetQuantity( itemId );
        if ( remainingQuantity < quantity ) {
            return false;
        }

        remainingQuantity -= quantity;
        _inventoryQuantities[itemId] = remainingQuantity;
        return true;
    }

    public static void RestoreDefaultContents() {
        // Pre-fill inventory
        _inventoryQuantities.Clear();
        foreach ( var quantity in InventoryConfig.Instance.initialQuantities ) {
            _inventoryQuantities[quantity.itemId] = quantity.quantity;
        }
    }

    public static void RestoreFromRecord( Record record ) {
        _inventoryQuantities.Clear();
        foreach ( var item in record.inventoryContents ) {
            _inventoryQuantities[item.itemId] = item.quantity;
        }
    }

    public static Record GetRecord() {
        return new Record( _inventoryQuantities );
    }

    [Serializable]
    public struct Quantity {
        public string itemId;
        public int quantity;
    }
    
    [Serializable]
    public class Record {
        public Quantity[] inventoryContents;

        public Record( IDictionary<string, int> inventoryDict ) {
            inventoryContents = new Quantity[inventoryDict.Count];
            int i = 0;
            foreach ( var item in inventoryDict ) {
                inventoryContents[i] = new Quantity { itemId = item.Key, quantity = item.Value };
                i++;
            }
        }
    }
}