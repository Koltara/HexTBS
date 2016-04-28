using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    public int allegiance = 0;

    public GameObject CombatText;
    public Canvas UnitCanvas;

    Canvas canvasInstance = null;

    public void InitiateCombatText(string text)
    {
        GameObject temp = Instantiate(CombatText) as GameObject;
        RectTransform tempRect = temp.GetComponent<RectTransform>();

        temp.transform.SetParent(canvasInstance.transform);

        tempRect.transform.localPosition = CombatText.transform.localPosition;
        //tempRect.transform.localScale = CombatText.transform.localScale;
        tempRect.transform.localRotation = CombatText.transform.localRotation;
        tempRect.transform.localPosition = new Vector3(0, 3f, 0);

        temp.GetComponent<Text>().text = text;
        Destroy(temp.gameObject, 2);
    }


	void Start () {
        instance = this;

        

        canvasInstance = Instantiate(UnitCanvas);
        canvasInstance.transform.SetParent(this.gameObject.transform);
        //canvasInstance.transform.localPosition = transform.localPosition;
        //canvasInstance.transform.localRotation = transform.localRotation;
        //canvasInstance.transform.localScale = transform.localScale;

	}
	
	// Update is called once per frame
	void Update () {

        canvasInstance.transform.localPosition = new Vector3(0, 0, 0);
        canvasInstance.transform.localRotation = this.gameObject.transform.localRotation;
        //canvasInstance.transform.localScale = this.gameObject.transform.localScale;

        int tempEvadeBonus = 0;

        if (friendship >= 0)
        {
            allegiance = TurnManager.kPlayer;
        }
        else allegiance = TurnManager.kEnemy;

        for (int i = 0; i < CharacterManager.instance.getCharacterInstanceListSize(); i++)
        {
            if (this.gameObject == CharacterManager.instance.getCharacterInstance(i))
            {
                this.allegiance = TurnManager.kPlayer;
            }
            
        }
        for (int i = 0; i < EnemyManager.instance.getEnemyInstanceListSize(); i++)
        {
            if (this.gameObject == EnemyManager.instance.getEnemyInstance(i))
            {
                this.allegiance = TurnManager.kEnemy;
            }

        }

        tempEvadeBonus = GetComponent<SimpleCharacterMovement>().currentTB.terrainType.evadeBonus;
        defenseMod = GetComponent<SimpleCharacterMovement>().currentTB.terrainType.defenseBonus;

        if (allegiance == TurnManager.kPlayer)
        {

            if (ableToMove || TurnManager.instance.getPhaseStatus() == TurnManager.kEnemy)
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
            else this.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        }
        else if (allegiance == TurnManager.kEnemy)
        {

            if (ableToMove || TurnManager.instance.getPhaseStatus() == TurnManager.kPlayer)
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            else this.gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        }

        accuracy = 80 + (skill * 2);
        evasion = 15 + (agility * 2) + tempEvadeBonus;


        

        if (healthCurrent <= 0)
        {
            if (allegiance == TurnManager.kPlayer)
            {
                TileBehaviour tempTB = this.gameObject.GetComponent<SimpleCharacterMovement>().currentTB;

                CharacterManager.instance.removeCharacterInstance(this.gameObject);

                tempTB.setContainedCharacter(null);
                Destroy(this.gameObject);
            } else if (allegiance == TurnManager.kEnemy)
            {
                TileBehaviour tempTB = this.gameObject.GetComponent<SimpleCharacterMovement>().currentTB;
                EnemyManager.instance.removeEnemyInstance(this.gameObject);
                tempTB.setContainedCharacter(null);
                tempTB.tile.Passable = true;
                tempTB.setEnemy(false);
                Destroy(this.gameObject);
            }
            //for (int i = 0; i < CharacterManager.instance.getCharacterInstanceListSize(); i++)
            //{
            //    if (this.gameObject == CharacterManager.instance.getCharacterInstance(i))
            //    {
            //        CharacterManager.instance.removeCharacterInstance(this.gameObject);
            //    }
            //}
            //for (int i = 0; i < EnemyManager.instance.getEnemyInstanceListSize(); i++)
            //{
            //    if (this.gameObject == EnemyManager.instance.getEnemyInstance(i))
            //    {
            //        EnemyManager.instance.removeEnemyInstance(this.gameObject);
            //    }
            //}
            
            this.gameObject.SetActive(false);
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
