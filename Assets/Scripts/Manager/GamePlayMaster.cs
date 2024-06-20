using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayMaster : MgGeneric<GamePlayMaster>, IOrderCustomer
{
    public MagnetItem testMangetSample;
    public NaviPin testNaviPin;
    #region  ����
    public enum PlayerMember
    {
        LivePlayer, AI
    }
    public float m_moveSpeed = 0.5f;
    public const float c_movePrecision = 0.1f; //������ ���е�
    public bool m_testAuto = true;

    private GamePlayData m_playData = new();

    [SerializeField]
    private CameraFollow m_camFollow;
    private int m_turnNationNumber = 0; //���� ���� ���� �ѹ� 
    private CharTurnStep m_playStep = CharTurnStep.ChooseNation;
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
            //MgToken.GetInstance().MakeMap(); // �� �ٽø����
            //FirstStart();
            MgGameLoader.GetInstance().SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
           
            MgUI.GetInstance().ShowQuestList();

        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            //Debug.Log("�÷��̾� �� ���� ����");
            PlayerManager.GetInstance().EndTurn();
        }
    }

    #region �ʱ�ȭ �� ����
    public override void ManageInitiSet()
    {
        base.ManageInitiSet();
        RuleBook = new();
        RuleBook.m_PlayMaster = this;
        Announcer play = new(); // �Ƴ�� �̱������
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void GameInitialSetting()
    {
      //  Debug.Log("ó�� ����");
        m_aiPlayer.SetInitial(); //mgtoken�� ��ū �� ����� ���� �����ؾ���.
        m_playerMemeber = PlayerMember.LivePlayer; //������ �÷��̾�
        //�÷��̾� �Ҵ�
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        //�� �غ�
        ReadyNextTurn();
        //ó�� ���۽� �÷��̾� ����ĳ���Ϳ� ī�޶� ��Ŀ��.
        CamFocus(PlayerManager.GetInstance().GetMainChar());
        PlayerManager.GetInstance().FirstStart();
        SelectFirstNation();
        DoneStep(GamePlayStep.GameInitialSetting); //���� �ʱ� ���� ������ ȣ��
    }

    private int TempNationSelectOrderSerialNum = 1;
    private void SelectFirstNation()
    {
        //�ʱ� ����� �߻��� �������� �÷��̾ ������ ������ ������ ��. 
        int nationNumber = MgNation.GetInstance().GetNationList().Count;
        List<TOrderItem> nationItemList = new();
        int tempOrderNum = TempNationSelectOrderSerialNum; //���� �ֹ����� ���������� �����ϱ� ���� �ø��� �ѹ�. 
        for (int i = 1; i <= nationNumber; i++)
        {
            TOrderItem nationItem = new TOrderItem(TokenType.Nation, i, i); //�ش� ������ ���������� ���� 
            nationItemList.Add(nationItem);
        }
        OrderExcutor excutor = new();
        TTokenOrder nationSelectOrder = new TTokenOrder().Select(EOrderType.ItemSelect, nationItemList, 0, tempOrderNum);
        nationSelectOrder.SetOrderCustomer(this);
        excutor.ExcuteOrder(nationSelectOrder);
    }
    #endregion

    #region �׼� ����
    //���Ӹ����ʹ� �÷��̾���� �����ᶧ���� ���, ������ ĳ���� �׼���ū�� ���ؼ� �䱸�ϸ� �ش� �׼��� �������ִ� ���� 

    //1. �ش� ���� �÷��̾�� �׼� �����϶�� ����
    public bool TracePlayChar = false;
    public void NoticeTurnPlayer()
    {
        //�÷��̾� �������� �ִٰ� �����ϰ� ������� ���� - ����� 1.�� 2.AI �� ���� 
        int turn = (int)m_playerMemeber;
        m_players[turn].PlayTurn();

        //�ش� �÷������� �ɶ� ī�޶� ������ Ǯ�ų�, �ϵ���
        if(TracePlayChar == true)
        CamFocus(MgToken.GetInstance().GetNpcPlayerList()[turn]);

    }

    //2. ���� ���� ĳ���� �׼��� ���� ����
    public void PlayCharAction(TokenChar _charToken)
    {
        //ĳ���� ��ū�� �غ�ȴ�� ��ϰ����� �׼��� ����
        ResetEmphasize();
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
        MGContent.g_instance.WriteContentWhenCharAction(_charToken, _charToken.GetNextAction());
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
            DoneStep(GamePlayStep.CharTurn); //EndPlayTurn���� ��� ĳ�� �� ������ ȣ��
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
   
    #region UI �׼� ����
    public void PlayTileAction(TokenTile _tile, TokenAction _action)
    {
        //Ÿ�� �׼��� �����ڴ� CharMain.
        RuleBook.ConductTileAction(_tile, _action);
    }

    public void IntenseStat(TokenChar _char, CharStat _stat)
    {
        RuleBook.IntenseStat(_char, _stat);
    }

    public void ClickOccupy(TokenTile _tile)
    {
        Debug.Log("�ش� Ÿ�� ������.");
        int tempNationNum = 0; //
        MgNation.GetInstance().AddTerritoryToNation(tempNationNum, _tile);
    }

    #endregion

    #region �̺�Ʈ �߻�

    private void OccurMoveEvent(TokenChar _charToken)
    {
        if (IsPlayerMoveDone(_charToken) == false)
            return;
        //���� �÷��̾� ĳ���Ͱ� �̵��ѰŶ�� �ش� ��ġ���� �ٽ� �Ȱ� ����
        FogContorl(_charToken);

        //1. ���� �̺�Ʈ�� �ִ°�
        TokenEvent enterEvent = RuleBook.CheckEnteranceEvent(_charToken.GetMapIndex());
        if (enterEvent != null)
        {
            enterEvent.ActiveEvent();
            return;
        }
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

    #region �� ����
    private void ReadyNextTurn()
    {
        PopupMessage("�� ����"); 
        EffectEndTurn(); //���� �����Ҷ� ���ֵǴ� ȿ���� ����Ұ͵� ����
        RecoverResource(); //�Һ�Ǿ��� �׼� �������� ȸ��
        SettleWorldTurn(); //���� �� ��ȭ ����
        ResetNationTurn(); //���� ������� ����
        ResetPlayerTurn(); //�÷��̾� ������ ����
        ResetPlayDataUI(); //�÷��� ������ ����
        EffectStartTurn(); //�� ���۽� ���ֵǴ� ȿ�� ����
        DoneStep(GamePlayStep.ReadyNextTurn); //ReadyNextTurn()���� �� ���� ������ ȣ�� 
     
    }

    private void PopupMessage(string message)
    {
        AlarmPopup.GetInstance().PopUpMessage(message);
    }

    private void EffectEndTurn()
    {
        m_playerMemeber = 0;
        //ȿ�� ��ū�� ���� ������ ���� �Ǵ� �κ� ����
    }

    private void RecoverResource() 
    {
        //�Ͽ� �Һ�Ǿ��� �ڿ��� ȸ��
        // Debug.Log("�ൿ Ƚ�� �ʱ�ȭ");
        RecoverActionCount();
        RecoverActionEnergy();
    }

    private void RecoverActionCount()
    {
       AnnounceState("�Һ�� �׼� ī��Ʈ ȸ��");
        for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
        {
            MgToken.GetInstance().GetNpcPlayerList()[i].RecoverActionCount(); //
            MgToken.GetInstance().GetNpcPlayerList()[i].RecoverActionTokenCount();
        }
    }

    private void RecoverActionEnergy()
    {
        for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
        {
            MgToken.GetInstance().GetNpcPlayerList()[i].RecoverActionEnergy();
        }
    }

    private void SettleWorldTurn()
    {
        AnnounceState("������ ��ȭ ���� ���� ����");
        m_playData.PlayTime += 1; //���� ������ ��
        //������ ���� ���� �߻� 
        MGContent.g_instance.WriteContentWhenNextTurn();
        MgNation.GetInstance().ManageNationTurn();
       

    }

    private void ResetNationTurn()
    {
        m_turnNationNumber = 0;
    }

    private void ResetPlayerTurn()
    {
        //AI �ϱ��� ������ �ٽ� �����÷��̾� ������ �ʱ�ȭ �ؼ� ���� 
        m_playerMemeber = PlayerMember.LivePlayer; //ó�� ������ �÷��̾� �ڵ� ����. 
    }

    private void ResetPlayDataUI()
    {
        AnnounceState("�÷��� ������ ����");
        PlayerManager.GetInstance().OnChangePlayData();
    }

    private void EffectStartTurn()
    {

    }

    private void ResetPlayStep(CharTurnStep _step  = CharTurnStep.ChooseNation)
    {
        m_playStep = CharTurnStep.ChooseNation;
    }
    #endregion

    #region �� ����
    public float startTermTime = 0.3f;
    
     public void DoneStep(GamePlayStep _step)
    {
        switch(_step)
        {
            case GamePlayStep.GameInitialSetting:
                StartCharTurn(); //�÷��� �� ���� ����. 
                break;
            case GamePlayStep.ReadyNextTurn:
                //�������� �غ� ������ ���� ���� ����
                StartNationTurn();
                break;
            case GamePlayStep.NationTurn:
                StartCharTurn();
                break;
            case GamePlayStep.CharTurn:
                ReadyNextTurn();
                break;
        }
    }

    private void StartNationTurn()
    {
        //���� �Ŵ����� ���� ���� �ൿ�� ����
        MgNation.GetInstance().ManageNationTurn();
        //���� �Ŵ������� ���� �ൿ�� ������ ���÷��� �����Ϳ��� ���� �ൿ�� �������� �ݹ�?
    }

    private void StartCharTurn()
    {
        Invoke(nameof(NoticeTurnPlayer), startTermTime); //ĳ���� �׼� �� ����
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

    public void EmphasizeTargetTileObject(TokenChar _char, TokenAction _actionToken)
    {
        //��ū������Ʈ�� �����ϴ� �κ� (UI �κ��� UIPlayGame ���� ����)
        List<ObjectTokenBase> tileObjList = GameUtil.GetTileObjectInRange(_actionToken.GetFinalRange(_char), _char.GetXIndex(), _char.GetYIndex(), _actionToken.GetStat(CharActionStat.MinRange));
        EmphasizeTool.Emphasize(tileObjList);
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

    private void AnnounceState(string message)
    {
        //  Debug.Log(message);
        Announcer.Instance.AnnounceState(message);
    }

    public bool AdaptActionCountRestrict = false;
    public bool AdaptInTileForAct = false; //�ش� Ÿ�Ͽ� �־�� �۾��� �����ϰ� 
    public bool TempAdaptEvent = false;

    #endregion

    public GamePlayData GetPlayData()
    {
        return m_playData;
    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt)
    {
        TTokenOrder order = _orderReceipt.Order;
        int orderSerialNum = order.OrderSerialNumber;
        //���� ���� �ֹ��� ���� �ݹ��� ���
        if (orderSerialNum.Equals(TempNationSelectOrderSerialNum))
        {
            //������ ����
            Nation nation = MgNation.GetInstance().GetNation(order.SelectItemNum);
            //�� ������ ���� 
            TokenTile nationCapitalTile = nation.GetCapital();
            //���� ĳ����
            TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
            //������ġ�� ĳ���� �̵�
            RuleBook.Migrate(mainChar, nationCapitalTile);
            //ī�޶� ��Ŀ��
            CamFocus(PlayerManager.GetInstance().GetMainChar());
        }
        
    }
}

//���� ��ü �������� �ܰ�
public enum GamePlayStep
{
    GameInitialSetting, NationTurn, CharTurn, ReadyNextTurn
}

//ĳ���� ���� ���� �ܰ�
public enum CharTurnStep
{
    //���Ӹ����Ͱ� ������ �����ϸ鼭 ���� ������ ���� 
    ChooseNation, SelectNationPolicy,
    ChooseChar, SelectCharAct, FillCharActiContent, PlayCharAction, EndCharTurn
}

public class GamePlayData
{
    public int PlayTime = 0;
}