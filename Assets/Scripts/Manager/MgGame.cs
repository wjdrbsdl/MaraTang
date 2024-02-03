using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGame : MonoBehaviour
{
    //�ΰ��ӿ��� ������ ����(�ε�) ���� �帧�� ����.

    #region �Ŵ���
    [SerializeField]
    MgToken m_tokenManager;
    [SerializeField]
    MgGameLoader m_loadManager;
    [SerializeField]
    GamePlayMaster m_gamePlayMaster;
    [SerializeField]
    PlayerManager m_playerManager;
    [SerializeField]
    SoundManager m_soundManager;
    [SerializeField]
    CapitalObjManager m_capitalManager;

    #endregion
 
    void Start()
    {
        //������ ��Ʈ 
        SetDataPart();
        //������ ��Ʈ 
        SetUIPart();
        //���� ����
        PlayGame();
    }

    #region ������ ���� �κ�
    private void SetDataPart()
    {
        //1. �Ŵ��� �غ�
        InitiDataManager();
        //2. ������ �ε�
        LoadMasterData();
        //3. ���� �÷��� ����
        InitiGameSetting(); //���⼱ GO������ ��.
    }

    //�Ŵ����� �ʱ�ȭ - ������ ��Ʈ ���� ����. 
    private void InitiDataManager()
    {
        //���� �Ŵ��� �ʱ���ÿ� ������ �߿�ġ ����.
        m_tokenManager.InitiSet();
        m_gamePlayMaster.InitiSet();
        m_playerManager.InitiSet();
        m_soundManager.InitiSet();
        m_capitalManager.InitiSet();
    }
   
    private void LoadMasterData()
    {
        m_loadManager.MasterDataLoad(); //��ū�� ���� ������ ������ �ε�. 
    }

    private void InitiGameSetting()
    {
        //0. �⺻���� ��ū�� ������ ����� -> ���� �����Ǵ� ��ū�� ���������� �����ϸ鼭 �̹��� ������ �ϳ��� �ڷḦ �����ؼ� ���� ������� 
        //1. ���� ����� - ������ʰ� �ű⿡ �ش��ϴ� Ÿ�� ��ū�� �����ϸ鼭 
        m_tokenManager.MakeMap();
        //2. ĳ�� ��ū�� ����� - ���� ȯ�濡 �°� ���������� ĳ�� ��ū ����
        // m_tokenManager.MakePlayerToken();
        m_tokenManager.MakeMonsterToken();

   
    }
    #endregion

    #region UI ���� �κ� - �������� ǥ�� �� �÷��̾��� �Է¸��� ���.
    private void SetUIPart()
    {
        //1. �Ŵ���
        InitiUIManager();
    }
    private void InitiUIManager()
    {
  
    }


    #endregion

    private void PlayGame()
    {
        //4. �����÷��̰� �÷��� ����
        m_gamePlayMaster.FirstStart();
    }

}
