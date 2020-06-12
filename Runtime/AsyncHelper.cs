using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

namespace Utilikit {
    public class AsyncHelper : IUnityLifecycleListener {

        public static DelayedJob DoAfterDelay( float delaySeconds, Action action, bool useScaledTime = true, Component owner = null ) {
            if ( delaySeconds < 0 )
                throw new ArgumentException( "Must have positive delay" );

            var job = new DelayedJob { ExecuteInSeconds = delaySeconds, Job = action, UseScaledTime = useScaledTime, Owner = owner };
            Instance.jobs.Add( job );
            return job;
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


        HashSet<DelayedJob> jobs = new HashSet<DelayedJob>();

        public AsyncHelper() {
            UnityLifecycleSubscriber.AddListener( this );
        }

        void IUnityLifecycleListener.OnApplicationQuit() {
            jobs.Clear();
        }

        HashSet<DelayedJob> completedJobs = new HashSet<DelayedJob>();
        void IUnityLifecycleListener.Update() {
            completedJobs.Clear();
            foreach ( var job in jobs ) {
                if ( job.IsCancelled ) {
                    completedJobs.Add( job );
                }
                else {
                    job.ExecuteInSeconds -= job.UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                    if ( job.ExecuteInSeconds <= 0 ) {
                        // delay complete
                        completedJobs.Add( job );
                    }
                }
            }

            foreach ( var job in completedJobs ) {
                try {
                    if ( job.CanRun )
                        job.Job?.Invoke();
                }
                catch ( Exception e ) {
                    Debug.LogError( "Error excecuting job: " + e.Message );
                }
                finally {
                    jobs.Remove( job );
                }
            }
        }

        public class DelayedJob {
            public float ExecuteInSeconds { get; set; }
            public Action Job { get; set; }
            public bool UseScaledTime { get; set; } = true;

            bool _needValidOwner;
            Component _owner;
            public Component Owner {
                get => _owner;
                set {
                    _needValidOwner = value != null;
                    _owner = value;
                }
            }

            public bool IsCancelled { get; private set; } = false;
            public bool Cancel() => IsCancelled = true;

            public bool CanRun => !IsCancelled && !( _needValidOwner && Owner == null );
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

        public RepeatingJob( Action job, float secondsBetweenJobs, bool waitForExecute = false ) {
            this._minTimeBetweenJobs = secondsBetweenJobs;
            this._maxTimeBetweenJobs = secondsBetweenJobs;
            this.job = job;
            if ( waitForExecute )
                thisJobInterval = secondsBetweenJobs;

            UnityLifecycleSubscriber.AddListener( this );
        }

        public RepeatingJob( Action job, float minSecondsBetweenJobs, float maxSecondsBetweenJobs, bool waitForExecute = false ) {
            this._minTimeBetweenJobs = minSecondsBetweenJobs;
            this._maxTimeBetweenJobs = maxSecondsBetweenJobs;
            this.job = job;
            if ( waitForExecute )
                thisJobInterval = Random.Range( _minTimeBetweenJobs, _maxTimeBetweenJobs );

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

                try {
                    job?.Invoke();
                    timeSinceLastJob = 0;
                    thisJobInterval = Random.Range( _minTimeBetweenJobs, _maxTimeBetweenJobs );
                }
                catch ( Exception e ) {
                    Debug.LogError( "Error excecuting repeating job: " + e.Message );
                    Stop();
                }
            }
        }

        void IUnityLifecycleListener.OnApplicationQuit() {
            Stop();
        }
    }
}
