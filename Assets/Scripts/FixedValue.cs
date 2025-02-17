using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedValue
{
   #region Int
    public const int
        ALL = int.MaxValue,
        NO_NATION_NUMBER = -1,
        No_INDEX_NUMBER = -1,
        No_VALUE = -1
         ;

    public static int
        Dice100 = 100
        ;
    #endregion

    #region Float
    public static float
        WORLD_SPEED = 1f
        ;
    #endregion
  
    #region String
    public const string
        PARSING_TYPE_DIVIDE = "_",
        PARSING_VALUE_ALL = "All"
        ;

    public const char
        PARSING_LINE_DIVIDE = ' ',
        PARSING_LIST_DIVIDE = '/'
        ;
    #endregion

    public static bool
        SAY_CHUNKRESET = false,
        QUEST_ARALM = false,
        CAPITAL_NONE_ARALM = false
        ;

}
