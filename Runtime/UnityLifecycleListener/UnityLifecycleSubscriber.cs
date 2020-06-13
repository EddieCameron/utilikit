using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    /// <summary>
    /// Subscribes to unity lifecycle events for static listeners
    /// </summary>
    public class UnityLifecycleSubscriber : MonoBehaviour {

        static List<IUnityLifecycleListener> _listeners = new List<IUnityLifecycleListener>();

        static UnityLifecycleSubscriber _instance;

        public static void AddListener( IUnityLifecycleListener listener ) {
            SubscribeToUnityEvents();
            _listeners.Add( listener );
        }

        public static void RemoveListener( IUnityLifecycleListener listener ) {
            _listeners.Remove( listener );
        }

        static void SubscribeToUnityEvents() {
#if UNITY_EDITOR
            if ( !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                UnityEditor.EditorApplication.update -= OnEditorUpdate;
                UnityEditor.EditorApplication.update += OnEditorUpdate;
                UnityEditor.EditorApplication.quitting -= HandleOnApplicationQuit;
                UnityEditor.EditorApplication.quitting += HandleOnApplicationQuit;
            }
            else
#endif
        {
                if ( _instance == null ) {
                    _instance = new GameObject( "UnityLifecycleSubscriber" ).AddComponent<UnityLifecycleSubscriber>();
                    DontDestroyOnLoad( _instance );
                }
            }
        }

        static List<IUnityLifecycleListener> _listenerCache = new List<IUnityLifecycleListener>();
        static void HandleUpdate() {
            _listenerCache.Clear();
            _listenerCache.AddRange( _listeners );
            foreach ( var listener in _listenerCache ) {
                try {
                    listener?.Update();
                }
                catch ( Exception e ) {
                    Debug.LogException( e );
                }
            }
        }

        static void HandleOnApplicationQuit() {
            _listenerCache.Clear();
            _listenerCache.AddRange( _listeners );
            foreach ( var listener in _listenerCache ) {
                try {
                    listener?.OnApplicationQuit();
                }
                catch ( Exception e ) {
                    Debug.LogException( e );
                }
            }
        }

        void Update() {
            HandleUpdate();
        }

        void OnApplicationQuit() {
            HandleOnApplicationQuit();
        }

#if UNITY_EDITOR
        static void OnEditorUpdate() {
            if ( UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ) {
                return;
            }
            HandleUpdate();
        }
#endif
    }

    public interface IUnityLifecycleListener {
        void Update();

        void OnApplicationQuit();
    }
}
