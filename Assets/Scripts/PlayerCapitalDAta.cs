using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    None, Wood, Food, Person, Mineral,
    RedMushRoom, Paparu, Subak
}

public enum CapitalStat
{
    Amount
}

public class PlayerCapitalData : ITradeCustomer
{
    //������ �÷����ϸ鼭 �����Ǵ� ������ 
    private Dictionary<Capital, TokenBase> m_dicCapital;
    public static PlayerCapitalData g_instance;

    #region ����
    public PlayerCapitalData()
    {
        g_instance = this;
        m_dicCapital = new();
    }
    //�ҷ��� �����ͷ� �ε�� 
    public PlayerCapitalData(TokenBase[] _loadCapital)
    {
        //�ҷ��� �����ͷ� �ε�� 
        g_instance = this;
        m_dicCapital = new();
        for (int i = 0; i < _loadCapital.Length; i++)
        {
            m_dicCapital.Add((Capital)_loadCapital[i].GetPid(), _loadCapital[i]);
        }
    }
    #endregion

    public bool CheckInventory(TItemListData _costData)
    {
        List<TOrderItem> CostList = _costData.GetItemList();
        for (int i = 0; i < CostList.Count; i++)
        {
          //  Debug.LogFormat("{0}�׷��� {1} �ε����� �ʿ���� {2}", CostList[i].Tokentype, CostList[i].SubIdx, CostList[i].Value);
            TokenType costType = CostList[i].Tokentype;
            //�� ��ūŸ���� ���Ұ��� ���¸� ���� �Ұ����ϸ� �ٷ� false ��ȯ 
            switch (costType)
            {
                case TokenType.Capital:
                    if (IsEnough((Capital)CostList[i].SubIdx, CostList[i].Value) == false)
                    {
                        Debug.Log("���� �׸����� �κ�üũ ���� �������� ���� üũ");
                        if(GamePlayMaster.GetInstance().m_testCheckPlayerInventory == true)
                        {
                            return false;
                        }
                        Debug.LogWarning("ġƮ�� �ڿ� ��� ����");
                    }
                    break;
                default:
                    Debug.LogError("���� case ��ūŸ��");
                    break;
            }
        }

        return true;
    }

    public void PayCostData(TItemListData _costData, bool _isPay = true)
    {
        List<TOrderItem> BuildCostList = _costData.GetItemList();
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].Tokentype;
            int subIdx = BuildCostList[i].SubIdx;
            int value = -BuildCostList[i].Value;
            if (_isPay == false)
                value *= -1; //������ �ƴ϶� �޴°Ÿ� +�� ��ȯ

            //�� ��ūŸ���� ���Ұ��� ���¸� ���� �Ұ����ϸ� �ٷ� false ��ȯ 
            switch (costType)
            {
                case TokenType.Capital:
                    CalCapital((Capital)subIdx, value);
                    break;
                default:
                    Debug.Log("��� ��Ʈ �ƴ� �κ�");
                    break;
            }
            
        }

    }

    public void CalCapital(Capital _capital, int _value)
    {
        string reward = string.Format("{0} �ڿ� {1} Ȯ��", _capital, _value);
        Announcer.Instance.AnnounceState(reward);
        //Debug.Log("�߰��� ��ƾ���� ����" + _capital +"."+_value);
        if (m_dicCapital.ContainsKey(_capital))
        {
            //�ش� ��ū ������ ��ȭ
            m_dicCapital[_capital].CalStat(CapitalStat.Amount, _value);
        }
        else
        {
            //������ �߰�
            TokenBase tokenCapital = new TokenBase(_capital, _value);
            m_dicCapital.Add(_capital, tokenCapital);
        }
        //��ᰡ 0���Ϸ� �������°��� ����߸�, ���� �Ѿ�������� �Һ� ��Ű�� �ܰ迡�� ����üũ�� �ؾ���. 
        //if (m_dicCapital[_capital].GetStat(CapitalStat.Amount)== 0)
        //{
        //    //�ٵ� ���� ������ 0 ���� ���϶��
        //    m_dicCapital.Remove(_capital); //�ٽ� ���� 
        //}
        //else
        if(m_dicCapital[_capital].GetStat(CapitalStat.Amount) < 0)
        {
            Debug.LogError("���� üũ ����");
        }
        //��ȭ�� �ڿ��� �׼��ڵ�� ����
        MGContent.GetInstance().SendActionCode(new TOrderItem(TokenType.Capital, (int)_capital, m_dicCapital[_capital].GetStat(CapitalStat.Amount))); //�÷��̾� �ڿ���ȭ �ڵ� ����
        MgUI.GetInstance().ResetCapitalInfo(this);
    }

    public Dictionary<Capital, TokenBase> GetHaveCapitalDic()
    {
        return m_dicCapital;
    }

    public bool IsEnough(Capital _capital, int _need)
    {
        if (m_dicCapital.ContainsKey(_capital) == false)
            return false;

        if (m_dicCapital[_capital].GetStat(CapitalStat.Amount) < _need)
            return false;

        return true;
    }

    public List<TOrderItem> GetItemList()
    {
        List<TOrderItem> list = new();
        foreach (var item in m_dicCapital)
        {
            TOrderItem capitalItem = new TOrderItem(TokenType.Capital, (int)item.Key, item.Value.GetStat(CapitalStat.Amount));
            list.Add(capitalItem);
        }
        return list;
    }
}
