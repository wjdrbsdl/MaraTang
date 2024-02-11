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
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0�� �÷��̾�, 1�� AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;

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
            if (m_playerMemeber.Equals(PlayerMember.LivePlayer))
            {
                if (m_players[(int)PlayerMember.LivePlayer].GetCurPlayStep() != GamePlayStep.ChooseChar)
                    return;
                //���� �÷��̾� �����϶� ������ ���̺� �÷��̾� �� ���� ����
                PlayerManager.GetInstance().EndTurn();
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
        Announcer play = new();
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void FirstStart()
    {
        Debug.Log("ó�� ����");
        RuleBook.ParseTileActions();
        m_aiPlayer.SetInitial(); //mgtoken�� ��ū �� ����� ���� �����ؾ���.
        m_playerMemeber = PlayerMember.LivePlayer; //������ �÷��̾�
        //�÷��̾� �Ҵ�
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        //�� �غ�
        ReadyNextTurn();
        //ó�� ���۽� �÷��̾� ����ĳ���Ϳ� ī�޶� ��Ŀ��.
        GamePlayMaster.GetInstance().CamFocus(PlayerManager.GetInstance().GetMainChar());
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
       //CamFocus(MgToken.GetInstance().GetNpcPlayerList()[turn]);

    }

    //2. ���� ���� ĳ���� �׼��� ���� ����
    public void PlayCharAction(TokenChar _charToken)
    {
        //ĳ���� ��ū�� �غ�ȴ�� ��ϰ����� �׼��� ����
        ResetEmphasize();
        _charToken.actionCount -= 1;
        _charToken.ShowAction(true);
        RuleBook.ReadCharAction(_charToken); //��Ͽ� �׼� ������Ʈ �б�
    }

    //3. TokenObject �ִϸ��̼� ����
    private IEnumerator m_tokenActionCoroutine;
    private Action m_actionEffect;
    [SerializeField] private bool m_isPlayAnimate = true;
    public void AnimateTokenObject(IEnumerator _aniCoroutine, Action _actionEffect, TokenChar _playChar)
    {
        //�������̴� �ڷ�ƾ�� �ִٸ� ����
        StopAnimateTokenObject();

        m_actionEffect = _actionEffect;
        m_tokenActionCoroutine = _aniCoroutine;
        //�ڷ�ƾ�� ������ �����ϰ�
        if (m_tokenActionCoroutine != null && m_isPlayAnimate)
        {
            StartCoroutine(m_tokenActionCoroutine);
            return;
        }
        //������ ȿ���� ������ ����
        if (m_actionEffect != null)
            m_actionEffect();

        //�ڷ�ƾ�� �ƴ� ���⼭ �׼� �������� ����
        DoneCharAction(_playChar);
    }

    //4. �׼� ���� �������� �ش� �÷��̾�� �ش� ĳ���� �׼Ǽ��� �������� �˸�. 
    public void DoneCharAction(TokenChar _charToken)
    {
        //   Debug.Log("<<color=red>������ :  " + _charToken.num + "�� �� ������ �ൿ���� �ֳ�?</color>");
        ResetAnimateValue();
        CamTraceOff();
        _charToken.SetState(CharState.Idle);
        _charToken.ShowAction(false);
        m_players[(int)m_playerMemeber].DoneCharAction(_charToken); //���� �÷��̾�� �ش� ĳ������ �׼��� �Ϸ�Ǿ����� �˸���.
        OccurMoveEvent(_charToken);
    }

    //5. �ش� �÷��̾ ���� �����ϸ�, ���� ���� �÷��̾ �̰ų� �������� ����
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

    public void StopAnimateTokenObject()
    {
        //�ڷ�ƾ �������ΰ� ���߱�
        if (m_tokenActionCoroutine != null)
        {
            //�ڷ�ƾ �ߴ�
            StopCoroutine(m_tokenActionCoroutine);
            //ȿ�� �ٷ� ����
            if (m_actionEffect != null)
                m_actionEffect();
        }
        ResetAnimateValue();
    }

    private void ResetAnimateValue()
    {
        m_tokenActionCoroutine = null;
        m_actionEffect = null;
    }

    #endregion
   
    #region Ÿ�� �׼� ����
    public void PlayTileAction(TokenTile _tile, TokenAction _action)
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

    #region �̺�Ʈ �߻�
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
      
        //Debug.Log("���� �̺�Ʈ ����");
       
        //���ٸ� ������ ���� Ȯ���� �̺�Ʈ ����
        int setCount = UnityEngine.Random.Range(1, 100);
        if(setCount >= 90)
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
            MgToken.GetInstance().GetNpcPlayerList()[i].RecoverActionTokenCount();
        }
    }
    private void SettleWorldTurn()
    {
        AnnounceState("������ ��ȭ ���� ���� ����");
      
    }

    private void SetPlayDataUI()
    {
        AnnounceState("�÷��� ������ ����");
        PlayerManager.GetInstance().OnChangePlayData();
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

    #region ������Ʈ ����
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
    #endregion

    #region ī�޶� ����
    public void CamFocus(TokenChar _char)
    {
        //�ش� ���� ��Ŀ��
        m_camFollow.FocusTarget(_char.GetObject());
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
    #endregion

    public void AnnounceState(string message)
    {
        //  Debug.Log(message);
        Announcer.Instance.AnnounceState(message);
    }

    public bool TempAdaptActionCount = false;
    public bool TempAdaptEvent = false;

    #endregion

    public GamePlayData GetPlayData()
    {
        return m_playData;
    }
 }

public enum GamePlayStep
{
    //���Ӹ����Ͱ� ������ �����ϸ鼭 ���� ������ ���� 
    ChooseChar, SelectAct, FillContent, CheckDecision, PlayAction, TriggerEvent, EndTurn
}

public class GamePlayData
{
    public int PlayTime = 0;
}