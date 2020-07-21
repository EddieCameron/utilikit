using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    [Serializable]
    public class Polygon2D {
        [SerializeField]
        List<Vector2> vertices;

        public int NumVertices => vertices.Count;

        bool? _isClockwiseCached;
        public bool IsClockwise {
            get  {
                if ( !_isClockwiseCached.HasValue )
                    _isClockwiseCached = CalculateWinding();
                return _isClockwiseCached.Value;
            }
        }

        Rect? _boundsCache = null;
        public Rect Bounds {
            get {
                if ( !_boundsCache.HasValue ) {
                    _boundsCache = new Rect( this.vertices[0], Vector2.zero );
                    for ( int i = 1; i < NumVertices; i++ ) {
                        _boundsCache = Bounds.Encapsulate( this.vertices[i] );
                    }
                }
                return _boundsCache.Value;
            }
        }

        public bool IsValid => NumVertices >= 3;

        public Polygon2D() {
            this.vertices = new List<Vector2>();
        }

        public Polygon2D( IEnumerable<Vector2> vertices ) {
            this.vertices = new List<Vector2>( vertices );
            if ( this.vertices.Count < 3 )
                throw new InvalidOperationException( "Need at least 3 vertices to make a polygon" );

        }
        public Polygon2D( Polygon2D other ) {
            this.vertices = new List<Vector2>( other.vertices );
        }

        public Polygon2D Clone() {
            return new Polygon2D( this );
        }

        public void Translate( Vector2 translation ) {
            for ( int i = 0; i < NumVertices; i++ ) {
                vertices[i] += translation;
            }
            SetDirty();
        }

        public void Rotate( float radiansAnticlockwise ) {
            float sin = Mathf.Sin( radiansAnticlockwise );
            float cos = Mathf.Cos( radiansAnticlockwise );
            for ( int i = 0; i < NumVertices; i++ ) {
                var p = vertices[i];
                Vector2 q;
                q.x = p.x * cos - p.y * sin;
                q.y = p.x * sin + p.y * cos;
                vertices[i] = q;
            }
            SetDirty();
        }

        public void Scale( Vector2 scale ) {
            for ( int i = 0; i < NumVertices; i++ ) {
                vertices[i] *= scale;
            }
            SetDirty();
        }

        public void SetVertex( int atIdx, Vector2 point ) {
            vertices[atIdx] = point;
            SetDirty();
        }

        public void InsertVertex( int atIdx, Vector2 point ) {
            vertices.Insert( atIdx, point );
            SetDirty();
        }

        public void AddVertex( Vector2 point ) {
            vertices.Add( point );
            SetDirty();
        }

        public void AddVertices( IEnumerable<Vector2> points ) {
            vertices.AddRange( points );
            SetDirty();
        }

        public void RemoveVertex( int atIdx ) {
            vertices.RemoveAt( atIdx );
            SetDirty();
        }

        public void Clear() {
            vertices.Clear();
            SetDirty();
        }

        public Vector2 GetVertex( int idx ) {
            return vertices[idx];
        }

        public bool Contains( Vector2 point ) {
            // first just check bounding box
            if ( !Bounds.Contains( point ) )
                return false;

            return GetWindingNumber( point ) != 0;
        }

        /// <summary>
        /// How many times does the shape "wind" around this point
        /// - 0 means point is outside shape
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

#region Intersections

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

        public bool DoesIntersectWithRect( Rect rect ) {
            if ( !IsValid )
                return false;

            // https://stackoverflow.com/questions/7136547/method-to-detect-intersection-between-a-rectangle-and-a-polygon

            // check if rect is fully inside poly or vice versa
            if ( Contains( rect.min ) )
                return true;

            if ( rect.Contains( vertices[0] ) )
                return true;

            // otherwise if there is intersection, one of the rect lines must intersect the polygon edge
            Vector2 a = rect.min;
            Vector2 b = new Vector2( rect.xMin, rect.yMax );
            Vector2 c = new Vector2( rect.xMax, rect.yMax );
            Vector2 d = new Vector2( rect.xMax, rect.yMin );
            return DoesIntersectWithLine( a, b ) ||
                DoesIntersectWithLine( b, c ) ||
                DoesIntersectWithLine( c, d ) ||
                DoesIntersectWithLine( d, a );
        }

        #endregion

        /// <summary>
        /// Get the closest vertex in this polygon to the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="vertexIdx"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public int GetNearestVertex( Vector2 p, out float distance ) {
            distance = float.MaxValue;
            int closestIdx = -1;
            for ( int i = 0; i < NumVertices; i++ ) {
                float dist = ( p - vertices[i] ).sqrMagnitude;
                if ( i == 0 || dist < distance ) {
                    distance = dist;
                    closestIdx = i;
                }
            }

            if ( closestIdx >= 0 ) {
                distance = Mathf.Sqrt( distance );
            }
            return closestIdx;
        }

        /// <summary>
        /// Get the cclosest edge (from edgeVertexA to edgeVertexB) in this polygon to the given point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="edgeVertexA"></param>
        /// <param name="edgeVertexB"></param>
        /// <param name="distance"></param>
        public void GetNearestEdge( Vector2 p, out int edgeVertexA, out int edgeVertexB, out float distance ) {
            if ( NumVertices == 0 ) {
                edgeVertexA = edgeVertexB = -1;
                distance = 0;
                return;
            }
            if ( NumVertices == 1 ) {
                edgeVertexA = 0;
                edgeVertexB = 0;
                distance = ( p - vertices[0] ).magnitude;
                return;
            }


            distance = float.MaxValue;
            edgeVertexA = -1;
            for ( int i = 0; i < NumVertices; i++ ) {
                Vector2 p0 = vertices[i];
                Vector2 p1 = vertices[( i + 1 ) % NumVertices];
                float dist = MathUtils.DistanceFromPointToLine( p0, p1, p );
                if ( i == 0 || dist < distance ) {
                    distance = dist;
                    edgeVertexA = i;
                }
            }

            edgeVertexB = ( edgeVertexA + 1 ) % NumVertices;
        }

        /// <summary>
        /// Is a point to the left, right, or on, an infinite line passing between two points
        /// > 0 : left of line
        /// < 0 : right of line
        /// == 0 : on line
        /// </summary>
        /// <returns></returns>
        static float IsLeft( Vector2 p0, Vector2 p1, Vector2 pTest ) {
            return ( ( p1.x - p0.x ) * ( pTest.y - p0.y )
                    - ( pTest.x - p0.x ) * ( p1.y - p0.y ) );
        }

        /// <summary>
        /// Returns true if polygon is clockwise, or false if anticlockwise.
        /// Only makes sense for simple polygons
        /// https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order/1180256#1180256
        /// </summary>
        /// <returns></returns>
        bool CalculateWinding() {
            if ( !IsValid )
                return false;

            // find point on convex hull
            int aIdx = 0;
            Vector2 a = vertices[0];
            for ( int i = 1; i < vertices.Count; i++ ) {
                if ( vertices[i].x < a.x ) {
                    aIdx = i;
                    a = vertices[i];
                }
                else if ( vertices[i].x == a.x && vertices[i].y < a.y ) {
                    aIdx = i;
                    a = vertices[i];
                }
            }

            int bIdx = aIdx == 0 ? vertices.Count - 1 : aIdx - 1;
            int cIdx = ( aIdx + 1 ) % vertices.Count;

            bool isCToLeft = IsLeft( vertices[bIdx], a, vertices[cIdx] ) > 0;
            return !isCToLeft;
        }

        /// <summary>
        /// Mark lazy calculated values as needing to be refreshed
        /// </summary>
        void SetDirty() {
            _boundsCache = null;
            _isClockwiseCached = null;
        }

#region Triangulation
        /// <summary>
        /// Uses the ear-clipping algorithm to break the polygon into a list of triangles, for rendering.async ONly works if polygon is simple (non-self-intersecting)
        /// </summary>
        /// <param name="outTriangleIndices"></param>
        public void Triangulate( List<int> outTriangleIndices ) {
            if ( !IsValid )
                return;

            // https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
            var remainingVertices = new LinkedList<int>();
            var ears = new LinkedList<LinkedListNode<int>>();
            var convexVertices = new HashSet<LinkedListNode<int>>();
            var reflexVertices = new HashSet<LinkedListNode<int>>();

            bool IsReflex( LinkedListNode<int> v ) {
                // is angle > 180?
                var next = v.Next ?? v.List.First;
                var previous = v.Previous ?? v.List.Last;
                bool isLeft = IsLeft( vertices[previous.Value], vertices[v.Value], vertices[next.Value] ) > 0;
                return isLeft;
            }

            bool IsEar( LinkedListNode<int> v ) {
                if ( reflexVertices.Count == 0 )
                    return true;    // if polygon is convex, everybody is an ear

                // if any reflex vertex is within the triangle formed by this and neighbouring points, it's not an ear
                var next = v.Next ?? v.List.First;
                var previous = v.Previous ?? v.List.Last;

                Vector2 a = vertices[v.Value];
                Vector2 b = vertices[next.Value];
                Vector2 c = vertices[previous.Value];
                foreach ( var r in reflexVertices ) {
                    if ( r == next || r == previous )
                        continue;   // don't look at triangle vertices

                    Vector2 p = vertices[r.Value];
                    // if p is to the left of any side in clockwise triangle, it is not inside the triangle
                    if ( IsLeft( a, b, p ) > 0 )
                        continue;
                    if ( IsLeft( b, c, p ) > 0 )
                        continue;
                    if ( IsLeft( c, a, p ) > 0 )
                        continue;

                    // p must be inside or on triangle
                    return false;
                }

                // no points inside triangle
                return true;
            }

            // add vertices in clockwise order
            if ( IsClockwise ) {
                for ( int i = 0; i < vertices.Count; i++ ) {
                    remainingVertices.AddLast( i );
                }
            }
            else {
                for ( int i = 0; i < vertices.Count; i++ ) {
                    remainingVertices.AddFirst( i );
                }
            }
            if ( remainingVertices.Count == 3 ) {
                outTriangleIndices.AddRange( remainingVertices );
                return;
            }


            // find reflex (concave) vertices and "ears" (confirmed triangle tips)
            var node = remainingVertices.First;
            while ( node != null ) {
                if ( IsReflex( node ) )
                    reflexVertices.Add( node );
                else
                    convexVertices.Add( node );

                node = node.Next;
            }
            foreach ( var v in convexVertices ) {
                if ( IsEar( v ) )
                    ears.AddLast( v );
            }

            // ok, now we remove each ear in turn, and updte thei adjacent vertices
            while ( remainingVertices.Count > 3 ) {

                if ( ears.Count == 0 ) {
                    var debugNode = remainingVertices.First;
                    while ( debugNode != null ) {
                        var nextDebugNode = debugNode.Next ?? remainingVertices.First;
                        Debug.DrawLine( vertices[debugNode.Value], vertices[nextDebugNode.Value], Color.red, .1f );
                        debugNode = debugNode.Next;
                    }

                    for ( int i = 0; i < vertices.Count; i++ ) {
                        Debug.DrawLine( vertices[i], vertices[( i + 1 ) % vertices.Count] );
                    }
                    throw new InvalidOperationException( "Ran out of possible ear vertices, was this polygon complex?" );
                }

                // Debug.Log( "Ear clip" );
                // Debug.Log( remainingVertices.GetLogString() );
                // Debug.Log( ears.GetLogString( e => e.Value.ToString() ) );
                // Debug.Log( reflexVertices.GetLogString( v => v.Value + " " + vertices[v.Value] ) );

                var ear = ears.First;
                var next = ear.Value.Next ?? ear.Value.List.First;
                var previous = ear.Value.Previous ?? ear.Value.List.Last;

                // clip triangle
                outTriangleIndices.Add( ear.Value.Value );
                outTriangleIndices.Add( next.Value );
                outTriangleIndices.Add( previous.Value );

                Vector2 a = vertices[ear.Value.Value];
                Vector2 b = vertices[next.Value];
                Vector2 c = vertices[previous.Value];

                var col = ears.Count == 1 ? Color.green : Color.cyan;
                // Debug.DrawLine( a, b, col, .1f );
                // Debug.DrawLine( b, c, col, .1f );
                // Debug.DrawLine( c, a, col, .1f );
                // Debug.Log( IsEar( ear.Value ) );

                remainingVertices.Remove( ear.Value );

                if ( remainingVertices.Count == 3 ) {
                    // last triangle
                    outTriangleIndices.AddRange( remainingVertices );
                    return;
                }

                // look at adjacent vertices and see if they need updating
                // previous
                if ( previous == ears.Last.Value ) {
                    // previous neighbour is an ear, make sure it still is
                    if ( !IsEar( previous) ) {
                        ears.RemoveLast();
                    }
                }
                else {
                    bool checkIsEar = true;
                    if ( reflexVertices.Contains( previous ) ) {
                        if ( !IsReflex( previous ) ) {
                            reflexVertices.Remove( previous );
                        }
                        else {
                            checkIsEar = false; // if still reflex, dont check if ear
                        }
                    }

                    if ( checkIsEar && IsEar( previous ) ) {
                        ears.AddFirst( previous );
                    }
                }

                // next
                if ( ear.Next != null && next == ear.Next.Value ) {
                    // next neighbour is an ear, make sure it still is
                    if ( !IsEar( next) ) {
                        ears.Remove( ear.Next );
                    }
                }
                else {
                    bool checkIsEar = true;
                    if ( reflexVertices.Contains( next ) ) {
                        if ( !IsReflex( next ) ) {
                            reflexVertices.Remove( next );
                        }
                        else {
                            checkIsEar = false;
                        }
                    }

                    if ( checkIsEar && IsEar( next ) ) {
                        ears.AddAfter( ear, next );
                    }
                }

                ears.Remove( ear );
            }

            throw new InvalidProgramException( "Something bad happened with the ears" );
        }


        /// <summary>
        /// Inserts the vertices of the given polygon into this polygon to form a 'hole' connected to the outisde edge (so that the outer 'wraps' around the hole). The hole must fit entirely inside this polygon.
        /// </summary>
        /// <param name="holePolygon"></param>
        public void InsertHole( Polygon2D holePolygon ) {
            int innerConnection, outerConnection;
            /*
            // Find 'mutually visible points in both polygons to make the cut
            2. Intersect the ray M + t(1, 0) with all directed edges ⟨Vi, Vi+1⟩ of the outer polygon for which M is to the left of the line containing the edge (M is inside the outer polygon). Let I be the closest visible point to M on this ray.
            3. If I is a vertex of the outer polygon, then M and I are mutually visible and the algorithm terminates.
            4. Otherwise, I is an interior point of the edge ⟨Vi, Vi+1⟩. Select P to be the endpoint of maximum x-value
            for this edge.
            5. Search the reflex vertices of the outer polygon (not including P if it happens to be reflex). If all of these vertices are strictly outside triangle ⟨M,I,P⟩, then M and P are mutually visible and the algorithm terminates.
            6. Otherwise, at least one reflex vertex lies in ⟨M,I,P⟩. Search for the reflex R that minimizes the angle between (1,0) and the line segment ⟨M,R⟩. Then M and R are mutually visible and the algorithm terminates. It is possible in this step that there are multiple reflex vertices that minimize the angle,
            */

            // 1. Search the inner polygon for vertex M of maximum x-value.
            Vector2 maxInsideX = holePolygon.GetVertex( 0 );
            int maxInsideXIdx = 0;
            for ( int i = 1; i < holePolygon.NumVertices; i++ ) {
                if ( holePolygon.GetVertex( i ).x > maxInsideX.x ) {
                    maxInsideX = holePolygon.GetVertex( i );
                    maxInsideXIdx = i;
                }
            }

            // 2. Intersect the ray M + t(1, 0) with all directed edges ⟨Vi, Vi+1⟩ of the outer polygon for which M is to the left of the line containing the edge (M is inside the outer polygon). Let I be the closest visible point to M on this ray.
            Vector2 closestIntersection = Vector2.zero;
            bool isEnd = false;
            float closestIntersectionDist = float.MaxValue;
            int closestSegmentEnd = -1;
            int closestVertex = -1;
            for ( int i = 1; i < NumVertices; i++ ) {
                Vector2 a = vertices[i - 1];
                Vector2 b = vertices[i];
                if ( a.x < maxInsideX.x && b.x < maxInsideX.x ) {
                    // segment to left of hole
                    continue;
                }


                Vector2 v1 = maxInsideX - a;
                Vector2 v2 = b - a;
                Vector2 v3 = new Vector2(0, 1);

                float dot = v2.y;
                if ( Mathf.Abs( dot ) < .00001f )
                    continue;   // parallel to ray

                float t1 = ( ( v2.x * v1.y ) - ( v2.y * v1.x ) ) / dot;
                if ( t1 < 0 )
                    continue;

                float t2 = v1.y / dot;
                if ( t2 >= 0.0 && t2 <= 1.0 ) {
                    // intersection, is it the closest?
                    if ( t1 < closestIntersectionDist ) {
                        closestIntersection = new Vector2( maxInsideX.x + t1, maxInsideX.y );
                        isEnd = t2 == 0 || t2 == 1;
                        closestIntersectionDist = t1;
                        closestSegmentEnd = i;
                        closestVertex = t2 < 0.5 ? i - 1 : i;
                    }
                }
            }

            if ( closestSegmentEnd == -1 ) {
                throw new ArgumentException( "no intersections from hole to outer polygon, is it inside?" );
            }

            innerConnection = maxInsideXIdx;

            // 3. If I is a vertex of the outer polygon, then M and I are mutually visible and the algorithm terminates.
            if ( isEnd ) {
                outerConnection = closestVertex;
            }
            else {
                // 4. Otherwise, I is an interior point of the edge ⟨Vi, Vi+1⟩. Select P to be the endpoint of maximum x-value
                // for this edge.
                int testVertex = closestSegmentEnd;
                if ( vertices[testVertex].x < vertices[testVertex - 1].x ) {
                    testVertex -= 1;    // previous vertex has higherr x
                }

                bool isOuterClockwise = IsClockwise;
                // 5. Search the reflex vertices of the outer polygon (not including P if it happens to be reflex). If all of these vertices are strictly outside triangle ⟨M,I,P⟩, then M and P are mutually visible and the algorithm terminates.
                // 6. Otherwise, at least one reflex vertex lies in ⟨M,I,P⟩. Search for the reflex R that minimizes the angle between (1,0) and the line segment ⟨M,R⟩. Then M and R are mutually visible and the algorithm terminates.
                // 7 It is possible in this step that there are multiple reflex vertices that minimize the angle,in which case all of them lie on a ray with M as the origin. Choose the reflex vertex on this ray that is closest to M.
                int vertWIthSmallestAngle = -1;
                float smallestAngle = float.MaxValue;
                for ( int i = 0; i < vertices.Count; i++ ) {
                    if ( i == testVertex )
                        continue;

                    int nextVertex = ( i + 1 ) % NumVertices;
                    int previousVertex = i == 0 ? NumVertices - 1 : i - 1;

                    bool isNextToLeft = IsLeft( vertices[previousVertex], vertices[i], vertices[nextVertex] ) > 0;
                    bool isReflex = isOuterClockwise == isNextToLeft;
                    if ( !isReflex )
                        continue;   // ignore non reflex

                    Vector2 fromHole = vertices[i] - maxInsideX;
                    float angleTo = Mathf.Atan2( fromHole.y, fromHole.x );
                    angleTo = Mathf.Abs( angleTo );
                    if ( vertWIthSmallestAngle == -1 || angleTo < smallestAngle || ( angleTo == smallestAngle && vertices[i].x < vertices[vertWIthSmallestAngle].x ) ) {
                        vertWIthSmallestAngle = i;
                        smallestAngle = angleTo;
                    }
                }

                if ( vertWIthSmallestAngle == -1 ) {
                    // stick with testVertex
                    outerConnection = testVertex;
                }
                else {
                    outerConnection = vertWIthSmallestAngle;
                }
            }

            // oooooook, now we no where to cut the polygon, insert vertices to connect them
            InsertVertex( outerConnection + 1, vertices[outerConnection] ); // suplicate outer hole

            // add hole vertices
            Vector2 startHole = Vector2.zero;
            for ( int i = 0; i < holePolygon.NumVertices; i++ ) {
                bool reverseHole = IsClockwise == holePolygon.IsClockwise;  // insert hole vertices in reverse direction of outer to keep it correct (eg if outer is clockwise, hole needs to be anticloskwise)
                int holeVertIdx;
                if ( reverseHole ) {
                    holeVertIdx = innerConnection - 1;
                    if ( holeVertIdx < 0 )
                        holeVertIdx += holePolygon.NumVertices;
                }
                else
                    holeVertIdx = ( innerConnection + i ) % holePolygon.NumVertices;

                Vector2 holeVertex = holePolygon.GetVertex( holeVertIdx );
                if ( i == 0 )
                    startHole = holeVertex;
                InsertVertex( outerConnection + 1 + i, holeVertex );
            }
            // then add start of hole again
            InsertVertex( outerConnection + 1 + holePolygon.NumVertices, startHole );
        }

        #endregion

        System.Text.StringBuilder debugSb;
        public void PrintVertices() {
            if ( debugSb == null )
                debugSb = new System.Text.StringBuilder();
            debugSb.Clear();

            for ( int i = 0; i < NumVertices; i++ ) {
                Vector2 p0 = vertices[i];
                debugSb.Append( $"p{i}: {p0} |" );
            }
            Debug.Log( debugSb.ToString() );
        }

        #if UNITY_EDITOR
        public void DrawGizmosInScene() {
            Color c = Gizmos.color;
            Gizmos.color = Color.cyan;
            for ( int i = 0; i < NumVertices; i++ ) {
                Vector2 p0 = vertices[i];
                Vector2 p1 = vertices[( i + 1 ) % NumVertices];
                Gizmos.DrawLine( p0, p1 );
            }
            Gizmos.color = c;
        }
        #endif
    }
}
