using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : IOrderCustomer
{
    //����
    //Ŭ��������
    //������ ����� ����Ʈ
    public int QuestPid = 0; //�ش� ����Ʈ pid mgContet ����Ʈ�� �߰��Ǵ� ������ �ε�����
    public int RestWoldTurn = 3; //�����Ǵ� �Ⱓ 
    public int ChunkNum = 0;
    public int CurStep = 1;
    public ContentData ContentData;
    public List<TokenBase> QuestTokens = new(); //����Ʈ�� ���õ� ��ū�� 

    #region ����
    public Quest()
    {
       
    }

    public Quest(ContentData _contentData, int _chunkNum)
    {
        QuestPid = _contentData.ContentPid;
        ContentData = _contentData;
       ChunkNum = _chunkNum;

    }
    #endregion

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
     
    }
}

