using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class ContentCtrl : MonoBehaviour
{
    public GameObject patternContainer;

    public Button multiBtn;
    public TextMeshProUGUI multiText;

    void Awake()
    {
        InitPattern();
    }

    void InitPattern()
    {
        string _thisPattern = JsonManager.Instance.LoadPattern(GameManager.Instance.curPatternIndex);

        foreach (var curNote in _thisPattern)
        {
            if (curNote == 'd')
            {
                NoteManager.Instance.GenerateNote(noteType.don, patternContainer.transform);
            }
            else if (curNote == 'k')
            {
                NoteManager.Instance.GenerateNote(noteType.kat, patternContainer.transform);
            }
        }
    }
}
