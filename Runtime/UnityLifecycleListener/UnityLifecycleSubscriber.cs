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
            if ( !UnityEditor.EditorApplication.isPlaying ) {
                UnityEditor.EditorApplication.update -= HandleUpdate;
                UnityEditor.EditorApplication.update += HandleUpdate;
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

        static void HandleUpdate() {
            foreach ( var listener in _listeners ) {
                listener?.Update();
            }
        }

        static void HandleOnApplicationQuit() {
            foreach ( var listener in _listeners ) {
                listener?.OnApplicationQuit();
            }
        }

        void Update() {
            HandleUpdate();
        }

        void OnApplicationQuit() {
            HandleOnApplicationQuit();
        }
    }

    public interface IUnityLifecycleListener {
        void Update();

        void OnApplicationQuit();
    }
}
