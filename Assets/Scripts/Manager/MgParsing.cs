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
        public List<int[]> MatchCode; //enum�� ��ġ�Ǵ� �ε���
        public List<string[]> DbValueList; //��񿡼� ���� ����

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
         delegate { MgGame.GetInstance().DoneInitiDataManager("�Ľ̿Ϸ�"); },
         ClassfyDataBase));
    }

    private void ClassfyDataBase(bool _successLoad, System.Enum parseEnum, string message)
    {
        //��� �Ŵ������� Ŭ������ ������ֵ��� ������ �з�
        if (_successLoad)
        {
            string parseCode = parseEnum.GetType().ToString();
            Debug.Log(parseEnum.GetType().ToString());
            List<TokenAction> parseActions = new();
            string[] enterDivde = message.Split('\n'); //���� - �� �и�

            string[] dbValueCode = enterDivde[0].Trim().Split('\t'); //enumCode - ù��° ���� ����
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbValueCode);
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1����� �ڷ� ��
            {
                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //�� - �� �и� 
                dbValueList.Add(valueDivde);
            }

            dbContainer.Add(parseCode, new ParseContainer(matchCode, dbValueList));
            
        }
        else
            Debug.Log("����");
    }

    public ParseContainer GetDataBase(System.Enum _enum)
    {
        string parseCode = _enum.GetType().ToString();
        return dbContainer[parseCode];
    }
}
