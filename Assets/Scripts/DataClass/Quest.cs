using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : IOrderCustomer
{
    public int ContentPid = 0; //content pid
    public int SerialNum = 0;
    public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    public int ChunkNum = 0;
    public int CurStep = 1;
    public CurStageData CurStageData; //���� �������� ���� ����, �޼� ���� �����ϴ� ��. 

    #region ����
    public Quest()
    {
       
    }

    public Quest(ContentMasterData _contentData, int _chunkNum)
    {
        ContentPid = _contentData.ContentPid;
        ChunkNum = _chunkNum;
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
        Debug.Log(CurStep + "�ܰ� ����ȭ ����");
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        CurStageData = new CurStageData(stage);
        TTokenOrder order = new TTokenOrder(stage.SituationList, stage.SituAdapCount, SerialNum, this);
        OrderExcutor excutor = new OrderExcutor();
        excutor.ExcuteOrder(order);
        Debug.LogWarning("�� ����Ʈ �˶� �ݾƳ���");
        //MgUI.GetInstance().ShowQuest(this);
      //  Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� �ߵ� ��", SerialNum, CurStep);
    }

    public void ClearStage()
    {
        Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� Ŭ���� ��", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.SuccesStep;
        CurStep = nextStep;
        if(CurStep == 0)
        {
            MGContent.GetInstance().SuccessQuest(this); //ClearStage()
            return;
        }
        RealizeStage();
    }

    public void FailStage()
    {
        Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� ���� ��", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.PenaltyStep;
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
 
    public void OnOrderCallBack(OrderReceipt _orderReceipt)
    {
        TOrderItem doneItem = _orderReceipt.DoneItem;

        if (doneItem.Tokentype.Equals(TokenType.Conversation))
        {
            SelectItemInfo selectInfo = new SelectItemInfo(null, false);
            MgUI.GetInstance().SetScriptCustomer(selectInfo);
            return;
        }
    }
    #endregion

}

public class CurStageData
{
    //�������� Ŭ��� ���� ������ ���

    //1. �ش� �ǹ��� �����ߴ°� - ��������
    //2. ���͸� ��Ҵ°� - �ټ� ����
    //3. ��ȭ Ȯ���� �ߴ°� - ���� ���� 
    //4. ��Ḧ � ��Ƽ� �ǳٴ�
    //5. ������ �����Ѵ�. 
    //6. �ߵ� �����Ѵ�. 
    //7. ���� ������ �����ߴ�. 
    public int SuccesNeedCount = 0; //�ʿ� ���� �� 
    public List<TOrderItem> SuccesConList; //���߷��� ����
    public int FailNeedCount = 0;
    public List<TOrderItem> FailConList;
    public List<TOrderItem> CurConList; //���� ���� ��Ȳ
    public int SuccesStep; //������ �̵��� Stage ���
    public int PenaltyStep; //���н� �̵��� Stage ���
    public int StageNum;

    public CurStageData(StageMasterData _stageMasterData)
    {
        SuccesNeedCount = _stageMasterData.SuccedNeedCount;
        SuccesConList = _stageMasterData.SuccesConList; //���������� ������ �״��
        FailNeedCount = _stageMasterData.FailNeedCount;
        FailConList = _stageMasterData.FailConList;
        CurConList = new List<TOrderItem>(); //���� ��Ȳ�� ���� ����
        InitCurConditionValue(); //���� ���� �ʱⰪ ����
       // Debug.Log("���� �߰��� ���� ��" + CurConList.Count);
        InitCheck(); //���� ���� üũ
        SuccesStep = _stageMasterData.SuccesStep;
        PenaltyStep = _stageMasterData.PenaltyStep;
        StageNum = _stageMasterData.StageNum;
    }

    private void InitCurConditionValue()
    {
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
    private bool RequestAdaptResponse(TOrderItem _response)
    {
        //Ȯ��, ��� ���� ������ ������ �� ���� ����
        //1 �ش� ����� ���� ������ ���ؼ� ���� ���� index�� ���� ���ǰ��� ����
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem successItem = SuccesConList[i];
            if(successItem.SubIdx == _response.SubIdx && successItem.Value == _response.Value)
            {
                //���� ������ ��ġ�ϴ� ������ ������, ���� ������ index ���� ������������ �Ҵ� 
                CurConList[i] = successItem;
                return true;
            }
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
            Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� {2}�� �� ����\n������ ��{3}", _adaptItem.Tokentype, _adaptItem.SubIdx, _adaptItem.Value, CurConList[_index].Value);
            return true;
        }
        return false;
      //  Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� �ٸ�",_adaptItem.Tokentype,_adaptItem.SubIdx);
    }
    #endregion

    #region ���� ���� üũ 
    public bool CheckSuccess()
    {
        //���� ���¿� ��ǥ ���¸� �� ��ūŸ�Կ� ���� ������ ������
        int passCount = 0; //����Ѽ�
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            //1. ���� ���ǵ� �� 1�� ��
            TOrderItem successCondition = SuccesConList[i]; 
            for (int curConIdx = 0; curConIdx < CurConList.Count; curConIdx++)
            {
                //2. ���� ���ǰ� ���� ������ �ϳ��� �� 
                TOrderItem curCondtion = CurConList[curConIdx]; //���� ����
                TokenType conditionType = curCondtion.Tokentype;
                if(curCondtion.Tokentype != successCondition.Tokentype || curCondtion.SubIdx != successCondition.SubIdx)
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
                        isPass = IsEnoughValue(successCondition, curCondtion);
                        break;
                    default:
                        isPass = IsMatch(successCondition, curCondtion);
                        break;
                }

                if (isPass)
                    passCount += 1;

                //6. ���� ���ǿ� �´� ���� ������ ���������Ƿ� break;
                break; 
            }
            //7. ���� ���� ���� ���� 
        }
        Debug.LogFormat("�ʿ��{0} ������{1} ���Ǽ�{2}", SuccesNeedCount, passCount, SuccesConList.Count);
        if (SuccesNeedCount <= passCount)
        {
            return true;
        }
        return false;
    }

    public bool CheckFail()
    {
        //���� ���¿� ��ǥ ���¸� �� ��ūŸ�Կ� ���� ������ ������
        int failCount = 0; //�����Ѽ�
        for (int i = 0; i < FailConList.Count; i++)
        {
            //1. ���� ���ǵ� �� 1�� ��
            TOrderItem failCondition = FailConList[i];
            for (int curConIdx = 0; curConIdx < CurConList.Count; curConIdx++)
            {
                //2. ���� ���ǰ� ���� ������ �ϳ��� �� 
                TOrderItem curCondtion = CurConList[curConIdx]; //���� ����
                TokenType conditionType = curCondtion.Tokentype;
                if (curCondtion.Tokentype != failCondition.Tokentype || curCondtion.SubIdx != failCondition.SubIdx)
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
                        isPass = IsEnoughValue(failCondition, curCondtion);
                        break;
                    default:
                        isPass = IsMatch(failCondition, curCondtion);
                        break;
                }

                if (isPass)
                    failCount += 1;

                //6. ���� ���ǿ� �´� ���� ������ ���������Ƿ� break;
                break;
            }
            //7. ���� ���� ���� ���� 
        }
        Debug.LogFormat("���� �ʿ��{0} ������{1} ���Ǽ�{2}", FailNeedCount, failCount, FailConList.Count);
        if (FailNeedCount <= failCount)
        {
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
}