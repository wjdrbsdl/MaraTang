using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgParsing : MgGeneric<MgParsing>
{
    private string[] docuIDes = { "19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg" };
    private string[] sheetIDes = { "0" };
    private System.Enum[] parseTypes = { ActionStat.MaxCountInTurn };
    private Dictionary<string, ParseContainer> dbContainer = new();
    public struct ParseContainer
    {
        public List<int[]> MatchCode; //enum과 매치되는 인덱스
        public List<string[]> DbValueList; //디비에서 따온 값들

        public ParseContainer(List<int[]>_matchCode, List<string[]> _dbValues)
        {
            MatchCode = _matchCode;
            DbValueList = _dbValues;
        }
    }

    public override void InitiSet()
    {
        base.InitiSet();
        ParseSheetData();
    }
    public ParseContainer GetMasterData(System.Enum _enum)
    {
        string parseCode = _enum.GetType().ToString();
        return dbContainer[parseCode];
    }

    private void ParseSheetData()
    {
        StartCoroutine(GameUtil.GetSheetDataCo(docuIDes, sheetIDes, parseTypes,
         delegate { MgGame.GetInstance().DoneInitiDataManager("파싱완료"); },
         ClassfyDataBase));
    }

    private void ClassfyDataBase(bool _successLoad, System.Enum parseEnum, string message)
    {
        //담당 매니저에서 클래스를 만들수있도록 데이터 분류
        if (_successLoad)
        {
            //1. 파싱한 타입 - 수동으로 변수에서 매칭해둔거docuID - parseTypes
            string parseCode = parseEnum.GetType().ToString();
            Debug.Log(parseCode+"파싱");
            //2. sheetData 행마다 분리
            string[] enterDivde = message.Split('\n'); //엔터 - 행 분리
            //3. 첫번째 행은 enum string값 중 db로 관리할 목록을 칼럼명으로 적어놓은 부분
            string[] dbValueCode = enterDivde[0].Trim().Split('\t'); //enumCode를 칼럼명으로 - 첫번째 행의 역할
            //4. sheet 열과 현재 enum에서 해당 칼럼명의 인덱스가 다를 수 있으므로 조정
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbValueCode);
            //5. 나머지 행은 실제 값들.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1행부터 자료 값
            {
                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //탭 - 열 분리 
                dbValueList.Add(valueDivde);
            }
            //6. 파싱코드에 - 인덱스 매칭 코드와 실제 값들을 struct로 묶어서 dctionary에 저장 
            dbContainer.Add(parseCode, new ParseContainer(matchCode, dbValueList));
            
        }
        else
            Debug.Log("실패");
    }

 
}
