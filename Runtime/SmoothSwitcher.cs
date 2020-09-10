/* SmoothSwitcher.cs
 * Copyright Eddie Cameron 2015
 * ----------------------------
 * When you want some value to switch slowly between on and off without all that fuss about whether it's already changing or whatnot
 * (eg: light dimmer switch, audio fade in/out, zoom)
 * -----------------------------
 * Can be added to an object at runtime, with SmoothSwitcher.SetupSmoothSwitcher(...)
 * Otherwise, make sure to set the SmoothSwitcher's onAmountSet event in the inspector, which will be called whenever the 'on amount' changes
 *  (when setting an event listener method, make sure to choose a 'dynamic' method in the selection menu, otherwise it'll just call whatever number is in the box)
 * -----------------------------
 * eg use, for a light dimmer that starts turned off:
 *
 * public class LightDimmer : MonoBehaviour {
 *      public float dimTime = 2f;
 *      public float onLightIntensity = 2f;
 *
 *      SmoothSwitcher mySwitch;
 *
 *      void Start() {
 *          mySwitch = SmoothSwitcher.SetupSmoothSwitcher( this, dimTime, false, OnDimSet );
 *      }
 *
 *      public void SwitchOn() {
 *          mySwitch.SwitchOn();
 *      }
 *
 *      //... likewise for SwitchOff and Toggle, or can access switch directly, of course
 *
 *      void OnDimSet( float dimAmount ) {
 *          light.intensity = dimAmount * onLightIntensity;
 *      }
 * }
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Utilikit {
    public class SmoothSwitcher : MonoBehaviour {
        public float switchTime;
        public EasingFunction.Ease easeType;

        public enum SwitchState {
            Off,
            On,
            SwitchingOn,
            SwitchingOff
        }

        [SerializeField]
        bool _startOn;

        public float SwitchValue { get; private set; }

        public SwitchState State { get; private set; }

        float t;
        System.Action<bool> currentSwitchCompletedCallback;

        protected void Awake() {
            State = _startOn ? SwitchState.On : SwitchState.Off;
            SwitchValue = t = _startOn ? 1 : 0;
        }

        protected void Update() {
            if ( State == SwitchState.SwitchingOn ) {
                t += Time.deltaTime / switchTime;
                if ( t >= 1 ) {
                    // switched on
                    State = SwitchState.On;
                    SwitchValue = t = 1;

                    currentSwitchCompletedCallback?.Invoke( true );
                    currentSwitchCompletedCallback = null;
                }
                else {
                    SwitchValue = EasingFunction.GetEasingFunction( easeType )( 0, 1, t );
                }
            }
            else if ( State == SwitchState.SwitchingOff ) {
                t -= Time.deltaTime / switchTime;
                if ( t <= 0 ) {
                    // switched on
                    State = SwitchState.Off;
                    SwitchValue = t = 0;

                    currentSwitchCompletedCallback?.Invoke( true );
                    currentSwitchCompletedCallback = null;
                }
                else {
                    SwitchValue = EasingFunction.GetEasingFunction( easeType )( 0, 1, t );
                }
            }
        }



        public Task<bool> SwitchOnAsync() {
            TaskCompletionSource<bool> tcs;
            switch ( State ) {
            case SwitchState.On:
                // already on
                return Task.FromResult( true );

            case SwitchState.Off:
            case SwitchState.SwitchingOn:
                tcs = new TaskCompletionSource<bool>();
                currentSwitchCompletedCallback += ( didSwitchOn ) => {
                    tcs.TrySetResult( didSwitchOn );
                };
                State = SwitchState.SwitchingOn;
                return tcs.Task;

            case SwitchState.SwitchingOff:
                currentSwitchCompletedCallback?.Invoke( false );
                currentSwitchCompletedCallback = null;

                tcs = new TaskCompletionSource<bool>();
                currentSwitchCompletedCallback += ( didSwitchOn ) => {
                    tcs.TrySetResult( didSwitchOn );
                };
                State = SwitchState.SwitchingOn;
                return tcs.Task;

            default:
                throw new InvalidOperationException( "Unknown switch state" );
            }
        }

        public Task<bool> SwitchOffAsync() {
            TaskCompletionSource<bool> tcs;
            switch ( State ) {
            case SwitchState.Off:
                // already off
                return Task.FromResult( true );

            case SwitchState.On:
            case SwitchState.SwitchingOff:
                tcs = new TaskCompletionSource<bool>();
                currentSwitchCompletedCallback += ( didSwitchOff ) => {
                    tcs.TrySetResult( didSwitchOff );
                };
                State = SwitchState.SwitchingOff;
                return tcs.Task;

            case SwitchState.SwitchingOn:
                currentSwitchCompletedCallback?.Invoke( false );
                currentSwitchCompletedCallback = null;

                tcs = new TaskCompletionSource<bool>();
                currentSwitchCompletedCallback += ( didSwitchOff ) => {
                    tcs.TrySetResult( didSwitchOff );
                };
                State = SwitchState.SwitchingOff;
                return tcs.Task;

            default:
                throw new InvalidOperationException( "Unknown switch state" );
            }
        }

        public Task<bool> ToggleAsync() {
            if ( State == SwitchState.Off || State == SwitchState.SwitchingOff ) {
                return SwitchOnAsync();
            }
            else {
                return SwitchOffAsync();
            }
        }

        public async void SwitchOn() {
            await SwitchOnAsync();
        }

        public async void SwitchOff() {
            await SwitchOffAsync();
        }

        public async void Toggle() {
            await ToggleAsync();
        }
    }
}
