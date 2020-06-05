/* UIGradient.cs
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

namespace Utilikit {
    public class UIGradient : BaseMeshEffect {

        public Gradient gradient;

        public float angleDegrees;


        private Graphic _graphic;
        public Graphic Graphic {
            get {
                if ( _graphic == null )
                    _graphic = GetComponent<Graphic>();
                return _graphic;
            }
        }

        List<float> _axisDists = new List<float>();
        public override void ModifyMesh( VertexHelper vh ) {
            if ( !IsActive() )
                return;

            if ( Graphic == null )
                return;

            int vertexCount = vh.currentVertCount;
            if ( vertexCount <= 0 )
                return;

            float angleRadians = angleDegrees * Mathf.Deg2Rad;
            Vector2 axis = new Vector2( Mathf.Cos( angleRadians ), Mathf.Sin( angleRadians ) );
            UIVertex vertex = new UIVertex();
            _axisDists.Clear();
            // find distance of each point along axis
            float lowest = 0;
            float highest = 0;
            for ( int i = 0; i < vertexCount; i++ ) {
                vh.PopulateUIVertex( ref vertex, i );
                float axisDist = Vector2.Dot( vertex.position, axis );
                _axisDists.Add( axisDist );
                if ( i == 0 || axisDist < lowest )
                    lowest = axisDist;
                if ( i == 0 || axisDist > highest )
                    highest = axisDist;
            }

            for ( int i = 0; i < vertexCount; i++ ) {
                vh.PopulateUIVertex( ref vertex, i );
                float axisDist = _axisDists[i];
                float gradientPos = Mathf.InverseLerp( lowest, highest, axisDist );
                vertex.color *= gradient.Evaluate( gradientPos );
                vh.SetUIVertex( vertex, i );
            }
        }
    }
}
