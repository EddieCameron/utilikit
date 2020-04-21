using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace Utilikit {
    public class AsyncHelper : IUnityLifecycleListener {

        public static void DoAfterDelay( float delaySeconds, Action action, bool useScaledTime = true ) {
            if ( delaySeconds < 0 )
                throw new ArgumentException( "Must have positive delay" );

            Instance.jobs.Add( new DelayedJob { delaySeconds = delaySeconds, job = action, useScaledTime = useScaledTime } );
        }

        static AsyncHelper _instance;
        static AsyncHelper Instance {
            get {
                if ( _instance == null )
                    _instance = new AsyncHelper();
                return _instance;
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Initialize() {
            _instance = null;
        }


        List<DelayedJob> jobs = new List<DelayedJob>();

        public AsyncHelper() {
            UnityLifecycleSubscriber.AddListener( this );
        }

        void IUnityLifecycleListener.OnApplicationQuit() {
            jobs.Clear();
        }

        void IUnityLifecycleListener.Update() {
            for ( int i = jobs.Count - 1; i >= 0; i-- ) {
                var job = jobs[i];
                job.delaySeconds -= job.useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                if ( job.delaySeconds < 0 ) {
                    // delay complete
                    job.job?.Invoke();
                    jobs.RemoveAt( i );
                }
                else {
                    jobs[i] = job;
                }
            }
        }

        struct DelayedJob {
            public float delaySeconds;
            public Action job;
            public bool useScaledTime;
        }
    }

    public class RepeatingJob : IUnityLifecycleListener {

        float _minTimeBetweenJobs;
        public float MinTimeBetweenJobs {
            get {
                return _minTimeBetweenJobs;
            }
            set {
                _minTimeBetweenJobs = value;
                _maxTimeBetweenJobs = Mathf.Max( value, _maxTimeBetweenJobs );
                thisJobInterval = Mathf.Clamp( thisJobInterval, MinTimeBetweenJobs, MaxTimeBetweenJobs );
            }
        }

        float _maxTimeBetweenJobs;
        public float MaxTimeBetweenJobs {
            get {
                return _maxTimeBetweenJobs;
            }
            set {
                _maxTimeBetweenJobs = value;
                _minTimeBetweenJobs = Mathf.Min( value, _minTimeBetweenJobs );
                thisJobInterval = Mathf.Clamp( thisJobInterval, MinTimeBetweenJobs, MaxTimeBetweenJobs );
            }
        }

        public bool UseScaledTime { get; set; } = true;

        public bool IsRunning { get; set; } = true;

        public Action job;

        float timeSinceLastJob;
        float thisJobInterval;

        public RepeatingJob( Action job, float secondsBetweenJobs ) {
            this._minTimeBetweenJobs = secondsBetweenJobs;
            this._maxTimeBetweenJobs = secondsBetweenJobs;
            this.job = job;

            UnityLifecycleSubscriber.AddListener( this );
        }

        public RepeatingJob( Action job, float minSecondsBetweenJobs, float maxSecondsBetweenJobs ) {
            this._minTimeBetweenJobs = minSecondsBetweenJobs;
            this._maxTimeBetweenJobs = maxSecondsBetweenJobs;
            this.job = job;

            UnityLifecycleSubscriber.AddListener( this );
        }

        public void Start() {
            if ( IsRunning )
                return;

            UnityLifecycleSubscriber.AddListener( this );
            IsRunning = true;
        }

        public void Stop() {
            UnityLifecycleSubscriber.RemoveListener( this );
            IsRunning = false;
        }

        void IUnityLifecycleListener.Update() {
            timeSinceLastJob += UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

            if ( timeSinceLastJob >= thisJobInterval ) {
                job?.Invoke();

                timeSinceLastJob = 0;
                thisJobInterval = Random.Range( _minTimeBetweenJobs, _maxTimeBetweenJobs );
            }
        }

        void IUnityLifecycleListener.OnApplicationQuit() {
        }
    }
}
