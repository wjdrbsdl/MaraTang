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
            PopupDamage.GetInstance().DamagePop(_target.GetObject().gameObject, damage);
            _target.CalStat(CharStat.CurHp, -damage);
            MgHud.GetInstance().ShowCharHud(_target);
            if (_target.GetStat(CharStat.CurHp) <= 0)
            {
           //     Debug.Log(_target.GetItemName()+"��� ü��"+ _target.GetStat(CharStat.CurActionEnergy));
                _target.Death();
                return;
            }
            //Revenge(_target);
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
                MgSkillFx.GetInstance().MakeSkillFx(targetTile, "�׽�Ʈ");
            };
            //2. ���� ���� Ÿ���� ������
            List<TokenChar> enemies = targetTile.GetCharsInTile();
            //3. �ش� Ÿ�ٿ��� �ش� ������ ȿ���� ���� 
            effectDelegate = delegate
            {
                TAttackProgress attackProgress = new TAttackProgress(_playChar, actionToken);
                for (int i = 0; i < enemies.Count; i++)
                {
                    //   Debug.Log(_playChar.GetItemName() + "�� " + enemies[i].GetItemName() + "�� ����");
            
                    attackProgress.ApplyDamage(enemies[i]);
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

        //5. �غ�� �������� �ִϸ��̼� ���� 
        GamePlayMaster.GetInstance().AnimateTokenObject(animateCoroutine, effectDelegate, _playChar);
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //�ش� Ÿ�Ϸ� �ش� ĳ���͸� ���� ��Ű�� 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].RemoveCharToken(_char); //�̻� ������
        _targetTile.Migrate(_char); //�̻� �ְ� 
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
        while (Vector2.Distance(_char.GetObject().transform.position, goal) > GamePlayMaster.c_movePrecision)
        {
           _char.GetObject().transform.position += (dir.normalized * GamePlayMaster.GetInstance().m_moveSpeed * Time.deltaTime);
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
        if (attackSfx != null)
            attackSfx();
        float waitTime = 1f;
        while (waitTime>0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        if (effectAction != null)
             effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);

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
    public bool IsInRangeTarget(TokenChar _char, TokenAction _action, TokenBase _target)
    {
        TMapIndex mapIndex = new TMapIndex(_char, _target);
        int targetRange = GameUtil.GetMinRange(mapIndex);

        //Debug.Log(_char.GetXIndex() + "," + _char.GetYIndex() + "���� " + _target.GetXIndex() + "," + _target.GetYIndex() + "�Ÿ��� " + targetRange);
        if (targetRange < _action.GetStat(CharActionStat.MinRange))
            return false;

        if (_action.GetStat(CharActionStat.Range) < targetRange)
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
        if (_char.GetStat(CharStat.CurActionEnergy) < _action.GetStat(CharActionStat.NeedActionEnergy))
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
   
    public TokenEvent CheckEnteranceEvent(int[] mapCoordi)
    {
        //Ÿ�Ͽ� ��߿� �̺�Ʈ�� �ִ��� Ȯ��
        TokenTile mapTile = GameUtil.GetTileTokenFromMap(mapCoordi);
        //�ʿ� �ִ��� �������� ��ȯ
        TokenEvent enterEvent = mapTile.GetEneteranceEvent();

        return enterEvent;
    }

    #region Ÿ�� �׼�
    public bool AbleOccupy(TokenTile _tile)
    {
        //�ش� Ÿ�� �Ҽ� ������ ������ ����
        if (_tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
            return true;

        return false;
    }

    public TokenAction[] RequestTileActions(TokenTile _tile)
    {
        //�ش� Ÿ���� ������ ������ �׼��� �̾���. 
        List<TokenAction> ableList = new List<TokenAction>();
        //1.�ش� Ÿ���� pid�� Ȯ��
        int tileType = (int)_tile.tileType;
        //2. pid�� �´� Ÿ�� ������ ������
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData(tileType); //�ش� Ÿ���� Ÿ�ϵ����͸� ������
        if (tileData == null)
            return ableList.ToArray();
        //3. ������ �׼� ��ū �迭�� ��ȯ
        for (int i = 0; i < tileData.AbleTileActionPID.Length; i++)
        {
            //4. Ÿ�� �������� �۾� ������ �׼� pid ����Ʈ�� �ش��ϴ� Ÿ�Ͼ׼ǵ��� ã�Ƽ� ��ȯ
            TokenAction ableAction = MgMasterData.GetInstance().GetTileActions(tileData.AbleTileActionPID[i]);
            if(ableAction == null)
            {
                Debug.Log("�ش� pid�� �ش��ϴ� �׼� ����");
                continue;
            }
            ableList.Add(ableAction);
        }
        //����
        /*
         * Ÿ�� �׼��� �ش� Ÿ�Ͽ� ���ӵǾ���ϴµ�, ���� ���� �ڷ� ���� ������� ��� Ÿ���� �� �ൿ�� �����ؼ�
         * ��� ���µ��� ���� �ǹ���. 
         */

        
        return ableList.ToArray();
    }

    public void ConductTileAction(TokenTile _tile, TokenAction _action)
    {
        TokenChar player = MgToken.GetInstance().GetMainChar();
        if(GameUtil.GetMinRange(player, _tile) > 0 && GamePlayMaster.GetInstance().AdaptInTileForAct == true)
        {
            Debug.Log("�ش� Ÿ�Ͽ� �־�� ����");
            return;
        }
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //�ش� Ÿ�Կ��� �������� ����
        //tileActionType���� ���� ���� 

        switch (tileActionType)
        {
            case TileActionType.Harvest:
                HarvestTile(_tile);
                MgUI.GetInstance().CancleLastUI();
                break;
            case TileActionType.Build:
                BuildTile(_tile, (TileType)subValue);
                MgUI.GetInstance().CancleLastUI();
                break;

            case TileActionType.CapitalChef:
                CapitalAction doCode = (CapitalAction)subValue;
                //��� ��ȯ 
                MgUI.GetInstance().ShowCapitalWorkShop(doCode, _tile, _action);
                break;

            case TileActionType.LandUsage:
                UseTownFunction(_tile, subValue);
                break;

            case TileActionType.Destroy:
                BuildTile(_tile, TileType.Nomal);
                MgUI.GetInstance().CancleLastUI();
                break;
            default:
                MgUI.GetInstance().CancleLastUI();
                break;
        }
    
    }

    private void HarvestTile(TokenTile _tile)
    {
        List<(Capital, int)> mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(_tile).GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            //  Debug.Log(mineResult[i].Item1 + " �ڿ� ä��" + mineResult[i].Item2);
            PlayerCapitalData.g_instance.CalCapital(mineResult[i].Item1, mineResult[i].Item2);
        }
    }
    public OrderCostData GetTileChangeCost(TileType _tileType)
    {
        OrderCostData costData = MgMasterData.GetInstance().GetTileData((int)_tileType).BuildCostData;
        return costData;
    }
    private void BuildTile(TokenTile _tile, TileType _tileType)
    {
        if (PlayerCapitalData.g_instance.CheckInventory(GetTileChangeCost(_tileType)) == true)
        {
            _tile.ChangeTileType(_tileType);
        }
    }

    private void UseTownFunction(TokenTile _tile, int _subValue)
    {
        Debug.Log(_subValue + "�۾� ���� ��û");
        TownFuction townFuction = (TownFuction)_subValue;
        switch (townFuction)
        {
            case TownFuction.GiveMoney:
                PlayerCapitalData _playerCapital = PlayerCapitalData.g_instance;
                //�÷��̾ ������ �ڿ��� ���� ������ �ڽ�Ʈ�����͸� ����
                List<(Capital, int)> _capitalList = new();
                _capitalList.Add((Capital.Food, 50));
                _capitalList.Add((Capital.Mineral, 50));
                _capitalList.Add((Capital.Person, 50));
               // OrderCostData _costData = _playerCapital.GetTrade(_capitalList);
                break;
        }
    }

    public enum TownFuction
    {
       GiveMoney = 1
    }

    #endregion

    #region ĳ�� ���� ��ȭ
    public void IntenseStat(TokenChar _targetChar, CharStat _targetStat)
    {
        _targetChar.CalStat(_targetStat, 100);
    }
    #endregion

    public TMineTileResult MineResource(TokenTile _tile)
    {
        List<(Capital, int)> result = new();
        TMineTileResult mineResult = new(result);

        TileType territory = _tile.GetTileType();
        int mainType = _tile.GetStat(TileStat.MainResource);
        int tileEnegy = _tile.GetStat(TileStat.TileEnergy);
        
        //���̺�� �� Ÿ�Կ��� ���� �� �ִ� �ڿ��� �����س���?
        switch (territory)
        {
            case TileType.Nomal:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Town:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Farm:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Mine:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.WoodLand:
                result.Add((Capital.Food, tileEnegy*0));
                result.Add((Capital.Mineral, tileEnegy*1));
                result.Add((Capital.Wood, (int)(tileEnegy*1.5)));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Capital:
                break;
            default:
                break;
        }

        return mineResult;
    }

    public void MixCapital(List<(Capital, int)> _resources)
    {
        PlayerCapitalData playerCapital = PlayerCapitalData.g_instance;
        OrderCostData costData = new OrderCostData(_resources);
        if (playerCapital.CheckInventory(costData) == false)
        {
            Announcer.Instance.AnnounceState("�ռ� ��� ����", true);
            return;
        }
        playerCapital.PayCostData(costData);//����Ѹ�ŭ ���� ��Ű��
        (Capital, int) mixed = m_capitalRecipe.MixCapital(_resources);
        OrderCostData mixedData = new OrderCostData(mixed);
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

    public void AnnounceCapitalIncome()
    {

    }
}

public enum CapitalAction
{
    //ActionToken 3���� ��� �ռ� Ÿ�Կ��� �߰� �з�
    MixCapital, ChageCapital
}
