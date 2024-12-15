using UnityEngine;
using UnityEngine.UI;

public class backgroundScrollCtrl : MonoBehaviour
{
    public float speed = 0;
 
    private Image _img;

    void Awake()
    {
        InitComponent();
    }

    void InitComponent()
    {
        _img = GetComponent<Image>();
    }

    void Update()
    {
        _img.material.mainTextureOffset += new Vector2(speed, 0);
    }
}
