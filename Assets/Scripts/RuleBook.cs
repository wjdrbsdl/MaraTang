using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBook
{
    public static GamePlayMaster m_PlayMaster;
    private RouteDisplay m_routeDisplayTool = new();
  
    #region �׼� ���� ����
    public void ReadCharAction(TokenChar _playChar)
    {
        TokenAction action = _playChar.GetNextActionToken();
        ActionType actionType = action.GetActionType();
        //1. �׼���ū Ƚ�� ����
        action.CalStat(ActionStat.RemainCountInTurn, -1); //�׼���ū�� ��� Ƚ�� ����

        int[] targetPos = action.GetTargetPos();

        Action effectDelegate = null;
        IEnumerator animateCoroutine = null;

        //2. ������ Ÿ�� ���� ���� ������ ���� �����Ͽ� ����
        if (actionType == ActionType.Attack)
        {
            Debug.Log("���� ���� �����Ѵ�");
            //������
            //0. ������ �巯���� �׼��� 1��. �ֵθ��ų� ��ų� �߻��ϰų� 
            //1. �ش� ���ݾ׼��� ������ ����
            TokenTile targetTile = GameUtil.GetTileTokenFromMap(targetPos);
            //2. ���� ���� Ÿ���� ������
            List<TokenChar> enemies = targetTile.GetCharsInTile();
            //3. �ش� Ÿ�ٿ��� �ش� ������ ȿ���� ���� 
            effectDelegate = delegate
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    Debug.Log(_playChar.GetItemName() + "�� " + enemies[i].GetItemName() + "�� ����");
                }
            };
            animateCoroutine = co_AttacAction(_playChar, effectDelegate);

        }
        //3. �̵��� Ÿ�� ���� ��ġ�� �̵�
        else if (actionType == ActionType.Move)
        {
            TokenTile targetTile = GameUtil.GetTileTokenFromMap(targetPos);
            effectDelegate = delegate 
            { 
                Migrate(_playChar, targetTile); 
            };
            animateCoroutine = co_MoveAction(_playChar, targetTile, effectDelegate);
        }
        GamePlayMaster.GetInstance().AnimateTokenObject(animateCoroutine, effectDelegate, _playChar);
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //�ش� Ÿ�Ϸ� �ش� ĳ���͸� ���� ��Ű�� 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].Immigrate(_char); //�̻� ������
        _targetTile.Migrate(_char); //�̻� �ְ� 
        _char.GetObject().SyncObjectPosition();
    }

    IEnumerator co_MoveAction(TokenChar _char, TokenTile _goalTile, Action effectAction)
    {
        //   Debug.Log("�̵� �ڷ�ƾ ���� �ܰ�" + m_MaxStep+"/ " + curStep);

        Vector3 goal = _goalTile.GetObject().transform.position;

        _char.SetState(CharState.Move);

        Vector3 dir = goal - _char.GetObject().transform.position;
        while (true)
        {
            if (Vector2.Distance(_char.GetObject().transform.position, goal) < GamePlayMaster.c_movePrecision)
            {
                //Debug.Log("�Ÿ� ������� �ߴ�");
                break;
            }


            _char.GetObject().transform.position += (dir.normalized * GamePlayMaster.GetInstance().m_moveSpeed * Time.deltaTime);
            yield return null;
        }

        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);
    }

    IEnumerator co_AttacAction(TokenChar _char, Action effectAction)
    {
        //   Debug.Log("�̵� �ڷ�ƾ ���� �ܰ�" + m_MaxStep+"/ " + curStep);
        _char.SetState(CharState.Move);
        float waitTime = 1f;
        while (waitTime>0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);

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
        if (_action.GetTargetPos() == null)
        {
            m_PlayMaster.AnnounceState("Ÿ���� ����Ȯ");
            return false;
        }
            

        return true;
    }
    #endregion

    #region �̺�Ʈ �߻� 
    public void PlayEntranceEvent(TokenEvent _eneterEvent)
    {
        //Ÿ�Ͽ� �̵��� �͸����� �߻��ϴ� �̺�Ʈ
        Debug.Log("����� �̺�Ʈ �߻�");
    }

    public TokenEvent CheckEnteranceEvent(int[] mapCoordi)
    {
        //Ÿ�Ͽ� ��߿� �̺�Ʈ�� �ִ��� Ȯ��
        TokenTile mapTile = GameUtil.GetTileTokenFromMap(mapCoordi);
        //�ʿ� �ִ��� �������� ��ȯ
        TokenEvent enterEvent = mapTile.GetEneteranceEvent();

        return enterEvent;
    }

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
        PlayerManager.GetInstance().OnTriggerEvent(eventList);
    }

    #endregion

    #region �̺�Ʈ ����
    public void AdaptEvent(TokenEvent _event)
    {
        //��Ͽ��� ���� �����ͻ� ��� �̺�Ʈ �����ϰ�
        //�̺�Ʈ �߻� -> �÷��̾�� �̺�Ʈ ����â ��� -> �÷��̾ ���� -> ���õ� �̺�Ʈ ��Ͽ� ���� -> ��Ͽ��� ���� -> �÷��̾�� ����Ȱž˸�

        //������ AI���� ���ϰ� Player�� �ϴϱ�
        
        PlayerManager.GetInstance().AdaptCapitalStat(Capital.Grass, 50, true);
        PlayerManager.GetInstance().DoneAdaptEvent(); //�������� �̺�Ʈ ���볡������ �÷��̾�� ����. 
    }
    #endregion
}

