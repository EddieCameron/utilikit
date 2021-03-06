﻿/* ComponentExtensions.cs
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
    /// Like GetComponent, but returns a script component that implements a given interface
    /// Not thread safe
    /// </summary>
    /// <typeparam name="T">Component interface</typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static bool TryGetComponentWithInterface<T>( this GameObject gameObject, out T interfaceInstance ) where T : class {
        _tempComponentList.Clear();
        gameObject.GetComponents( _tempComponentList );
        int componentCount = _tempComponentList.Count;
        for ( int i = 0; i < componentCount; i++ ) {
            if ( _tempComponentList[i] is T t ) {
                interfaceInstance = t;
                return true;
            }
        }

        interfaceInstance = null;
        return false;
    }

    /// <summary>
    /// Like GetComponent, but returns a script component that implements a given interface
    /// Not thread safe
    /// </summary>
    /// <typeparam name="T">Component interface</typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static bool TryGetComponentWithInterface<T>( this Component component, out T interfaceInstance ) where T : class {
        return TryGetComponentWithInterface( component.gameObject, out interfaceInstance );
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

    /// <summary>
    /// Get all components on same object or parents that implement interface <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Interface type</typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public static void GetComponentsInParentsWithInterface<T>( this GameObject gameObject, List<T> listToFill, bool includeInactive = false ) where T : class {
        if ( includeInactive || gameObject.activeInHierarchy )
            GetComponentsWithInterface<T>( gameObject, listToFill );

        // if we're the root, no components have type T
        Transform parent = gameObject.transform.parent;
        if ( parent == null )
            return;

        // otherwise recurse through parents
        parent.gameObject.GetComponentsInParentsWithInterface<T>( listToFill );
    }
    #endregion

    #region TryGet
    public static bool TryGetComponentInParent<T>( this GameObject gameObject, out T componentInstance ) where T : Component {
        if ( gameObject.TryGetComponent<T>( out componentInstance ) ) {
            return true;
        }

        // if we're the root, no components have type T
        Transform parent = gameObject.transform.parent;
        if ( parent == null )
            return false;

        // otherwise recurse through parents
        return parent.gameObject.TryGetComponentInParent<T>( out componentInstance );
    }

    public static bool TryGetComponentInParent<T>( this Component component, out T componentInstance ) where T : Component {
        return component.gameObject.TryGetComponentInParent( out componentInstance );
    }

    /// <summary>
    /// Like GetComponentInParent, but returns a script component that implements a given interface
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
    public static bool TryGetComponentInParentWithInterface<T>( this GameObject gameObject, out T interfaceInstance ) where T : class {
        if ( TryGetComponentWithInterface<T>( gameObject, out interfaceInstance ) )
            return true;

        // if we're the root, no components have type T
        Transform parent = gameObject.transform.parent;
        if ( parent == null )
            return false;

        // otherwise recurse through parents
        return parent.gameObject.TryGetComponentInParentWithInterface<T>( out interfaceInstance );
    }

    /// <summary>
    /// Like GetComponentInParent, but returns a script component that implements a given interface
    /// </summary>
    /// <returns>The first found component with interface T.</returns>
    /// <typeparam name="T">The interface type to look for</typeparam>
    public static bool TryGetComponentInParentWithInterface<T>( this Component component, out T interfaceInstance ) where T : class {
        return component.gameObject.TryGetComponentInParentWithInterface<T>( out interfaceInstance );
    }
    #endregion

    #region Graphic
    /// <summary>
    /// Set only the alpha channel on a UI graphic
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="alpha"></param>
    public static void SetAlpha( this Graphic graphic, float alpha ) {
        var c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }
    #endregion
}
