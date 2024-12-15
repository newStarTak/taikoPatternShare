using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScRecordManager : MonoBehaviour
{
    [Header("Content Prefab")]
    public GameObject content;

    [Header("ScRecord UI Components")]
    public RectTransform contentsContainerRectTr;
    public Transform contentsContainerTr;
    public GameObject fixedContent;
    public Button exitBtn;
    public Button readyBtn;
    public GameObject multiTitleObj;

    [Header("Popup Components")]
    public GameObject readyPopup;
    public GameObject recordOptionPopup;
    public Button recordOptionPopupOpenBtn;
    public Button playMultiBtn;
    public Button cancelBtn;

    [Header("KatChan in Popup")]
    public Image katChan;
    public List<Sprite> katChanSprs;


    private List<GameObject> _contents;

    private List<GameObject> _playBtnObjs;
    private List<GameObject> _deleteBtnObjs;
    private List<Button> _multiBtns;
    private List<TextMeshProUGUI> _multiTexts;

    void Awake()
    {
        InitBgm();

        InitComponents();

        InitContents();

        InitBtnListener();

        SetContentsContainerSize();
    }

    void InitBgm()
    {
        AudioManager.Instance.PlayBgm(bgmType.record);
    }

    void InitComponents()
    {
        _contents = new List<GameObject>();

        _playBtnObjs = new List<GameObject>();
        _deleteBtnObjs = new List<GameObject>();
        _multiBtns = new List<Button>();
        _multiTexts = new List<TextMeshProUGUI>();
    }

    void InitContents()
    {
        for (int contentIdx = 0; JsonManager.Instance.LoadPattern(contentIdx) != null; contentIdx++)
        {
            GameManager.Instance.curPatternIndex = contentIdx;

            GameObject contentObj = Instantiate(content, contentsContainerTr);
            _contents.Add(contentObj);

            Button curPlayBtn = contentObj.transform.GetChild(1).GetComponent<Button>();
            curPlayBtn.onClick.AddListener(() => OpenReadyPopup(contentObj));
            _playBtnObjs.Add(curPlayBtn.gameObject);

            Button curDeleteBtn = contentObj.transform.GetChild(2).GetComponent<Button>();
            curDeleteBtn.onClick.AddListener(() => DeleteContent(contentObj));
            _deleteBtnObjs.Add(curDeleteBtn.gameObject);

            Button curMultiBtn = contentObj.transform.GetChild(3).gameObject.GetComponent<Button>();
            TextMeshProUGUI curMultiText = curMultiBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _multiBtns.Add(curMultiBtn);
            _multiTexts.Add(curMultiText);
            InitMultiBtn();
            curMultiBtn.onClick.AddListener(() => AddOrRemoveThisPattern(contentObj, curMultiBtn, curMultiText));
            curMultiBtn.gameObject.SetActive(false);
        }

        multiTitleObj.SetActive(false);
        readyBtn.gameObject.SetActive(false);
    }

    void InitBtnListener()
    {
        recordOptionPopupOpenBtn.onClick.AddListener(() => OpenRecordOptionPopup());
        exitBtn.onClick.AddListener(() => ClickExit());

        playMultiBtn.onClick.AddListener(ClickMulti);
        cancelBtn.onClick.AddListener(ClickCancel);
        readyBtn.onClick.AddListener(ClickReady);
    }

    void SetContentsContainerSize()
    {
        contentsContainerRectTr.sizeDelta = new Vector2(contentsContainerRectTr.sizeDelta.x, 100f + (contentsContainerRectTr.childCount * 220f));
    }

    void OpenReadyPopup(GameObject contentObj = null)
    {
        AudioManager.Instance.PlayKat();

        if(contentObj != null)
        {
            GameManager.Instance.AddToPlayPattern(_contents.IndexOf(contentObj));
        }

        readyPopup.SetActive(true);
    }

    void DeleteContent(GameObject contentObj)
    {
        AudioManager.Instance.PlayKat();

        JsonManager.Instance.DeletePattern(_contents.IndexOf(contentObj));

        _contents.Remove(contentObj);

        Destroy(contentObj);

        SetContentsContainerSize();
    }

    void InitMultiBtn()
    {
        foreach(var curMultiBtn in _multiBtns)
        {
            curMultiBtn.image.color = new Color(1, 1, 1, 0.5f);
        }

        foreach(var curMultiText in _multiTexts)
        {
            curMultiText.text = string.Empty;
        }
    }

    void AddOrRemoveThisPattern(GameObject contentObj, Button multiBtn, TextMeshProUGUI curMultiText)
    {
        AudioManager.Instance.PlayKat();

        string curContentObjPattern = JsonManager.Instance.LoadPattern(_contents.IndexOf(contentObj));

        // 이미 추가된 경우, 즉 제거하려는 상황
        if(GameManager.Instance.playPatterns.Contains(curContentObjPattern))
        {
            multiBtn.image.color = new Color(1, 1, 1, 0.5f);

            curMultiText.text = string.Empty;

            GameManager.Instance.RemoveFromPlayPattern(curContentObjPattern);

            int i = 0;

            foreach (var multiText in _multiTexts)
            {
                // 몇 번째로 선택되었는지 확인 검사할 패턴
                var curPattern = JsonManager.Instance.LoadPattern(i++);

                if (multiText.text != string.Empty)
                {
                    multiText.text = (GameManager.Instance.playPatterns.IndexOf(curPattern) + 1).ToString();
                }
            }
        }

        // 아직 추가되지 않은 경우, 즉 추가하려는 상황
        else
        {
            multiBtn.image.color = new Color(1, 0.5f, 0.5f, 1f);

            curMultiText.text = (GameManager.Instance.playPatterns.Count + 1).ToString();

            GameManager.Instance.AddToPlayPattern(curContentObjPattern);
        }
    }

    void OpenRecordOptionPopup()
    {
        AudioManager.Instance.PlayKat();

        recordOptionPopup.SetActive(true);
    }

    void ClickExit()
    {      
        ScLoadManager.Instance.LoadSceneAsync("ScMain");
    }

    void ClickMulti()
    {
        AudioManager.Instance.PlayKat();

        recordOptionPopup.SetActive(false);

        fixedContent.SetActive(false);

        multiTitleObj.SetActive(true);

        cancelBtn.gameObject.SetActive(true);

        readyBtn.gameObject.SetActive(true);

        foreach(var multiBtnObj in _multiBtns)
        {
            multiBtnObj.gameObject.SetActive(true);
        }

        foreach(var playBtnObj in _playBtnObjs)
        {
            playBtnObj.SetActive(false);
        }

        foreach(var deleteBtn in _deleteBtnObjs)
        {
            deleteBtn.SetActive(false);
        }
    }

    void ClickCancel()
    {
        AudioManager.Instance.PlayAudio(sfxType.cancel);

        recordOptionPopup.SetActive(true);

        fixedContent.SetActive(true);

        multiTitleObj.SetActive(false);

        cancelBtn.gameObject.SetActive(false);

        readyBtn.gameObject.SetActive(false);

        foreach(var multiBtnObj in _multiBtns)
        {
            multiBtnObj.gameObject.SetActive(false);
        }

        foreach(var playBtnObj in _playBtnObjs)
        {
            playBtnObj.SetActive(true);
        }

        foreach(var deleteBtn in _deleteBtnObjs)
        {
            deleteBtn.SetActive(true);
        }

        GameManager.Instance.ClearPlayPattern();
    }

    void ClickReady()
    {
        if(GameManager.Instance.playPatterns.Count == 0)
        {
            return;
        }

        InitMultiBtn();

        OpenReadyPopup();
    }
}
