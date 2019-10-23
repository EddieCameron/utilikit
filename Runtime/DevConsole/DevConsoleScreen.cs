/* DevConsoleScreen.cs
 * ----------------------------
 */
#if DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Utilikit {
    /// <summary>
    /// Separate screen of the dev console
    /// </summary>
    public class DevConsoleScreen : MonoBehaviour {

        public string ScreenName { get; set; }

        public Text nameText;
        public Button backButton;
        public Transform controlRoot;

        void OnEnable() {
            backButton.onClick.AddListener( OnBackButtonClicked );
        }

        void OnDisable() {
            backButton.onClick.RemoveListener( OnBackButtonClicked );
        }

        public void Init( string screenName ) {
            this.ScreenName = screenName;
            nameText.text = screenName;
            transform.localScale = Vector3.one;
        }

        void OnBackButtonClicked() {
            DevConsole.Instance.OnBackButton();
        }
    }
}
#endif
