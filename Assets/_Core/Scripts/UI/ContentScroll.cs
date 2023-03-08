using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ContentScroll : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private List<RectTransform> _testImages;

    private int _testImagesLength;

    private int _lastTopIndex;
    private int _lastBottomIndex;

    private bool _isPositiveScroll;
    private int _positiveCheckYPos;

    private void Start()
    {
        _testImagesLength = _testImages.Count;
        _lastTopIndex = 0;
        _lastBottomIndex = _testImagesLength - 1;
        _positiveCheckYPos = (int)_rectTransform.anchoredPosition.y;
    }

    private void Update()
    {
        var currentYPos = (int)_rectTransform.anchoredPosition.y + 25;
        if (currentYPos != _positiveCheckYPos)
        {
            ScrollCheck(currentYPos);
        }
    }

    private void ScrollCheck(int currentYPos)
    {
        _isPositiveScroll = currentYPos > _positiveCheckYPos;
        _positiveCheckYPos = currentYPos;
        var lastExpectedTopIndex = currentYPos / 2025; 
        if (currentYPos < 0) lastExpectedTopIndex -= 1;
        lastExpectedTopIndex %= _testImagesLength;
        if (lastExpectedTopIndex < 0) lastExpectedTopIndex += _testImagesLength; 
        if (lastExpectedTopIndex != _lastTopIndex)
        {
            RecycleItems();
        }
    }

    private void RecycleItems()
    {
        if (_isPositiveScroll)
        {
            _testImages[_lastTopIndex].anchoredPosition +=
                Vector2.up * (-2025 * _testImagesLength); 
            _lastBottomIndex = _lastTopIndex;
            _lastTopIndex = (_lastTopIndex + 1) % _testImagesLength;
        }
        else
        {
            _testImages[_lastBottomIndex].anchoredPosition +=
                Vector2.up * (2025 * _testImagesLength);
            _lastTopIndex = _lastBottomIndex;
            _lastBottomIndex = (_lastBottomIndex - 1) % _testImagesLength;
            if (_lastBottomIndex < 0) _lastBottomIndex += _testImagesLength;
        }
    }
}