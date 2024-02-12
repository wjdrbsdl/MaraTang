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
    public ParseContainer GetMasterData(System.Enum _enum)
    {
        string parseCode = _enum.GetType().ToString();
        return dbContainer[parseCode];
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
            //1. �Ľ��� Ÿ�� - �������� �������� ��Ī�صа�docuID - parseTypes
            string parseCode = parseEnum.GetType().ToString();
            //2. sheetData �ึ�� �и�
            string[] enterDivde = message.Split('\n'); //���� - �� �и�
            //3. ù��° ���� enum string�� �� db�� ������ ����� Į�������� ������� �κ�
            string[] dbValueCode = enterDivde[0].Trim().Split('\t'); //enumCode�� Į�������� - ù��° ���� ����
            //4. sheet ���� ���� enum���� �ش� Į������ �ε����� �ٸ� �� �����Ƿ� ����
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbValueCode);
            //5. ������ ���� ���� ����.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1����� �ڷ� ��
            {
                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //�� - �� �и� 
                dbValueList.Add(valueDivde);
            }
            //6. �Ľ��ڵ忡 - �ε��� ��Ī �ڵ�� ���� ������ struct�� ��� dctionary�� ���� 
            dbContainer.Add(parseCode, new ParseContainer(matchCode, dbValueList));
            
        }
        else
            Debug.Log("����");
    }

 
}
