using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScStartManager : MonoBehaviour
{
    public Button touchScreen;

    void Awake()
    {
        InitGame();

        InitBgm();

        InitBtnListener();
    }

    void InitGame()
    {
        Application.targetFrameRate = 120;

        PlayerPrefsManager.Instance.InitPlayerPrefs();
    }

    void InitBgm()
    {
        AudioManager.Instance.PlayBgm(bgmType.start);
    }

    void InitBtnListener()
    {
        touchScreen.onClick.AddListener(() => ScLoadManager.Instance.LoadSceneAsync("ScMain"));
    }
}
