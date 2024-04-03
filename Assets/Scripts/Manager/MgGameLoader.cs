using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameLoad
{
    New, Load
}

public enum LoadMenuEnum
{
    New, Continue, Quit, Setting
}

public class MgGameLoader : MgGeneric<MgGameLoader>
{ 
    [SerializeField] private GameLoad m_gameLoad;
    [SerializeField] private GameObject m_loadScene;

    public void MasterDataLoad()
    {
        //�ε��� �ڷḦ ���� diction ��, �ε��� ���̺��� ��Ʈ�� ������ ����
        //�� ������ �ε�
        //ĳ�� ������ ������ �ε�
        //�׼� ������ ������ �ε� �� �� ��� ������ ������ �ε�
    }

    public void GameLoadButton(LoadMenuEnum _button)
    {
        switch (_button)
        {
            case LoadMenuEnum.New:
                m_gameLoad = GameLoad.New;
                LoadGame();
                break;
            case LoadMenuEnum.Continue:
                m_gameLoad = GameLoad.Load;
                LoadGame();
                break;
            case LoadMenuEnum.Quit:
                Debug.Log("���� ����");
                break;
            case LoadMenuEnum.Setting:
                Debug.Log("ȯ�� ����â �ѱ�");
                break;
        }

    }

    private void InitialSystemSetting()
    {
 
    }

    private void IntroScene()
    {
        Camera.main.orthographicSize = 3.7f;
  
        m_loadScene.SetActive(true);
        SystemPause.g_instance.Pause(PauseReason.Intro);
    }

    public void SaveGame()
    {
        DBToJson.SaveCharToken(MgToken.GetInstance().GetNpcPlayerList().ToArray(), GameLoad.Load);
    }

    private void LoadGame()
    {
        SystemLoading.g_instance.PlayLoadingScene();

        SystemPause.g_instance.Play(PauseReason.Intro);
        Camera.main.orthographicSize = 4;
        m_loadScene.SetActive(false);

    }


    #region Get Set

    public GameLoad GetLoadMode()
    {
        return m_gameLoad;
    }
    #endregion
}

public class SaveData
{
    
}
