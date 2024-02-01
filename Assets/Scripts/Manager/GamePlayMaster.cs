using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayMaster : MgGeneric<GamePlayMaster>
{
    #region  ����
    public enum PlayerMember
    {
        LivePlayer, AI
    }
    public float m_moveSpeed = 0.5f;
    public const float c_movePrecision = 0.1f; //������ ���е�
    public Sprite[] m_testActionIcon;
    public bool m_testAuto = true;

    
    private GamePlayData m_playData = new();
    [SerializeField]
    private UIPlayData m_playDataUI;

    [SerializeField]
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0�� �÷��̾�, 1�� AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;

    
    [Header("[��Ʋ��]")]
    [SerializeField] private UIBattle m_battleUI;
    #region �ൿ ���� ����
    Queue<IEnumerator> actionReserVationQueue = new(); //������ �ڷ�ƾ
    private bool isPlayingCorutine = false; //�������� �ڷ�ƾ�� �ִ��� üũ
    private int m_FinalActionStep = 0;
    public float waitTime = 2f;
    TokenChar m_actionChar;
    #endregion

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
           m_playData.PlayTime = 0;
            m_playerMemeber = 0;
            NoticeTurnPlayer();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
            {
                TokenChar charToken = MgToken.GetInstance().GetNpcPlayerList()[i];
                MgToken.GetInstance().TempPosRandomPlayer(charToken);
            }
           
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            MgToken.GetInstance().MakeMap(); // �� �ٽø����
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if(m_playerMemeber.Equals(PlayerMember.LivePlayer))
            {
                if (m_players[(int)PlayerMember.LivePlayer].GetCurPlayStep() != GamePlayStep.ChooseChar)
                    return;
                //���� �÷��̾� �����϶� ������ ���̺� �÷��̾� �� ���� ����
                EndPlayerTurn();
                Debug.Log("�÷��̾� �� ���� ����");
            }
        }
    }

    #region �ʱ�ȭ �� ����
    public override void InitiSet()
    {
        base.InitiSet();
        RuleBook = new();
        RuleBook.m_PlayMaster = this;
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void FirstStart()
    {
        m_aiPlayer.SetInitial(); //mgtoken�� ��ū �� ����� ���� �����ؾ���.
        m_playerMemeber = PlayerMember.LivePlayer; //������ �÷��̾�
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        ReadyNextTurn();
    }
    #endregion

    #region �׼� ����
    //���Ӹ����ʹ� �÷��̾���� �����ᶧ���� ���, ������ ĳ���� �׼���ū�� ���ؼ� �䱸�ϸ� �ش� �׼��� �������ִ� ���� 

    //1. �ش� ���� �÷��̾�� �׼� �����϶�� ����
    public void NoticeTurnPlayer()
    {
        //�÷��̾� �������� �ִٰ� �����ϰ� ������� ���� - ����� 1.�� 2.AI �� ���� 
        int turn = (int)m_playerMemeber;
        m_players[turn].PlayTurn();

        //�ش� �÷������� �ɶ� ī�޶� ������ Ǯ�ų�, �ϵ���
        CamTraceOn(MgToken.GetInstance().GetNpcPlayerList()[turn]);
        CamTraceOff();
    }
 
    //2. ���� ���� ĳ���� �׼��� ���� ����
    public void PlayCharAction(TokenChar _charToken)
    {
        //ĳ���� ��ū�� �غ�ȴ�� ��ϰ����� �׼��� ����
        ResetEmphasize();
        _charToken.actionCount -= 1;
        _charToken.ShowAction(true);
        CamTraceOn(_charToken);
        RuleBook.PlayAction(_charToken);
    }

    //3. �׼� ���� �������� �ش� �÷��̾�� �ش� ĳ���� �׼Ǽ��� �������� �˸�. 
    public void DoneCharAction(TokenChar _charToken)
    {
        //   Debug.Log("<<color=red>������ :  " + _charToken.num + "�� �� ������ �ൿ���� �ֳ�?</color>");
        CamTraceOff();
        _charToken.ShowAction(false);
        RuleBook.OffRouteNumber();
        m_players[(int)m_playerMemeber].DoneCharAction(_charToken); //���� �÷��̾�� �ش� ĳ������ �׼��� �Ϸ�Ǿ����� �˸���.
        OccurMoveEvent(_charToken);
    }

    //4. �ش� �÷��̾ ���� �����ϸ�, ���� ���� �÷��̾ �̰ų� �������� ����
    public void EndPlayerTurn()
    {
        int cur = (int)m_playerMemeber;
        cur += 1; //��������

        if ((int)PlayerMember.AI < cur)
        {
            //���� ai ���� �������̶��
            ReadyNextTurn(); //�ش� �� ����� ������ �غ�
            return;
        }

        //�װ� �ƴ϶��
        m_playerMemeber = (PlayerMember)cur; //�� �÷��̾� ��� �ٲٰ�
        NoticeTurnPlayer();//���� ���� �÷��̾ ȣ�� 
    }

    #endregion

    #region �׼� ���� �� ���� 
    #region 1. ��Ͽ��� �׼� ������ �Ŵ� �κ� - �ִϸ��̼ǰ��� �ð��� �ʿ��� �׼��� �����Ƿ�, �ϳ��� ������ �ɾ�ΰ� �����ϴ� ���. 
    public void RservereInfo(TokenChar _actionChar, int maxStep)
    {
        m_actionChar = _actionChar;
        m_FinalActionStep = maxStep;
    }

    public void Reservateinstance(int curStep, Action effectAction)
    {
        //��� ���ֵǴ� �׼ǵ� �����ϴ� �κ�
        PlayReservation(curStep); //�׼� �÷��̸� �Ѵ�. 
    }

    public void ReservateMove(TokenChar _interViewChar, TokenTile _tile, int effectTiming, int curstep, Action<TokenChar, TokenTile> effectAction = null)
    {
        //��Ͽ��� �ش� pid �׼�Ÿ�Կ� �°� ������ ������ �����ϴ� �κ�
        //ex �̺κ��� �������� ���� �׼��� �����ϴ� �κ� 
        IEnumerator moveCoroutin = co_MoveAction(_interViewChar, _tile, effectTiming, curstep, effectAction);
        actionReserVationQueue.Enqueue(moveCoroutin);
        PlayReservation(curstep);
    }

    //������ �ƴϴ��� ��󿡰� ��� ȿ���� �ִ� ��쿡�� ���ص� �ɰ� ������
    public void ReservateAttack()
    {
        m_battleUI.Switch(true);
        //���� ��ū�� ��뿡�� ���� 
        //------------ ������
        //1. ������ū�� ��� + ����� ������ ���� ���� 
        //2. ������� ���� ���� ���� ������ ����
        //3. ������� �ݰ� ��� ���� - 
        //4. ���� ���� - ������ �κ�
        //------------ UI
        //1. ĳ���� ��ū�� ����, ���� ǥ�� 
        //2. ���� ���� ��ū ���� ���� ���� ǥ�� 
        //3. ���� ���� ���� ǥ�� 
        //-> Ȯ�δ����� �ݰ� ���� - ������ �ݺ� 
        //4. �� ��Ʈ ��� ���ͷ� ǥ�� - A�� B�� ���� - C�� D��ŭ ���� ��. 
        //5. ����Ȯ�ν� â ���� 


    }

    public void DoneReserve(int _curStep)
    {
        PlayReservation(_curStep); //������κ��� ������ ������ �޴� �κ� - 
        //������ ��߷� �̷��� �׼���ū�� ��� - ���ᰡ 2�� ȣ��� �� �ְڴ�. 
    }
    #endregion

    #region 2. �ൿ ���� �ڷ�ƾ
    //�׼Ǹ��� ���� �ִϸ��̼��� �ٸ��Ƿ� ���� ���� 
    IEnumerator co_MoveAction(TokenChar _char, TokenTile _goalTile, int effectTiming, int curStep, Action<TokenChar, TokenTile> effectAction)
    {
        //   Debug.Log("�̵� �ڷ�ƾ ���� �ܰ�" + m_MaxStep+"/ " + curStep);

        Vector3 goal = _goalTile.GetObject().transform.position;
        if (effectTiming == 0 && effectAction != null)
            effectAction(_char, _goalTile);

        if (effectTiming == 1 && effectAction != null)
            effectAction(_char, _goalTile);

        _char.SetState(CharState.Move);

        Vector3 dir = goal - _char.GetObject().transform.position;
        while (true)
        {
            if (Vector2.Distance(_char.GetObject().transform.position, goal) < c_movePrecision)
            {
                //Debug.Log("�Ÿ� ������� �ߴ�");
                break;
            }


            _char.GetObject().transform.position += (dir.normalized * m_moveSpeed * Time.deltaTime);
            yield return null;
        }

        _char.SetState(CharState.Idle);

        if (effectTiming == 2 && effectAction != null)
            effectAction(_char, _goalTile);

        yield return new WaitForSeconds(waitTime);
        DoneCoroutineAction(curStep);
    }

    IEnumerator co_AttackAction(TokenChar _char, int v, int d, int curStep)
    {
        DoneCoroutineAction(curStep);
        yield return null;

    }
    #endregion

    private void PlayReservation(int _curStep)
    {
        if (isPlayingCorutine == true)
            return; //�������� �ڷ�ƾ ������ �Ұ� ����. - ������ �߰� �����ϴ� ��쿡 �߻� 

        if (isPlayingCorutine == false && actionReserVationQueue.Count >= 1) //CheckMaxStep�� Reservate() �� ��ο��� ȣ��ǹǷ� �������� �ڷ�ƾ üũ �ʿ�. 
        {
            isPlayingCorutine = true;
            StartCoroutine(actionReserVationQueue.Dequeue()); //�����ɷ� ����
            return;
        }

        //�׼� �ܰ��� �ڷ�ƾ�� ���� step�� ��� ������ �� �����Ƿ�, �� ������ ���� �������� ������ �׼� ����.
        if (m_FinalActionStep == _curStep)
        {
            //�ش� �׼��� �ڷ�ƾ�� ���� �༮�ε�. 
            DoneCharAction(m_actionChar);
        }
    }

    private void DoneCoroutineAction(int curStep)
    {
        isPlayingCorutine = false; //�������̴� �ڷ�ƾ �����ٰ� üũ. -> ��� �Լ��� �̳༮�� ȣ���ؼ� �������̴� �ڷ�ƾ �ߴܽ�Ű�� �ȵ�. 
        PlayReservation(curStep); //��� ���� 
    }
    #endregion // �׼� ���� �� ���� 

    #region Ÿ�� �׼� ����
    public void PlayTileAction(TokenTile _tile, string _action)
    {

        if (_action.Equals(TileAction.Grass.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Grass, 50, true);
            return;
        }
        if (_action.Equals(TileAction.Mineral.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Stone, 50, true);
            return;
        }
        if (_action.Equals(TileAction.RemoveMineral.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Stone, -50, true);
            return;
        }
        if (_action.Equals(TileAction.RemoveGrass.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Grass, -50, true);
            return;
        }

    }
    #endregion

    #region ��� �߻�
    public void SelectEvent(TokenEvent _eventToken)
    {
        AnnounceState("�÷��̾ ������ �̺�Ʈ�� " + _eventToken.GetPid());
        RuleBook.AdaptEvent(_eventToken);
    }

    private void OccurMoveEvent(TokenChar _charToken)
    {
        if (IsPlayerMoveDone(_charToken) == false)
            return;
        //���� �÷��̾� ĳ���Ͱ� �̵��ѰŶ�� �ش� ��ġ���� �ٽ� �Ȱ� ����
        FogContorl(_charToken);

        //���� ���ÿ� �߹��ϴ� �̺�Ʈ�� ������ ����
        TokenEvent enterEvent = RuleBook.CheckEnteranceEvent(_charToken.GetMapIndex());
        if (enterEvent != null)
        {
            RuleBook.PlayEntranceEvent(enterEvent);
            return;
        }
        else
        {
            Debug.Log("���� �̺�Ʈ ����");
        }

        //���ٸ� ������ ���� Ȯ���� �̺�Ʈ ����
        RuleBook.OnTileArrive(_charToken);
    }

    private bool IsPlayerMoveDone(TokenChar _charToken)
    {
        //�÷��̾��� ���� ĳ���͸鼭 �ֱ��� �ൿ�� Move�� ��� �̺�Ʈ �߻�
        if (_charToken.isMainChar == false)
            return false;
        if (_charToken.GetNextActionToken().GetActionType().Equals(ActionType.Move) == false)
            return false;

        return true;
    }

    #endregion

    #region �� ���� �κ�
    /*
     *  �� �ѱ� ���� 
        �÷��̾� �� ��ȭ�� ���� ����
        ������Ʈ�� �� ��ȭ�� ���� �׼� ��ȭ ����
     */

    private void ReadyNextTurn()
    {
        SettleActionTurn(); //�׼� �� ����
        SettleWorldTurn(); //���� �� ����
        SettingPlayerTurn(); //�÷��̾� ������ ����
        SetPlayDataUI(); //�÷��� ������ ����
        StartActionTurn(); //�׼� �� ����
    }

    private void SettleActionTurn()
    {
        m_playData.PlayTime += 1; //���� ������ ��
       // Debug.Log("�ൿ Ƚ�� �ʱ�ȭ");
        RecoverActionCount();
      //  Debug.Log("������ �÷��̾� �ѹ� �ʱ�ȭ");
        m_playerMemeber = 0;
    }
    private void RecoverActionCount()
    {
       AnnounceState("�Һ�� �׼� ī��Ʈ ȸ��");
        for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
        {
            MgToken.GetInstance().GetNpcPlayerList()[i].actionCount = i + 1; //
        }
    }
    private void SettleWorldTurn()
    {
        AnnounceState("������ ��ȭ ���� ���� ����");
      
    }

    private void SetPlayDataUI()
    {
        AnnounceState("�÷��� ������ ����");
        m_playDataUI.ShowPlayData(m_playData);
    }

    private void SettingPlayerTurn()
    {
        //AI �ϱ��� ������ �ٽ� �����÷��̾� ������ �ʱ�ȭ �ؼ� ���� 
        m_playerMemeber = PlayerMember.LivePlayer; //ó�� ������ �÷��̾� �ڵ� ����. 
        
    }

    private void StartActionTurn()
    {
        Invoke(nameof(NoticeTurnPlayer), 0.2f); //�ٽ� ����
    }
    #endregion

    #region �ΰ� ���� ���
    public void FogContorl(TokenChar _char)
    {
        //�ϴ� �Ȱ� �ȴ� ��ɸ�
        int tempSight = 2;
        List<TokenTile> tiles = GameUtil.GetTileTokenListInRange(tempSight, _char.GetXIndex(), _char.GetYIndex());
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].ChangeViewState(TileViewState.Sight);
        }
    }

    public void EmphasizeTarget(TokenBase _token)
    {
        EmphasizeTool.Emphasize(_token);
    }

    public void EmphasizeTargetObject(int _centerX, int _centerY, TokenAction _actionToken)
    {
        //��ū������Ʈ�� �����ϴ� �κ� (UI �κ��� UIPlayGame ���� ����)
        List<ObjectTokenBase> objList = GameUtil.GetTokenObjectInRange(_actionToken.GetStat(ActionStat.Range), _centerX, _centerY);
        EmphasizeTool.Emphasize(objList);
    }

    public void ResetEmphasize()
    {
        EmphasizeTool.ResetEmphasize(); //���ý� �ʿ��ߴ� �����κ��� �ʱ�ȭ 
    }

    public void CamTraceOn(TokenChar _char)
    {
        m_camFollow.SetTarget(_char.GetObject());
        m_camFollow.TraceOnOff(true);
    }

    public void CamTraceOff()
    {
        m_camFollow.TraceOnOff(false);
    }

    public void AnnounceState(string message)
    {
      //  Debug.Log(message);
      // üũ �����
    }

    public bool AdaptActionCount = false;
    public bool AdaptEvent = false;

    #endregion

 }

public enum GamePlayStep
{
    //���Ӹ����Ͱ� ������ �����ϸ鼭 ���� ������ ���� 
    ChooseChar, SelectAct, FillContent, CheckDecision, PlayAction, TriggerEvent
}

public class GamePlayData
{
    public int PlayTime = 0;
}