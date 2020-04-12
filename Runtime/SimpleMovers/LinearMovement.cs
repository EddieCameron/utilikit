/* LinearMovement.cs
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

public class LinearMovement : MonoBehaviour {
    public Vector3 velocity;
    public Space space;

    void Update() {
        transform.Translate( velocity * Time.deltaTime, space );
    }
}
