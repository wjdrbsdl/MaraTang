using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMasterData
{
    ContentData, TileType, CharData, CharActionData, EventData, NationTechTree, Conversation, God, GodBless, BlessSynerge,
    Equipment, ChunkContent, CapitalData
}
public struct ParseData
{
    public List<int[]> MatchCode; //enum과 매치되는 인덱스
    public List<string[]> DbValueList; //디비에서 따온 값들

    public ParseData(List<int[]> _matchCode, List<string[]> _dbValues)
    {
        MatchCode = _matchCode;
        DbValueList = _dbValues;
    }

    public void NullCheck()
    {
        if(MatchCode == null)
        {
            MatchCode = new();
            DbValueList = new();
        }
    }
}

public class MgParsing : MgGeneric<MgParsing>
{
    private static string docuIDes =  "19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg";
    private string[] sheetIDes = { "1960523724", "1971334673", "1134768741",
                                    "1603700320","218824529","1858334671" ,
                                    "1780035322", "242617216", "1085432251",
                                    "1134239208", "539688768", "1379552483"};
    private EMasterData[] dbId = { EMasterData.ContentData, EMasterData.TileType, EMasterData.CharData, 
                                   EMasterData.CharActionData, EMasterData.NationTechTree, EMasterData.Conversation,
                                   EMasterData.God, EMasterData.GodBless, EMasterData.BlessSynerge,
                                    EMasterData.Equipment, EMasterData.ChunkContent, EMasterData.CapitalData};
    //stat[] 를 사용하는경우 db에 enum값 MatchValue를 만들기 위해 어떤 enum을 쓰는지 
    private System.Enum[] matchTypes = { null, ETileStat.Nation, CharStat.CurActionCount, 
                                    CharActionStat.CoolTime, null, ConversationStat.Pid,
                                   null, null, null,
                                   null, null, null};
    private Dictionary<EMasterData, ParseData> dbContainer = new(); //파싱한값을 그냥 갖고만 있는상태 - 사용하는곳에서 다시 가공 필요. 
 
    public override void ManageInitiSet()
    {
        base.ManageInitiSet();
        ParseSheetData();
    }
    public ParseData GetMasterData(EMasterData _dataId)
    {
        dbContainer.TryGetValue(_dataId, out ParseData _parseData);
        _parseData.NullCheck();
        return _parseData;
    }

    private void ParseSheetData()
    {
        StartCoroutine(
            GameUtil.GetSheetDataCo(docuIDes, sheetIDes,
         delegate { MgGameIniti.GetInstance().PlayMgInitiWorkStep("파싱완료"); },
         ClassfyDataBase)
            );
    }

    private void ClassfyDataBase(bool _successLoad, int _index, string message)
    {
        System.Enum parseEnum = matchTypes[_index];
        //담당 매니저에서 클래스를 만들수있도록 데이터 분류
        if (_successLoad)
        {
            //1. 파싱한 타입 - 수동으로 변수에서 매칭해둔거docuID - parseTypes
            EMasterData parseData = dbId[_index];
            //2. sheetData 행마다 분리
            string[] enterDivde = message.Split('\n'); //엔터 - 행 분리
            //3. 첫번째 행은 enum string값 중 db로 관리할 목록을 칼럼명으로 적어놓은 부분
            string[] dbEnumCode = enterDivde[0].Trim().Split('\t'); //enumCode를 칼럼명으로 - 첫번째 행의 역할
            //4. sheet 열과 현재 enum에서 해당 칼럼명의 인덱스가 다를 수 있으므로 조정
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbEnumCode);
            //5. 나머지 행은 실제 값들.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1행부터 자료 값
            {
                if (enterDivde[i][0].Equals('#'))
                {
                 //   Debug.Log(enterDivde[i] + "첫열이 #으로 시작하는 행은 건너띔");
                    continue;
                }
                    

                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //탭 - 열 분리 
     
               // Debug.Log(parseData + "행 사이즈" + valueDivde.Length);
                dbValueList.Add(valueDivde);
            }
            //6. 파싱코드에 - 인덱스 매칭 코드와 실제 값들을 struct로 묶어서 dctionary에 저장 
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));
            
        }
        else
            Debug.LogWarning(matchTypes[_index]+ "파싱 실패");
    }

 
}
