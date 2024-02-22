using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    //����
    //Ŭ��������
    //������ ����� ����Ʈ
    public int QuestPid = 0; //�ش� ����Ʈ pid mgContet ����Ʈ�� �߰��Ǵ� ������ �ε�����
    public int RestWoldTurn = 5; //�����Ǵ� �Ⱓ 
    public int TempCompleteCode = 5; //�Ϸ� ����
    public int ChunkNum = 0;
    public QuestCondition condition; //���� ����
    public RewardData reward; //����
    public List<TokenBase> TempQuestTokens = new(); //����Ʈ�� ���õ� ��ū�� 

    #region ����
    public Quest()
    {
        reward = new RewardData(RewardType.None); //�ӽ÷� �ڿ� ����
    }

    public Quest(int _monsterPID, int _monsterCount, RewardType _rewardType)
    {
        QuestPid = MGContent.g_instance.GetQuestList().Count;
        condition = new QuestCondition(_monsterPID, _monsterCount);
        reward = new RewardData(_rewardType); //�ӽ÷� �ڿ� ����
    }
    #endregion

    public void RemoveTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //token�� Ÿ�Կ� ���� ��� �ڵ� ����
        int resultCode = 0;
        TokenType type = _token.GetTokenType();
        if (type.Equals(TokenType.Char))
        {
            //������ ��� ��ū�� ���¿� ���� �ڵ带 ���� ���� - �� �׾��� ���, � ������ ��� ���� �ڵ带 ���� �س�����.
            resultCode = 5;
        }
        CheckCallBackCode(_token, resultCode);
    }

    public void CheckCallBackCode(TokenBase _token, int _concludeCode)
    {
        //���޹��� �ڵ��, ����Ʈ ���ǰ� ���� 
   
        //����� ���� ������� ��ƾ��� ���� ����Ʈ���� �����ϴ� ��
        TempQuestTokens.Remove((TokenChar)_token);
        CheckQuestComplete();
    }

    private void CheckQuestComplete()
    {
        //��ū�� ȣ��� ���� ��� �ڵ带 ����ϰ� ����Ʈ �Ϸ� ���θ� üũ�Ѵ�. 
        
        if (TempQuestTokens.Count == 0)
        {
            Debug.Log("������ ����");
            GamePlayMaster.GetInstance().RewardQuest(this);
        }
            
    }
}
public enum QuestType
{
    Battle, Action, Item
}

public class QuestCondition
{
    //���� ���� ����
    public int monsterPID;
    public int monsterCount;

    //�����ؾ��� �׼� ����

    //�����ؾ��� ������ ����

    public QuestCondition(int _monsterPID, int _monsterCount)
    {
        monsterPID = _monsterPID;
        monsterCount = _monsterCount;
    }

}