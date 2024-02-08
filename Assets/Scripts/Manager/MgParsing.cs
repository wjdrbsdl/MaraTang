using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgParsing : MgGeneric<MgParsing>
{
    private string[] docuIDes = { "19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg" };
    private string[] sheetIDes = { "0" };
    private System.Enum[] parseTypes = { ActionStat.MaxCountInTurn };
    private Dictionary<string, ParseContainer> dbContainer = new();
    public class ParseContainer
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
            string parseCode = parseEnum.GetType().ToString();
            Debug.Log(parseEnum.GetType().ToString());
            List<TokenAction> parseActions = new();
            string[] enterDivde = message.Split('\n'); //엔터 - 행 분리

            string[] dbValueCode = enterDivde[0].Trim().Split('\t'); //enumCode - 첫번째 행의 역할
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbValueCode);
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1행부터 자료 값
            {
                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //탭 - 열 분리 
                dbValueList.Add(valueDivde);
            }

            dbContainer.Add(parseCode, new ParseContainer(matchCode, dbValueList));
            
        }
        else
            Debug.Log("실패");
    }

    public ParseContainer GetDataBase(System.Enum _enum)
    {
        string parseCode = _enum.GetType().ToString();
        return dbContainer[parseCode];
    }
}
