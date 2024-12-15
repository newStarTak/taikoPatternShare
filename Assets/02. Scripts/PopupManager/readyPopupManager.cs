using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class readyPopupManager : MonoBehaviour
{
    public GameObject randomArea;

    [Header("Right Side KatChan Image")]
    public Image katChan;
    public List<Sprite> katChanSprs;

    [Header("More Ready Popup")]
    public GameObject moreReadyPopup;

    private List<Button> _popupOptionBtns;
    private List<Button> _popupRightBtns;
    private List<TextMeshProUGUI> _popupOptionTexts;

    private List<Button> _morePopupBtns;
    private List<Button> _moreRightBtns;
    private List<TextMeshProUGUI> _moreOptionTexts;

    void Awake()
    {
        InitUIComponent();
        InitBtnListener();
        InitTextNBtn();

        InitByActiveScene();

        SetKatChan();
    }

    void OnEnable()
    {
        moreReadyPopup.SetActive(false);
    }

    void InitUIComponent()
    {
        _popupOptionBtns = transform.GetChild(0).GetChild(0).GetComponentsInChildren<Button>().ToList();
        _popupRightBtns = transform.GetChild(0).GetChild(1).GetComponentsInChildren<Button>().ToList();

        _popupOptionTexts = transform.GetChild(0).GetChild(0).GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.name.Contains("Option")).ToList();


        _morePopupBtns = transform.GetChild(1).GetChild(0).GetComponentsInChildren<Button>().ToList();
        _moreRightBtns = transform.GetChild(1).GetChild(1).GetComponentsInChildren<Button>().ToList();
        
        _moreOptionTexts = transform.GetChild(1).GetChild(0).GetComponentsInChildren<TextMeshProUGUI>().Where(obj => obj.name.Contains("Option")).ToList();
    }

    void InitBtnListener()
    {
        _popupOptionBtns[0].onClick.AddListener(() => ClickOption(GameManager.Instance.cycleDict, "cycleSpeed", _popupOptionTexts[0], btnClickType.left));
        _popupOptionBtns[1].onClick.AddListener(() => ClickOption(GameManager.Instance.cycleDict, "cycleSpeed", _popupOptionTexts[0], btnClickType.right));

        _popupOptionBtns[2].onClick.AddListener(() => ClickBpm(_popupOptionTexts[1], btnClickType.left));
        _popupOptionBtns[3].onClick.AddListener(() => ClickBpm(_popupOptionTexts[1], btnClickType.right));

        _popupOptionBtns[4].onClick.AddListener(() => ClickOption(GameManager.Instance.dualHandDict, "isDualHand", _popupOptionTexts[2], btnClickType.left));
        _popupOptionBtns[5].onClick.AddListener(() => ClickOption(GameManager.Instance.dualHandDict, "isDualHand", _popupOptionTexts[2], btnClickType.right));

        _popupOptionBtns[6].onClick.AddListener(() => ClickOption(GameManager.Instance.randomDict, "isRandomIncluded", _popupOptionTexts[3], btnClickType.left));
        _popupOptionBtns[7].onClick.AddListener(() => ClickOption(GameManager.Instance.randomDict, "isRandomIncluded", _popupOptionTexts[3], btnClickType.right));

        _popupRightBtns[0].onClick.AddListener(() => ClickAdvance());
        _popupRightBtns[1].onClick.AddListener(() => ClickReset());
        _popupRightBtns[2].onClick.AddListener(() => ClickExit(gameObject));
        _popupRightBtns[3].onClick.AddListener(() => ClickStart());


        _morePopupBtns[0].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[0], "pattern_1_included"));
        _morePopupBtns[1].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[1], "pattern_2_included"));
        _morePopupBtns[2].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[2], "pattern_3_included"));
        _morePopupBtns[3].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[3], "pattern_4_included"));
        _morePopupBtns[4].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[4], "pattern_5_included"));
        _morePopupBtns[5].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[5], "pattern_6_included"));
        _morePopupBtns[6].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[6], "pattern_7_included"));
        _morePopupBtns[7].onClick.AddListener(() => ClickPatternNum(_morePopupBtns[7], "pattern_8_included"));

        _morePopupBtns[8].onClick.AddListener(() => ClickOption(GameManager.Instance.onlyRecordDict, "useOnlyRecordPattern", _moreOptionTexts[0], btnClickType.left));
        _morePopupBtns[9].onClick.AddListener(() => ClickOption(GameManager.Instance.onlyRecordDict, "useOnlyRecordPattern", _moreOptionTexts[0], btnClickType.right));


        _moreRightBtns[0].onClick.AddListener(() => ClickExit(moreReadyPopup));
        _moreRightBtns[1].onClick.AddListener(() => ClickReset());
        _moreRightBtns[2].onClick.AddListener(() => ClickExit(gameObject));
        _moreRightBtns[3].onClick.AddListener(() => ClickStart());
    }

    void InitByActiveScene()
    {
        if (SceneManager.GetActiveScene().name == "ScMain")
        {
            randomArea.SetActive(false);
        }
    }

    void InitTextNBtn()
    {
        _popupOptionTexts[0].text = GameManager.Instance.cycleDict[PlayerPrefs.GetInt("cycleSpeed")];
        _popupOptionTexts[1].text = PlayerPrefs.GetFloat("bpm").ToString();
        _popupOptionTexts[2].text = GameManager.Instance.dualHandDict[PlayerPrefs.GetInt("isDualHand")];
        _popupOptionTexts[3].text = GameManager.Instance.randomDict[PlayerPrefs.GetInt("isRandomIncluded")];


        _morePopupBtns[0].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_1_included"));
        _morePopupBtns[1].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_2_included"));
        _morePopupBtns[2].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_3_included"));
        _morePopupBtns[3].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_4_included"));
        _morePopupBtns[4].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_5_included"));
        _morePopupBtns[5].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_6_included"));
        _morePopupBtns[6].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_7_included"));
        _morePopupBtns[7].image.color = ReturnBtnColor(PlayerPrefs.GetInt("pattern_8_included"));

        _moreOptionTexts[0].text = GameManager.Instance.onlyRecordDict[PlayerPrefs.GetInt("useOnlyRecordPattern")];
    }

    #region OptionBtns Func

    /// <param name="dict">GameManager가 가지고 있는 readyPopup 딕셔너리들입니다.</param>
    /// <param name="pKey">딕셔너리에 상응하는 PlayerPrefs 키입니다.</param>
    /// <param name="optionText">수정하고자 하는 옵션 텍스트입니다.</param>
    /// <param name="pClickType">왼쪽 버튼인지 오른쪽 버튼인지 전달해주어야 합니다.</param>
    void ClickOption(Dictionary<int, string> dict, string ppKey, TextMeshProUGUI optionText, btnClickType pClickType)
    {
        AudioManager.Instance.PlayKat();

        int ppValue = PlayerPrefs.GetInt(ppKey);
        int max = dict.Count;

        switch(pClickType)
        {
            case btnClickType.left:

                ppValue = GameManager.Instance.ReturnValidIndex(--ppValue, max);

                break;

            case btnClickType.right:

                ppValue = GameManager.Instance.ReturnValidIndex(++ppValue, max);

                break;
        }

        PlayerPrefs.SetInt(ppKey, ppValue);
        optionText.text = dict[ppValue];

        SetKatChan();
    }

    void ClickBpm(TextMeshProUGUI optionText, btnClickType pClickType)
    {
        AudioManager.Instance.PlayKat();

        var curBpm = PlayerPrefs.GetFloat("bpm");

        switch(pClickType)
        {
            case btnClickType.left:

                if (curBpm > 30)
                {
                    curBpm -= 5;
                    PlayerPrefs.SetFloat("bpm", curBpm);
                }

                break;

            case btnClickType.right:

                if (curBpm < 300)
                {
                    curBpm += 5;
                    PlayerPrefs.SetFloat("bpm", curBpm);
                }

                break;
        }

        optionText.text = curBpm.ToString();

        SetKatChan();
    }

    void ClickPatternNum(Button btn, string ppKey)
    {
        AudioManager.Instance.PlayDon();

        int ppValue = GameManager.Instance.ReturnValidIndex(PlayerPrefs.GetInt(ppKey) + 1, 2);

        PlayerPrefs.SetInt(ppKey, ppValue);

        btn.image.color = ReturnBtnColor(ppValue);
    }

    Color ReturnBtnColor(int ppValue)
    {
        switch(ppValue)
        {
            case 1:

                return Color.red;

            case 0:
            default:

                return new Color(1, 1, 1, 0.5f);
        }
    }

    #endregion

    #region RightBtns Func

    void ClickAdvance()
    {
        AudioManager.Instance.PlayAudio(sfxType.emptyInput);
        
        moreReadyPopup.SetActive(true);
    }

    void ClickReset()
    {
        AudioManager.Instance.PlayKat();

        PlayerPrefsManager.Instance.ResetSelectedPlayerPrefs(ppType.recordOption);

        InitTextNBtn();

        SetKatChan();
    }

    void ClickExit(GameObject targetPopup)
    {
        AudioManager.Instance.PlayAudio(sfxType.cancel);

        if(targetPopup == gameObject)
        {
            GameManager.Instance.ClearPlayPattern();
        }

        targetPopup.SetActive(false);
    }

    void ClickStart()
    {
        var curSceneName = SceneManager.GetActiveScene().name;

        switch(curSceneName)
        {
            case "ScMain":

                GameManager.Instance.curPlayType = playType.training;

                break;

            case "ScRecord":

                GameManager.Instance.curPlayType = playType.recordTraining;

                break;
        }

        ScLoadManager.Instance.LoadSceneAsync("ScPlay");
    }

    #endregion

    void SetKatChan()
    {
        if(katChan == null)
        {
            return;
        }

        katChan.sprite = katChanSprs[Mathf.Clamp(PlayerPrefs.GetInt("cycleSpeed") + PlayerPrefs.GetInt("isDualHand"), 0, 3)];

        if (PlayerPrefs.GetFloat("bpm") > 150)
        {
            katChan.color = new Color(katChan.color.r,
                                        1f - ((PlayerPrefs.GetFloat("bpm") - 150) / 255f),
                                        1f - ((PlayerPrefs.GetFloat("bpm") - 150) / 255f));
        }
        else
        {
            katChan.color = Color.white;
        }
    }
}
