#if DEVELOPMENT_BUILD
/* DevConsole.cs
 * ----------------------------
 */
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;

namespace Utilikit {
    /// <summary>
    /// Manages the dev console
    /// </summary>
    public class DevConsole : MonoBehaviour {
        private const string _DEV_CONSOLE_RESOURCE_PATH = "DevConsole/Prefabs/DevConsole";

        public DevConsoleScreen devConsoleScreenPrefab;
        public Button devConsoleScreenButtonPrefab;

        public DevConsoleButton devConsoleButtonPrefab;
        public DevConsoleSlider devConsoleSliderPrefab;

        private int currentScreen;
        private List<DevConsoleScreen> _screens = new List<DevConsoleScreen>();

        private static DevConsole _instance_;
        public static DevConsole Instance {
            get {
                if ( _instance_ == null ) {
                    GameObject prefab = Resources.Load<GameObject>( _DEV_CONSOLE_RESOURCE_PATH );
                    _instance_ = Instantiate<GameObject>( prefab ).GetComponent<DevConsole>();
                }
                return _instance_;
            }
        }

        public static void ShowDevConsole() {
            Instance.gameObject.SetActive( true );
        }

        #region Setup

        protected void Awake() {
            // make home screen
            var homeScreen = GetScreen( "" );

#if UNITY_EDITOR
            Assembly editorAssembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault( a => a.FullName.StartsWith( "Assembly-CSharp-Editor," ) ); // ',' included to ignore  Assembly-CSharp-Editor-FirstPass
#endif

            // populate buttons
            var consoleMethods = GetMethodsWithAttributeFromAssembly<DevConsoleButtonAttribute>( Assembly.GetExecutingAssembly() ).ToArray();

#if UNITY_EDITOR
            if ( editorAssembly != null ) {
                // get editor methods too
                consoleMethods.Concat( GetMethodsWithAttributeFromAssembly<DevConsoleButtonAttribute>( editorAssembly ) );
            }
#endif

            for ( int i = 0; i < consoleMethods.Length; i++ ) {
                var methodAttrs = consoleMethods[i];
                DevConsoleButton newButton = CreateDevConsoleItem( devConsoleButtonPrefab, methodAttrs.attribute );
                newButton.Init( methodAttrs.method, methodAttrs.attribute );
            }

            // populate sliders
            var sliderProperties = GetPropertiesWithAttributeFromAssembly<DevConsoleSliderAttribute>( Assembly.GetExecutingAssembly() ).ToArray();
#if UNITY_EDITOR
            if ( editorAssembly != null ) {
                // get editor fields too
                sliderProperties.Concat( GetPropertiesWithAttributeFromAssembly<DevConsoleSliderAttribute>( editorAssembly ) );
            }
#endif
            for ( int i = 0; i < sliderProperties.Length; i++ ) {
                var sliderAttr = sliderProperties[i];
                var slider = CreateDevConsoleItem( devConsoleSliderPrefab, sliderAttr.attribute );
                slider.Init( sliderAttr.property, sliderAttr.attribute );
            }
        }

        private IEnumerable<(MethodInfo method, TAttributeType attribute)> GetMethodsWithAttributeFromAssembly<TAttributeType>( Assembly assembly ) where TAttributeType : DevConsoleAttribute {
            return assembly.GetTypes()
                .SelectMany( t => t.GetMethods( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ) )   // all static methods
                .Select( method => (method, attributes: method.GetCustomAttributes<TAttributeType>( false )) )      // all attributes
                .Where( pa => pa.attributes.Count() > 0 )                                                                        // throw out the non-testing console things
                .Select( pa => (pa.method, pa.attributes.First()) );
        }

        private IEnumerable<(PropertyInfo property, TAttributeType attribute)> GetPropertiesWithAttributeFromAssembly<TAttributeType>( Assembly assembly ) where TAttributeType : DevConsoleAttribute {
            return assembly.GetTypes()
                .SelectMany( t => t.GetProperties( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ) )   // all static properties
                .Select( property => (property, attributes: property.GetCustomAttributes<TAttributeType>( false )) )      // all attributes
                .Where( pa => pa.attributes.Count() > 0 )                                                                        // throw out the non-testing console things
                .Select( pa => (pa.property, pa.attributes.First()) );
        }

        private T CreateDevConsoleItem<T>( T itemPrefab, DevConsoleAttribute attr ) where T : Component {
            var onScreen = GetScreen( attr.screen );
            var item = Instantiate( itemPrefab, onScreen.controlRoot );
            item.transform.localScale = Vector3.one;
            return item;
        }

        private void MakeHomeScreenButton( string toScreen ) {
            var homeScreen = GetScreen( "" );
            var button = Instantiate( devConsoleScreenButtonPrefab, homeScreen.controlRoot );
            button.onClick.AddListener( () => GoToScreen( toScreen ) );
        }

        private DevConsoleScreen GetScreen( string screenName ) {
            for ( int i = 0; i < _screens.Count; i++ ) {
                if ( _screens[i].ScreenName == screenName )
                    return _screens[i];
            }

            // create new screen
            var screen = Instantiate( devConsoleScreenPrefab, transform.position, transform.rotation, transform );
            screen.Init( screenName );
            _screens.Add( screen );

            // add button to home screen
            if ( !string.IsNullOrEmpty( screenName ) ) {
                MakeHomeScreenButton( screenName );
            }
            return screen;
        }
        #endregion

        private void Update() {
            if ( Input.GetKeyDown( KeyCode.Escape ) ) {
                // close immediately!
                GoToScreen( "Home" );
                Close();
            }
        }

        public void GoHome() {
            GoToScreen( "" );
        }

        public void GoToScreen( string screenName ) {
            for ( int i = 0; i < _screens.Count; i++ ) {
                DevConsoleScreen screen = _screens[i];
                bool isAtScreen = screen.ScreenName == screenName;
                screen.gameObject.SetActive( isAtScreen );
                if ( isAtScreen )
                    currentScreen = i;
            }
        }

        public void OnBackButton() {
            DevConsoleScreen onScreen = _screens[currentScreen];
            if ( string.IsNullOrEmpty( onScreen.ScreenName ) ) {
                Close();
                return;
            }

            GoToScreen( "" );
        }

        public void Close() {
            gameObject.SetActive( false );
        }
    }
}
#endif
