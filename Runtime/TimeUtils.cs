/* TimeUtils.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {

    [Serializable]
    public struct SerializableDateTime : ISerializationCallbackReceiver {
        [SerializeField]
        string timeString; // string so you can read it

        DateTime _time;
        public DateTime Time {
            get => _time;
            set {
                _time = value;
                IsValid = true;
            }
        }
        public bool IsValid { get; private set; }   // false if parsing failed

        public SerializableDateTime( DateTime time ) {
            this._time = time;
            this.timeString = time.ToString( "o" );

            this.IsValid = true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if ( !IsValid )
                timeString = "";
            else
                timeString = Time.ToString( "o" );
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if ( DateTime.TryParse( timeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,  out DateTime parsedTime ) ) {
                this._time = parsedTime;
                this.IsValid = true;
            }
            else {
                Debug.LogError( "Couldn't parse saved DateTime: " + timeString );
                this._time = default( DateTime );
                this.IsValid = false;
            }
        }

        public static implicit operator DateTime(SerializableDateTime d) => d.Time;
        public static implicit operator SerializableDateTime(DateTime d) => new SerializableDateTime( d );
    }
}
