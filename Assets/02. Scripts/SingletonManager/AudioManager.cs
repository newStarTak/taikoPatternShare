using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonObject<AudioManager>
{
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;

    public AudioSource bgmAuSrc;
    public List<AudioSource> sfxAuSrcs;
    private int _curChIndex;

    protected override void Awake()
    {
        base.Awake();

        InitVolume();
    }

    #region PlayBGM Func

    public void PlayBgm(bgmType audio)
    {
        if((int)audio >= bgmClips.Count)
        {
            return;
        }

        bgmAuSrc.clip = bgmClips[(int)audio];

        bgmAuSrc.Play();
    }

    public void PauseBgm()
    {
        bgmAuSrc.Pause();
    }

    public void UnpauseBgm()
    {
        bgmAuSrc.UnPause();
    }

    #endregion

    #region PlaySFX Func

    public void PlayDon()
    {
        _curChIndex = GameManager.Instance.ReturnValidIndex(++_curChIndex, sfxAuSrcs.Count);

        sfxAuSrcs[_curChIndex].clip = sfxClips[(int)sfxType.don];

        sfxAuSrcs[_curChIndex].Play();
    }

    public void PlayKat()
    {
        _curChIndex = GameManager.Instance.ReturnValidIndex(++_curChIndex, sfxAuSrcs.Count);

        sfxAuSrcs[_curChIndex].clip = sfxClips[(int)sfxType.kat];

        sfxAuSrcs[_curChIndex].Play();
    }
    public void PlayAudio(sfxType sound)
    {
        if((int)sound >= sfxClips.Count)
        {
            return;
        }

        _curChIndex = GameManager.Instance.ReturnValidIndex(++_curChIndex, sfxAuSrcs.Count);

        sfxAuSrcs[_curChIndex].clip = sfxClips[(int)sound];

        sfxAuSrcs[_curChIndex].Play();
    }

    public void PlayAudio(sfxType startPoint, sfxType endPoint)
    {
        if((int)startPoint >= sfxClips.Count)
        {
            return;
        }

        _curChIndex = GameManager.Instance.ReturnValidIndex(++_curChIndex, sfxAuSrcs.Count);

        int rand = Random.Range((int)startPoint, (int)endPoint + 1);
        sfxAuSrcs[_curChIndex].clip = sfxClips[rand];

        sfxAuSrcs[_curChIndex].Play();
    }

    #endregion

    #region SetVolume Func

    void InitVolume()
    {
        float bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");

        SetBgmVolume(bgmVolume);
        SetSfxVolume(sfxVolume);
    }

    public void SetBgmVolume(float volumeValue)
    {
        PlayerPrefs.SetFloat("bgmVolume", volumeValue);

        bgmAuSrc.volume = Mathf.Clamp01(volumeValue);
    }

    public void SetSfxVolume(float volumeValue)
    {
        PlayerPrefs.SetFloat("sfxVolume", volumeValue);

        foreach(var sfxAuSrc in sfxAuSrcs)
        {
            sfxAuSrc.volume = Mathf.Clamp01(volumeValue);
        }
    }

    #endregion
}
