using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.UI.NewUI.Utils
{
    [ExecuteInEditMode]
    public class AnimatedHorizontalLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform _bg;
        private float _width;
        private float _height;
        private Vector2 _cellSize;
        private Vector2 _currentCellSize;
        private Vector2 _othersCellSize;

        public List<RectTransform> elements;
        public List<RectTransform> separators;
        public RectTransform currentElement;
        [SerializeField] private ChildAlignment _childAlignment;
        [SerializeField] private float horizontalCellSpace = 0;
        [Range(1f, 1.2f)] [SerializeField] private float cellSizeMultiplier;

        private void Start()
        {
            _width = _bg.rect.width;
            _height = _bg.rect.height;

            float totalSpace = horizontalCellSpace * elements.Count;
            
            _cellSize = new Vector2((_width-totalSpace) / elements.Count, (_width-totalSpace) / elements.Count);

            foreach (var obj in elements)
            {
                obj.sizeDelta = _cellSize;
            }

            for (int i = 0; i < elements.Count; i++)
            {
                
                if (_childAlignment == ChildAlignment.Left)
                {
                    elements[i].anchorMax = new Vector2(0f, 0.5f);
                    elements[i].anchorMin = new Vector2(0f, 0.5f);
                    
                    float x = ((_cellSize.x / 2) + i * (_cellSize.x+horizontalCellSpace));
                    
                    Vector2 pos = new Vector2(x, 0);
                    elements[i].anchoredPosition = pos;
                }
                else if (_childAlignment == ChildAlignment.Center)
                {
                    elements[i].anchorMax = new Vector2(0.5f, 0.5f);
                    elements[i].anchorMin = new Vector2(0.5f, 0.5f);

                    float x = ((_cellSize.x / 2) + i * (_cellSize.x + horizontalCellSpace)) - _width / 2 +
                              horizontalCellSpace/2;
                    Vector2 pos = new Vector2(x, 0);
                    elements[i].anchoredPosition = pos;
                }
                else
                {
                    elements[i].anchorMax = new Vector2(1f, 0.5f);
                    elements[i].anchorMin = new Vector2(1f, 0.5f);

                    int index = elements.Count - (i+1);
                    float x = ((_cellSize.x / 2) + index * (_cellSize.x+horizontalCellSpace)) *-1;
                    
                    Vector2 pos = new Vector2(x, 0);
                    elements[index].anchoredPosition = pos;
                }
                
            }
            
            
            ChangeCurrentSelectedElement(currentElement);
            
        }

        public void ChangeCurrentSelectedElement(RectTransform element)
        {
            _currentCellSize = _cellSize*cellSizeMultiplier;

            _othersCellSize = _cellSize;
            
            currentElement = element;
            currentElement.DOSizeDelta(_currentCellSize,0.1f).SetEase(Ease.OutSine);
            
            foreach (var obj in elements)
            {
                if(obj == currentElement) continue;
                
                obj.DOSizeDelta(_othersCellSize,0.1f).SetEase(Ease.OutSine);
            }


            int id = elements.IndexOf(currentElement);
            List<Vector2> elementsTargetPos = new List<Vector2>();
            for (int i = 0; i < elements.Count; i++)
            {
                float offset = 0;
                if (id < i)
                    offset = (_currentCellSize.x - _othersCellSize.x) / 2;
                else if(id>i)
                    offset = (_currentCellSize.x - _othersCellSize.x) / 2*-1;
            
                // Vector2 targetPos = new Vector2(_cellSize.x / 2 + i * _cellSize.x + offset, 0);
                // elementsTargetPos.Add(targetPos);
                // elements[i].DOAnchorPos(targetPos, 0.1f).SetEase(Ease.OutSine);
                
                if (_childAlignment == ChildAlignment.Left)
                {
                    float x = ((_cellSize.x / 2) + i * (_cellSize.x+horizontalCellSpace) + offset);
                    if (x < _othersCellSize.x / 2)
                    {
                        x += _othersCellSize.x / 2 - x;
                    }
                    Vector2 pos = new Vector2(x, 0);
                    elements[i].DOAnchorPos(pos, 0.1f).SetEase(Ease.OutSine);
                }
                else if (_childAlignment == ChildAlignment.Center)
                {
                    float x = ((_cellSize.x / 2) + i * (_cellSize.x + horizontalCellSpace)) - _width / 2 +
                              horizontalCellSpace/2 + offset;
                    Vector2 pos = new Vector2(x, 0);
                    elements[i].DOAnchorPos(pos, 0.1f).SetEase(Ease.OutSine);
                }
                else
                {
                    int index = elements.Count - (i+1);
                    float x = ((_cellSize.x / 2) + index * (_cellSize.x+horizontalCellSpace) - offset) *-1;
                    if (Mathf.Abs(x) < _othersCellSize.x / 2)
                    {
                        x -= _othersCellSize.x / 2 - Mathf.Abs(x);
                    }
                    Vector2 pos = new Vector2(x, 0);
                    elements[i].DOAnchorPos(pos, 0.1f).SetEase(Ease.OutSine);
                }
            }

            for (int i = 0; i < separators.Count; i++)
            {
                separators[i]
                    .DOAnchorPosX(elementsTargetPos[i].x + (elementsTargetPos[i + 1].x - elementsTargetPos[i].x) / 2,
                        0.1f);
            }
            
        }

        private void OnValidate()
        {
            Start();
        }

        private void OnEnable()
        {
            OnChangeSelectedElement += ChangeCurrentSelectedElement;
        }

        private void OnDisable()
        {
            OnChangeSelectedElement -= ChangeCurrentSelectedElement;
        }

        #region events
        public delegate void ChangeSelectedElementEvent(RectTransform element);

        private static event ChangeSelectedElementEvent OnChangeSelectedElement;

        public static void ChangeSelectedElement(RectTransform element)
        {
            OnChangeSelectedElement?.Invoke(element);
        }
    


    #endregion
    }

    enum ChildAlignment
    {
        Left,
        Center,
        Right
    }
}