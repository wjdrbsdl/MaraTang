using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStudyAction : UIBase
{
    public void SetStudyInfo(TokenTile _tile)
    {
        //해당 학습소에서 배울수있는 액션들을 나열 
        UISwitch(true);
        //나라 종류에 따라 배울 수 있는 액션들이 다름 
        Debug.Log("학습 나열");
    }
}
