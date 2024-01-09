using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedValue
{
    #region Int
    public static int

        BASE_CROPRATIO = 20, //���ַ� Ÿ���� ����Ȯ�� %. �ش� ������ ������ �۹� ���� n�� �Ҵ�Ǿ��� �� ���ַ����� ���� ���� %.
        BASE_DROP = 1000,
        BASE_LANND_PERCENT = 10000
         ;
    #endregion

    #region Float
    public static float
        SKILL_DESTROY_DELAY = 0.03f,
        SKILL_VECTOR_Y = 0.5f, //��ų�� ����
        WORLD_SPEED = 1f
        ;
    #endregion

    #region LandInfo ���� ������

    public static int
        Land_Rotate_Angle = -45,
        Wolrd_Rotate_Angle = 45,
        ENERGY_RATE_BASE = 200, // ȿ���� 50%�� �޼���Ű�� �⺻��ġ. 
        MYLAND_SIZE = 5,
        MAP_MAX_SIZE = 3,
        LAND_MIN_TIER = 0,
        LAND_MAX_TIER = 5;

    //���� �ֿ� �Ӽ��� Ƽ� ���� ���������� ȯ���� �� �������� Ƽ�� ȿ��. 
    public static float
         LAND_TIER_RATIO = 1f / (LAND_MAX_TIER - LAND_MIN_TIER);

    //�ֿ� �Ӽ��� Ƽ� �ƴ� ���� ���� ������ �ּ� �ִ�. = Ƽ��(Min Tier ~ Max Tier)�� ������ �������� ���� �� ����
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

    //�� ũ�⿡ ���� Node ��� �� ��ȯ�ϴ°�. 
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

    public static int[,] ABSOLUTE_DIRECTION = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; //������� ������, �����ǥ�� ���� �س���. 
    public static int[] No_PARENT_POS = { -1, -1 };
    public static float[,] ROOT_POS = { { -1,1}};
}
