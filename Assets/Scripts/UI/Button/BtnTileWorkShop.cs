using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnTileWorkShop : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpText;
    private TokenTile m_tile;
  
    public void SetButtonInfo(TokenTile _tile)
    {
        Debug.Log("Ÿ�� ��� ��ư �� ���� �ʿ�");
    }

    public void OnButtonClick()
    {
    
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
