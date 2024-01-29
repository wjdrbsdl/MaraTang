using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MgGeneric<SoundManager>
{
    public enum EfxList
    {
        Choose, Cancle, Check
    }
    [SerializeField]
    AudioClip[] m_efxSoundClips;
    [SerializeField] AudioSource[] m_efxSpeaker;

    public enum BgmList
    {
        Base
    }
    [SerializeField]
    AudioClip[] m_bgmSoundClips;
    [SerializeField] AudioSource m_bgmSpeaker;

    public override void InitiSet()
    {
        base.InitiSet();
        g_instance = this;
        PlayBGM(BgmList.Base);
    }

    public void PlayBGM(BgmList _bgm)
    {
        m_bgmSpeaker.clip = m_bgmSoundClips[(int)_bgm];
        m_bgmSpeaker.Play();
    }
    
    public void PlayEfx(EfxList _efx)
    {
        AudioSource playSpeaker = m_efxSpeaker[(int)_efx];
        playSpeaker.clip = m_bgmSoundClips[(int)_efx];
        playSpeaker.Play();
        
        //����Ŀ n���ΰ� ��ȯ �ʿ�
        //�켱���� ���� �԰ų� �����ų�
        //�켱���� ���� ���� �Ҹ� ��ü
    }

    public void PlayEfx(AudioClip _clip)
    {
        AudioSource playSpeaker = m_efxSpeaker[0];
        playSpeaker.clip = _clip;
        playSpeaker.Play();
    }

    #region ��������
    public void ReviseEfxValue(float _vlaue)
    {

    }
    public void ReviseSfxValue(float _vlaue)
    {

    }
    #endregion

    public void ResetSfx()
    {
        for (int i = 0; i < m_efxSpeaker.Length; i++)
        {
            m_efxSpeaker[i].Stop();
        }
    }
}
