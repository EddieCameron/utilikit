/* UIMesh.cs
 * © Eddie Cameron 2021
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

/**
 *  ============================================================================
 *  MIT License
 *
 *  Copyright (c) 2017 Eric Phillips
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),
 *  to deal in the Software without restriction, including without limitation
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,
 *  and/or sell copies of the Software, and to permit persons to whom the
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 *  DEALINGS IN THE SOFTWARE.
 *  ============================================================================
 *
 *
 *  This script renders a mesh in canvas space using a CanvasRenderer.
 *  This allows all the scaling and canvas layout constraints to be
 *  applied to the mesh.
 *
 *  Created by Eric Phillips on January 1, 2017.
 */

[ExecuteInEditMode]
public class CanvasMesh : Graphic
{
    // Inspector properties
    public Mesh Mesh = null;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    /// <summary>
    /// Callback function when a UI element needs to generate vertices.
    /// </summary>
    /// <param name="vh">VertexHelper utility.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        verts.Clear();
        uvs.Clear();
        tris.Clear();

        if (Mesh == null) return;

        // Get data from mesh
        Mesh.GetVertices( verts );
        Mesh.GetUVs( 0, uvs );
        Mesh.GetTriangles( tris, 0 );

        // Get mesh bounds parameters
        Vector2 meshMin = Mesh.bounds.min;
        Vector2 meshSize = Mesh.bounds.size;
        // Add scaled vertices
        for (int ii = 0; ii < verts.Count; ii++)
        {
            Vector2 v = verts[ii];
            v.x = (v.x - meshMin.x) / meshSize.x;
            v.y = (v.y - meshMin.y) / meshSize.y;
            v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
            vh.AddVert(v, color, uvs[ii]);
        }
        // Add triangles
        for (int ii = 0; ii < tris.Count; ii += 3)
            vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
    }

    protected override void UpdateMaterial() {
        if ( !IsActive() )
            return;

        canvasRenderer.materialCount = 1;
        canvasRenderer.SetMaterial( materialForRendering, 0 );
    }

    /// <summary>
    /// Converts a vertex in mesh coordinates to a point in world coordinates.
    /// </summary>
    /// <param name="vertex">The input vertex.</param>
    /// <returns>A point in world coordinates.</returns>
    public Vector3 TransformVertex(Vector3 vertex)
    {
        // Convert vertex into local coordinates
        Vector2 v;
        v.x = (vertex.x - Mesh.bounds.min.x) / Mesh.bounds.size.x;
        v.y = (vertex.y - Mesh.bounds.min.y) / Mesh.bounds.size.y;
        v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
        // Convert from local into world
        return transform.TransformPoint(v);
    }

    /// <summary>
    /// Converts a vertex in world coordinates into a vertex in mesh coordinates.
    /// </summary>
    /// <param name="vertex">The input vertex.</param>
    /// <returns>A point in mesh coordinates.</returns>
    public Vector3 InverseTransformVertex(Vector3 vertex)
    {
        // Convert from world into local coordinates
        Vector2 v = transform.InverseTransformPoint(vertex);
        // Convert into mesh coordinates
        v.x /= rectTransform.rect.size.x;
        v.y /= rectTransform.rect.size.y;
        v += rectTransform.pivot;
        v = Vector2.Scale(v, Mesh.bounds.size);
        v.x += Mesh.bounds.min.x;
        v.y += Mesh.bounds.min.y;
        return v;
    }
}
