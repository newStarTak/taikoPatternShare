using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeCtrl : MonoBehaviour
{
    public Image leftFade;
    public Image rightFade;
    public Image bg;

    public Sprite leftDanSpr;
    public Sprite rightDanSpr;
    public Sprite bgDanSpr;

    public Slider progressBar;

    public TextMeshProUGUI loadText;

    void Awake()
    {
        InitFade();
    }

    void InitFade()
    {
        if(GameManager.Instance.curFadeType == fadeType.dan)
        {
            leftFade.sprite = leftDanSpr;
            rightFade.sprite = rightDanSpr;
            bg.sprite = bgDanSpr;
        }
    }

    void Update()
    {
        if(progressBar != null)
        {
            progressBar.value = ScLoadManager.Instance.progress;

            if(progressBar.value >= 1)
            {
                loadText.text = "로딩 끝났다동";
            }
        }
    }
}
