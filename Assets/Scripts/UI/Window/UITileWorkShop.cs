using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITileWorkShop : UIBase
{
    //�ش� Ÿ���� ���� ǥ�� �� Ÿ�Ͽ��� �� �� �ִ� �۾��� �����ϴ� UI
    [SerializeField]
    private Transform m_tileActionBox; //�׼� ����Ʈ ��ư ���� ���
    [SerializeField]
    private BtnTileWorkShop m_workButtonSample;

    [SerializeField]
    private BtnTileWorkShop[] m_workButtones;
    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        
        _tile.GetInfoForTileWorkShop(); //Ÿ�� ���� ���� ���
        //�⺻ Ÿ�� �׼ǵ�
        string[] baseWorks = System.Enum.GetNames(typeof(TileAction));

        //���� �����ؼ� ������ �׼ǵ� �̴´�
        string[] confirmWorks = ConfirmTileActions(baseWorks);
        
        //����ϴ� ��ŭ ��ư Ȱ��ȭ 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, confirmWorks.Length, m_tileActionBox);
        //��ư ����
        SetButtons(_tile, confirmWorks);

    }

    private string[] ConfirmTileActions(string[] _baseWorks)
    {
        //���� ���� Ȱ���ؼ� Ÿ�Ͽ��� �� �� �ִ� �׼��� ���������� Ȯ��.

        return _baseWorks;
    }

    private void SetButtons(TokenTile _tile, string[] _workes)
    {
        int setCount = Random.Range(1, 3);
        for (int i = 0; i < setCount; i++)
        {
            m_workButtones[i].gameObject.SetActive(true);
            m_workButtones[i].SetButtonInfo(_tile, _workes[i]);
        }
        for (int i = setCount; i < _workes.Length; i++)
        {
            m_workButtones[i].gameObject.SetActive(false);
        }
    }
}
