using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI.NewUI.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity_Utils
{
    public class IH_AnimatedLayout : MonoBehaviour,IPointerDownHandler
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AnimatedHorizontalLayout.ChangeSelectedElement(_rectTransform);       
        }
    }
}
