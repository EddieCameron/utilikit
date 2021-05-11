using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    [RequireComponent( typeof( Button ) )]
    public abstract class ButtonBehaviour : MonoBehaviour {

        private Button _button;
        public Button Button {
            get {
                if ( _button == null )
                    _button = GetComponent<Button>();
                return _button;
            }
        }

        protected virtual void OnEnable() {
            Button.onClick.AddListener( OnButtonClicked );
        }

        protected virtual void OnDisable() {
            Button.onClick.RemoveListener( OnButtonClicked );
        }

        protected abstract void OnButtonClicked();
    }
}
