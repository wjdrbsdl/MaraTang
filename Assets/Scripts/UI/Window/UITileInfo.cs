using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITileInfo : UIBase
{
    #region 변수
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private TMP_Text m_statText; //토지 스텟 표기 
    [SerializeField]
    private TMP_Text m_placeText; //장소 이름 표기 
    [SerializeField]
    private TMP_Text m_workText; //토지 스텟 표기 
    [SerializeField]
    private Transform m_LaborSlotBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private LaborSlot m_LaborSlotSample;
    [SerializeField]
    private LaborSlot[] m_LaborSlots;

    [SerializeField]
    private Transform m_requestBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private LaborRequestBtn m_LaborReqeustBtnSample;
    [SerializeField]
    private LaborRequestBtn[] m_LaborRequestBtns;


    [SerializeField]
    private Transform m_placeBox; //내부장소 버튼 담을 포지션
    [SerializeField]
    private BtnPlace m_placeButtonSample;
    [SerializeField]
    private BtnPlace[] m_placeButtones;

    [SerializeField]
    private Transform m_buildBox; //외부건설 버튼 담을 포지션
    [SerializeField]
    private BtnBuild m_buildButtonSample;
    [SerializeField]
    private BtnBuild[] m_buildButtones;

    [SerializeField]
    private BtnInputResource m_putButton; //토지 점령 버튼
    [SerializeField]
    private BtnOccupy m_occupyButton; //토지 점령 버튼
    [SerializeField]
    private BtnDestroy m_destroyButton; //토지 점령 버튼
    #endregion

    Stack<TileType> m_placeStack = new(); //입장한 로드 
    TileType m_curPlace = TileType.Nomal;
    TokenTile m_curTile = null;

    #region 정보 세팅 호출
    public void SetTileInfo(TokenTile _tile, TileType _tileType)
    {
        UISwitch(true);
        m_curTile = _tile;
        m_curPlace = _tileType;
        m_placeText.text = _tileType.ToString();
        PlayerManager.GetInstance().SetHeroPlace(_tileType);
        MakeLaborSlots();
        MakeLaborRequestBtns();
        //Debug.Log("메인 캐릭 있다 " + inMain);
        ResetUI();
    }

    public void ResetSetPlace()
    {
        SetInPlace();
    }

    public void ResetUI()
    {
        SetTileWork(); //타일에서 수행중인 작업
        SetLaborCoin();
        SetInPlace(); //타일 내부 장소들
        SetOutBuildList(); //타일에서 건설가능한 외부건물들
        SetOccupyButton(); //점령하기버튼 - 이건쓸지 잘
        SetDestroyButton(); //파괴하기 버튼
        SetTileStat(); //해당 타일 정보
    }
    
    private void MakeLaborSlots()
    {
        int placeLaborCount = m_curTile.GetMaxLaborCoinCount();
        //Debug.Log("현재 타일 최대 노동 가능 수 " + placeLaborCount);
        MakeSamplePool<LaborSlot>(ref m_LaborSlots, m_LaborSlotSample.gameObject, placeLaborCount, m_LaborSlotBox);
    }

    private void MakeLaborRequestBtns()
    {
        int placeLaborCount = m_curTile.GetMaxLaborCoinCount();
        //요청 버튼은 항시 켜놓음. 
        MakeSamplePool<LaborRequestBtn>(ref m_LaborRequestBtns, m_LaborReqeustBtnSample.gameObject, placeLaborCount, m_requestBox);
        for (int i = 0; i < m_LaborRequestBtns.Length; i++)
        {
            m_LaborRequestBtns[i].gameObject.SetActive(true);
        }
    }
    #endregion

    public void OnClickTileAction()
    {
        m_curTile.DoInhereceWork(m_curPlace);
    }

    public void OnClickLaborCoin(int _index)
    {
        //여기서 해당 라보슬롯에 있으면 해당 슬롯 반환을 요청하는거
        LaborCoin laborCoin = m_LaborSlots[_index].GetLaborCoin();
        if (laborCoin != null)
        {
            laborCoin.BackCapital();
        }
        else
        {
            //비어있으면 수도에 요청
            TokenTile capital = m_curTile.GetNation().GetCapital();
            LaborCoin capitalLaborCoin = capital.RequestLaborCoin();
            //받은 코인이 있으면 해당 코인을 여기 타일로 일보냄 
            if (capitalLaborCoin != null)
                capitalLaborCoin.SendLaborCoin(m_curTile);
        }
            

       // Debug.Log(_index + "쪽 변환 필요");
        SetLaborCoin();
    }

    #region UI 세팅

    private void SetOutBuildList()
    {
        //해당 장소에서 만들 수 있는 건축물
        int[] buildPlace = MgMasterData.GetInstance().GetTileData((int)m_curPlace).AbleBuildPid.ToArray();
        MakeSamplePool<BtnBuild>(ref m_buildButtones, m_buildButtonSample.gameObject, buildPlace.Length, m_buildBox);
        //버튼 세팅
        SetBuildButtons(m_curTile, buildPlace);
    }

    private void SetBuildButtons(TokenTile _selectedTile, int[] _place)
    {
        //입장 가능한 장소를 버튼으로 만들어서 정렬
        for (int i = 0; i < _place.Length; i++)
        {
            m_buildButtones[i].SetActive(true);
            m_buildButtones[i].SetButton(_selectedTile, (TileType)_place[i], this);
        }
        for (int dontUse = _place.Length; dontUse < m_buildButtones.Length; dontUse++)
        {
            m_buildButtones[dontUse].SetActive(false);
        }
    }

    private void SetTileWork()
    {
        SetPushButton(); //자원투입 - 작업에 필요한 부분이므로 타일워크에서 진행 
        WorkOrder work = m_curTile.GetWorkOrder();
        if (work == null)
        {
            m_workText.text = "진행중인 일 없음";
            return;
        }

        m_workText.text = string.Format("{0} 작업중 {1}/{2}\n턴{3}/{4} ", work.workType.ToString(),
                        (work.OriginWorkGauge-work.RestWorkGauge),work.OriginWorkGauge,
                        (work.OriginNeedTurn - work.RestTurn), work.OriginNeedTurn);    

    }

    private void SetLaborCoin()
    {
        int placeLaborCount = m_curTile.GetMaxLaborCoinCount();
        List<LaborCoin> haveLaborList = m_curTile.GetLaborList();
        
        //보유가능한 코인수 만큼 슬롯 열고
        for (int i = 0; i < placeLaborCount; i++)
        {
            m_LaborSlots[i].gameObject.SetActive(true);
            m_LaborRequestBtns[i].gameObject.SetActive(true);
            //보유 코인만큼 정보 세팅하고
            if (i < haveLaborList.Count)
            {
                m_LaborSlots[i].SetSlot(haveLaborList[i]);
                m_LaborRequestBtns[i].SetSlot(i, true);
            }
            else
            {
                m_LaborSlots[i].ResetCoin();
                m_LaborSlots[i].gameObject.SetActive(false);
                m_LaborRequestBtns[i].SetSlot(i, false);
            }
        }
        //보유가능한 슬롯 이외 슬롯은 닫기
        for (int x = placeLaborCount; x < m_LaborSlots.Length; x++)
        {
            m_LaborSlots[x].gameObject.SetActive(false);
            m_LaborRequestBtns[x].gameObject.SetActive(false);
        }
    }

    private void SetInPlace()
    {
        //해당 장소에서 들어갈 수 있는 장소 
        //조건이 안되있으면 어둠으로 뜬다 
        int[] interiorPlace = MgMasterData.GetInstance().GetTileData((int)m_curPlace).AbleInteriorPid.ToArray();
        MakeSamplePool<BtnPlace>(ref m_placeButtones, m_placeButtonSample.gameObject, interiorPlace.Length, m_placeBox);
        //버튼 세팅
        SetPlaceButtons(m_curTile, interiorPlace);
    }

    private void SetPlaceButtons(TokenTile _selectedTile, int[] _place)
    {
        //입장 가능한 장소를 버튼으로 만들어서 정렬
        for (int i = 0; i < _place.Length; i++)
        {
            m_placeButtones[i].SetActive(true);
            m_placeButtones[i].SetButton(_selectedTile, (TileType)_place[i], this);
        }
        for (int dontUse = _place.Length; dontUse < m_placeButtones.Length; dontUse++)
        {
            m_placeButtones[dontUse].SetActive(false);
        }
    }

    private void SetTileStat()
    {
        TokenTile _tile = m_curTile;
        //현재 땅의 스텟 정보를 보여주기 
        MainResource mainResource = m_curTile.GetMainResource();
        TileType tileType = _tile.GetTileType();
        int NationNum = _tile.GetStat(ETileStat.Nation);
        string tileStat = string.Format("소속 국가 : {0}\n토지 용도 {1}\n 토지 적합도{2}\n좌표 {3},{4}\n노동 코인{5}",
            NationNum, tileType, mainResource, _tile.GetMapIndex()[0], _tile.GetMapIndex()[1], _tile.GetLaborCoinCount());
        m_statText.text = tileStat;

        if (m_curTile.GetWorkOrder() != null)
            m_curTile.GetWorkOrder().NoticeNeedResource();



        if (_tile.GetTileType().Equals(TileType.Capital))
        {
            Nation nation = _tile.GetNation();
            string nationStat = string.Format("국가 번호 : {0}\n보유 자원 \n" +
                "{1}:{2} / {3}:{4} / {5}:{6} / {7}:{8}",
                NationNum,
                (Capital)0, nation.GetResourceAmount((Capital)0),
                (Capital)1, nation.GetResourceAmount((Capital)1),
                (Capital)2, nation.GetResourceAmount((Capital)2),
                (Capital)3, nation.GetResourceAmount((Capital)3));
        
        }
    }

    private void SetOccupyButton()
    {
        TokenTile _tile = m_curTile;
        // 점령 버튼 
        //1.불가능하면 버튼 끄고 종료
        if (GamePlayMaster.GetInstance().RuleBook.AbleOccupy(_tile) == false)
        {
            m_occupyButton.SetActive(false);
            return;
        }
        //2. 가능하면 버튼 활성화
        m_occupyButton.SetActive(true);
        //3. 세팅
        m_occupyButton.SetButton(_tile, this); 
        
    }

    private void SetDestroyButton()
    {
        TokenTile _tile = m_curTile;
        if(_tile.GetTileType() == TileType.Nomal)
        {
            m_destroyButton.SetActive(false);
            return;
        }
        m_destroyButton.SetActive(true);
        m_destroyButton.SetButton(_tile, this);
    }

    private void SetPushButton()
    {
        TokenTile _tile = m_curTile;
        m_putButton.SetButton(_tile, this);
    }
    #endregion

    #region UI OnOff
    public void EnterPlace(TokenTile _tile, TileType _tileType)
    {
        //이미 UI가 켜진 해당 장소에서 내부 장소로 이동시 
        //1. 현재 위치를 스택에 추가 
        m_placeStack.Push(m_curPlace);
        //2. 들어갈 장소로 다시 타일 정보 세팅 
        SetTileInfo(_tile, _tileType);
    }

    public override void ReqeustOff()
    {
        OutPlace();
    }

    public void OutPlace()
    {
        if(m_placeStack.Count == 0)
        {
            //돌아갈 장소가 없으면 uioff
            UISwitch(false);
            return;
        }
        TileType priorPlace = m_placeStack.Pop();
        SetTileInfo(m_curTile, priorPlace);
    }

    public override void OffWindow()
    {
        base.OffWindow();
        PlayerManager.GetInstance().SetHeroPlace(TileType.Nomal);
    }
    #endregion
}
