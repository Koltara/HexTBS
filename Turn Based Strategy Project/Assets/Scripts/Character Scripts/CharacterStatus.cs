using UnityEngine;
using System.Collections;

public class CharacterStatus : MonoBehaviour {
    private int slot;
    public static CharacterStatus instance;
    public bool ableToMove = true;

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
	    
	}
    public void setSlot(int index)
    {
        slot = index;
    }
}
