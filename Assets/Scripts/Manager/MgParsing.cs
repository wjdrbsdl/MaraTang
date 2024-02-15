using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MasterData
{
    TileActionData, ContentData
}
public struct ParseData
{
    public List<int[]> MatchCode; //enum�� ��ġ�Ǵ� �ε���
    public List<string[]> DbValueList; //��񿡼� ���� ����

    public ParseData(List<int[]> _matchCode, List<string[]> _dbValues)
    {
        MatchCode = _matchCode;
        DbValueList = _dbValues;
    }
}

public class MgParsing : MgGeneric<MgParsing>
{
    private static string docuIDes =  "19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg";
    private string[] sheetIDes = { "0" , "85445904" };
    private MasterData[] dbId = { MasterData.TileActionData, MasterData.ContentData };
    private System.Enum[] matchTypes = { ActionStat.MaxCountInTurn, MGContent.ContentEnum.�߻������� };
    private Dictionary<MasterData, ParseData> dbContainer = new(); //�Ľ��Ѱ��� �׳� ���� �ִ»��� - ����ϴ°����� �ٽ� ���� �ʿ�. 
 
    public override void InitiSet()
    {
        base.InitiSet();
        ParseSheetData();
    }
    public ParseData GetMasterData(MasterData _dataId)
    {
        return dbContainer[_dataId];
    }

    private void ParseSheetData()
    {
        StartCoroutine(GameUtil.GetSheetDataCo(docuIDes, sheetIDes,
         delegate { MgGame.GetInstance().DoneInitiDataManager("�Ľ̿Ϸ�"); },
         ClassfyDataBase));
    }

    private void ClassfyDataBase(bool _successLoad, int _index, string message)
    {
        System.Enum parseEnum = matchTypes[_index];
        //��� �Ŵ������� Ŭ������ ������ֵ��� ������ �з�
        if (_successLoad)
        {
            //1. �Ľ��� Ÿ�� - �������� �������� ��Ī�صа�docuID - parseTypes
            MasterData parseData = dbId[_index];
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
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));
            
        }
        else
            Debug.Log("����");
    }

 
}
