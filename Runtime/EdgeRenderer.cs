/* EdgeRenderer.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

[RequireComponent( typeof( MeshRenderer ), typeof( MeshFilter ))]
public class EdgeRenderer : MonoBehaviour {
    public Color color;

    [Tooltip( "Mesh will extend this distance from the points")]
    public Vector2 extrudeAmount;

    int meshPointCount = 0;
    List<Vector2> _points = new List<Vector2>();
    bool _isDirty = true;
    Mesh _mesh;

    void Awake() {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    void Update() {
        if ( _isDirty ) {
            if ( _points.Count != meshPointCount ) {
                GenerateMeshLayout();
                meshPointCount = _points.Count;
            }
            GenerateMesh();
            _mesh.RecalculateBounds();
            _isDirty = false;
        }
    }

    public void SetPoints( IEnumerable<Vector2> points ) {
        _points.Clear();
        _points.AddRange( points );
        _isDirty = true;
    }

    public void SetPoint( int idx, Vector2 point ) {
        _points[idx] = point;
        _isDirty = true;
    }

    void GenerateMeshLayout() {
        _mesh.Clear( keepVertexLayout: false );

        int vertexCount = _points.Count * 2;
        int trisCount = ( _points.Count - 1 ) * 6;

        _mesh.SetVertexBufferParams( vertexCount, new VertexAttributeDescriptor( VertexAttribute.Position ),
            new VertexAttributeDescriptor( VertexAttribute.Color, VertexAttributeFormat.UNorm8, dimension: 4 ) );
        //_mesh.SetIndexBufferParams( trisCount, IndexFormat.UInt16 );
    }

    void GenerateMesh() {
        if ( _points.Count < 2 ) {
            return; // no mesh
        }
        else {
            int vertexCount = _points.Count * 2;
            int trisCount = ( _points.Count - 1 ) * 6;

            NativeArray<Vector3> verts = new NativeArray<Vector3>( vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
            NativeArray<Color32> colors = new NativeArray<Color32>( vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
            NativeArray<ushort> tris = new NativeArray<ushort>( trisCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory );
            try {
                for ( ushort i = 0; i < _points.Count; i++ ) {
                    verts[i * 2] = _points[i];
                    verts[i * 2 + 1] = _points[i] + extrudeAmount;
                    colors[i * 2] = color;
                    colors[i * 2 + 1] = color;

                    if ( i < _points.Count - 1 ) {
                        tris[i * 6] = (ushort)( i * 2 );
                        tris[i * 6 + 1] = (ushort)( i * 2 + 3 );
                        tris[i * 6 + 2] = (ushort)( i * 2 + 1 );
                        tris[i * 6 + 3] = (ushort)( i * 2 );
                        tris[i * 6 + 4] = (ushort)( i * 2 + 2 );
                        tris[i * 6 + 5] = (ushort)( i * 2 + 3 );
                    }
                }

                _mesh.SetVertices( verts );
                _mesh.SetColors( colors );
                _mesh.SetIndices( tris, MeshTopology.Triangles, 0, calculateBounds: true );
            } finally {
                verts.Dispose();
                tris.Dispose();
                colors.Dispose();
            }
        }
    }
}
