/* DragButton.cs
 * © Eddie Cameron 2019
 * ----------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilikit;
using Random = UnityEngine.Random;

namespace Utilikit {
    /// <summary>
    /// Just a UI object that sends events when dragged from
    /// </summary>
    public class DragButton : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler {

        public class DragButtonEvent : UnityEvent<PointerEventData> { }

        [SerializeField]
        private DragButtonEvent _onBeginDrag = new DragButtonEvent();
        public DragButtonEvent OnBeginDrag => _onBeginDrag;

        [SerializeField]
        private DragButtonEvent _onDrag = new DragButtonEvent();
        public DragButtonEvent OnDrag => _onDrag;

        [SerializeField]
        private DragButtonEvent _onEndDrag = new DragButtonEvent();
        public DragButtonEvent OnEndDrag => _onEndDrag;

        PointerEventData _currentDrag;

        void IBeginDragHandler.OnBeginDrag( PointerEventData eventData ) {
            if (!IsActive() || !IsInteractable() || _currentDrag != null )
                return;

            _currentDrag = eventData;
            _onBeginDrag.Invoke( eventData );
        }

        void IDragHandler.OnDrag( PointerEventData eventData ) {
            if (!IsActive() || !IsInteractable() || _currentDrag == null || _currentDrag.pointerId != eventData.pointerId )
                return;

            _onDrag.Invoke( eventData );
        }

        void IEndDragHandler.OnEndDrag( PointerEventData eventData ) {
            if (!IsActive() || !IsInteractable() || _currentDrag == null || _currentDrag.pointerId != eventData.pointerId )
                return;

            _currentDrag = null;
            _onEndDrag.Invoke( eventData );
        }
    }
}
