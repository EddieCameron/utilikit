using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlasher : Flasher
{
    private SpriteRenderer _spriteRenderer;
    public SpriteRenderer SpriteRenderer {
        get {
            if ( _spriteRenderer == null )
                _spriteRenderer = GetComponent<SpriteRenderer>();
            return _spriteRenderer;
        }
    }

    protected override Color ObjectColor { get => SpriteRenderer.color; set => SpriteRenderer.color = value; }
}
