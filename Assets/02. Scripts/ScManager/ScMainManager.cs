using UnityEngine;
using UnityEngine.UI;

public class ScMainManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button recordBtn;
    public Button trainingBtn;
    public Button settingBtn;

    [Header("Current Scene Popup")]
    public GameObject readyPopup;
    public GameObject settingPopup;

    void Awake()
    {
        InitBgm();

        InitBtnListener();
    }

    void InitBgm()
    {
        AudioManager.Instance.PlayBgm(bgmType.main);
    }

    void InitBtnListener()
    {
        recordBtn.onClick.AddListener(() => ClickRecordBtn());
        trainingBtn.onClick.AddListener(() => OpenReadyPopup());
        settingBtn.onClick.AddListener(() => OpenSettingPopup());
    }
    
    void ClickRecordBtn()
    {
        ScLoadManager.Instance.LoadSceneAsync("ScRecord");
    }

    void OpenReadyPopup()
    {
        AudioManager.Instance.PlayKat();

        readyPopup.SetActive(true);
    }

    void OpenSettingPopup()
    {
        AudioManager.Instance.PlayKat();

        settingPopup.SetActive(true);
    }
}