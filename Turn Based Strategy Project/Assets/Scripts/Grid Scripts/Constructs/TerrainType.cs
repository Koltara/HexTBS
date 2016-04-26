using UnityEngine;
using System.Collections;



public struct TerrainType
{
    public int cost;
    public int evadeBonus;
    public int defenseBonus;
    public Color tileColor;

    public string name;
    
    
    public TerrainType(int type)
    {
        cost = 1;
        evadeBonus = 0;
        defenseBonus = 0;
        tileColor = TileBehaviour.lightGreen;
        name = "Plain";

        switch (type)
        {
            case (int)TileBehaviour.TerrainTypes.kNormal:
                cost = 1;
                evadeBonus = 0;
                defenseBonus = 0;
                tileColor = TileBehaviour.lightGreen;
                name = "Plain";
                break;
            case (int)TileBehaviour.TerrainTypes.kForest:
                cost = 2;
                evadeBonus = 10;
                defenseBonus = 1;
                tileColor = TileBehaviour.darkGreen;
                name = "Forest";
                
                break;
            case (int)TileBehaviour.TerrainTypes.kHill:
                cost = 3;
                evadeBonus = 20;
                defenseBonus = 1;
                tileColor = Color.grey;
                name = "Hill";
                break;
            default:
                cost = 1;
                evadeBonus = 0;
                defenseBonus = 0;
                tileColor = TileBehaviour.lightGreen;
                name = "Plain";
                break;

        }
    }

}
