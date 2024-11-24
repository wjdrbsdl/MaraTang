using System.Collections;
using UnityEngine;


public class UINationReport : UIBase
{
    private int curNationNum = FixedValue.NO_NATION_NUMBER;
    private NationManageStepEnum curStep = NationManageStepEnum.NationEvent;

    public void SetReport(Nation _nation, NationManageStepEnum _curStep)
    {
        UISwitch(true);
        curNationNum = _nation.GetNationNum();
        curStep = _curStep;
    }

    public void OnClickConfirm()
    {
        Nation nation = MgNation.GetInstance().GetNation(curNationNum);
        nation.DoJob(curStep);
    }
}
