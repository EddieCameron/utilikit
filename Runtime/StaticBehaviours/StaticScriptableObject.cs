using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Utilikit {
    public class StaticScriptableObject<T> : ScriptableObject where T : ScriptableObject {
        static string FileName => typeof( T ).Name;

#if UNITY_EDITOR
        static string AssetDirectory => $"Assets/Resources/StaticScriptableObjects/";
        static string AssetPath => $"{AssetDirectory}{FileName}.asset";
#endif
        static string ResourcesPath => "StaticScriptableObjects/" + FileName;

        static T _instance;
        public static T Instance {
            get {
                if ( _instance == null ) {
                    _instance = Resources.Load<T>( ResourcesPath );
                    if ( _instance == null ) {
                        // create scriptableobject asset
                        _instance = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
                        void SaveScriptableObjectAsset( T asset ) {
                            if ( !Directory.Exists( AssetDirectory ) ) {
                                Directory.CreateDirectory( AssetDirectory );
                            }
                            AssetDatabase.CreateAsset( _instance, AssetPath );
                            AssetDatabase.SaveAssets();
                            Debug.Log( "Saved " + FileName + " instance" );
                        }

                        SaveScriptableObjectAsset( _instance );
#else
                    Debug.LogError("No instance of Static Scriptable found: " + FileName);
#endif
                    }
                }
                return _instance;
            }
        }
    }
}
