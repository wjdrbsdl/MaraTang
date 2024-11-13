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

    public void SaveGame()
    {
        DBToJson.SaveCharToken(MgToken.GetInstance().GetCharList().ToArray(), GameLoad.Load);
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

public class KMP
{
    string origin = "abcdfabcggeaabcde";
    string find = "abcde";

    public void Search()
    {

        int[] pattern = {-1,0,0,0,0};
        //각 일치 수에 따라 패턴수 찾아놓기

        bool isFind = false;
        int originIdx = 0; //오리진에서 틀렸던 자리 
        int correct = 0; //일치한 패턴 수 
        int startIdx = originIdx;
        while (isFind == false)
        {
            for(int oriIdx = startIdx; oriIdx<origin.Length; oriIdx++)
            {
                char oriChar = origin[oriIdx]; //살필 문자
                char findChar = find[correct]; //현재 일치한 숫자 0 번째 부터 문자확인
                originIdx = oriIdx; //검사했던 부분 체크
                Debug.LogFormat("기존idx{0}문자{1} 동일한수{2}문자{3}", oriIdx, oriChar, correct, findChar);
                if(oriChar == findChar)
                {
                    correct += 1;
                    if(correct == find.Length)
                    {
                        Debug.Log(startIdx + "번재에서 일치하는거 찾음");
                        isFind = true;
                        break;
                    }
                    continue;
                }

                //만약 다르다면 해당 포문은 종료하고 다시 시작위치와 경계성을 이동해서 검색시작
                
                break;
            }
            if (isFind == true)
                break;
            int back = pattern[correct]; //경계성 불러오고
            //원래라면 경계선 만큼 이동해서 해당 위치에서 oriIdx를 시작하면됨. 근데 경계 패턴수만큼이니까 
            //originIdx에서부터 correct0을해야하지만 일치된 패턴 back 만큼 당겨서
            correct = back; //일치한 수를 정의하고 
            //2만큼 당겨졌다면 위에서 
            //틀렸던 부분부터 오리진을 돌면서
            //찾으려는 문자의 당겨진 경계선 다음부터 다시 확인이 들어간다. 
            //correct가 0이 되는순간 틀렸던 부분에 find의 0번째가 자리하고, 
            if(correct == -1)
            {
                originIdx += 1;
                correct = 0; 
            }

            if (originIdx +1 >= origin.Length)
            {
                //마지막 기록을 해봤다면 종료 
                Debug.Log("없다");
                break;
            }

            startIdx = originIdx;
            //만약 경계값이 -1 즉 처음부터 틀린곳이라면 강제로 다시 초기화 해서 진행 
        }
    }


}
