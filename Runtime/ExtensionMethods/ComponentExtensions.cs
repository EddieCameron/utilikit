/* ComponentExtensions.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

public static class ComponentExtensions {

    #region Interfaces

    static List<Component> _tempComponentList = new List<Component>();


    /// <summary>
    /// Like GetComponent, but returns a script component that implements a given interface
    /// Not thread safe
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
    public static T GetComponentWithInterface<T>( this GameObject gameObject ) where T : class {
        _tempComponentList.Clear();
        gameObject.GetComponents( _tempComponentList );
        int componentCount = _tempComponentList.Count;
        for ( int i = 0; i < componentCount; i++ ) {
            if ( _tempComponentList[i] is T t )
                return t;
        }

        return null;
    }

    /// <summary>
    /// Like GetComponent, but returns a script component that implements a given interface
    /// Not thread safe
    /// </summary>
    /// <typeparam name="T">Component interface</typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static T GetComponentWithInterface<T>( this Component component ) where T : class {
        return component.gameObject.GetComponentWithInterface<T>();
    }

    /// <summary>
    /// Like GetComponents, but returns script components that implement a given interface
    /// </summary>
    /// <returns>An array of components with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
	public static void GetComponentsWithInterface<T>( this GameObject gameObject, List<T> results ) where T : class {
        _tempComponentList.Clear();
        gameObject.GetComponents( _tempComponentList );
        int componentCount = _tempComponentList.Count;
        for ( int i = 0; i < componentCount; i++ ) {
            if ( _tempComponentList[i] is T t )
                results.Add( t );
        }
    }

    /// <summary>
    /// Like GetComponents, but returns script components that implement a given interface
    /// </summary>
    /// <returns>An array of components with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
	public static void GetComponentsWithInterface<T>( this Component component, List<T> results ) where T : class {
        component.gameObject.GetComponentsWithInterface( results );
    }

    /// <summary>
    /// Like GetComponentInParent, but returns a script component that implements a given interface
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
    public static T GetComponentInParentWithInterface<T>( this GameObject gameObject ) where T : class {
        T t = GetComponentWithInterface<T>( gameObject );
        if ( t != null )
            return t;

        // if we're the root, no components have type T
        Transform parent = gameObject.transform.parent;
        if ( parent == null )
            return null;

        // otherwise recurse through parents
        return parent.gameObject.GetComponentInParentWithInterface<T>();
    }

    /// <summary>
    /// Like GetComponentInParent, but returns a script component that implements a given interface
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
    public static T GetComponentInParentWithInterface<T>( this Component component ) where T : class {
        return component.gameObject.GetComponentInParentWithInterface<T>();
    }

    /// <summary>
    /// Get all components on same object or children that implement interface <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Interface type</typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static void GetComponentsInChildrenWithInterface<T>( this GameObject gameObject, List<T> listToFill, bool includeInactive = false ) where T : class {
        _tempComponentList.Clear();
        gameObject.GetComponentsInChildren( includeInactive, _tempComponentList );
        int componentCount = _tempComponentList.Count;
        for ( int i = 0; i < componentCount; i++ ) {
            if ( _tempComponentList[i] is T t )
                listToFill.Add( t );
        }
    }
    #endregion
}
