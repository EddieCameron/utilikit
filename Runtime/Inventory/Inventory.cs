using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public class Inventory {

        #region Global inventory
        static Inventory _global;
        public static Inventory Global {
            get {
                if ( _global == null ) {
                    _global = new Inventory();  // only create when accessed to keep the config file optional
                }
                return _global;
            }
        }

#if UNITY_EDITOR
        // for domain reloading
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
        static void Initialize() {
            _global = null;
        }
#endif
        #endregion

        Dictionary<string, int> _inventoryQuantities = new Dictionary<string, int>();

        public Inventory( bool loadDefaultContents = true ) {
            if ( loadDefaultContents )
                LoadDefaultContents();
        }

        public int GetQuantity( string itemId ) {
            if ( _inventoryQuantities.TryGetValue( itemId, out int q ) )
                return q;
            return 0;
        }

        public IEnumerable<Quantity> AllItems {
            get {
                foreach ( var item in _inventoryQuantities )
                    yield return new Quantity( item.Key, item.Value );
            }
        }

        public void AddItems( string itemId, int quantity = 1 ) {
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

        public bool TryTakeItems( string itemId, int quantity ) {
            return TryTakeItems( itemId, quantity, out int _ );
        }

        public bool TryTakeItems( string itemId, int quantity, out int remainingQuantity ) {
            if ( quantity <= 0 )
                throw new ArgumentException( "Invalid number of items to use" );

            remainingQuantity = GetQuantity( itemId );
            if ( remainingQuantity < quantity ) {
                return false;
            }

            remainingQuantity -= quantity;
            if ( remainingQuantity == 0 ) {
                _inventoryQuantities.Remove( itemId );
            }
            else {
                _inventoryQuantities[itemId] = remainingQuantity;
            }
            return true;
        }

        public void LoadDefaultContents() {
            // Pre-fill inventory
            _inventoryQuantities.Clear();
            foreach ( var quantity in InventoryConfig.Instance.initialQuantities ) {
                _inventoryQuantities[quantity.itemId] = quantity.quantity;
            }
        }

        public void RestoreFromRecord( Record record ) {
            _inventoryQuantities.Clear();
            foreach ( var item in record.inventoryContents ) {
                _inventoryQuantities[item.itemId] = item.quantity;
            }
        }

        public Record GetRecord() {
            return new Record( _inventoryQuantities );
        }

        [Serializable]
        public struct Quantity {
            public string itemId;
            public int quantity;

            public Quantity( string itemId, int quantity ) {
                this.itemId = itemId;
                this.quantity = quantity;
            }
        }

        [Serializable]
        public class Record {
            public Quantity[] inventoryContents;

            public Record( IDictionary<string, int> inventoryDict ) {
                inventoryContents = new Quantity[inventoryDict.Count];
                int i = 0;
                foreach ( var item in inventoryDict ) {
                    inventoryContents[i] = new Quantity( item.Key, item.Value );
                    i++;
                }
            }
        }
    }
}
