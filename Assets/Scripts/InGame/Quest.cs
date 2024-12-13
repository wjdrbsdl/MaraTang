using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Quest
{
    [JsonProperty] public int ContentPid = 0; //content pid
    [JsonProperty] public int SerialNum = 0;
    [JsonProperty] public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    [JsonProperty] public int ChunkNum = 0;
    [JsonProperty] public int CurStep = 1;
    [JsonProperty] public CurrentStageData CurStageData; //���� �������� ���� ����, �޼� ���� �����ϴ� ��. 

    #region ����
    public Quest()
    {

    }

    public Quest(ContentMasterData _contentData, int _chunkNum)
    {
        ContentPid = _contentData.ContentPid;
        ChunkNum = _chunkNum;
        //�ܰ��� ���������� �߰�
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        CurStageData = new CurrentStageData(stage);
        SerialNum = MGContent.GetInstance().GetSerialNum();
    }
    #endregion

    public void FlowTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    #region �������� ����
    public void RealizeStage()
    {
        Debug.LogFormat("{0}�� ����Ʈ, {1}�������� ����, ������ȣ{2}", ContentPid, CurStep, SerialNum);
        TTokenOrder order = new TTokenOrder(CurStageData.SituationList, CurStageData.AbleSelect, CurStageData.SituAdapCount, SerialNum);
        OrderExcutor excutor = new OrderExcutor();
        excutor.ExcuteOrder(order);

        //�ѹ��� �˸��ޱ�
        if(FixedValue.QUEST_ARALM == false)
        {
            FixedValue.QUEST_ARALM = true;
            Debug.LogWarning("�� ����Ʈ �˶� �ݾƳ���");
            //MgUI.GetInstance().ShowQuest(this);
        }
    }

    public void ClearStage()
    {
      //  Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� Ŭ���� ��", SerialNum, CurStep);
        ResetSituation(); //�������� ������ ���� �����Ǿ����� �ʱ�ȭ
        CurStageData.AdaptSucces(); //�������� ������ ���� �����ؾ��Ұ͵� ����
        int nextStep = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep).SuccesStep; //���罺������ �������� ������ ���� �ѹ� ����
        CurStep = nextStep;
        if(CurStep == 0)
        {
            MGContent.GetInstance().SuccessQuest(this); //ClearStage()
            return;
        }
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        CurStageData = new CurrentStageData(stage);
        RealizeStage();
    }

    public void FailStage()
    {
      //  Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� ���� ��", SerialNum, CurStep);
        ResetSituation();
        
        int nextStep = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep).PenaltyStep; //���罺������ �������� ���н� ���� �ѹ� ����
        CurStep = nextStep;
        if (CurStep == 0)
        {
            MGContent.GetInstance().FailQuest(this); //FailStage���� ȣ��
            return;
        }
        RealizeStage();
    }

    public void ResetSituation()
    {
   //     Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� ����", SerialNum, CurStep);
        StageMasterData stageInfo = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        TOrderItem doneItem = stageInfo.SituationList[0];
    }

    #endregion

}

public class CurrentStageData
{
    //�������� Ŭ��� ���� ������ ���

    //1. �ش� �ǹ��� �����ߴ°� - ��������
    //2. ���͸� ��Ҵ°� - �ټ� ����
    //3. ��ȭ Ȯ���� �ߴ°� - ���� ���� 
    //4. ��Ḧ � ��Ƽ� �ǳٴ�
    //5. ������ �����Ѵ�. 
    //6. �ߵ� �����Ѵ�. 
    //7. ���� ������ �����ߴ�. 
    public bool AutoClear; //���������� �ڵ� Ŭ���� ����
    public bool AbleClear = false; // Ŭ���� ������ ��������
    public List<TOrderItem> SituationList;
    public bool AbleSelect; //���� ���� ���ɿ��� 
    public int SituAdapCount = 0;
    public int SuccesNeedCount = 0; //�ʿ� ���� �� 
    public List<TOrderItem> SuccesConList; //���߷��� ����
    public int FailNeedCount = 0;
    public List<TOrderItem> FailConList;
    public List<TOrderItem> CurConList; //���� ���� ��Ȳ
    public bool[] SuccessState;
    public bool[] FailState;

    public CurrentStageData()
    {
        //json ������ �� ������
    }

    public CurrentStageData(StageMasterData _stageMasterData)
    {
        SituationList = CopyList(_stageMasterData.SituationList);
        AbleSelect = _stageMasterData.AbleSelect;
        SituAdapCount = _stageMasterData.SituAdapCount;
        SuccesNeedCount = _stageMasterData.SuccedNeedCount;
        AutoClear = _stageMasterData.AutoClear;
        SuccesConList = CopyList(_stageMasterData.SuccesConList); //���������� ������ �״��
        FailNeedCount = _stageMasterData.FailNeedCount;
        FailConList = CopyList(_stageMasterData.FailConList);
        CurConList = new List<TOrderItem>(); //���� ��Ȳ�� ���� ����
        InitCurConditionValue(); //���� ���� �ʱⰪ ����
        InitCheck(); //���۽� ���� ���� üũ 
    }

    public void ResetCurCondtion()
    {
        //���� �������� ���ǰ��� �ٲ۰�� ������� ���� ��Ž��
        CurConList = new List<TOrderItem>();
        InitCurConditionValue(); //���� ���� �ʱⰪ ����
        InitCheck(); //���۽� ���� ���� üũ 
    }

    private List<TOrderItem> CopyList(List<TOrderItem> _origin)
    {
        List<TOrderItem> _copy = new List<TOrderItem>();
        for(int i = 0; i < _origin.Count; i++)
        {
            _copy.Add(_origin[i]);
        }
        return _copy;
    }


    private void InitCurConditionValue()
    {
        //0. ���� ���Ǽ� ��ŭ ���� ���� bool�� �迭 ����
        SuccessState = new bool[SuccesConList.Count];
        FailState = new bool[FailConList.Count];
        //1. ���� ���ǰ� ���� ������ TOrder�� �����Ѵ�
        //2. TokenType�� SubIdx�� �ߺ��Ǵ� �͵��� 1���� �����Ѵ�. - �ߺ� üũ �ʿ�. 
        //3. �ʱⰪ�� �� TokenType, Subidx�� ���� �����Ѵ�. 
        //���� ���� �߰�
        //     Debug.Log("���� ���� �߰� " + SuccesConList.Count);
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = SuccesConList[i];
            if (CheckOverlap(conditionItem))
            {
                //�̹� �߰��� �������̸� �о�
                continue;
            }
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //�ʱⰪ�� �⺻������ �� ����, ������ value�� 0�ΰ�쵵 �־ -1�� �⺻ ����. 
            switch (conditionType)
            {
                case TokenType.Char:
                    intialValue = 0; //�����ǰ�� 0 => �����ǰ�� value�� ���� ���� ���̹Ƿ� 0 ���� ����. 
                    break;
                case TokenType.Action:
                    intialValue = PlayerManager.GetInstance().GetPlayerActionLevel(conditionItem.SubIdx); 
                    break;
            }
            
            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //������ value�� 0���� �ؼ� ����
            CurConList.Add(curConItem);
        }
        //���� ���� �߰�
   //     Debug.Log("���� ���� �߰� " + FailConList.Count);
        for (int i = 0; i < FailConList.Count; i++)
        {
            TOrderItem conditionItem = FailConList[i];
            if (CheckOverlap(conditionItem))
            {
                //�̹� �߰��� �������̸� �о�
                continue;
            }
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //�ʱⰪ�� �⺻������ �� ����, ������ value�� 0�ΰ�쵵 �־ -1�� �⺻ ����. 
            switch (conditionType)
            {
                case TokenType.Char:
                    intialValue = 0; //�����ǰ�� 0 => �����ǰ�� value�� ���� ���� ���̹Ƿ� 0 ���� ����. 
                    break;
                case TokenType.Action:
                    intialValue = PlayerManager.GetInstance().GetPlayerActionLevel(conditionItem.SubIdx);
                    break;
            }

            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //������ value�� 0���� �ؼ� ����
            CurConList.Add(curConItem);
        }
    }

    private bool CheckOverlap(TOrderItem _item)
    {
        //�ߺ� üũ
        for (int i = 0; i < CurConList.Count; i++)
        {
            TOrderItem curItem = CurConList[i];
            if (curItem.Tokentype == _item.Tokentype && curItem.SubIdx == _item.SubIdx)
            {
         //       Debug.Log(curItem.Tokentype + ":" + curItem.SubIdx+"�ߺ� "); 
                return true;
            }
                
        }
        return false;
    }

    private void InitCheck()
    {
      
    }


    #region �׼��� ���� ���¿� �ݿ�
    public void AdaptCondtion(TOrderItem _adaptItem)
    {
        //�� item�� ���� ���¿� ����
        TokenType adaptType = _adaptItem.Tokentype;
        for (int i = 0; i < CurConList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //���� ����
            //1. ���� ���� ������ TokenType�� ������������ üũ 
            if(_adaptItem.Tokentype != curCondtion.Tokentype)
            {
               // Debug.LogFormat("���� Ÿ�� {0}, ���� Ÿ�� {1}���� Ÿ���� �ٸ�", _adaptItem.Tokentype, curCondtion.Tokentype);
                continue;
            }

            //2. ��ūŸ�Կ� ���� ���������� ����
            bool isAdapt = false;
            switch (adaptType)
            {
                case TokenType.Char: //���ʹ� ��ǥ pid(sub) óġ�� value 1 ���
                    isAdapt = RequestAdaptHunt(_adaptItem, curCondtion, i);
                    break;
                case TokenType.Action: //pid ������ �ش� �׼� ������ value �� ����(adapt�� ��ūŸ���� �������)
                    isAdapt = RequestAdaptActionLv(_adaptItem, curCondtion, i);
                    break;
                //case TokenType.Conversation: //pid ������ �ش� �׼� ������ value �� ����(adapt�� ��ūŸ���� �������)
                //    isAdapt = RequestAdaptResponse(_adaptItem);
                //    break;
                default: //�׿� �Էµ� ������ ���ǰ��� �ٲٸ� �Ǵ� ��� ����
                    isAdapt = RequestAdaptValue(_adaptItem, curCondtion, i);
                    break;
            }
            //3. ���� ���� �� ��� �ϳ��� �����ߴٸ� ������ ������ �Ⱥ��� ��. �׷����� Adapt�� ���θ� ��ȯ�޾ƾ���. 
            if (isAdapt)
                break;

        }
    }
    private bool RequestAdaptHunt(TOrderItem _huntMonItem, TOrderItem _curRecord, int _index)
    {
        //���� ���Ϳ� ��ƾ��ϴ� ���� pid�� �����ϸ� ���� ���ǿ� +1
        if(_curRecord.SubIdx == _huntMonItem.SubIdx)
        {
            TOrderItem newCurCondition = _curRecord;
            newCurCondition.SetValue(_curRecord.Value + 1);
            CurConList[_index] = newCurCondition; //���� ��Ȳ�� �������� ���� �Ҵ�
            return true;
        }
        return false;
     }
    private bool RequestAdaptActionLv(TOrderItem _studyAction, TOrderItem _curRecord, int _index)
    {
        //Ȯ���Ϸ��� action Pid�� ������ �÷��̾� ���� ���� 
        //1. ��ų pid Ȯ��
        if (_curRecord.SubIdx == _studyAction.SubIdx) 
        {
            TOrderItem newCurCondition = _curRecord; //���� ����
            //2. ���� ������
            int actionLevel = PlayerManager.GetInstance().GetPlayerActionLevel(_studyAction.SubIdx);
            //3. ���� ����
            newCurCondition.SetValue(actionLevel); 
            //4. ���ǿ� ���� ����
            CurConList[_index] = newCurCondition; //���� ��Ȳ�� �������� ���� �Ҵ�
            return true;
        }
        return false;
    }
    private bool RequestAdaptValue(TOrderItem _adaptItem, TOrderItem _curRecord, int _index)
    {
        //�ܼ��� �ֱ� ���� ���� ���·� �����ϸ� �Ǵ� ��� 
      //  Debug.LogFormat("�����Ϸ��� Adapt ����� {0}Ÿ�Կ� sub{1}���� üũ ���� �����{2}", _curRecord.Tokentype, _curRecord.SubIdx,_curRecord.Value);
        //subIdx Ȯ���� 
        if (_curRecord.SubIdx == _adaptItem.SubIdx)
        {
            //�׳� �Ҵ�
            CurConList[_index] = _adaptItem;
           // Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� {2}�� �� ����\n������ ��{3}", _adaptItem.Tokentype, _adaptItem.SubIdx, _adaptItem.Value, CurConList[_index].Value);
            return true;
        }
        return false;
      //  Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� �ٸ�",_adaptItem.Tokentype,_adaptItem.SubIdx);
    }
    #endregion

    #region ���� ���� üũ 
    public bool CheckSuccess()
    {
       return CheckCondition(SuccesConList, SuccesNeedCount, SuccessState, ref AbleClear);
    }

    public bool CheckFail()
    {
        bool isFail = false;
        return CheckCondition(FailConList, FailNeedCount, FailState, ref isFail);
    }

    private bool CheckCondition(List<TOrderItem> _condtions, int _needCount, bool[] _state, ref bool _isDone)
    {
        //���� ���¿� ��ǥ ���¸� �� ��ūŸ�Կ� ���� ������ ������
        int conditionCount = 0; //�����Ѽ�
        _isDone = false; //���з� �ʱ�ȭ 
        for (int i = 0; i < _condtions.Count; i++)
        {
            //1. ���� ���ǵ� �� 1�� ��
            TOrderItem checkCondition = _condtions[i];
            _state[i] = false; //�ش� ���� ���� ���з� �ʱ�ȭ
            for (int curConIdx = 0; curConIdx < CurConList.Count; curConIdx++)
            {
                //2. ���� ���ǰ� ���� ������ �ϳ��� �� 
                TOrderItem curCondtion = CurConList[curConIdx]; //���� ����
                TokenType conditionType = curCondtion.Tokentype;
                if (curCondtion.Tokentype != checkCondition.Tokentype || curCondtion.SubIdx != checkCondition.SubIdx)
                {
                    //3. ���� ���ǰ� ���� ������ �� ������� üũ 
                    //4. �ٸ��� �н�
                    continue;
                }

                //5. �� �����̸� ���� ���� üũ 
                bool isPass = false; //���� ���� ����
                                     //����� ���� Ÿ�Կ� ���� ���� ���� ���¸� ��ȭ
                switch (conditionType)
                {
                    case TokenType.Char: //������ ��� ��ǥ ���� óġ�� ���� ���� value 1 ���
                    case TokenType.Action:
                    case TokenType.Capital:
                        isPass = IsEnoughValue(checkCondition, curCondtion);
                        break;
                    default:
                        isPass = IsMatch(checkCondition, curCondtion);
                        break;
                }

                if (isPass)
                {
                    _state[i] = true;
                    conditionCount += 1;
                }
                    

                //6. ���� ���ǿ� �´� ���� ������ ���������Ƿ� break;
                break;
            }
            //7. ���� ���� ���� ���� 
        }
        //   Debug.LogFormat("���� �ʿ��{0} ������{1} ���Ǽ�{2}", FailNeedCount, failCount, FailConList.Count);
        if (_needCount <= conditionCount)
        {
            _isDone = true;
            return true;
        }
        return false;
    }

    private bool IsEnoughValue(TOrderItem _target, TOrderItem _cur)
    {
        if (_target.Value <= _cur.Value)
            return true;

        return false;
    }
    private bool IsMatch(TOrderItem _target, TOrderItem _cur)
    {
        //���ǰ��� ���簪�� �����ϸ� �Ǵ� ��� 
        if (_target.Value == _cur.Value)
            return true;

        return false;
    }
    #endregion

    public void AdaptSucces()
    {
        //���� �� �����Ұ͵� - �����ϱ� ���� ��� �ش� ���� ��ŭ ���� 
        int adaptCount = 0; //�ʿ��� ���� �� ��ŭ ���� ���� 
        //ex �ڿ� A, B ���� �ϳ��� 50 ������ �����ϴ� ���, �ش� adaptCount�� ������������ A,B �Ѵ� 50�� ��� ��찡 �߻�
        //�� �ڿ��� �����ȴٸ� �տ� �ڿ��� ������ ������ �ϳ��� �������� 
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            //1. �������� ���� ���������� ��쿡�� �ѱ�
            if (SuccessState[i] == false)
                continue;

            //2. ������ ��� ���� ������ ����
            TOrderItem successCondtion = SuccesConList[i]; 

            //3. ��ūŸ�Կ� ���� ���������� ����
            TokenType adaptType = successCondtion.Tokentype;
            switch (adaptType)
            {
                case TokenType.Capital: //�ʿ����� �� ��ŭ �ڿ� �Һ�. 
                    PlayerCapitalData.g_instance.CalCapital((Capital)successCondtion.SubIdx, -successCondtion.Value);
                    adaptCount += 1;
                    break;
                    
            }

            if (adaptCount.Equals(SuccesNeedCount))
                break;
        }
    }
}