using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : IOrderCustomer
{
    //����
    //Ŭ��������
    //������ ����� ����Ʈ
    public int ContentPid = 0; //content pid
    public int SerialNum = 0;
    public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    public int ChunkNum = 0;
    public int CurStep = 1;
    public List<TokenBase> QuestTokens = new(); //����Ʈ�� ���õ� ��ū�� 
    public CurStageData CurStageData;

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

    public void FlowTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    private enum QuestCode
    {
        MonsterDie, EventActive
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //�� ��ū���� ���������� �ڽ��� ���¸� �ݹ���. 
        QuestCode resultCode = QuestCode.MonsterDie;
        TokenType type = _token.GetTokenType();

        //1. ��ū Ÿ�Կ� ���� ���� ��ū�� ���¿� ���� �ݹ� �ڵ带 ����
        if (type.Equals(TokenType.Char))
        {
            //������ ��� ��ū�� ���¿� ���� �ڵ带 ���� ���� - �� �׾��� ���, � ������ ��� ���� �ڵ带 ���� �س�����.
            resultCode = QuestCode.MonsterDie;
        }
        else if (type.Equals(TokenType.Event))
        {
            Debug.Log("�̺�Ʈ ���� ���� �˸�" + QuestTokens.IndexOf(_token));
            resultCode = QuestCode.EventActive;
        }
        //2. ������ �ڵ带 ��ū�� �Բ� ����
        FindCallBackCode(_token, resultCode);
    }

    public void ClearStage()
    {
        Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� Ŭ���� ��", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.SuccesStep;
        CurStep = nextStep;
        RealizeStage();
    }

    public void ResetSituation()
    {
        Debug.LogFormat("�ø��� �ѹ�{0} �� {1}�������� ����", SerialNum, CurStep);
        StageMasterData stageInfo = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        TOrderItem doneItem = stageInfo.SituationList[0];
    }

    public void CleanQuest()
    {
        //����Ʈ �λ깰 �����ϴ� �κ�
        //1. ��� ����Ʈ�� ��� ���� ���Ϳ� ���� ���� 
        for (int i = 0; i < QuestTokens.Count; i++)
        {
            QuestTokens[i].CleanToken();
        }
    }

    private void FindCallBackCode(TokenBase _token, QuestCode _concludeCode)
    {
        //���޹��� �ڵ�� ��ū���� �ش� ����Ʈ�� ��� �������� ����
   
        //����� ���� ������� ��ƾ��� ���� ����Ʈ���� �����ϴ� ��
        if(_concludeCode.Equals(QuestCode.MonsterDie))
           QuestTokens.Remove(_token);
        else if (_concludeCode.Equals(QuestCode.EventActive))
        {
            //�̺�Ʈ ����Ʈ�� ��� �ϳ��� ����Ǿ����� ����ó��
            MGContent.GetInstance().SuccessQuest(this); //
            return;
        }

        bool isComplete = CheckQuestComplete();
        //����Ʈ ���� �����ߴٸ�
        if (isComplete)
        {
            MGContent.GetInstance().SuccessQuest(this);
        }
            
    }

    private bool CheckQuestComplete()
    {
        //��ū�� ȣ��� ���� ��� �ڵ带 ����ϰ� ����Ʈ �Ϸ� ���θ� üũ�Ѵ�. 
        if (QuestTokens.Count == 0)
        {
            return true;
        }
        return false;
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
                Debug.LogFormat("{0}Ÿ���� �ٸ�", _adaptItem.Tokentype);
                continue;
            }
                
            //2. ����� ��ūŸ�Կ� ���� ���� ���� ����
            switch (adaptType)
            {
                case TokenType.Char: //������ ��� ��ǥ ���� óġ�� ���� ���� value 1 ���
                    AdaptHunt(_adaptItem, curCondtion, i);
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
                    isPass = IsEnoughHunt(i);
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
    private bool IsEnoughHunt(int _index)
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