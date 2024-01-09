using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBook
{
    public static GamePlayMaster m_PlayMaster;
    private RouteDisplay m_routeDisplayTool = new();
  
    #region �׼� ���� ����
    public void PlayAction(TokenChar _char)
    {
        //��Ͽ��� ����ܰ�
        //0. �ش� �׼��� ��ܰ����� �ѱ��
        //1. �ش� �׼��� �����ϱ� ���� ������ �Ǵ�
        //2. ����� �༮�� �ٷ� ������ �ȴ�.
        //3. ��Ͽ��� �׼��� ��� ������ �Ǵ�. 
        //4. �����ϴ� �ڷ�ƾ�� ����Ǹ� ������ �������� üũ�Ͽ� DoneAction�� ȣ���Ѵ�. 

        TokenAction action = _char.GetNextActionToken();
        ActionType actionType = action.GetActionType();
        int curStep = 0;
        if (actionType == ActionType.Attack)
        {
            Debug.Log("���� ���� �����Ѵ�");
            //�ľ��ص� ĳ������Ʈ �� ������� Ȥ�� �� �߿� �������� �̱�
            List<TokenChar> charList = action.GetTargetList().ConvertAll(tokenBase => (TokenChar)tokenBase);//��� ��Ҹ� Char�� ��ȯ
            m_PlayMaster.RservereInfo(_char, 0);
            for (int i = 0; i < charList.Count; i++)
            {
                //1. _char�� ������ _charList[i]�� �����ڷ� ���� ��Ģ ���� 
           
            }
        }
        else if (actionType == ActionType.Move)
        {
            //Debug.Log("�̵� ���� �����Ѵ�"+_char.charNum);
            //Ÿ�ٸ���Ʈ�� ���� Ÿ�Ϸ� ���� �̵��� �Ͼ�� �κ� 
            
            List<TokenTile> targetTile = action.GetTargetList().ConvertAll(tokenBase => (TokenTile)tokenBase);//��� ��Ҹ� Tile�� ��ȯ
            ShowRouteNumber(targetTile);
            m_PlayMaster.RservereInfo(_char, targetTile.Count);
            int tempTiming = 2;
            for (int i = 0; i < targetTile.Count; i++)
            {
                curStep += 1;
                m_PlayMaster.ReservateMove(_char, targetTile[i], tempTiming, curStep, Migrate);
                
            }
         
        }
        m_PlayMaster.DoneReserve(curStep); //�ƹ� ���൵ ���� ���� ��Ȳ�� ��� �ƴ��� �׳� DoneReserve ȣ���� ���ڴ�.
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //�ش� Ÿ�Ϸ� �ش� ĳ���͸� ���� ��Ű�� 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].Immigrate(_char); //�̻� ������
        _targetTile.Migrate(_char); //�̻� �ְ� 
    }

    public void ShowRouteNumber(List<TokenTile> _tiles)
    {
        m_routeDisplayTool.ShowRoute(_tiles);
    }

    public void OffRouteNumber()
    {
        m_routeDisplayTool.ResetPreRoute();
    }
    #endregion

    #region ���� üũ 
    public bool IsInRangeTarget(TokenChar _char, TokenAction _action, TokenBase _target)
    {
        TMapIndex mapIndex = new TMapIndex(_char, _target);
        int targetRange = GameUtil.GetMinRange(mapIndex);

        //Debug.Log(_char.GetXIndex() + "," + _char.GetYIndex() + "���� " + _target.GetXIndex() + "," + _target.GetYIndex() + "�Ÿ��� " + targetRange);
        if (_action.GetStat(ActionStat.Range) < targetRange)
           return false;

        return true;
    }

    public bool IsMatchTargetType(TokenAction _action, TokenBase _target)
    {
        if (_action.GetTargetType().Equals(_target.GetTokenType()) == false)
            return false;

        return true;
    }

    public bool CheckUsableToken(TokenChar _char, TokenAction _action)
    {
        if(_char.GetActionCount()<= 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckActionContent(TokenChar _char, TokenAction _action)
    {
        if (_action.GetTargetList().Count == 0)
        {
            m_PlayMaster.AnnounceState("Ÿ���� ����Ȯ");
            return false;
        }
            

        return true;
    }
    #endregion

    #region �̺�Ʈ �߻� 

    public void OnTileArrive(TokenChar _char)
    {
        //�÷��̾� ���� ĳ���Ͱ� ���ο� Ÿ�Ͽ� ������ ��� 
        TokenTile arriveTile = GameUtil.GetTileTokenFromMap(_char);

        //�� �Ӽ����� ���� �̺�Ʈ �߻� 
        TokenEvent event1 = new TokenEvent(1,2);
        TokenEvent event2 = new TokenEvent(2, 2);
        TokenEvent event3 = new TokenEvent(3, 2);

        List<TokenEvent> eventList = new List<TokenEvent>() { event1, event2, event3 };
        if(m_PlayMaster.AdaptEvent)
        PlayerManager.g_instance.OnTriggerEvent(eventList);
    }

    #endregion

    #region �̺�Ʈ ����
    public void AdaptEvent(TokenEvent _event)
    {
        //��Ͽ��� ���� �����ͻ� ��� �̺�Ʈ �����ϰ�
        // �̺�Ʈ �߻� -> �÷��̾�� �̺�Ʈ ����â ��� -> �÷��̾ ���� -> ���õ� �̺�Ʈ ��Ͽ� ���� -> ��Ͽ��� ���� -> �÷��̾�� ����Ȱž˸�


        PlayerManager.g_instance.OnAdaptEvent(); //�������� �̺�Ʈ ���볡������ �÷��̾�� ����. 
    }
    #endregion
}

