using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayMaster : MgGeneric<GamePlayMaster>
{
    public BtnReportCheck m_checkBtn;
    public MagnetItem testMangetSample;
    public NaviPin testNaviPin;
    #region  ����
    public enum PlayerMember
    {
        LivePlayer, AI
    }
    
    public float m_moveSpeed = 0.5f;
    public const float c_movePrecision = 0.1f; //������ ���е�
    public bool m_autoReportCheck = true;
    public bool m_testCheckPlayerInventory = false; //�÷��̾� �κ� üũ�ϴ��� 
    public bool m_testAuto = true;

    private GamePlayData m_playData = new();

    [SerializeField]
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0�� �÷��̾�, 1�� AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;
    public List<WorkOrder> m_globalWorkList = new();

    public int tempDevilBirthrestTurm = 3; //�߻��ֱ� ��Ʈ

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
            //  MGContent.GetInstance().m_devilIncubator.SetRestBrithTurn(tempDevilBirthrestTurm);
            KMP kmpTest = new KMP();
            kmpTest.Search();
            
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {

              MgUI.GetInstance().ShowQuestList();
            //int randomSkill = UnityEngine.Random.Range(1, 3);
            //Debug.Log(randomSkill + "��ų ����");
            //PlayerManager.GetInstance().StudyPlayerAction(randomSkill);

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
        //������ ���۸� ���� 
        ReadyNextWorldTurn(); //���� �� ��ȭ ����
        //ó�� ���۽� �÷��̾� ����ĳ���Ϳ� ī�޶� ��Ŀ��.
        CamFocusMainChar();
        PlayerManager.GetInstance().FirstStart();
    
    }
    #endregion

    #region ���� �� ����
    public void ReportNationStep(NationManageStepEnum _step, Nation _nation)
    {
        //�ش� ���ܿ� ���� UIǥ�ⰰ���� ����. 
        Action nationCallBack = delegate
        {
            //Debug.Log("�� ���� ����Ʈ ��ȯ");
            _nation.DoneJob(_step);
        };

        if (m_autoReportCheck)
        {
          //  Debug.LogWarning("���� �ڵ�Ȯ������ �÷��̾� üũ���� ����");
            nationCallBack();
        }
        else
        {
            m_checkBtn.SetEvent(nationCallBack);
            m_checkBtn.gameObject.SetActive(true);
        }
    }

    #endregion

    #region ĳ���� �� ����
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

    #region �̵� �� ó��

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
        ReadyNextWorldTurn(); //���� �� ��ȭ ����
        ResetNationTurn(); //���� ������� ����
        ResetPlayerTurn(); //�÷��̾� ������ ����
        ResetPlayDataUI(); //�÷��� ������ ����
        EffectStartTurn(); //�� ���۽� ���ֵǴ� ȿ�� ����
        DoneStep(GamePlayStep.ReadyNextTurn); //ReadyNextTurn()���� �� ���� ������ ȣ�� 
     
    }

   

    private void EffectEndTurn()
    {
        m_playerMemeber = 0;
        DoWorkList();
        RemoveCompleteWorkOrder();
      //  Debug.Log("�׸����� �Ϸ� ���� �� ���� �۾��� " + m_globalWorkList.Count);
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

    private void ReadyNextWorldTurn()
    {
        AnnounceState("������ ��ȭ ���� ���� ����");
        m_playData.PlayTime += 1; //���� ������ ��
        //������ ���� ���� �߻� 
        MGContent.g_instance.WriteContentWhenNextTurn();
        MGContent.g_instance.m_devilIncubator.ChangeWorldTurn(m_playData.PlayTime);
    }

    private void ResetNationTurn()
    {
        MgNation.GetInstance().ResetNationNum();
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
        MGContent.g_instance.m_devilIncubator.BirthDevil();
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
                SettleNationPolicy();
                break;
            case GamePlayStep.NationSettle:
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

    private void SettleNationPolicy()
    {
        //���� ��å ���� 
        MgNation.GetInstance().SettleNationEndTurn();
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
    public void CamFocusMainChar()
    {
        CamFocus(PlayerManager.GetInstance().GetMainChar());
    }

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

    private void PopupMessage(string message)
    {
        AlarmPopup.GetInstance().PopUpMessage(message);
    }

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

    public void CharMoveToCapital(int _nationNum)
    {
        Nation nation = MgNation.GetInstance().GetNation(_nationNum);
        //�� ������ ���� 
        TokenTile nationCapitalTile = nation.GetCapital();
        //���� ĳ����
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        //������ġ�� ĳ���� �̵�
        RuleBook.Migrate(mainChar, nationCapitalTile);
        //ī�޶� ��Ŀ��
        CamFocusMainChar();
        //�ʱ� ���� �����ϴ� ������ �κ����� ���� ���۵Ǿ����. �̺κ��� 
        //CharMoveToCapital()�� ȣ���ؼ� �������ε� �ش� �Լ��� ��Step����� ������ �ȵ�. ���� �ʿ�. 
        Debug.LogWarning("CharMoveToCapital()�� ȣ���ؼ� �������ε� �ش� �Լ��� ��Step����� ������ �ȵ�. ���� �ʿ�. ");
        DoneStep(GamePlayStep.GameInitialSetting); //���� �ʱ� ���� ���� ���� ���ñ��� ��ġ�� �ʱ� ���� ��. 
    }

    public void RegistorWork(WorkOrder _order)
    {
        if(m_globalWorkList.IndexOf(_order) == -1)
        {
            m_globalWorkList.Add(_order);
        }
    }

    private void RemoveWork(WorkOrder _work)
    {
        m_globalWorkList.Remove(_work);
    }

    private void DoWorkList()
    {
        WorkOrder[] workArray = m_globalWorkList.ToArray();
        for (int i = 0; i < workArray.Length; i++)
        {
            workArray[i].DoWork();
        }
    }

    private void RemoveCompleteWorkOrder()
    {
        //do����Ʈ�� removeWork�� ���ʷ� ����Ǹ�, �ǽð� ����Ʈ ī��Ʈ�� �پ �޺κ� ȿ���� ���ְ� �ȵ�.
        WorkOrder[] orders = m_globalWorkList.ToArray();
        for (int i = 0; i < orders.Length; i++)
        {
            if (orders[i].IsCompleteWork() || orders[i].IsCancle)
            {
                RemoveWork(orders[i]);
            }
        }
    }
}

//���� ��ü �������� �ܰ�
public enum GamePlayStep
{
    GameInitialSetting, NationTurn, CharTurn, ReadyNextTurn, NationSettle
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