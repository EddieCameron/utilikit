using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public class StaticBehaviour<T> : MonoBehaviour where T : MonoBehaviour {

        static object _lock = new object();
        static bool applicationIsQuitting;

        static List<T> _sceneInstances = new List<T>( 1 );

        private static T _instance;
        public static T Instance {
            get {
                lock ( _lock ) {
                    if ( _instance == null ) {
                        if ( applicationIsQuitting ) {
                            Debug.LogWarning( "Application is quitting but something is trying to reference a static behaviour" );
                            return null;
                        }

                        var instances = FindObjectsOfType<T>();
                        if ( instances.Length > 1 ) {
                            Debug.LogError( "Multiple singleton instances of type " + typeof( T ) );
                            _instance = instances[0];
                        }
                        else if ( instances.Length == 1 )
                            _instance = instances[0];
                        else {  // no instances
                            Debug.Log( "Static Behaviour instanced, creating: " + typeof( T ) );
                            _instance = new GameObject( typeof( T ) + " - Static" ).AddComponent<T>();
                        }

                        DontDestroyOnLoad( _instance );
                    }
                    return _instance;
                }
            }
        }

        protected virtual void Awake() {
            if ( _instance && _instance != this ) {
                Debug.LogWarning( "Multiple singleton instances of type " + typeof( T ) );
                Destroy( this );
            }
            _instance = Instance;
        }

        protected virtual void OnApplicationQuit() {
            applicationIsQuitting = true;
        }
    }
}