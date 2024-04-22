using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{
    SpawnMonster, SpawnEvent
}

public enum ERewardType
{
   None, CharStat, Capital, ActionToken, EventToken, CharToken
}

public class Quest 
{
    //����
    //Ŭ��������
    //������ ����� ����Ʈ
    public int QuestPid = 0; //�ش� ����Ʈ pid mgContet ����Ʈ�� �߰��Ǵ� ������ �ε�����
    public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    public int ChunkNum = 0;
    public ContentData ContentData;
    public QuestCondition Condition; //���� ����
    public RewardData Reward; //����
    public PenaltyData Penalty;
    public List<TokenBase> QuestTokens = new(); //����Ʈ�� ���õ� ��ū�� 

    #region ����
    public Quest()
    {
       
    }

    public Quest(EQuestType _questType, ERewardType _rewardType, int _chunkNum, ContentData _contentData)
    {
        QuestPid = _contentData.ContentPid;
        ChunkNum = _chunkNum;
        //����Ʈ Ÿ�Կ� �°� ���Ǽ� �ۼ�
        Condition = new QuestCondition(_contentData, _chunkNum);
        //���� Ÿ�Կ� �°� ���󳻿� �ۼ�
        Reward = new RewardData(_rewardType, _chunkNum); //�ӽ÷� �ڿ� ����
        Penalty = new PenaltyData();
    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt) //����Ʈ ��
    {
        //�����ͽ�ť�ͷ� ������ ��ū���� �ݹ������ �ű⿡ �ڽ��� �Ҵ� 
        TokenBase madeToken = _orderReceipt.MadeToken;
        if (madeToken == null)
            return;

        madeToken.SetQuest(this);
        QuestTokens.Add(madeToken);

        if (madeToken.GetTokenType().Equals(TokenType.Event))
        {
            Debug.Log("�̺�Ʈ Ÿ���̹Ƿ� ���ݴ� �۾��ʿ�");
            //���͸� ��ȯ�ϴ� �� ����
            TOrderItem monster1 = new TOrderItem((int)ETokenGroup.Charactor, 2, 3);
            List<TOrderItem> monsterOrderItemlist = new List<TOrderItem>() { monster1 };
            //������� ��ū �̺�Ʈ�� ����ȯ��
            TokenEvent eventToken = (TokenEvent)madeToken;
            eventToken.MakeEventContent(EOrderType.SpawnMonster, monsterOrderItemlist);
        }
    }

    #endregion

    public void UseTurn(int _count = 1)
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

    
}
public enum QuestType
{
    Battle, Action, Item
}

public class QuestCondition : IOrderCustomer
{
    //����Ʈ ����
    public EQuestType QuestType;
    public TTokenOrder TokenOrder;

    public QuestCondition(ContentData _questType, int _chunkNum)
    {
        OrderMaker orderMaker = new();
        TokenOrder = orderMaker.MakeOrder(_questType.ConditionType, _questType.ConditionMainItemList, this, _chunkNum);
    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt) //���� ��
    {
        Debug.Log("���� ��");
        //�����ͽ�ť�ͷ� ������ ��ū���� �ݹ������ �ű⿡ �ڽ��� �Ҵ� 
        TokenBase madeToken = _orderReceipt.MadeToken;
        if (madeToken == null)
            return;

        //�����, ���Ƽ �� �� �׹����� �ڽ��� �ֹ����� ������� ��ū�� ���¸� �����ϸ� ���� ����. 
        //madeToken.SetQuest(this);
        //QuestTokens.Add(madeToken);

        if (madeToken.GetTokenType().Equals(TokenType.Event))
        {
            Debug.Log("�̺�Ʈ Ÿ���̹Ƿ� ���ݴ� �۾��ʿ�");
            //���͸� ��ȯ�ϴ� �� ����
            TOrderItem monster1 = new TOrderItem((int)ETokenGroup.Charactor, 2, 3);
            List<TOrderItem> monsterOrderItemlist = new List<TOrderItem>() { monster1 };
            //������� ��ū �̺�Ʈ�� ����ȯ��
            TokenEvent eventToken = (TokenEvent)madeToken;
            eventToken.MakeEventContent(EOrderType.SpawnMonster, monsterOrderItemlist);
        }
    }
}