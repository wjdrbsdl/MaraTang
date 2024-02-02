using System.Collections;
using UnityEngine;


public class Announcer
{
    public static Announcer Instance;

    public Announcer()
    {
        Instance = this;
    }

    public void AnnounceState(string message)
    {

    }
}
