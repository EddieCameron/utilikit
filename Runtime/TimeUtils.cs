﻿/* TimeUtils.cs
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
            if ( string.IsNullOrEmpty( timeString ) ) {
                // no saved infos
                this._time = default( DateTime );
                this.IsValid = false;
            }
            else if ( DateTime.TryParse( timeString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,  out DateTime parsedTime ) ) {
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
    [Serializable]
    public struct SerializableTimeSpan : ISerializationCallbackReceiver {
        [SerializeField]
        long _spanTicks; // string so you can read it

        TimeSpan _time;
        public TimeSpan Time {
            get => _time;
            set {
                _time = value;
            }
        }

        public SerializableTimeSpan( TimeSpan time ) {
            this._time = time;
            this._spanTicks = time.Ticks;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            _spanTicks = _time.Ticks;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            this._time = new TimeSpan( _spanTicks );
        }

        public static implicit operator TimeSpan(SerializableTimeSpan d) => d.Time;
        public static implicit operator SerializableTimeSpan(TimeSpan d) => new SerializableTimeSpan( d );
    }
}
