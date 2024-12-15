using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasCtrl : MonoBehaviour
{
    private CanvasScaler _canvasScaler;

    private float _referenceRatio = 1280f / 720f;
    private float _lastCheckedRatio;

    void Awake()
    {
        InitCanvas();

        InitRatio();
    }

    void InitCanvas()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
    }

    void InitRatio()
    {
        _lastCheckedRatio = (float)Screen.width / Screen.height;
    }

    void Update()
    {
        if(_lastCheckedRatio != (float)Screen.width / Screen.height)
        {
            if((float)Screen.width / Screen.height > _referenceRatio)
            {
                _canvasScaler.matchWidthOrHeight = 0f;
            }
            else
            {
                _canvasScaler.matchWidthOrHeight = 1f;
            }

            InitRatio();
        }
    }
}
