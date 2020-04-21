/* Rotate.cs
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

public class Rotate : MonoBehaviour {
    public Vector3 angularVelocity;
    public bool startRandom;

    public Space space;

    void Start() {
        if ( startRandom ) {
            Vector3 startEulers = transform.localEulerAngles + new Vector3( angularVelocity.x * 360f * Random.value, angularVelocity.y * 360f * Random.value, angularVelocity.z * 360f * Random.value);
            transform.localEulerAngles = startEulers;
        }
    }

    void Update() {
        transform.Rotate( angularVelocity * Time.deltaTime, space );
    }

}
