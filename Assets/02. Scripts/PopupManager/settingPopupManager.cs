using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingPopupManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Image bgmHandle;
    [Space()]
    public Slider sfxSlider;
    public Image sfxHandle;

    [Space()]
    public Button hardResetBtn;
    public Button resetBtn;
    public Button exitBtn;

    [Space()]
    public Sprite muteYellowChan;
    public Sprite defaultYellowChan;
    public Sprite fullYellowChan;

    void Awake()
    {
        InitBtnListener();

        InitSliderState();
    }

    void InitBtnListener()
    {
        bgmSlider.onValueChanged.AddListener(ChangeBgmVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);

        hardResetBtn.onClick.AddListener(ClickHardReset);
        resetBtn.onClick.AddListener(ClickReset);
        exitBtn.onClick.AddListener(ClickExit);
    }

    void InitSliderState()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }

    void ChangeBgmVolume(float value)
    {
        AudioManager.Instance.SetBgmVolume(value);

        if (value <= 0)
        {
            bgmHandle.sprite = muteYellowChan;
        }
        else if (value >= 1)
        {
            bgmHandle.sprite = fullYellowChan;
        }
        else
        {
            bgmHandle.sprite = defaultYellowChan;
        }
    }

    void ChangeSfxVolume(float value)
    {
        AudioManager.Instance.SetSfxVolume(value);

        if (value <= 0)
        {
            sfxHandle.sprite = muteYellowChan;
        }
        else if (value >= 1)
        {
            sfxHandle.sprite = fullYellowChan;
        }
        else
        {
            sfxHandle.sprite = defaultYellowChan;
        }
    }

    void ClickHardReset()
    {
        JsonManager.Instance.DeleteAll();

        PlayerPrefsManager.Instance.InitPlayerPrefs(true);

        ScLoadManager.Instance.LoadSceneAsync("ScStart");
    }

    void ClickReset()
    {
        AudioManager.Instance.PlayKat();

        PlayerPrefsManager.Instance.ResetSelectedPlayerPrefs(ppType.audio);

        InitSliderState();
    }

    void ClickExit()
    {
        AudioManager.Instance.PlayAudio(sfxType.cancel);

        gameObject.SetActive(false);
    }
}
