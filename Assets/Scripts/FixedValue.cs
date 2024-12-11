using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedValue
{
    #region Int
    public static int
        ALL = int.MaxValue,
        NO_NATION_NUMBER = -1,
        No_INDEX_NUMBER = -1,
        No_VALUE = -1
         ;
    #endregion

    #region Float
    public static float
        WORLD_SPEED = 1f
        ;
    #endregion
  
    #region String
    public static string
        PARSING_TYPE_DIVIDE = "_",
        PARSING_VALUE_ALL = "All"
        ;

    public static char
        PARSING_LINE_DIVIDE = ' ',
        PARSING_LIST_DIVIDE = '/'
        ;
    #endregion

    public static bool
        SAY_CHUNKRESET = false,
        QUEST_ARALM = false
        ;

}
