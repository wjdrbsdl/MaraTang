using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharHud : MonoBehaviour
{
    private TokenBase m_token;

    [SerializeField]
    private Image m_hpBar;

    public void SetHp(TokenBase _base)
    {
        float ratio = _base.GetStat(CharStat.CurHp) / _base.GetStat(CharStat.MaxHp);

        m_hpBar.fillAmount = ratio;
    }
    
}
