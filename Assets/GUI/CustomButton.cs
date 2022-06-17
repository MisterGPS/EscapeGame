using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace UnityEngine.UI
{
    public class CustomButton : Button
    {
        public delegate void OnPointerDownEvent();
        public OnPointerDownEvent onPointerDown;

        public delegate void OnPointerUpEvent();
        public OnPointerUpEvent onPointerUp;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!interactable)
                return;
            if (onPointerDown != null)
                onPointerDown.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!interactable)
                return;
            if (onPointerUp != null)
                onPointerUp.Invoke();
        }
    }
}
