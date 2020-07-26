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

    [SerializeField]
    private bool isFlashing;

    [SerializeField]
    private bool isRepeating;
    private Color targetColor;
    private float flashStartTime;

    protected abstract Color ObjectColor { get; set; }

    void Awake() {
        if ( setBaseColorOnStart )
            baseColor = ObjectColor;
        targetColor = baseColor;
        flashStartTime = Time.time;
    }

    void Update()
    {
        if ( !isFlashing )
            return;

        float t = ( Time.time - flashStartTime ) / flashTime;
        if ( t > 1 ) {
            if ( isRepeating ) {
                Flash( targetColor, isRepeating: true );   // reset flash
            }
            else {
                isFlashing = false;
                ObjectColor = baseColor;
            }
            return;
        }

        t = flashEase.Evaluate( t );
        ObjectColor = Color.Lerp( flashColor, baseColor, t );
    }

    public void Flash( bool isRepeating = false ) {
        Flash( flashColor, isRepeating );
    }

    public void Flash( Color overrideFlashColor, bool isRepeating = false ) {
        this.isFlashing = true;
        this.isRepeating = isRepeating;
        this.targetColor = overrideFlashColor;
        this.flashStartTime = Time.time;
        this.ObjectColor = overrideFlashColor;
    }

    public void StopRepeatingFlash() {
        this.isRepeating = false;
    }
}
