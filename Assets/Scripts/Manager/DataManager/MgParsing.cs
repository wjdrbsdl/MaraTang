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
    public List<int[]> MatchCode; //enum�� ��ġ�Ǵ� �ε���
    public List<string[]> DbValueList; //��񿡼� ���� ����

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
    //stat[] �� ����ϴ°�� db�� enum�� MatchValue�� ����� ���� � enum�� ������ 
    private System.Enum[] matchTypes = { null, ETileStat.Nation, CharStat.CurActionCount, 
                                    CharActionStat.CoolTime, null, ConversationStat.Pid,
                                   null, null, null,
                                   null, null, null};
    private Dictionary<EMasterData, ParseData> dbContainer = new(); //�Ľ��Ѱ��� �׳� ���� �ִ»��� - ����ϴ°����� �ٽ� ���� �ʿ�. 
 
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
         delegate { MgGameIniti.GetInstance().PlayMgInitiWorkStep("�Ľ̿Ϸ�"); },
         ClassfyDataBase)
            );
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
                 //   Debug.Log(enterDivde[i] + "ù���� #���� �����ϴ� ���� �ǳʶ�");
                    continue;
                }
                    

                string[] valueDivde = enterDivde[i].Trim().Split('\t'); //�� - �� �и� 
     
               // Debug.Log(parseData + "�� ������" + valueDivde.Length);
                dbValueList.Add(valueDivde);
            }
            //6. �Ľ��ڵ忡 - �ε��� ��Ī �ڵ�� ���� ������ struct�� ��� dctionary�� ���� 
            dbContainer.Add(parseData, new ParseData(matchCode, dbValueList));
            
        }
        else
            Debug.LogWarning(matchTypes[_index]+ "�Ľ� ����");
    }

 
}
