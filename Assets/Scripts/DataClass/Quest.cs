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
            MGContent.GetInstance().SuccessQuest(this);
            return;
        }
        RealizeStage();
    }

    public void ResetSituation()
    {
        Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� ����", SerialNum, CurStep);
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
    public List<TOrderItem> SuccesConList; //���߷��� ����
    public List<TOrderItem> CurConList; //���� ���� ��Ȳ
    public List<bool> CurPassRecordList; //���� �޼� ��Ȳ
    public int SuccesStep; //������ �̵��� Stage ���
    public int PenaltyStep; //���н� �̵��� Stage ���
    public int StageNum;

    public CurStageData(StageMasterData _stageMasterData)
    {
        SuccesConList = _stageMasterData.SuccesConList; //���������� ������ �״��
        CurConList = new List<TOrderItem>(); //���� ��Ȳ�� ���� ����
        CurPassRecordList = new List<bool>();
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = SuccesConList[i];
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //�ʱⰪ�� �⺻������ �� ����, ������ value�� 0�ΰ�쵵 �־ -1�� �⺻ ����. 
            if (conditionType.Equals(TokenType.Char))
                intialValue = 0; //�����ǰ�� 0 => �����ǰ�� value�� ���� ���� ���̹Ƿ� 0 ���� ����. 
            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //������ value�� 0���� �ؼ� ����

            CurConList.Add(curConItem);
            CurPassRecordList.Add(false);
        }
        SuccesStep = _stageMasterData.SuccesStep;
        PenaltyStep = _stageMasterData.PenaltyStep;
        StageNum = _stageMasterData.StageNum;
    }


    #region �׼��� ���� ���¿� �ݿ�
    public void AdaptCondtion(TOrderItem _adaptItem)
    {
        //�� item�� ���� ���¿� ����
        TokenType adaptType = _adaptItem.Tokentype;
        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //���� ����
            //1. ���� ���� ������ TokenType�� ������������ üũ 
            if(_adaptItem.Tokentype != curCondtion.Tokentype)
            {
                Debug.LogFormat("���� Ÿ�� {0}, ���� Ÿ�� {1}���� Ÿ���� �ٸ�", _adaptItem.Tokentype, curCondtion.Tokentype);
                continue;
            }
                
            //2. ����� ��ūŸ�Կ� ���� ���� ���� ����
            switch (adaptType)
            {
                case TokenType.Char: //������ ��� ��ǥ ���� óġ�� ���� ���� value 1 ���
                    AdaptHunt(_adaptItem, curCondtion, i);
                    break;
                case TokenType.Action:
                    AdaptActionLv(i);
                    break;
                default:
                    AdaptValue(_adaptItem, curCondtion, i);
                    break;
            }

        }
    }
    private void AdaptHunt(TOrderItem _huntMonItem, TOrderItem _curRecord, int _index)
    {
        //���� ���Ϳ� ��ƾ��ϴ� ���� pid�� �����ϸ� ���� ���ǿ� +1
        if(_curRecord.SubIdx == _huntMonItem.SubIdx)
        {
            TOrderItem newCurCondition = _curRecord;
            newCurCondition.SetValue(_curRecord.Value + 1);
            CurConList[_index] = newCurCondition; //���� ��Ȳ�� �������� ���� �Ҵ�
            return;
        }
     }
    private void AdaptActionLv(int _index)
    {
        TOrderItem needAction = CurConList[_index]; //���ǰ� ����
        List<TokenAction> actionList = PlayerManager.GetInstance().GetPlayerActionList();
        int needActionPid = needAction.SubIdx;
        Debug.Log(_index+"��° ������"+needActionPid+"�� �����ߴ��� üũ ���� ���� ������ "+needAction.Value);
        //�ϴ� �������� ������ ����
        for (int i = 0; i < actionList.Count; i++)
        {
            int actionPid = actionList[i].GetPid();
            
            if (needActionPid == actionPid)
            {
                needAction.SetValue(1); //������ ��ų�� �����ϸ� �ش� ���� ���� 1�� ����
                CurConList[_index] = needAction;
                Debug.LogFormat("�ʿ��� ��ų pid{0}�� �����ؼ� ���� ���� value�� {1}�� ����",actionPid, CurConList[_index].Value);
            }
                
        }
    }
    private void AdaptValue(TOrderItem _adaptItem, TOrderItem _curRecord, int _index)
    {
        //�ܼ��� �ֱ� ���� ���� ���·� �����ϸ� �Ǵ� ��� 
        Debug.LogFormat("�����Ϸ��� Adapt ����� {0}Ÿ�Կ� sub{1}���� üũ ���� �����{2}", _curRecord.Tokentype, _curRecord.SubIdx,_curRecord.Value);
        //subIdx Ȯ���� 
        if (_curRecord.SubIdx == _adaptItem.SubIdx)
        {
            //�׳� �Ҵ�
            CurConList[_index] = _adaptItem;
            Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� {2}�� �� ����\n������ ��{3}", _adaptItem.Tokentype, _adaptItem.SubIdx, _adaptItem.Value, CurConList[_index].Value);
            return;
        }
        Debug.LogFormat("{0}Ÿ�� ������ sub{1}�� �ٸ�",_adaptItem.Tokentype,_adaptItem.SubIdx);
    }
    #endregion

    #region ���� ���°� ���� ���¿� �����ߴ��� üũ
    public bool CheckCondition()
    {
        //���� ���¿� ��ǥ ���¸� �� ��ūŸ�Կ� ���� ������ ������
        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //���� ����
            TokenType conditionType = curCondtion.Tokentype;
            bool isPass = false; //���� ���� ����
            //����� ���� Ÿ�Կ� ���� ���� ���� ���¸� ��ȭ
            switch (conditionType)
            {
                case TokenType.Char: //������ ��� ��ǥ ���� óġ�� ���� ���� value 1 ���
                case TokenType.Action:
                    isPass = IsEnoughValue(i);
                    break;
                default:
                    isPass = IsMatch(i);
                    break;
            }
            //���� �� �������� üũ 
            CurPassRecordList[i] = isPass;
        }

        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            if (CurPassRecordList[i] == false)
                return false;
        }
        return true;
    }
    private bool IsEnoughValue(int _index)
    {
        if (SuccesConList[_index].Value <= CurConList[_index].Value)
            return true;

        return false;
    }
    private bool IsMatch(int _index)
    {
        //���ǰ��� ���簪�� �����ϸ� �Ǵ� ��� 
        if (SuccesConList[_index].Value == CurConList[_index].Value)
            return true;

        return false;
    }
    #endregion
}