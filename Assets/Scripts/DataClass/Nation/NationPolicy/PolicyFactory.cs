using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicyFactory
{
   
    public NationPolicy MakePolicy(MainPolicyEnum _mainPolicy, Nation _nation)
    {
        NationPolicy policy = new PolicyManageLand();
        switch (_mainPolicy)
        {
            case MainPolicyEnum.ExpandLand:
                policy = new PolicyExpandLand();
                break;
            case MainPolicyEnum.ManageLand:

                break;
            case MainPolicyEnum.NationLevelUP:
                policy = new PolicyLevelUp();
                break;
            case MainPolicyEnum.TechTree:
                policy = new PolicyResearch();
                break;
        }
        
        policy.SetInfo(_mainPolicy, _nation);

        return policy;
    }

}

