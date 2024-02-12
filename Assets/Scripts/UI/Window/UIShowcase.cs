using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowcase : UIBase
{
    [SerializeField]
    private ShowcaseSlot m_showcaseSample;
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private ShowcaseSlot[] m_showcaseSlots;
    public void OpenWindow(InputSlot _inputSlot)
    {
        base.OpenWindow();
        //1. 캐릭터가 보유한 자원 리스트를 가져온다. 
        int[] capitals = PlayerCapitalData.g_instance.GetCapitalValue();

        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, capitals.Length, m_box);

        for (int i = 0; i < m_showcaseSlots.Length; i++)
        {
            m_showcaseSlots[i].gameObject.SetActive(true);
            m_showcaseSlots[i].ShowCaseSet(((Capital)i).ToString(), _inputSlot);
        }
    }

   
}
