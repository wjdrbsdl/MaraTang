using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{
    SpawnMonster, SpawnEvent
}

public enum ERewardType
{
    CharStat, Capital, ActionToken, EventToken, CharToken
}

public class Quest : IOrderCustomer
{
    //����
    //Ŭ��������
    //������ ����� ����Ʈ
    public int QuestPid = 0; //�ش� ����Ʈ pid mgContet ����Ʈ�� �߰��Ǵ� ������ �ε�����
    public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    public int ChunkNum = 0;
    public QuestCondition Condition; //���� ����
    public RewardData Reward; //����
    public PenaltyData Penalty;
    public List<TokenBase> TempQuestTokens = new(); //����Ʈ�� ���õ� ��ū�� 

    #region ����
    public Quest()
    {
       
    }

    public Quest(EQuestType _questType, ERewardType _rewardType, int _chunkNum)
    {
        QuestPid = MGContent.g_instance.m_questCount;
        ChunkNum = _chunkNum;
        //����Ʈ Ÿ�Կ� �°� ���Ǽ� �ۼ�
        Condition = new QuestCondition(_questType, _chunkNum);
        //����Ʈ ������ �ֹ����� �ݹ� ������� �ڽ��� �Ҵ� 
        Condition.TokenOrder.SetOrderCustomer(this);
        //���� Ÿ�Կ� �°� ���󳻿� �ۼ�
        Reward = new RewardData(_rewardType, _chunkNum); //�ӽ÷� �ڿ� ����
        Penalty = new PenaltyData();
    }

    public void OrderCallBack(OrderReceipt _orderReceipt) //����Ʈ ��
    {
        //�����ͽ�ť�ͷ� ������ ��ū���� �ݹ������ �ű⿡ �ڽ��� �Ҵ� 
        Debug.Log("����Ʈ���� ���� �༮���� �� ����Ʈ");
        TokenBase tokens = _orderReceipt.MadeToken;
        if (tokens == null)
            return;

        tokens.SetQuest(this);
        TempQuestTokens.Add(tokens);
    }

    #endregion

    public void UseTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //�� ��ū���� ���������� �ڽ��� ���¸� �ݹ���. 
        int resultCode = 0;
        TokenType type = _token.GetTokenType();

        //1. ��ū Ÿ�Կ� ���� ���� ��ū�� ���¿� ���� �ݹ� �ڵ带 ����
        if (type.Equals(TokenType.Char))
        {
            //������ ��� ��ū�� ���¿� ���� �ڵ带 ���� ���� - �� �׾��� ���, � ������ ��� ���� �ڵ带 ���� �س�����.
            resultCode = 5;
        }
        //2. ������ �ڵ带 ��ū�� �Բ� ����
        FindCallBackCode(_token, resultCode);
    }

    public void CleanQuest()
    {
        //����Ʈ �λ깰 �����ϴ� �κ�
        //1. ��� ����Ʈ�� ��� ���� ���Ϳ� ���� ���� 
        for (int i = 0; i < TempQuestTokens.Count; i++)
        {
            TempQuestTokens[i].Clean();
        }
    }

    private void FindCallBackCode(TokenBase _token, int _concludeCode)
    {
        //���޹��� �ڵ�� ��ū���� �ش� ����Ʈ�� ��� �������� ����
   
        //����� ���� ������� ��ƾ��� ���� ����Ʈ���� �����ϴ� ��
        TempQuestTokens.Remove((TokenChar)_token);
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
        if (TempQuestTokens.Count == 0)
        {
            return true;
        }
        return false;
    }

    
}
public enum QuestType
{
    Battle, Action, Item
}

public class QuestCondition
{
    //����Ʈ ����
    public EQuestType QuestType;
    public TTokenOrder TokenOrder;
    public int monsterPID;
    public int monsterCount;

    //�����ؾ��� �׼� ����

    //�����ؾ��� ������ ����

    public QuestCondition(int _monsterPID, int _monsterCount)
    {
        monsterPID = _monsterPID;
        monsterCount = _monsterCount;
    }

    public QuestCondition(EQuestType _questType, int _chunkNum)
    {
        //����Ʈ Ÿ�Կ� ���� ���� ������ ä��� 
        QuestType = _questType;
        ESpawnPosType tempSpawnPos = ESpawnPosType.Random;
        switch (_questType)
        {
            //�̺�Ʈ��ū �����ϴ� ���
            case EQuestType.SpawnEvent:
                TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnEvent,1,3,tempSpawnPos, _chunkNum);
                break;
            case EQuestType.SpawnMonster:
                //� ���͸� �󸶳� ������� ��ȯ���� �ʿ�
                int tempPid = 2;
                int tempCount = 1;
                TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, tempPid, tempCount, tempSpawnPos, _chunkNum);
                break;
        }
        
    }

}