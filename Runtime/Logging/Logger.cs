/* Logger.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    public static class Logger {
        static StringBuilder _sb = new StringBuilder();
        public static string GetLogString( this IEnumerable collection ) {
            _sb.Clear();
            _sb.Append( "[" );
            bool isFirst = true;
            foreach (var item in collection)
            {
                if ( !isFirst )
                    _sb.Append( ", " );
                _sb.Append( item );
                isFirst = false;
            }
            _sb.Append( "]" );
            return _sb.ToString();
        }

        public static string GetLogString<T>( this IEnumerable<T> collection, Func<T,string> getElementStringMethod ) {
            _sb.Clear();
            _sb.Append( "[" );
            bool isFirst = true;
            foreach (var item in collection)
            {
                if ( !isFirst )
                    _sb.Append( ", " );
                _sb.Append( getElementStringMethod( item ) );
                isFirst = false;
            }
            _sb.Append( "]" );
            return _sb.ToString();
        }
    }
}
