using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    public class Polygon2D {
        Vector2[] vertices;

        public readonly int NumVertices;

        public readonly Rect Bounds;

        public Polygon2D( ICollection<Vector2> vertices ) {
            this.NumVertices = vertices.Count;
            if ( NumVertices < 3 )
                throw new InvalidOperationException( "Need at least 3 vertices to make a polygon" );
            this.vertices = new Vector2[NumVertices];
            vertices.CopyTo( this.vertices, 0 );


            Bounds = new Rect( this.vertices[0], Vector2.zero );
            for ( int i = 1; i < NumVertices; i++ ) {
                Bounds = Bounds.Encapsulate( this.vertices[i] );
            }
        }

        public bool Contains( Vector2 point ) {
            // first just check bounding box
            if ( !Bounds.Contains( point ) )
                return false;

            return GetWindingNumber( point ) > 0;
        }

        /// <summary>
        /// How many times does the shape "wind" around this point
        /// - 0 means point is outside shape
        /// >= 1 is inside
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int GetWindingNumber( Vector2 point ) {
            //http://geomalgorithms.com/a03-_inclusion.html
            /*
            wn_PnPoly( Point P, Point V[], int n )
            {
                int    wn = 0;    // the  winding number counter

                // loop through all edges of the polygon
                for (each edge E[i]:V[i]V[i+1] of the polygon) {
                    if (E[i] crosses upward ala Rule #1)  {
                        if (P is  strictly left of E[i])    // Rule #4
                            ++wn;   // a valid up intersect right of P.x
                    }
                    else
                    if (E[i] crosses downward ala Rule  #2) {
                        if (P is  strictly right of E[i])   // Rule #4
                            --wn;   // a valid down intersect right of P.x
                    }
                }
                return wn;    // =0 <=> P is outside the polygon

            }
            */

            int windingNumber = 0;

            for ( int i = 0; i < NumVertices; i++ ) {
                var e0 = vertices[i];
                var e1 = vertices[( i + 1 ) % NumVertices];   // TODO precalc edges?
                if ( e0.y <= point.y ) {
                    if ( e1.y > point.y &&      // an upward crossing
                    IsLeft( e0, e1, point ) > 0 ) { // on left
                        windingNumber++;
                    }
                }
                else if ( e1.y <= point.y &&    // a downward crossing
                    IsLeft( e0, e1, point ) < 0 ) { // on right
                    windingNumber--;
                }
            }
            return windingNumber;
        }

        /// <summary>
        /// Get a random point inside this polygon
        /// </summary>
        /// <returns></returns>
        public Vector2 RandomPointWithin() {
            // brute force
            Vector2 point = Vector2.zero;
            int safetyCheck = 1000;
            do {
                point = Bounds.RandomPointWithin();
                safetyCheck--;
            }
            while ( safetyCheck > 0 && !Contains( point ) );

            return point;
        }

        /// <summary>
        /// Is a point to the left, right, or on, an infinite line passing between two points
        /// > 0 : left of line
        /// < 0 : right of line
        /// == 0 : on line
        /// </summary>
        /// <returns></returns>
        float IsLeft( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            return ( ( p1.x - p0.x ) * ( pTest.y - p0.y )
                    - ( pTest.x - p0.x ) * ( p1.y - p0.y ) );
        }

        /// <summary>
        /// Test whether the given line segment intersects with any of the polygon edges
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool DoesIntersectWithLine( Vector2 a, Vector2 b ) {
            for ( int i = 0; i < NumVertices; i++ ) {
                var e0 = vertices[i];
                var e1 = vertices[( i + 1 ) % NumVertices];   // TODO precalc edges?
                if ( MathUtils.TestLineIntersection( a, b, e0, e1 ) )
                    return true;
            }

            return false;
        }
    }
}
