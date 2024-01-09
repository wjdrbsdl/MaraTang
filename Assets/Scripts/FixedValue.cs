using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedValue
{
    #region Int
    public static int

        BASE_CROPRATIO = 20, //비주류 타입의 등장확률 %. 해당 땅에서 등장할 작물 수가 n개 할당되었을 때 비주류에서 뽑을 비율 %.
        BASE_DROP = 1000,
        BASE_LANND_PERCENT = 10000
         ;
    #endregion

    #region Float
    public static float
        SKILL_DESTROY_DELAY = 0.03f,
        SKILL_VECTOR_Y = 0.5f, //스킬의 높이
        WORLD_SPEED = 1f
        ;
    #endregion

    #region LandInfo 규정 사이즈

    public static int
        Land_Rotate_Angle = -45,
        Wolrd_Rotate_Angle = 45,
        ENERGY_RATE_BASE = 200, // 효율의 50%을 달성시키는 기본수치. 
        MYLAND_SIZE = 5,
        MAP_MAX_SIZE = 3,
        LAND_MIN_TIER = 0,
        LAND_MAX_TIER = 5;

    //땅의 주요 속성을 티어에 따라 실제값으로 환산할 때 곱해지는 티어 효율. 
    public static float
         LAND_TIER_RATIO = 1f / (LAND_MAX_TIER - LAND_MIN_TIER);

    //주요 속성을 티어가 아닌 실제 적용 값에서 최소 최댓값. = 티어(Min Tier ~ Max Tier)의 비율이 곱해져서 최종 값 도출
    public static float[,] MIN_MAX_VALUE =
    {
        {0, 0}, //soilamount
        {-400, 400}, //energyRate
        {3, 6}, //mineralBosster
        {2, 5}, //grassbooster
        {0, 10}, //stability
        {1, 6}, //lucky
        {3, 5} //grow
    };

    //맵 크기에 따라 Node 블록 수 반환하는곳. 
    public static void ReturnSize(int mapSize, ref int _sizeX, ref int _sizeZ)
    {
        switch (mapSize)
        {
            case 1:
                _sizeX = 15;
                _sizeZ = 15;
                break;
            case 2:
                _sizeX = 25;
                _sizeZ = 25;
                break;
            case 3:
                _sizeX = 30;
                _sizeZ = 30;
                break;

        }
    }

    #endregion

    #region String
    public static string
        MOUSE_LAYER = "Mouse",
        NODE_BUILD_LAYER = "NodeBuild",
        NODE_MOVE_LAYER = "NodeMove",
        INTER_OBJECT_LAYER = "InterObj",
        INTER_2DOBJECT_LAYER = "Inter2DObj",
        FOLDER_PATH_SKILL_PREFEB = "SkillPrefeb/",
        FOLDER_PATH_SKILL_ICON = "SkillIcon/",
        FOLDER_PATH_CROP_PREFEB = "CropPrefeb/",
        FOLDER_PATH_CROP_ICON = "CropIcon/";
    #endregion

    public static int[,] ABSOLUTE_DIRECTION = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; //상우하좌 순서로, 상대좌표를 정의 해놓음. 
    public static int[] No_PARENT_POS = { -1, -1 };
    public static float[,] ROOT_POS = { { -1,1}};
}
