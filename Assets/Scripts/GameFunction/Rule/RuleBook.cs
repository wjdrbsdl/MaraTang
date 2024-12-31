using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RuleBook
{
    public static GamePlayMaster m_PlayMaster;
    private RouteDisplay m_routeDisplayTool = new();
    private Dictionary<int, TokenAction> m_tileActionDic;
    private CapitalRecipe m_capitalRecipe = new();
    private RuleBookTileAction m_tileActionPart;
    private RuleBookActionPart m_skillActionPart;

    public struct TAttackProgress
    {
        public float t_oriignDamage;
        public float t_reductedDamage;
        public TokenChar t_attacker;
        public int t_revengeStep; //������ �ܰ�

        public TAttackProgress(TokenChar _attackChar, TokenAction _attackAction, int _revenge = 1)
        {
            //������ ����鼭 ���ο��� �������ط� ����
            t_oriignDamage = _attackAction.GetStat(CharActionStat.Power);
           // Debug.Log("��ų ��� ���ϱ�");
            for (int i = 0; i < _attackAction.GetPowerRatio().Count; i++)
            {
                TOrderItem ratio = _attackAction.GetPowerRatio()[i];
             //   Debug.Log((CharStat)ratio.SubIdx + " " + ratio.Value+ "�⺻ ������ "+t_oriignDamage);
                t_oriignDamage += _attackChar.GetStat((CharStat)ratio.SubIdx) * ratio.Value;
             //   Debug.Log("���� �� ������ "+t_oriignDamage);
            }
            t_reductedDamage = t_oriignDamage;
            t_attacker = _attackChar;
            t_revengeStep = _revenge;
        }

        public float CalDamageByDefense(TokenChar _defenseChar)
        {
            float reductedDamage = t_oriignDamage * 0.8f;
            return reductedDamage;
        }

        public void ApplyDamage(TokenChar _target)
        {
            float reductedDamage = CalDamageByDefense(_target);
            int damage = (int)reductedDamage;
            _target.AttackChar(damage);
         }

        public void AttackPlace(TokenTile _tile)
        {
            _tile.AttackTile((int)t_oriignDamage);
        }

        public void Revenge(TokenChar _defenseChar)
        {
         //   Debug.Log("������ ���� :" + t_revengeStep);
            if (t_revengeStep >= 2)
                return;

            TAttackProgress revenge = new TAttackProgress(_defenseChar, new TokenAction(), t_revengeStep +1);
            revenge.ApplyDamage(t_attacker);

        }
    }

    public struct TMineTileResult
    {
        List<(Capital, int)> resourceResultList; //�� �ڿ� �׸��� �׾��� �󸶳� ĺ���� �Ҵ�

        public TMineTileResult(List<(Capital, int)> _resourceResultList)
        {
            resourceResultList = _resourceResultList;
        }

        public List<(Capital, int)> GetResourceAmount()
        {
            return resourceResultList;
        }
    }

    public RuleBook()
    {
        m_tileActionPart = new RuleBookTileAction();
        m_skillActionPart = new RuleBookActionPart();
    }

    #region �׼� ���� ����
    public void ReadCharAction(TokenChar _playChar)
    {
        TokenAction actionToken = _playChar.GetNextActionToken();
        ActionType actionType = actionToken.GetActionType();

        CalActionEnergy(_playChar, actionToken);

        //1. �ʿ��� �ൿ�� ����
        Action effectDelegate = null; //�׼� ���� ȿ��
        Action attackSfx = null; //�׼��� ��ų ����Ʈ
        IEnumerator animateCoroutine = null; //�׼� ���� ����

        //2. �÷��̾ ������ Ÿ�� Ÿ��
        TokenTile clickedTile = GameUtil.GetTileTokenFromMap(actionToken.GetTargetPos()); //Ÿ�� Ÿ��
        GameUtil.LookTargetTile(_playChar, clickedTile); //ĳ���� ���� ����

        //3. ������ Ÿ�� ���� ���� ������ ���� �����Ͽ� ����
        if (actionType == ActionType.Attack)
        {
          //  Debug.Log("���� ���� �����Ѵ�");
            //������
            //0. ������ �巯���� �׼��� 1��. �ֵθ��ų� ��ų� �߻��ϰų� 
            //1. �ش� ���ݾ׼��� ������ ����
            TokenTile targetTile = clickedTile;
            attackSfx = delegate
            {
                MgSkillFx.GetInstance().MakeSkillFx(targetTile, actionToken.GetPid());
            };
            //2. Ÿ�� ������ �������� ����� ��Ҹ� ����
            List<TokenTile> targetTiles = m_skillActionPart.GetActionRangeTile(_playChar.GetMapIndex(), targetTile.GetMapIndex(), 15);
        
            //3. ��� ��Ҹ� ���� ���� ã�� ���� ����
            effectDelegate = delegate
            {
                TAttackProgress attackProgress = new TAttackProgress(_playChar, actionToken);
                for (int i = 0; i < targetTiles.Count; i++)
                {
                    TokenTile attackTile = targetTiles[i];
                    List<TokenChar> enemies = attackTile.GetCharsInTile();
                    for (int x = 0; x < enemies.Count; x++)
                    {
                        Debug.Log(_playChar.GetItemName() + "�� " + enemies[x].GetItemName() + "�� ����");

                        attackProgress.ApplyDamage(enemies[x]);
                    }

                    if (attackTile.m_Side != _playChar.m_Side)
                        attackProgress.AttackPlace(attackTile);
                }
                
            };
            animateCoroutine = co_AttacAction(_playChar, attackSfx, effectDelegate);

        }
        //4. �̵��� Ÿ�� ���� ��ġ�� �̵�
        else if (actionType == ActionType.Move)
        {
            TokenTile targetTile = clickedTile;
            effectDelegate = delegate 
            { 
                Migrate(_playChar, targetTile); 
            };
            animateCoroutine = co_MoveAction(_playChar, targetTile, effectDelegate);
        }
        else if(actionType == ActionType.Wrongfulness)
        {
            animateCoroutine = co_WrongFulAction(_playChar, actionToken);
        }
        //5. �غ�� �������� �ִϸ��̼� ���� 
        GamePlayMaster.GetInstance().AnimateTokenObject(animateCoroutine, effectDelegate, _playChar);
    }

    public static int curChunkNum = FixedValue.No_VALUE;
    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //�ش� Ÿ�Ϸ� �ش� ĳ���͸� ���� ��Ű�� 
        //���� �ɸ� ������ �⺻ ��ǥ�� 0,0 �̶� GetMap���� null�� ��ȯ���� �ʴ´�.
        TokenTile preTile = MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()];
        preTile.RemoveCharToken(_char); //�̻� ������
        _targetTile.Migrate(_char); //�̻� �ְ�
        //�ٸ� ��ġ�� �̵��ϰ� �� ���
        if(_char.isMainChar && _targetTile.ChunkNum != curChunkNum)
        {
            Chunk preChunk = MGContent.GetInstance().GetChunk(curChunkNum);
            if (preChunk != null)
                preChunk.OnExitChunk();
            
            Chunk curChunk = MGContent.GetInstance().GetChunk(_targetTile.ChunkNum);
            if(curChunk != null)
            {
                curChunkNum = _targetTile.ChunkNum; //���� �Ҵ�
                curChunk.OnEnterChunk(); //�� Ÿ�� �������� ���ɷ�
            }
 
            
        }
        _char.GetObject().SyncObjectPosition();
    }

    public static void FirstMigrate(TokenChar _char, int[] _position)
    {
        //
        TokenTile tile = GameUtil.GetTileTokenFromMap(_position);
        tile.Migrate(_char);
        _char.GetObject().SyncObjectPosition();
    }

    IEnumerator co_MoveAction(TokenChar _char, TokenTile _goalTile, Action effectAction)
    {
        //   Debug.Log("�̵� �ڷ�ƾ ���� �ܰ�" + m_MaxStep+"/ " + curStep);

        Vector3 goal = _goalTile.GetObject().transform.position;

        _char.SetState(CharState.Move);

        Vector3 dir = goal - _char.GetObject().transform.position;
        while (Vector2.Distance(_char.GetObject().transform.position, goal) > GamePlayMaster.c_movePrecision* GamePlayMaster.GetInstance().m_playSpeed)
        {
           _char.GetObject().transform.position += (dir.normalized * GamePlayMaster.GetInstance().m_moveSpeed * Time.deltaTime * GamePlayMaster.GetInstance().m_playSpeed);
            yield return null;
        }
        if (effectAction != null)
        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);
    }

    IEnumerator co_AttacAction(TokenChar _char, Action attackSfx, Action effectAction)
    {
        //   Debug.Log("�̵� �ڷ�ƾ ���� �ܰ�" + m_MaxStep+"/ " + curStep);
        _char.SetState(CharState.Attack);
        SoundManager.GetInstance().PlayEfx(SoundManager.EfxList.Attack);
    
        //��ų�� �ǰݱ����� �ð�
        float waitTime = 1f;
        while (waitTime>0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

        //��ų �ǰݽ� ����Ʈ
        if (attackSfx != null)
            attackSfx();
        if (effectAction != null)
             effectAction();

        GamePlayMaster.GetInstance().DoneCharAction(_char);

    }

    IEnumerator co_WrongFulAction(TokenChar _devil, TokenAction _wrongAction)
    {
        //�ִϸ��̼� ���۽ð� 
        float waitTime = 0.5f; //���Ƿ� 0.5 ���� �ش� �׼� �̺�Ʈ ���� �ð��� ���߱� 
        while (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

     //   Debug.Log(_devil.GetItemName() + "�� " + _wrongAction.GetItemName() + "������ ���ƴ�.");
        TokenTile tile = _devil.GetTargetTile();
        tile.GetNation().CalStat(NationStatEnum.Happy, -_wrongAction.GetStat(CharActionStat.Power));

        GamePlayMaster.GetInstance().DoneCharAction(_devil);
    }

    private void CalActionEnergy(TokenChar _playChar, TokenAction _action)
    {
        //�׼��� �����Կ� �־� �ʿ��� �ڿ����� ����ϴ� �� 

        //1. �ش� ĳ���Ϳ��� �������� �׼� Ƚ���� ����
        int needCount = _action.GetStat(CharActionStat.NeedActionCount);
        _playChar.UseActionCount(needCount);
        _playChar.UseActionEnergy(_action.GetStat(CharActionStat.NeedActionEnergy));

        //2. �׼��� �ϳ� ��� Ƚ�� ���� 
        _action.CalStat(CharActionStat.RemainCountInTurn, -1); //�׼���ū�� �ش��Ͽ��� ������ ��� Ƚ�� ����

    }

    #endregion

    #region ���� üũ 
    public bool IsInLichTarget(TokenChar _char, TokenAction _action, TokenBase _target)
    {
        TMapIndex mapIndex = new TMapIndex(_char, _target);
        int targetDistance = GameUtil.GetMinDistance(mapIndex);

        //Debug.Log(_char.GetXIndex() + "," + _char.GetYIndex() + "���� " + _target.GetXIndex() + "," + _target.GetYIndex() + "�Ÿ��� " + targetRange);
        if (targetDistance < _action.GetStat(CharActionStat.MinLich))
            return false;

        if (_action.GetStat(CharActionStat.Lich) < targetDistance)
           return false;

        return true;
    }

    public bool IsAbleAction(TokenChar _char, TokenAction _action, ref string _failMessage)
    {
        //1. �׼��� �Һ� �׼� ī��Ʈ�� ��(�׼ǼҺ�ī��Ʈ�� 0 �� �༮�� ��� true)
        if(_char.GetActionCount() < _action.GetStat(CharActionStat.NeedActionCount))
        {
            _failMessage = "�ൿ ī��Ʈ ���� \n���� :" + _char.GetActionCount() + "\n�ʿ�:"
                + _action.GetStat(CharActionStat.NeedActionCount);
            return false;
        }

        //2. �׼��� �Һ� ������
        if (_char.IsPlayerChar() && _char.GetStat(CharStat.CurActionEnergy) < _action.GetStat(CharActionStat.NeedActionEnergy))
        {
            _failMessage = "�ൿ ������ ���� \n���� :"+ _char.GetStat(CharStat.CurActionEnergy) +"\n�ʿ�:"
                + _action.GetStat(CharActionStat.NeedActionEnergy);
            return false;
        }

        //3. �׼� ��ü�� ���Ƚ��(�� �Ͽ� ��������� �־��ϴ� ���)
        if (_action.AbleUse() == false)
        {
            _failMessage = "���Ұ� ���� �׼�";
            return false;
        }

        return true;
    }

    public bool CheckActionContent(TokenChar _char, TokenAction _action)
    {
        if (_action.GetTargetPos() == null)
        {
            Announcer.Instance.AnnounceState("Ÿ���� ����Ȯ", true);
            return false;
        }
            

        return true;
    }
    #endregion

    #region Ÿ�� �׼�
    public bool AbleOccupy(TokenTile _tile)
    {
        //�ش� Ÿ�� �Ҽ� ������ ������ ����
        if (_tile.GetStat(ETileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
            return true;

        return false;
    }

    public void ConductTileAction(TokenTile _tile, TOrderItem _actionOrder, TileType _actionPlace)
    {
        m_tileActionPart.ConductTileAction(_tile, _actionOrder, _actionPlace);
    }

    #endregion

    #region ĳ�� ���� ��ȭ
    public void IntenseStat(TokenChar _targetChar, CharStat _targetStat)
    {
        _targetChar.CalStat(_targetStat, 100);
    }
    #endregion

    #region �ڿ� ��Ģ
    public TMineTileResult MineResource(TokenTile _tile, int _findAbility = 0)
    {
        List<(Capital, int)> result = new();
        TMineTileResult mineResult = new(result);
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)_tile.tileType);
        TileEffectEnum tileEffect = tileData.effectType;

        //�ش� Ÿ���� ���ް����� ������ �ƴϸ� ��°� ���� ��ȯ
        if (tileEffect != TileEffectEnum.Money)
            return mineResult;
        
        List<TOrderItem> resourceList = tileData.EffectData.GetItemList();
        //�ش� Ÿ�Ͽ��� ����Ǵ� �ڿ� ������ ������ ������ 
        for (int i = 0; i < resourceList.Count; i++)
        {
            TOrderItem resource = resourceList[i];
            //�ѹ��� �ڿ��������� üũ�ϰ�
            if (resource.Tokentype == TokenType.Capital)
            {
                if(AbleFindCapital((Capital)resource.SubIdx, _findAbility))
                    result.Add(((Capital)resource.SubIdx, resource.Value));
                continue;
            }
            if(resource.Tokentype == TokenType.Random)
            {
                TOrderItem selectItem = new DiceRandomItem().GetDiceItem(resource); //�ڿ�ä�� ���� ĳ��
                if (AbleFindCapital((Capital)selectItem.SubIdx, _findAbility))
                    result.Add(((Capital)selectItem.SubIdx, selectItem.Value));
                continue;
            }
                
        }

        return mineResult;
    }

    private bool AbleFindCapital(Capital _cpaital, int _findAbility)
    {
        //�ش� �ڿ��� ã�Ҵ��� ���� 
        CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(_cpaital);
        int ratio = capitalData.baseRatio + _findAbility;
        if (ratio >= 100)
            return true;
        if (ratio <= 0)
            return false;
        int random = UnityEngine.Random.Range(1, 101);

        //Debug.Log(_cpaital+" �̴µ� ����" + ratio + "/" + random + "��ġ ����");
        return random <= ratio;
    }

    public void MixCapital(List<(Capital, int)> _resources)
    {
        PlayerCapitalData playerCapital = PlayerCapitalData.g_instance;
        TItemListData costData = new TItemListData(_resources);
        if (playerCapital.CheckInventory(costData) == false)
        {
            Announcer.Instance.AnnounceState("�ռ� ��� ����", true);
            return;
        }
        playerCapital.PayCostData(costData);//����Ѹ�ŭ ���� ��Ű��
        (Capital, int) mixed = m_capitalRecipe.MixCapital(_resources);
        TItemListData mixedData = new TItemListData(mixed);
        bool isPay = false; //������ �ƴ�
        playerCapital.PayCostData(mixedData, isPay); //���� ��ŭ �߰� ��Ű�� 
    }

    public void ChangeCapital((Capital, int) _input, Capital _outCapital)
    {
        PlayerCapitalData capitalData = PlayerCapitalData.g_instance;
        if (capitalData.IsEnough(_input.Item1, _input.Item2) == false)
        {
            Announcer.Instance.AnnounceState("��ȯ ��� ����", true);
            return;
        }
        capitalData.CalCapital(_input.Item1, -_input.Item2); //����Ѹ�ŭ ���� ��Ű��
        (Capital, int) mixed = m_capitalRecipe.ChangeCapital(_input, _outCapital);
        capitalData.CalCapital(mixed.Item1, mixed.Item2); //���� ��ŭ �߰� ��Ű�� 
    }
    #endregion

}

public enum CapitalAction
{
    //ActionToken 3���� ��� �ռ� Ÿ�Կ��� �߰� �з�
    MixCapital, ChageCapital
}
