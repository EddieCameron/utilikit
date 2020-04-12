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
    public Space space;

    void Update() {
        transform.Rotate( angularVelocity * Time.deltaTime, space );
    }

}
