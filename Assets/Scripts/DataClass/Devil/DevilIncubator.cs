using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum DevilState
{
    봉인, 유아, 성인, 토벌
}

public class DevilIncubator
{
    private List<TokenChar> m_devilList = new List<TokenChar>();
    private List<int> m_chunkNum = new();
    private List<TokenTile> m_birthTile = new();
    private List<bool> m_contentMeet = new();
    private List<DevilState> m_devilStateList = new();
    private int m_turnTerm = 15; //발생 주기
    private bool m_turnEnough = true;
    private int birthCount = 0;
    public void SetBirthRegion(List<int> _chunkList, List<TokenTile> _tileList)
    {
        m_chunkNum = _chunkList;
        m_birthTile = _tileList;
    }

    public void DiceDevilList(int _diceCount)
    {
        //등장할 악마를 골라서 생성 

        //1. 캐릭 마스터 데이터에서 악마 데이터만 추출하여 복사
        Dictionary<int,TokenChar> charDic = MgMasterData.GetInstance().GetCharDic();
        List<int> devilPid = new List<int>();
        foreach(KeyValuePair<int, TokenChar> tokenChar in charDic)
        {
            if (tokenChar.Value.GetCharType().Equals(CharType.Devil))
            {
                devilPid.Add(tokenChar.Key); //pid만 추출
            }
        }

        //2. 등장 악마 랜덤 뽑기
        List<int> randomDevilIdx = GameUtil.GetRandomNum(devilPid.Count, _diceCount);

        //3. 뽑힌 악마 pid로 tokenObj 생성해서 추가
        for (int i = 0; i < randomDevilIdx.Count; i++)
        {
            //캐릭obj 생성 및 tokenChar 할당
            TokenChar madeChar = MgToken.GetInstance().MakeCharToken(devilPid[randomDevilIdx[i]]);
            //악마 리스트에 추가
            m_devilList.Add(madeChar);
            m_contentMeet.Add(false); //조건 만족 false로 초기화
            m_devilStateList.Add(DevilState.봉인);
            //Debug.Log(m_devilList[i].GetItemName() + "악마 확정");
        }
    }

    public void ChangeWorldContent(int _pid, bool _result)
    {
        //해당 퀘스트 컨텐츠 결과값이 들어왔을 때 영향을 받는 악마에게 조건 적용
    }

    public void ChangeWorldTurn(int _currentTurn)
    {
        if(_currentTurn%m_turnTerm == 0)
        {
            m_turnEnough = true;
            Debug.Log("때 조건 충족");
        }
    }

    public void BirthDevil()
    {
        Debug.LogWarning("악마 부활 조건 체크 후 부활진행");
        //턴 조건 확인
        if (m_turnEnough == false)
            return;

        if(birthCount == m_devilList.Count)
        {
            Debug.Log("악마 모두 부활");
            return;
        }

        //개별 조건 확인해서 되는 애 중에 랜덤으로 생성
        int randomDevil = Random.Range(0, m_devilList.Count);
        //악마 수대로 랜덤수 뽑기 - 기존에 뽑은 애는 넘겨야 하므로 
        List<int> randomOrder = GameUtil.GetRandomNum(m_devilList.Count, m_devilList.Count);

        //이미만들어져있는 애를 엠지토큰 스폰으로 장소에 호출, 및 mgtoken의 charList에 추가 
        for (int i = 0; i < randomOrder.Count; i++)
        {
            int index = randomOrder[i]; //뽑을 악마 인덱스
            //이미 뽑은 애는 차례 넘김
            if (m_devilStateList[index] != DevilState.봉인)
                continue;

            MgToken.GetInstance().SpawnCharactor(m_devilList[index], m_birthTile[index].GetMapIndex());
            m_devilStateList[index] = DevilState.유아;
            birthCount += 1;
            Debug.Log(m_devilList[index].GetItemName() + "악마 부활");
            //뽑았으면 종료
            break;
        }
        
    }
}

