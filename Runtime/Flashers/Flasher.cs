using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Flasher : MonoBehaviour
{
    public Color baseColor;
    public bool setBaseColorOnStart = true;

    public Color flashColor = Color.white;

    public float flashTime = .2f;
    public AnimationCurve flashEase = AnimationCurve.Linear( 0, 0, 1, 1 );

    private bool isFlashing;
    private Color targetColor;
    private float flashStartTime;

    protected abstract Color ObjectColor { get; set; }

    void Start() {
        if ( setBaseColorOnStart )
            baseColor = ObjectColor;
    }

    void Update()
    {
        if ( !isFlashing )
            return;

        float t = ( Time.time - flashStartTime ) / flashTime;
        if ( t > 1 ) {
            isFlashing = false;
            ObjectColor = baseColor;
            return;
        }

        t = flashEase.Evaluate( t );
        ObjectColor = Color.Lerp( flashColor, baseColor, t );
    }

    public void Flash() {
        Flash( flashColor );
    }

    public void Flash( Color overrideFlashColor ) {
        isFlashing = true;
        targetColor = overrideFlashColor;
        flashStartTime = Time.time;
        ObjectColor = overrideFlashColor;
    }
}
