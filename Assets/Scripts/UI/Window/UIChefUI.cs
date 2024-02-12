using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChefUI : UIBase
{
    [SerializeField]
    GameObject[] m_subUies;

    public void SetChefUI(int subCode, TokenTile _tile, TokenAction _action)
    {
        m_window.SetActive(true);
       // m_subUies[subCode].SetActive(true);
    }
}
