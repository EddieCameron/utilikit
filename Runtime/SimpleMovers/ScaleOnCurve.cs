/* ScaleOnCurve.cs
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

public class ScaleOnCurve : MonoBehaviour {
    public bool perAxis;

    public float maxScale;
    public AnimationCurve globalScale;
    public AnimationCurve xScale, yScale, zScale;

    public float curveTime;

    float lifeTime;

    void Update() {
        lifeTime += Time.deltaTime;

        float normalizedTime = lifeTime / curveTime;

        Vector3 scale;
        if ( perAxis ) {
            scale.x = xScale.Evaluate( normalizedTime ) * maxScale;
            scale.y = yScale.Evaluate( normalizedTime ) * maxScale;
            scale.z = zScale.Evaluate( normalizedTime ) * maxScale;
        }
        else {
            scale = Vector3.one * globalScale.Evaluate( normalizedTime ) * maxScale;
        }

        transform.localScale = scale;

        if ( normalizedTime > 1 )
            enabled = false;
    }

    public void Replay() {
        lifeTime = 0;
        enabled = true;
    }
}
