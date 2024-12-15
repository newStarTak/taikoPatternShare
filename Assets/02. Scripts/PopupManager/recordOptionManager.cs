using UnityEngine;
using UnityEngine.UI;

public class recordOptionManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button addNewBtn;
    public Button backBtn;

    void Awake()
    {
        InitBtnListener();
    }

    void InitBtnListener()
    {
        addNewBtn.onClick.AddListener(() => ClickAddNew());
        backBtn.onClick.AddListener(() => ClickBack());
    }

    void ClickAddNew()
    {
        GameManager.Instance.curPlayType = playType.record;

        ScLoadManager.Instance.LoadSceneAsync("ScPlay");
    }

    void ClickBack()
    {
        AudioManager.Instance.PlayAudio(sfxType.cancel);

        gameObject.SetActive(false);
    }
}
