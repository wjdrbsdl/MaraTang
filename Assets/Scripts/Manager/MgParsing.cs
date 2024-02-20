using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMasterData
{
    TileActionData, ContentData, TileType, CharData
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

    public string[] ExportValues(int _idx)
    {
        //�ش� �ε����� ��Ʈ�� �迭�� ���� 
        return DbValueList[_idx];
    }
}

public class MgParsing : MgGeneric<MgParsing>
{
    private static string docuIDes =  "19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg";
    private string[] sheetIDes = { "0" , "85445904", "1971334673", "1134768741" };
    private EMasterData[] dbId = { EMasterData.TileActionData, EMasterData.ContentData, EMasterData.TileType, EMasterData.CharData };
    private System.Enum[] matchTypes = { TileActionStat.NeedActionCount, MGContent.ContentEnum.�߻�������, EMasterData.ContentData, CharStat.CurActionCount };
    private Dictionary<EMasterData, ParseData> dbContainer = new(); //�Ľ��Ѱ��� �׳� ���� �ִ»��� - ����ϴ°����� �ٽ� ���� �ʿ�. 
 
    public override void InitiSet()
    {
        base.InitiSet();
        ParseSheetData();
    }
    public ParseData GetMasterData(EMasterData _dataId)
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
            EMasterData parseData = dbId[_index];
            //2. sheetData �ึ�� �и�
            string[] enterDivde = message.Split('\n'); //���� - �� �и�
            //3. ù��° ���� enum string�� �� db�� ������ ����� Į�������� ������� �κ�
            string[] dbEnumCode = enterDivde[0].Trim().Split('\t'); //enumCode�� Į�������� - ù��° ���� ����
            //4. sheet ���� ���� enum���� �ش� Į������ �ε����� �ٸ� �� �����Ƿ� ����
            List<int[]> matchCode = GameUtil.MakeMatchCode(parseEnum, dbEnumCode);
            //5. ������ ���� ���� ����.
            List<string[]> dbValueList = new();
            for (int i = 1; i < enterDivde.Length; i++) //1����� �ڷ� ��
            {
                if (enterDivde[i][0].Equals('#'))
                {
                 //   Debug.Log(enterDivde[i] + "���� �Ľ� ����");
                    break;
                }
                    

                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //�� - �� �и� 
     
               // Debug.Log(parseData + "�� ������" + valueDivde.Length);
                dbValueList.Add(valueDivde);
            }
            //6. �Ľ��ڵ忡 - �ε��� ��Ī �ڵ�� ���� ������ struct�� ��� dctionary�� ���� 
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));
            
        }
        else
            Debug.Log("����");
    }

 
}
