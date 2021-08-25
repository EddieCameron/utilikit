#if DEVELOPMENT_BUILD
/* DevConsole.cs
 * ----------------------------
 */
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEngine.Profiling;

namespace Utilikit {
    /// <summary>
    /// Manages the dev console
    /// </summary>
    public class DevConsole : MonoBehaviour {
        private const string _DEV_CONSOLE_RESOURCE_PATH = "DevConsole/Prefabs/DevConsole";

        public DevConsoleScreen devConsoleScreenPrefab;
        public Button devConsoleScreenButtonPrefab;

        public DevConsoleButton devConsoleButtonPrefab;
        public DevConsoleToggle devConsoleTogglePrefab;
        public DevConsoleIntField devConsoleIntField;
        public DevConsoleTextField devConsoleTextField;
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
            var allAssemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where( a => !a.IsDynamic && a.Location.Contains( "ScriptAssemblies" ) );   // user assembly + packages
#else
            var allAssemblies = System.AppDomain.CurrentDomain.GetAssemblies().Where( a => !a.IsDynamic && !a.Location.StartsWith( "System" ) && !a.Location.StartsWith( "UnityEngine" ) );   // user assembly + packages
#endif

            IEnumerable<Type> allTypes = allAssemblies.SelectMany( a => a.GetTypes() );
            IEnumerable<MethodInfo> staticMethods = allTypes.SelectMany( t => t.GetMethods( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ) );   // all static methods
            IEnumerable<PropertyInfo> staticProps = allTypes.SelectMany( t => t.GetProperties( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ) );   // all static methods

            // populate buttons
            var consoleMethods = GetMethodsWithAttribute<DevConsoleButtonAttribute>( staticMethods );
            foreach ( var methodAttrs in consoleMethods ) {
                DevConsoleButton newButton = CreateDevConsoleItem( devConsoleButtonPrefab, methodAttrs.attribute, methodAttrs.method );
            }

            // populate toggles
            var toggleProperties = GetPropertiesWithAttribute<DevConsoleToggleAttribute>( staticProps );
            foreach ( var toggleAttr in toggleProperties ) {
                var toggle = CreateDevConsoleItem( devConsoleTogglePrefab, toggleAttr.attribute, toggleAttr.property );
            }

            // populate int fields
            var intFieldProps = GetPropertiesWithAttribute<DevConsoleIntFieldAttribute>( staticProps );
            foreach ( var intFieldAttr in intFieldProps ) {
                var intField = CreateDevConsoleItem( devConsoleIntField, intFieldAttr.attribute, intFieldAttr.property );
            }

            // populate text fields
            var textFieldProps = GetPropertiesWithAttribute<DevConsoleTextFieldAttribute>( staticProps );
            foreach ( var textFieldAttr in textFieldProps ) {
                var textField = CreateDevConsoleItem( devConsoleTextField, textFieldAttr.attribute, textFieldAttr.property );
            }

            // populate sliders
            var sliderProperties = GetPropertiesWithAttribute<DevConsoleSliderAttribute>( staticProps );
            foreach ( var sliderAttr in sliderProperties ) {
                var slider = CreateDevConsoleItem( devConsoleSliderPrefab, sliderAttr.attribute, sliderAttr.property );
            }
        }

        private IEnumerable<(MethodInfo method, TAttributeType attribute)> GetMethodsWithAttribute<TAttributeType>( IEnumerable<MethodInfo> methods ) where TAttributeType : DevConsoleAttribute {
            return methods
                .Select( method => (method, attribute: method.GetCustomAttribute<TAttributeType>( false )) )      // all attributes
                .Where( pa => pa.attribute != null );
        }

        private IEnumerable<(PropertyInfo property, TAttributeType attribute)> GetPropertiesWithAttribute<TAttributeType>( IEnumerable<PropertyInfo> properties ) where TAttributeType : DevConsoleAttribute {
            return properties
                .Select( property => (property, attribute: property.GetCustomAttribute<TAttributeType>( false )) )      // all attributes
                .Where( pa => pa.attribute != null );                                                                        // throw out the non-testing console things
        }

        private TUiElement CreateDevConsoleItem<TUiElement, TAttribute>( TUiElement itemPrefab, TAttribute attr, PropertyInfo property ) where TAttribute : DevConsoleAttribute where TUiElement : DevConsoleUIElement<TAttribute> {
            var onScreen = GetScreen( attr.screen );
            var item = Instantiate( itemPrefab, onScreen.controlRoot );
            item.transform.localScale = Vector3.one;

            item.Init( attr, targetProperty: property );
            return item;
        }


        private TUiElement CreateDevConsoleItem<TUiElement, TAttribute>( TUiElement itemPrefab, TAttribute attr, MethodInfo method ) where TAttribute : DevConsoleAttribute where TUiElement : DevConsoleUIElement<TAttribute> {
            var onScreen = GetScreen( attr.screen );
            var item = Instantiate( itemPrefab, onScreen.controlRoot );
            item.transform.localScale = Vector3.one;

            item.Init( attr, method );
            return item;
        }

        private void MakeHomeScreenButton( string toScreen ) {
            var homeScreen = GetScreen( "" );
            var button = Instantiate( devConsoleScreenButtonPrefab, homeScreen.controlRoot );
            button.transform.SetAsFirstSibling();
            button.GetComponentInChildren<Text>().text = toScreen;
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
                screen.gameObject.SetActive( false );
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
