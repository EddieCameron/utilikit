using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilikit {
    public class DevConsoleTrigger : MonoBehaviour {
#if DEVELOPEMENT_BUILD || UNITY_EDITOR
        float lastTap, nextLastTap;

        const float TRIPLE_TAP_TIME = 0.4f;

        void Update() {
#if UNITY_EDITOR || !UNITY_IOS || !UNITY_ANDROID
            if ( Input.GetMouseButtonDown( 0 ) ) {
                float tapTime = Time.realtimeSinceStartup;
                if ( tapTime - nextLastTap <= TRIPLE_TAP_TIME )
                    DevConsole.ShowDevConsole();

                nextLastTap = lastTap;
                lastTap = tapTime;
            }
#else
            if ( Input.touchCount >= 4 )
                DevConsole.ShowDevConsole();
#endif
        }
#endif
    }
}
