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
        //로드한 자료를 담을 diction 과, 로드할 테이블을 세트로 보내서 생성
        //맵 마스터 로드
        //캐릭 마스터 데이터 로드
        //액션 마스터 데이터 로드 등 각 모든 마스터 데이터 로드
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
                Debug.Log("게임 종료");
                break;
            case LoadMenuEnum.Setting:
                Debug.Log("환경 설정창 켜기");
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
