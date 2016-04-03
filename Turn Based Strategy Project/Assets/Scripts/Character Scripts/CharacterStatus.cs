using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {
    private int slot;
    public static CharacterStatus instance;
    public bool ableToMove = true;
    public string characterName = "";
    //Character Properties
    /*
    Properties:
    Name - Raw Value
    Mod - Modifier on the Property eg. +2 or -2
    Rate - Growth rate from leveling up
    */
    //Level
    public int currentLevel;
    public int levelCap;
    public int experience;

    //Strength
    public int strength;
    public int strengthMod; 
    public int strengthRate; 

    //Agility
    public int agility;
    public int agilityMod;
    public int agilityRate;

    //Skill
    public int skill;
    public int skillMod;
    public int skillRate;

    //Defense
    public int defense;
    public int defenseMod;
    public int defenseRate;

    //Health Max
    public int healthMax;
    public int healthMaxMod;
    public int healthRate;

    //Health Current
    public int healthCurrent;
    public int healthCurrentMod;

    //Movement
    public int moveDistance;
    public int moveDistanceMod;


    //Internal Combat Statistics
    private int accuracy;
    private int accuracyMod;
    private int evasion;
    private int evasionMod;

    //Personality Traits
    public int loyalty;
    public int courage;
    public int greed;
    public int patience;
    public int friendship;



	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        accuracy = 80 + (skill * 2);
        evasion = 15 + (agility * 2);
        if (healthCurrent <= 0)
        {
            for (int i = 0; i < CharacterManager.instance.getCharacterInstanceListSize(); i++)
            {
                if (this.gameObject == CharacterManager.instance.getCharacterInstance(i))
                {
                    CharacterManager.instance.removeCharacterInstance(this.gameObject);
                }
            }
            for (int i = 0; i < EnemyManager.instance.getEnemyInstanceListSize(); i++)
            {
                if (this.gameObject == EnemyManager.instance.getEnemyInstance(i))
                {
                    EnemyManager.instance.removeEnemyInstance(this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
	}
    public int getAccuracy()
    {
        return accuracy;
    }
    public int getEvasion()
    {
        return evasion;
    }
    public void setSlot(int index)
    {
        slot = index;
    }
}
