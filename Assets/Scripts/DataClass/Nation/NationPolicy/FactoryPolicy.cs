using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryPolicy
{
   
    public NationPolicy MakePolicy(MainPolicyEnum _mainPolicy, Nation _nation)
    {
        NationPolicy policy = new PolicyBuildResrouceTile();
        switch (_mainPolicy)
        {
            case MainPolicyEnum.ExpandLand:
                policy = new PolicyExpandLand();
                break;
            case MainPolicyEnum.ManageLand:
                policy = new PolicyBuildResrouceTile();
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

