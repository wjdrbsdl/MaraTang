using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGame : MonoBehaviour
{
    //인게임에서 게임의 시작(로드) 저장 흐름을 제어.

    #region 매니저
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
        //데이터 파트 
        SetDataPart();
        //유아이 파트 
        SetUIPart();
        //게임 시작
        PlayGame();
    }

    #region 데이터 세팅 부분
    private void SetDataPart()
    {
        //1. 매니저 준비
        InitiDataManager();
        //2. 데이터 로드
        LoadMasterData();
        //3. 게임 플레이 세팅
        InitiGameSetting(); //여기선 GO영역도 들어감.
    }

    //매니저들 초기화 - 데이터 파트 부터 진행. 
    private void InitiDataManager()
    {
        //아직 매니저 초기셋팅에 순서는 중요치 않음.
        m_tokenManager.InitiSet();
        m_gamePlayMaster.InitiSet();
        m_playerManager.InitiSet();
        m_soundManager.InitiSet();
        m_capitalManager.InitiSet();
    }
   
    private void LoadMasterData()
    {
        m_loadManager.MasterDataLoad(); //토큰에 관한 마스터 데이터 로드. 
    }

    private void InitiGameSetting()
    {
        //0. 기본적인 토큰의 원본을 만든다 -> 이후 생성되는 토큰은 오리지널을 복사하면서 이미지 같은건 하나의 자료를 참조해서 쓰는 방식으로 
        //1. 맵을 만든다 - 노이즈맵과 거기에 해당하는 타일 토큰을 생성하면서 
        m_tokenManager.MakeMap();
        //2. 캐릭 토큰을 만든다 - 맵의 환경에 맞게 지형적으로 캐릭 토큰 생성
        // m_tokenManager.MakePlayerToken();
        m_tokenManager.MakeMonsterToken();

   
    }
    #endregion

    #region UI 세팅 부분 - 데이터의 표현 및 플레이어의 입력만을 담당.
    private void SetUIPart()
    {
        //1. 매니저
        InitiUIManager();
    }
    private void InitiUIManager()
    {
  
    }


    #endregion

    private void PlayGame()
    {
        //4. 게임플레이가 플레이 진행
        m_gamePlayMaster.FirstStart();
    }

}
