using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum TileEffectEnum
{
    None, Money, Stat, Tool, UIOpen
}

public class TileTypeData
{
    public int TypePID;
    public string PlaceName;
    public TileEffectEnum effectType = TileEffectEnum.None;
    public TItemListData EffectData;
    public bool IsAutoReady = false; //자동준비를 할것인가 - 사전준비가 필요한 작업안에서만 결정
    public bool NeedReadyWork = false; //사전준비가 필요한 작업인가
    public bool ImmediatelyEffect = false; //때 되면 즉발 효과 인가 - 단순 효과인경우만 적용가능.
    public bool NeedCommand = false; //추가 조작이 필요한 효과인가
    //건설
    public int NeedLaborTurn;
    public int NeedLaborAmount;
    public int[] NeedTiles;
    public TItemListData BuildCostData;
    public int BuildNeedLaborValue;
    public bool IsInterior = false; //해당 장소는 인테리어 타입인지
    //파괴
    public int NeedDestroyTurn = 1; //보통 1
    public TItemListData DestroyCostData;

    public int[] TileStat; //해당 장소의 내구도 같은것들
    public List<int> AbleBuildPid = new(); //진화 가능한 외부 건물
    public List<int> AbleInteriorPid = new(); //지을 수 있는 내부 건물


    public TileTypeData(string[] _parsingData, List<int[]> _matchCode)
    {
        //TypePID = int.Parse(_parsingData[0]);
        int tileTypeIndex = 0;
        if (System.Enum.TryParse(typeof(TileType), _parsingData[tileTypeIndex], out object parseTileType))
            TypePID = (int)parseTileType;
        PlaceName = _parsingData[1];

        //스텟들 파싱
        TileStat = new int[System.Enum.GetValues(typeof(ETileStat)).Length];
        GameUtil.InputMatchValue(ref TileStat, _matchCode, _parsingData);
        //추가 설정이 필요한것들 진행
        TileStat[(int)ETileStat.CurDurability] = TileStat[(int)ETileStat.MaxDurability];

        int effectTypeIndex = 2;
        if (System.Enum.TryParse(typeof(TileEffectEnum), _parsingData[effectTypeIndex], out object ffectType))
            effectType = (TileEffectEnum)ffectType;
        //  Debug.Log(ffectType + " " + effectType);

        int effectIndex = effectTypeIndex + 1;
        if (_parsingData.Length > effectIndex)
        {
            EffectData = GameUtil.ParseCostDataArray(_parsingData, effectIndex);
        }

        int autoActReadyIndex = effectIndex + 1;
        if (_parsingData[autoActReadyIndex] == "T")
        {
            IsAutoReady = true;
        }

        int needReadyIndex = autoActReadyIndex + 1;
        if (_parsingData[needReadyIndex] == "T")
        {
            NeedReadyWork = true;
        }

        int autoEffectIndex = needReadyIndex += 1;
        if (_parsingData[autoEffectIndex] == "T")
        {
            ImmediatelyEffect = true;
        }

        int needCommandIdx = autoEffectIndex + 1;
        if (_parsingData[needCommandIdx] == "T")
        {
            NeedCommand = true;
        }

        int needLaborTurn = needCommandIdx + 1;
        NeedLaborTurn = int.Parse(_parsingData[needLaborTurn]);

        int needLaborIndex = needLaborTurn += 1;
        NeedLaborAmount = int.Parse(_parsingData[needLaborIndex]);

        int interiorTypeIdx = needLaborIndex + 1;
        if (_parsingData[interiorTypeIdx] == "T")
        {
            IsInterior = true;
        }

        int needTileTypeIdx = interiorTypeIdx + 1; //해당 건물(장소)를 짓는데 필요한 장소
        string[] needTiles = _parsingData[needTileTypeIdx].Split(",");
        NeedTiles = new int[needTiles.Length];
        for (int i = 0; i < needTiles.Length; i++)
        {
            if (System.Enum.TryParse(typeof(TileType), needTiles[i], out object parseNeedTileType))
                NeedTiles[i] = (int)parseNeedTileType;
            else
            {
                Debug.Log("정의 되지 않은 재료는 " + (TileType)NeedTiles[i]);
            }
            // Debug.Log(parseNeedTileType);
        }



        int buildCostIdx = needTileTypeIdx + 1;
        BuildCostData = GameUtil.ParseCostDataArray(_parsingData, buildCostIdx);

        int laborValueIdx = buildCostIdx += 1;
        if (_parsingData.Length > laborValueIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildNeedLaborValue = int.Parse(_parsingData[laborValueIdx]);
            // Debug.Log("짓는데 필요한 노동량 " + BuildNeedLaborValue);
        }

        int destroyTurnIdx = laborValueIdx + 1;
        if (int.TryParse(_parsingData[destroyTurnIdx], out int destroyTurn))
        {
            NeedDestroyTurn = destroyTurn;
        }

        int destroyCostIdx = destroyTurnIdx + 1;
        // CostData =  토큰그룹_pid_수량 으로 구성
        DestroyCostData = GameUtil.ParseCostDataArray(_parsingData, destroyCostIdx);

    }
}
