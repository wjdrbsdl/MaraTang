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
    public int TempMissonType = 5; //���� ���� 
    public int TempCompleteCode = 5; //�Ϸ� ����
    public int TempRewardCode = 5; //����
    public List<TokenBase> TempQuestTokens; //����Ʈ�� ���õ� ��ū�� 

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
        //���� ����� �˸��޴� ��� 
        Debug.Log(_token.GetItemName() + " ��ū" + _concludeCode + "�ڵ� ȣ��");
        CheckQuestComplete();
    }

    public void RemoveQuest()
    {

    }

    private void CheckQuestComplete()
    {
        //��ū�� ȣ��� ���� ��� �ڵ带 ����ϰ� ����Ʈ �Ϸ� ���θ� üũ�Ѵ�. 

    }
}
