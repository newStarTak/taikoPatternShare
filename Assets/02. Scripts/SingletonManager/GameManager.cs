using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : SingletonData<GameManager>
{
    public const int pattern_max = 8;

    public fadeType curFadeType;

    public playType curPlayType;

    #region Dictionarys

    public Dictionary<int, string> cycleDict = new Dictionary<int, string>
    {
        {0, "느리다쿵"},
        {1, "적당하다쿵"},
        {2, "빠르다쿵"},
        {3, "끝이없다쿵"},
    };

    public Dictionary<int, string> dualHandDict = new Dictionary<int, string>
    {
        {0, "하지않겠쿵"},
        {1, "해보겠다쿵"}
    };

    public Dictionary<int, string> randomDict = new Dictionary<int, string>
    {
        {0, "넣지말라쿵"},
        {1, "변덕"},
        {2, "대충"}
    };

    public Dictionary<int, string> onlyRecordDict = new Dictionary<int, string>
    {
        {0, "아니다쿵"},
        {1, "그러자쿵"}
    };

    #endregion

    
    public List<string> playPatterns = new List<string>();

    public int curPatternIndex;


    #region frequentlyUsed Func

    /// <param name="index">범위 내에 유효한지 검사할 index입니다.</param>
    /// <param name="max">index가 가질 수 있는 끝값입니다. (길이 - 1)</param>
    /// <param name="min">index가 가질 수 있는 시작값입니다. (대체로 0이지만 예외의 경우 인자 전달)</param>
    /// <returns>유효하다면 전달된 index 그대로, min(0) 미만 값이라면 max, max 이상 값이라면 min(0) 리턴</returns>
    public int ReturnValidIndex(int index, int maxExclusive, int minInclusive = 0)
    {
        if(index < 0)
        {
            return maxExclusive - 1;
        }

        if(index >= maxExclusive)
        {
            return minInclusive;
        }

        return index;
    }

    /// <summary>
    /// 0% ~ 100% 사이의 정수로만 이루어진 확률값에 대해 계산
    /// </summary>
    /// <param name="percent">결과를 얻고자 하는 확률값</param>
    public bool ReturnProbability(int percent)
    {
        System.Random rand = new System.Random();

        int randomVal = rand.Next(0, 100);

        return Mathf.Clamp(percent, 0, 100) > randomVal;
    }

    public int ReturnRandomNumber(int maxExclusive, int minInclusive = 0)
    {
        System.Random rand = new System.Random();

        return rand.Next(minInclusive, maxExclusive);
    }

    #endregion

    public void AddToPlayPattern(int addPatternIndex)
    {
        playPatterns.Add(JsonManager.Instance.LoadPattern(addPatternIndex));
    }

    public void AddToPlayPattern(string addPattern)
    {
        playPatterns.Add(addPattern);
    }

    public void RemoveFromPlayPattern(int removePatternIndex)
    {
        var targetPattern = JsonManager.Instance.LoadPattern(removePatternIndex);

        if(playPatterns.Contains(targetPattern))
        {
            playPatterns.Remove(targetPattern);
        }
    }

    public void RemoveFromPlayPattern(string removePattern)
    {
        if(playPatterns.Contains(removePattern))
        {
            playPatterns.Remove(removePattern);
        }
    }

    public void ClearPlayPattern()
    {
        playPatterns.Clear();
    }
}