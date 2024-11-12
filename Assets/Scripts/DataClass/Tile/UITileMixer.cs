using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UITileMixer : UIBase, KeyInterceptor
{
    private TileType m_madeTile = TileType.Nomal; //만들려는 타일
    private List<TokenTile> m_inTile = new(); //추가된 타일들
    private int m_nationNum = -1;
    private List<TileType> needTile = new(); //현재 부족한 부분
    private List<TileType> recipe = new(); //최초 필요한 부분
    
    #region 초기화
    public void SetMixInfo(TileType _goalTile, int _nationNum)
    {
        //만들 조합장소와 포함되는 국가 넘버를 세팅
        UISwitch(true);
        m_madeTile = _goalTile;
        m_nationNum = _nationNum;
        //해당 타입을 만들기 위한 재료 정보를 가져와야함 
        SetRecipe(_goalTile);
        SetKeyInteceptor();
    }

    public void SetMixInfo(TileType _goalTile, TokenTile _tile)
    {
        //만들 조합장소와 포함되는 국가 넘버를 세팅
        UISwitch(true);
        m_madeTile = _goalTile;
        m_nationNum = _tile.GetNationNum();
        //해당 타입을 만들기 위한 재료 정보를 가져와야함 
        SetRecipe(_goalTile);
        OnClickTile(_tile);
        SetKeyInteceptor();
    }
 
    private void SetKeyInteceptor()
    {
        SetPreIntecoptor(MgInput.GetInstance().curInterceptor); //이전꺼 받고
        MgInput.GetInstance().SetInterCeptor(this); //새로 세팅
    }

    private void SetRecipe(TileType _goalType)
    {
        int[] needTiles = MgMasterData.GetInstance().GetTileData((int)_goalType).NeedTiles;
        needTile = new();
        recipe = new();
        for (int i = 0; i < needTiles.Length; i++)
        {
            needTile.Add((TileType)needTiles[i]);
            recipe.Add((TileType)needTiles[i]);
        }

    }
    #endregion

    public void OnClickTile(TokenTile _tile)
    {
        //타일 누른걸 캐싱해와야하는데 

        //기존 있던 타일이라면 제외
        if(m_inTile.IndexOf(_tile)!= -1)
        {
            RemoveTile(_tile);
            return;
        }
        int tileNation = _tile.GetNationNum();
        if (tileNation != m_nationNum)
        {
            //국가영토 아닌 영토인경우 작용안함
          //  return;
        }

        if(_tile.IsBuilding() == true)
        {
            //다른 공사중이면 패스 
            return;
        }
        //재료에 필요한건지 체크
        TileType addType = _tile.GetTileType();
        if (InNeed(addType) == false)
        {
            //필요없는 재료면 리턴
            return;
        }
        AddTile(_tile);
    }

    #region 재료라인에 추가 제거
    private void AddTile(TokenTile _tile)
    {
        m_inTile.Add(_tile); //리스트에 넣고
        needTile.Remove(_tile.GetTileType()); //필요한 타입에서 제외하고
        Debug.Log(_tile.GetTileType() + "추가 남은 재료 수" + needTile.Count);
        //임시로 자동 만들기 진행
        if (needTile.Count == 0)
            MakeWorkOrder();
    }

    private void RemoveTile(TokenTile _tile)
    {
        m_inTile.Remove(_tile); //리스트에서 빼고
        needTile.Add(_tile.GetTileType()); //필요한 타입에 추가하고
        Debug.Log(_tile.GetTileType() + "제거");
    }

    private bool InNeed(TileType _type)
    {
        for (int i = 0; i < needTile.Count; i++)
        {
            if (needTile[i] == _type)
                return true;
        }
        return false;
    }
    #endregion

    private void MakeWorkOrder()
    {
        Debug.Log("작업서 생성 시작");
        if (needTile.Count != 0)
            return;

        TokenTile targetTile = m_inTile[0]; //첫번째 잇는 애가 직접적으로 바뀔 녀석
        WorkOrder order = new WorkOrder(null, 100, (int)m_madeTile, WorkType.ChangeBuild);
        //다른 타일로 작업시 m_tile이 변경될수있으므로 다른 인스턴스로 생성
        System.Action doneEffect = delegate
        {
           targetTile.CompleteOutBuild(m_madeTile);
        };
        for (int i = 1; i < m_inTile.Count; i++)
        {
            //그다음 타일부터는 종속시키고 상태도 그냥 바로 종속타일로 바꾸면되는데 
            m_inTile[i].ChangePlace(TileType.Child);
            m_inTile[i].SetParentIndex(targetTile); //목적 타일의 좌표값을 패런트로 세팅. 
        }
        order.SetDoneEffect(doneEffect);
        targetTile.RegisterWork(order);
        UISwitch(false);
    }

    #region 창 끄기
    public void ResetInfo()
    {
        needTile = new();
        recipe = new();
        m_inTile = new();
        m_nationNum = -1;
        m_madeTile = TileType.Nomal;
    }

    public override void OffWindow()
    {
        MgInput.GetInstance().SetInterCeptor(preCeptor);
        ResetInfo();
        base.OffWindow();
    }
    #endregion

    #region 키인터셉터
    public void ClickTokenBase(TokenBase _tokenBase)
    {
        TokenType type = _tokenBase.GetTokenType();
        if (type == TokenType.Tile)
            OnClickTile((TokenTile)_tokenBase);
           
    }
    public void DoubleClickTokenBase(TokenBase _tokenBase)
    {
        
    }

    public void PushNumKey(int _keyNum)
    {
    }

    KeyInterceptor preCeptor;
    public void SetPreIntecoptor(KeyInterceptor _ceptor)
    {
        preCeptor = _ceptor;
    }
    #endregion
}
