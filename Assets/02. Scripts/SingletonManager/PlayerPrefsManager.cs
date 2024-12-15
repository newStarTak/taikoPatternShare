using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsManager : SingletonObject<PlayerPrefsManager>
{
    private List<Dictionary<string, object>> defaultPPDicts = new List<Dictionary<string, object>>
    {
        // firstLogin
        new Dictionary<string, object>
        {
            { "isFirstLogin", 0 }
        },

        // audio
        new Dictionary<string, object>
        {
            { "bgmVolume", 0.5f },
            { "sfxVolume", 0.5f }
        },

        // recordOption
        new Dictionary<string, object>
        {
            { "cycleSpeed", 0 },
            { "bpm", 120f },
            { "isDualHand", 0 },
            { "isRandomIncluded", 0 },

            { "pattern_1_included", 0 },
            { "pattern_2_included", 0 },
            { "pattern_3_included", 0 },
            { "pattern_4_included", 0 },
            { "pattern_5_included", 0 },
            { "pattern_6_included", 0 },
            { "pattern_7_included", 0 },
            { "pattern_8_included", 0 },
            { "useOnlyRecordPattern", 0 },
        },

        new Dictionary<string, object>
        {
            {"leftKat", (int)KeyCode.D},
            {"leftDon", (int)KeyCode.F},
            {"rightDon", (int)KeyCode.J},
            {"rightKat", (int)KeyCode.K},
        }
    };

    /// <summary>
    /// 튜토리얼과 같이 최초 접속 시에만 행해야 할 이벤트들이 모두 수행된 후에 해당 함수 호출할 것.
    /// </summary>
    public void InitPlayerPrefs(bool isHardReset = false)
    {
        if(PlayerPrefs.HasKey("isFirstLogin") && isHardReset == false)
        {
            return;
        }

        foreach (var defaultPPDict in defaultPPDicts)
        {
            foreach (var kvp in defaultPPDict)
            {
                switch(kvp.Value)
                {
                    case int intVal:

                        PlayerPrefs.SetInt(kvp.Key, intVal);

                        break;

                    case float floatVal:

                        PlayerPrefs.SetFloat(kvp.Key, floatVal);

                        break;
                }
            }
        }
    }

    public void ResetSelectedPlayerPrefs(ppType selectedPPType)
    {
        foreach (var kvp in defaultPPDicts[(int)selectedPPType])
        {
            switch(kvp.Value)
            {
                case int intVal:

                    PlayerPrefs.SetInt(kvp.Key, intVal);

                    break;

                case float floatVal:

                    PlayerPrefs.SetFloat(kvp.Key, floatVal);

                    break;
            }
        }
    }
}
