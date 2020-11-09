/* SimpleHarmocOscillation.cs
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
    public static class HarmonicOscillation {

        public static float Oscillate( float currentValue, float targetValue, ref float velocity, float accel, float deltaTime = 0 ) {
            return Oscillate( currentValue, targetValue, ref velocity, accel, maxSpeed: float.PositiveInfinity, deltaTime: deltaTime );
        }

        public static float Oscillate( float currentValue, float targetValue, ref float velocity, float accel, float maxSpeed, float deltaTime = 0 ) {
            float diff = targetValue - currentValue;
            float totalForce = diff * accel;

            if ( deltaTime <= 0 )
                deltaTime = Time.deltaTime;

            velocity += totalForce * deltaTime;
            velocity = Mathf.Clamp( velocity, -maxSpeed, maxSpeed );
            return currentValue + velocity * deltaTime;
        }

        public static float OscillateAngle( float currentAngleDegrees, float targetAngleDegrees, ref float velocity, float accel, float deltaTime = 0 ) {
            return OscillateAngle( currentAngleDegrees, targetAngleDegrees, ref velocity, accel, maxSpeed: float.PositiveInfinity, deltaTime: deltaTime );
        }

        public static float OscillateAngle( float currentAngleDegrees, float targetAngleDegrees, ref float velocity, float accel, float maxSpeed, float deltaTime = 0 ) {
            float diff = Mathf.DeltaAngle( currentAngleDegrees, targetAngleDegrees );
            float totalForce = diff * accel;

            if ( deltaTime <= 0 )
                deltaTime = Time.deltaTime;

            velocity += totalForce * deltaTime;
            velocity = Mathf.Clamp( velocity, -maxSpeed, maxSpeed );
            return currentAngleDegrees + velocity * deltaTime;
        }

        public static float OscillateDamped( float currentValue, float targetValue, ref float velocity, float accel, float dampingRatio, float deltaTime = 0 ) {
            return OscillateDamped( currentValue, targetValue, ref velocity, accel, dampingRatio, float.PositiveInfinity, deltaTime );
        }

        public static float OscillateDamped( float currentValue, float targetValue, ref float velocity, float accel, float dampingRatio, float maxSpeed, float deltaTime = 0 ) {
            float dampingForce = dampingRatio * 2 * Mathf.Sqrt( accel );

            float diff = targetValue - currentValue;
            float totalForce = diff * accel - dampingForce * velocity;

            if ( deltaTime <= 0 )
                deltaTime = Time.deltaTime;

            velocity += totalForce * deltaTime;
            velocity = Mathf.Clamp( velocity, -maxSpeed, maxSpeed );
            return currentValue + velocity * deltaTime;
        }

        public static float OscillateAngleDamped( float currentAngleDegrees, float targetAngleDegrees, ref float velocity, float accel, float dampingRatio, float deltaTime = 0 ) {
            return OscillateAngleDamped( currentAngleDegrees, targetAngleDegrees, ref velocity, accel, dampingRatio, maxSpeed: float.PositiveInfinity, deltaTime: deltaTime );
        }

        public static float OscillateAngleDamped( float currentAngleDegrees, float targetAngleDegrees, ref float velocity, float accel, float dampingRatio, float maxSpeed, float deltaTime = 0 ) {
            float dampingForce = dampingRatio * 2 * Mathf.Sqrt( accel );

            float diff = Mathf.DeltaAngle( currentAngleDegrees, targetAngleDegrees );
            float totalForce = diff * accel - dampingForce * velocity;

            if ( deltaTime <= 0 )
                deltaTime = Time.deltaTime;

            velocity += totalForce * deltaTime;
            velocity = Mathf.Clamp( velocity, -maxSpeed, maxSpeed );
            return currentAngleDegrees + velocity * deltaTime;
        }
    }
}
