using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchThresholdCtrl : MonoBehaviour
{
    private Image img;
    
    void Awake()
    {
        img = GetComponent<Image>();

        img.alphaHitTestMinimumThreshold = 0.001f;
    }
}
