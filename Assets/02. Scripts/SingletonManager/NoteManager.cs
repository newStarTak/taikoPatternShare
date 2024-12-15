using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 수련 모드(trainingMode)에서 사용하는 노트에 대한 정보.
/// </summary>
public class noteInfo
{
    /// <summary>
    /// 해당 noteInfo를 가지는 노트 게임오브젝트
    /// </summary>
    public GameObject noteObj;

    /// <summary>
    /// 동인지 캇인지 노트 타입
    /// </summary>
    public noteType noteType;

    /// <summary>
    /// Checker(판정 원)까지 가는데 걸리는 시간
    /// </summary>
    public float timeToChecker;

    /// <summary>
    /// 발사 당시 시간(Time.time)
    /// </summary>
    public float timeOfShoot;

    public noteInfo(GameObject curNoteObj, noteType curNoteType, float pTimeToChecker, float pTimeOfShoot)
    {
        noteObj = curNoteObj;
        noteType = curNoteType;
        timeToChecker = pTimeToChecker;
        timeOfShoot = pTimeOfShoot;
    }
}

public class NoteManager : SingletonObject<NoteManager>
{
    [Header("Note Sprite")]
    public Sprite donSpr;
    public Sprite katSpr;

    public GameObject GenerateNote(noteType pNoteType, Transform parentTr, bool isInGame = false)
    {
        GameObject curNote = new GameObject("note");

        // 부모 설정 시 자식 오브젝트가 부모의 스케일을 따라가게 되어 크기가 비정상적인 경우 발생
        // 두 번째 인자가 부모의 스케일을 따라갈지(true) 무시할지(false) 결정, 따라서 false로 설정
        curNote.transform.SetParent(parentTr, false);

        Image curNoteImg = curNote.AddComponent<Image>();

        // 노트 타입에 따라 이미지 스프라이트(동, 캇) 결정
        switch(pNoteType)
        {
            case noteType.don:

                curNoteImg.sprite = donSpr;

                break;

            case noteType.kat:

                curNoteImg.sprite = katSpr;

                break;
        }

        // 게임 모드 내에 사용될 이동 노트의 경우 스크립트 추가
        if(isInGame)
        {
            curNote.AddComponent<InGameNoteCtrl>();
        }

        return curNote;
    }
}
