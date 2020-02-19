using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keep track of single occurance events
/// </summary>
public static class Flags {
    static HashSet<string> _setFlags = new HashSet<string>();

    #if UNITY_EDITOR
    // for domain reloading
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
    static void Initialize() {
        _setFlags.Clear();
    }
    #endif

    public static bool GetFlag( string flag ) {
        return _setFlags.Contains( flag );
    }

    public static void SetFlag( string flag ) {
        _setFlags.Add( flag );
    }

    public static void UnsetFlag( string flag ) {
        _setFlags.Remove( flag );
    }

    public static void RestoreFromRecord( Record record ) {
        _setFlags.Clear();
        foreach ( var flag in record.setFlags ) {
            _setFlags.Add( flag );
        }
    }

    public static Record GetRecord() {
        return new Record( _setFlags );
    }

    [Serializable]
    public class Record {
        public string[] setFlags;

        public Record( ICollection<string> setFlags ) {
            this.setFlags = new string[setFlags.Count];
            setFlags.CopyTo( this.setFlags, 0 );
        }
    }
}
