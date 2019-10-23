using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGraphicFlasher : Flasher
{
    private Graphic _graphic;
    public Graphic Graphic {
        get {
            if ( _graphic == null )
                _graphic = GetComponent<Graphic>();
            return _graphic;
        }
    }

    protected override Color ObjectColor { get => Graphic.color; set => Graphic.color = value; }
}
