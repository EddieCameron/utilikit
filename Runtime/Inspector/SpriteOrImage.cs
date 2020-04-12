/* SpriteOrImage.cs
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
    [Serializable]
    public class SpriteOrImage {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        [SerializeField]
        private Image _image;
        public Image Image => _image;

        public bool IsSpriteRenderer => _spriteRenderer != null;
        public bool IsImage => _image != null;

        public bool HasReference => _spriteRenderer != null || _image != null;
        public Component ReferencedComponent => IsSpriteRenderer ? (Component)_spriteRenderer : (Component)_image;

        public SpriteOrImage( GameObject go ) {
            GetFromGameObject( go );
        }

        public void GetFromGameObject( GameObject go ) {
            _spriteRenderer = go.GetComponent<SpriteRenderer>();
            if ( _spriteRenderer == null )
                _image = go.GetComponent<Image>();
        }

        public bool Enabled {
            get {
                if ( IsSpriteRenderer )
                    return _spriteRenderer.enabled;
                else if ( IsImage )
                    return _image.enabled;
                else
                    throw new NullReferenceException( "No sprite renderer or image defined" );
            }
            set {
                if ( IsSpriteRenderer )
                    _spriteRenderer.enabled = value;
                else if ( IsImage )
                    _image.enabled = value;
                else
                    throw new NullReferenceException( "No sprite renderer or image defined" );
            }
        }

        public Sprite Sprite {
            get {
                return _spriteRenderer?.sprite ?? _image?.sprite;
            }
            set {
                if ( IsSpriteRenderer )
                    _spriteRenderer.sprite = value;
                else if ( IsImage )
                    _image.sprite = value;
                else
                    throw new NullReferenceException( "No sprite renderer or image defined" );
            }
        }

        public Color Color {
            get {
                if ( IsSpriteRenderer )
                    return _spriteRenderer.color;
                else if ( IsImage )
                    return _image.color;
                else
                    throw new NullReferenceException( "No sprite renderer or image defined" );
            }
            set {
                if ( IsSpriteRenderer )
                    _spriteRenderer.color = value;
                else if ( IsImage )
                    _image.color = value;
                else
                    throw new NullReferenceException( "No sprite renderer or image defined" );
            }
        }
    }
}
