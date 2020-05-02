/* DoAfterDelay.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

public class DoAfterDelay : MonoBehaviour {

    public DelayedEvent[] delayEvents;

    [Serializable]
    public class DelayEvent : UnityEvent { }

    [Serializable]
    public struct DelayedEvent {
        public float delaySeconds;
        public DelayEvent onDelayAfterOnEnable;
    }

    AsyncHelper.DelayedJob[] _jobs;


    void OnEnable() {
        _jobs = new AsyncHelper.DelayedJob[delayEvents.Length];
        for ( int i = 0; i < delayEvents.Length; i++ ) {
            float delay = delayEvents[i].delaySeconds;
            DelayEvent onComplete = delayEvents[i].onDelayAfterOnEnable;
            _jobs[i] = AsyncHelper.DoAfterDelay( delay, () => {
                onComplete?.Invoke();
            }
            );
        }
    }

    void OnDisable() {
        foreach ( var job in _jobs ) {
            job.Cancel();
        }
    }
}
