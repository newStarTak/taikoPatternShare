using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScPlayManager : MonoBehaviour
{
    [Header("Mode-independent Objs")]
    public Button pauseBtn;
    public GameObject touchTaiko;
    public TextMeshProUGUI curModeText;

    [Space()]
    public GameObject donRedEffect;
    public GameObject katBlueEffect;


    [Header("Record Mode")]
    public Button removeOneBtn;
    public Button clearBtn;
    public Button saveBtn;
    public Button saveNExitBtn;

    /// <summary>
    /// 기록 모드(recordMode)에서 북 입력에 따라 나타나는 노트 Pool
    /// </summary>
    public GameObject recordingNotePool;

    /// <summary>
    /// 기록 모드(recordMode)에서 기록 중인 노트의 Image 리스트
    /// </summary>
    private List<Image> _recordingNoteImgs;


    [Header("Training Mode")]
    public float customOffset;

    public GameObject leftBlock;
    public GameObject rightBlock;

    [Space()]
    public GameObject shooter;

    public List<GameObject> textPool;

    public GameObject badUpperBgBlur;


    [Space()]
    public Image gauge;

    public GameObject perfectBoom;
    public GameObject goodBoom;

    /// <summary>
    /// 수련 모드(traingingMode)에서 활성화 리스트(_activeNotes)에 들어가기 전 대기 상태인 노트 Pool
    /// </summary>
    public GameObject notePool;

    /// <summary>
    /// 수련 모드(trainingMode)에서 노트 Pool 안 노트들의 Image 리스트
    /// </summary>
    private List<Image> _noteImgs;

    /// <summary>
    /// 수련 모드(trainingMode)에서 Shooter에 의해 활성화 시 추가되는 노트 리스트
    /// </summary>
    private List<noteInfo> _activeNotes;


    private Coroutine _coShoot;
    private float bpm;

    private bool _isBlackBlurActive = false;

    const float timing_bad = 0.108f;
    const float timing_good = 0.075f;
    const float timing_perfect = 0.025f;

    private KeyCode _leftKatKey;
    private KeyCode _leftDonKey;
    private KeyCode _rightDonKey;
    private KeyCode _rightKatKey;

    private List<int> _randomPatternLens;

    private List<string> _patternPool;

    private btnClickType _lastInputDirection;

    private bool _isDualHand;


    void Awake()
    {
        InitBgm();
        InitText();
        InitKeyCode();

        InitByPlayType();
    }

    void InitBgm()
    {
        AudioManager.Instance.PauseBgm();
    }

    void InitText()
    {
        switch(GameManager.Instance.curPlayType)
        {
            case playType.record:

                curModeText.text = "기록 모드";

                break;

            case playType.recordTraining:

                curModeText.text = "패턴 연습";

                break;

            case playType.training:

                curModeText.text = "수련 모드";

                break;
        }
    }

    void InitByPlayType()
    {
        switch(GameManager.Instance.curPlayType)
        {
            case playType.record:

                removeOneBtn.onClick.AddListener(() => ClickRemoveOne());
                clearBtn.onClick.AddListener(() => ClickClear());
                saveBtn.onClick.AddListener(() => ClickSave(false));
                saveNExitBtn.onClick.AddListener(() => ClickSave(true));

                _recordingNoteImgs = new List<Image>();

                InitRecordingNotes();

                break;

            case playType.recordTraining:
            case playType.training:

                removeOneBtn.gameObject.SetActive(false);
                clearBtn.gameObject.SetActive(false);
                saveBtn.gameObject.SetActive(false);
                saveNExitBtn.gameObject.SetActive(false);

                bpm = PlayerPrefs.GetFloat("bpm");

                _noteImgs = new List<Image>();
                _activeNotes = new List<noteInfo>();

                _randomPatternLens = new List<int>();
                for(int i = 1; i <= 8; i++)
                {
                    if(PlayerPrefs.GetInt("pattern_" + i + "_included") == 1)
                    {
                        _randomPatternLens.Add(i);
                    }
                }

                if(_randomPatternLens.Count == 0)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        _randomPatternLens.Add(i);
                    }
                }

                _patternPool = new List<string>();

                foreach (var targetLen in _randomPatternLens)
                {
                    foreach (var recordedPattern in JsonManager.Instance.LoadJsonData().patterns)
                    {
                        if (recordedPattern.Length == targetLen)
                        {
                            _patternPool.Add(recordedPattern);
                        }
                    }
                }

                InitNotePool();

                if(_coShoot == null)
                {
                    _coShoot = StartCoroutine(CoShoot());
                }

                break;
        }

        if(PlayerPrefs.GetInt("isDualHand") == 0)
        {
            _isDualHand = false;
        }
        else
        {
            _isDualHand = true;
        }
    }

    void InitKeyCode()
    {
        _leftKatKey = (KeyCode)PlayerPrefs.GetInt("leftKat");
        _leftDonKey = (KeyCode)PlayerPrefs.GetInt("leftDon");
        _rightDonKey = (KeyCode)PlayerPrefs.GetInt("rightDon");
        _rightKatKey = (KeyCode)PlayerPrefs.GetInt("rightKat");
    }

    void InitRecordingNotes()
    {
        for (int i = 0; i < GameManager.pattern_max; i++)
        {
            GameObject recordingNote = NoteManager.Instance.GenerateNote(noteType.don, recordingNotePool.transform);

            Image recordingNoteImg = recordingNote.GetComponent<Image>();

            _recordingNoteImgs.Add(recordingNoteImg);


            recordingNote.SetActive(false);
        }
    }

    void InitNotePool()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject note = NoteManager.Instance.GenerateNote(noteType.don, notePool.transform, true);

            note.SetActive(false);

            Image noteImg = note.GetComponent<Image>();

            _noteImgs.Add(noteImg);
        }
    }

    void Update()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR

            if (Input.GetKeyDown(_leftKatKey))
            {
                Kat(-1);
            }

            if(Input.GetKeyDown(_leftDonKey))
            {
                Don(-1);
            }

            if (Input.GetKeyDown(_rightDonKey))
            {
                Don(+1);
            }

            if (Input.GetKeyDown(_rightKatKey))
            {
                Kat(+1);
            }

        #endif
    }

    public void Don(int hitPoint = 0)
    {
        AudioManager.Instance.PlayDon();

        if(Input.mousePosition.x - touchTaiko.transform.position.x < 0f || hitPoint < 0)
        {
            CheckDualHand(-1);
        }
        else
        {
            CheckDualHand(1);
        }

        StartCoroutine(TouchInteract(noteType.don));

        switch(GameManager.Instance.curPlayType)
        {
            case playType.record:

                if (JsonManager.Instance.curRecordingPattern.Length < 8)
                {
                    JsonManager.Instance.curRecordingPattern += "d";
                }

                ArrangeRecordNote();

                break;
            
            case playType.recordTraining:
            case playType.training:

                CheckTiming(noteType.don);

                break;
        }
    }

    public void Kat(int hitPoint = 0)
    {
        AudioManager.Instance.PlayKat();

        if(Input.mousePosition.x - touchTaiko.transform.position.x < 0f || hitPoint < 0)
        {
            CheckDualHand(-1);
        }
        else
        {
            CheckDualHand(1);
        }
        
        StartCoroutine(TouchInteract(noteType.kat));

        switch(GameManager.Instance.curPlayType)
        {
            case playType.record:

                if (JsonManager.Instance.curRecordingPattern.Length < 8)
                {
                    JsonManager.Instance.curRecordingPattern += "k";
                }

                ArrangeRecordNote();

                break;
            
            case playType.recordTraining:
            case playType.training:

                CheckTiming(noteType.kat);

                break;
        }
    }

    void CheckDualHand(int hitPoint)
    {
        if(_isDualHand == false)
        {
            return;
        }
        
        if(GameManager.Instance.curPlayType == playType.record)
        {
            return;
        }

        // 이번에 누른 곳이 오른쪽일 경우
        if(hitPoint > 0)
        {
            leftBlock.SetActive(false);
            rightBlock.SetActive(true);
        }
        // 왼쪽일 경우
        else
        {
            rightBlock.SetActive(false);
            leftBlock.SetActive(true);
        }
    }

    IEnumerator TouchInteract(noteType pNoteType)
    {
        switch(pNoteType)
        {
            case noteType.don:

                donRedEffect.SetActive(false);
                donRedEffect.SetActive(true);

                touchTaiko.transform.position = new Vector3(touchTaiko.transform.position.x,
                                                    touchTaiko.transform.position.y - 10f,
                                                    touchTaiko.transform.position.z);

                yield return new WaitForSeconds(0.1f);

                touchTaiko.transform.position = new Vector3(touchTaiko.transform.position.x,
                                                                touchTaiko.transform.position.y + 10f,
                                                                touchTaiko.transform.position.z);

                break;

            case noteType.kat:

                katBlueEffect.SetActive(false);
                katBlueEffect.SetActive(true);

                break;
        }
    }

    public void ClickPause()
    {
        switch(GameManager.Instance.curPlayType)
        {
            case playType.record:

                JsonManager.Instance.RecordReset();
                ScLoadManager.Instance.LoadSceneAsync("ScRecord");

                break;

            case playType.recordTraining:

                GameManager.Instance.ClearPlayPattern();
                StopAllCoroutines();
                ScLoadManager.Instance.LoadSceneAsync("ScRecord");

                break;

            case playType.training:

                GameManager.Instance.ClearPlayPattern();
                StopAllCoroutines();
                ScLoadManager.Instance.LoadSceneAsync("ScMain");

                break;
        }
        
    }

    #region RecordMode

    void ClickRemoveOne()
    {
        AudioManager.Instance.PlayAudio(sfxType.emptyInput);

        if (JsonManager.Instance.curRecordingPattern.Length > 0)
        {
            JsonManager.Instance.curRecordingPattern = JsonManager.Instance.curRecordingPattern.Remove(JsonManager.Instance.curRecordingPattern.Length - 1);
        }

        ArrangeRecordNote();
    }

    void ClickClear()
    {
        AudioManager.Instance.PlayAudio(sfxType.cancel);

        JsonManager.Instance.RecordReset();

        ArrangeRecordNote();
    }

    void ClickSave(bool isExit)
    {
        if (JsonManager.Instance.curRecordingPattern == string.Empty)
        {
            return;
        }

        AudioManager.Instance.PlayDon();

        JsonManager.Instance.SaveToJson();

        if(isExit == true)
        {
            ScLoadManager.Instance.LoadSceneAsync("ScRecord");
        }
        else
        {
            ArrangeRecordNote();
        }
    }

    void ArrangeRecordNote()
    {
        int i = 0;

        for(; i < JsonManager.Instance.curRecordingPattern.Length; i++)
        {
            _recordingNoteImgs[i].gameObject.SetActive(true);

            switch(JsonManager.Instance.curRecordingPattern[i])
            {
                case 'd':

                    _recordingNoteImgs[i].sprite = NoteManager.Instance.donSpr;

                    break;

                case 'k':

                    _recordingNoteImgs[i].sprite = NoteManager.Instance.katSpr;

                    break;
            }
        }

        for(; i < GameManager.pattern_max; i++)
        {
            _recordingNoteImgs[i].gameObject.SetActive(false);
        }
    }

    #endregion

    #region TrainingMode

    IEnumerator CoShoot()
    {
        int shootIndex = 0;

        int patternIndex = 0;

        while(true)
        {
            string curPattern = ReturnRandomPattern();

            if(curPattern == null)
            {
                curPattern = GameManager.Instance.playPatterns[patternIndex];

                patternIndex = GameManager.Instance.ReturnValidIndex(++patternIndex, GameManager.Instance.playPatterns.Count);
            }

            foreach(var curNote in curPattern)
            {
                shootIndex = GameManager.Instance.ReturnValidIndex(++shootIndex, notePool.transform.childCount - curPattern.Length);

                GameObject curNoteObj = notePool.transform.GetChild(shootIndex).gameObject;

                noteType curNoteType = noteType.don;

                switch(curNote)
                {
                    case 'd':

                        _noteImgs[shootIndex].sprite = NoteManager.Instance.donSpr;

                        break;

                    case 'k':

                        _noteImgs[shootIndex].sprite = NoteManager.Instance.katSpr;

                        curNoteType = noteType.kat;

                        break;
                }
                
                curNoteObj.SetActive(true);
                curNoteObj.transform.position = shooter.transform.position;

                noteInfo curnoteInfo = new noteInfo(curNoteObj, curNoteType, 400f / bpm, Time.time);

                _activeNotes.Add(curnoteInfo);

                StartCoroutine(DelayDeleteMissNote(curnoteInfo));

                yield return new WaitForSeconds(60f / bpm / 4f);
            }

            yield return new WaitForSeconds(ReturnTerm());
        }
    }

    float ReturnTerm()
    {
        switch(PlayerPrefs.GetInt("cycleSpeed"))
        {
            // 느리다쿵 - 8번 기다림
            case 0:

                return 60f / bpm * 2f;

            // 적당하다쿵 - 4번 기다림
            case 1:

                return 60f / bpm;

            // 빠르다쿵 - 1번 기다림
            case 2:

                return 60f / bpm / 4f;

            // 끝이없다쿵 - 기다림 없음
            case 3:
            default:

                return 0f;
        }
    }

    /// <summary>
    /// 랜덤인 경우 랜덤 패턴 길이를 리턴하고, 아닐 경우 0 리턴
    /// </summary>
    string ReturnRandomPattern()
    {
        if(GameManager.Instance.curPlayType == playType.recordTraining)
        {
            switch (PlayerPrefs.GetInt("isRandomIncluded"))
            {
                // 랜덤 X
                case 0:
                    return null;

                // 변덕
                case 1:

                    if (GameManager.Instance.ReturnProbability(25) == false)
                    {
                        return null;
                    }
                    else
                    {
                        break;
                    }

                //대충
                case 2:

                    if (GameManager.Instance.ReturnProbability(50) == false)
                    {
                        return null;
                    }
                    else
                    {
                        break;
                    }
            }
        }

        int randomPatternLensIndex = GameManager.Instance.ReturnRandomNumber(_randomPatternLens.Count);

        int randomPatternLen = _randomPatternLens[randomPatternLensIndex];

        string randomPattern = string.Empty;

        bool isDon;

        switch(PlayerPrefs.GetInt("useOnlyRecordPattern"))
        {
            // 무작위 패턴 생성
            case 0:

                for (int i = 0; i < randomPatternLen; i++)
                {
                    isDon = GameManager.Instance.ReturnProbability(50);

                    if (isDon)
                    {
                        randomPattern += 'd';
                    }
                    else
                    {
                        randomPattern += 'k';
                    }
                }

                break;

            // 기록 패턴에서 무작위로 선정
            case 1:

                if(_patternPool.Count == 0)
                {
                    int index = GameManager.Instance.ReturnRandomNumber(JsonManager.Instance.LoadJsonData().patterns.Count);

                    randomPattern = JsonManager.Instance.LoadPattern(index);
                }
                else
                {
                    int index = GameManager.Instance.ReturnRandomNumber(_patternPool.Count);

                    randomPattern = _patternPool[index];
                }

                break;
        }        

        return randomPattern;
    }

    IEnumerator DelayDeleteMissNote(noteInfo targetNoteInfo)
    {
        yield return new WaitForSeconds(targetNoteInfo.timeToChecker + 0.108f);

        if(_activeNotes.Contains(targetNoteInfo))
        {
            TimingTextDisplay(100f);

            _activeNotes.Remove(targetNoteInfo);

            yield return new WaitForSeconds(1f);

            targetNoteInfo.noteObj.SetActive(false);
        }
    }

    void TimingTextDisplay(float curNoteTiming)
    {
        foreach(var text in textPool)
        {
            text.SetActive(false);
        }

        GameObject activeTextObj = null;

        if(curNoteTiming < timing_perfect)
        {
            activeTextObj = textPool[(int)timingType.perfect];

            _isBlackBlurActive = false;
        }
        else if (curNoteTiming < timing_good)
        {
            activeTextObj = textPool[(int)timingType.good];

            _isBlackBlurActive = false;
        }
        else
        {
            activeTextObj = textPool[(int)timingType.bad];
            _isBlackBlurActive = true;
        }

        activeTextObj.SetActive(true);

        badUpperBgBlur.SetActive(_isBlackBlurActive);
        
        CheckGaugeAndBoom(curNoteTiming);
    }

    void CheckTiming(noteType inputType)
    {
        if(_activeNotes.Count == 0)
        {
            return;
        }

        if(inputType != _activeNotes[0].noteType)
        {
            return;
        }

        if(_lastInputDirection == btnClickType.right)
        {
            _lastInputDirection = btnClickType.left;
        }
        else
        {
            _lastInputDirection = btnClickType.right;
        }

        // 현재 시각 - 발사 시각 - 목표 도달 시간, 즉 목표 도달 시간과 얼만큼의 시간 차이가 났는지 리턴
        float curNoteTiming = Mathf.Abs(Time.time - _activeNotes[0].timeOfShoot - _activeNotes[0].timeToChecker + customOffset);

        if(curNoteTiming >= timing_bad)
        {
            return;
        }

        TimingTextDisplay(curNoteTiming);

        _activeNotes[0].noteObj.SetActive(false);

        _activeNotes.RemoveAt(0);
    }

    void CheckGaugeAndBoom(float curNoteTiming)
    {
        if(curNoteTiming < timing_perfect)
        {
            gauge.fillAmount += 0.02f;

            perfectBoom.SetActive(false);
            perfectBoom.SetActive(true);
        }
        else if(curNoteTiming < timing_good)
        {
            goodBoom.SetActive(false);
            goodBoom.SetActive(true);
        }
        else
        {
            gauge.fillAmount -= 0.06f;
        }
    }

    #endregion
}