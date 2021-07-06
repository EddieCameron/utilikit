using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Utilikit {
    public class StaticScriptableObjectAddressable<T> : ScriptableObject where T : ScriptableObject {
        static string FileName => typeof( T ).Name;

        static string AssetDirectory => $"Assets/StaticScriptableObjects/";
        static string AssetPath => $"{AssetDirectory}{FileName}.asset";

        static T _instance;
        public static T Instance {
            get {
                if ( _instance == null ) {
                    var checkLocation = Addressables.LoadResourceLocationsAsync( AssetPath, typeof( T ) );
                    checkLocation.WaitForCompletion();
                    if ( checkLocation.Result.Count > 0 ) {
                        var load = Addressables.LoadAssetAsync<T>( checkLocation.Result[0] );
                        _instance = load.WaitForCompletion();
                    }
                    else {
                        // create scriptableobject asset
                        _instance = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
                        if ( !Directory.Exists( AssetDirectory ) ) {
                            Directory.CreateDirectory( AssetDirectory );
                        }
                        AssetDatabase.CreateAsset( _instance, AssetPath );
                        AssetDatabase.SaveAssets();
                        Debug.Log( "Saved " + FileName + " instance" );
#else
                        Debug.LogError( "No instance found at " + FileName );
#endif
                    }
                }
                return _instance;
            }
        }
    }
}
