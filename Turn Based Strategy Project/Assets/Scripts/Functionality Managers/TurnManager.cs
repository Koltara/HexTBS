using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;



public class TurnManager : MonoBehaviour {
    public int inspectorPhase = 0;
    private int phaseStatus;
    private int turnCount;

    public const int kPlayer = 0;
    public const int kEnemy = 1;
    public const int kNeutral = 2;

    public static TurnManager instance;

    public int getTurnCount()
    {
        return turnCount;
    }

    public int getPhaseStatus()
    {
        return phaseStatus;
    }
    public void nextPhase()
    {
        if (phaseStatus < kNeutral)
            phaseStatus++;
        else
        {
            phaseStatus = kPlayer;
            turnCount++;
        }
        switch (phaseStatus)
        {
            case kPlayer:
                for (int j = 0; j < CharacterManager.instance.getCharacterInstanceListSize(); j++)
                {
                    if (CharacterManager.instance.getCharacterInstance(j) != null)
                        CharacterManager.instance.getCharacterInstance(j).GetComponent<CharacterStatus>().ableToMove = true;
                }
                CharacterManager.instance.setActionableCharacters();
                break;
            case kEnemy:
                for (int j = 0; j < EnemyManager.instance.getEnemyInstanceListSize(); j++)
                {
                    if (EnemyManager.instance.getEnemyInstance(j) != null)
                        EnemyManager.instance.getEnemyInstance(j).GetComponent<CharacterStatus>().ableToMove = true;
                }
                EnemyManager.instance.setActionableEnemies();
                break;
            case kNeutral:
                nextPhase();
                break;
        }
    }
	// Use this for initialization
	void Start () {
        instance = this;
        turnCount = 0;
        phaseStatus = kPlayer;
        inspectorPhase = phaseStatus;
	}
	
	// Update is called once per frame
	void Update () {
        inspectorPhase = phaseStatus;
	}
    public void initiateCombat(GameObject initiator, GameObject target)
    {
        //Initiator Attacks
        int hitRoll = (int)(UnityEngine.Random.value * 100);
        if (hitRoll < (initiator.GetComponent<CharacterStatus>().getAccuracy() - target.GetComponent<CharacterStatus>().getEvasion()))
        {
            
            int tempDamage = initiator.GetComponent<CharacterStatus>().strength;
            int tempDefense = target.GetComponent<CharacterStatus>().defense;
            int netDamage = (tempDamage - tempDefense);
            if (tempDamage > tempDefense)
            {
                target.GetComponent<CharacterStatus>().healthCurrent -= netDamage;
                Debug.Log(initiator + "Hit a " + hitRoll + " for " + netDamage);
            }
            else Debug.Log(initiator + "Hit a " + hitRoll + " for " + 0);
            if (target.GetComponent<CharacterStatus>().healthCurrent <= 0)
            {
                return;
            }
            
        } else Debug.Log(initiator + "Missed with a " + hitRoll);

        //Target Retaliation
        hitRoll = (int)(UnityEngine.Random.value * 100);
        if (hitRoll < (target.GetComponent<CharacterStatus>().getAccuracy() - initiator.GetComponent<CharacterStatus>().getEvasion()))
        {
            int tempDamage = target.GetComponent<CharacterStatus>().strength;
            int tempDefense = initiator.GetComponent<CharacterStatus>().defense;
            int netDamage = (tempDamage - tempDefense);
            if (tempDamage > tempDefense)
            { 
                initiator.GetComponent<CharacterStatus>().healthCurrent -= (tempDamage - tempDefense);
                Debug.Log(target + "Hit a " + hitRoll + " for " + netDamage);
            }
                else Debug.Log(target + "Hit a " + hitRoll + " for " + 0);
            if (initiator.GetComponent<CharacterStatus>().healthCurrent <= 0)
            {
                return;
            }
        }
        else Debug.Log(target + "Missed with a " + hitRoll);

    }
}
