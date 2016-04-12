

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
/*
 * @author koltara
 * This class handles turn management as well as the combat functionality
 * 
 * nextPhase() - Cycles to the next phase of the game once a faction has moved all of their units
 * Player Phase -> Enemy Phase -> Neutral Phase
 * 
 * initiateCombat() - Initiates combat between two units. Each is given an opportunity to attack, provided the initial attack does not kill the target.
 * After combat, the units undergo friendship modification depending on the net damage dealt/received during this combat.
 * 
 * Friendship is gained - When the unit deals more damage to the enemy than they received.
 * Friendship is lost - When the unit receives more damage than they dealt.
 * Nothing is happens - When the net damage is equal.
 * 
 * Friendship lost is a function of their loyalty value.
 * 
 * 
 */




public class TurnManager : MonoBehaviour {
    public int inspectorPhase = 0;
    private int phaseStatus;
    private int turnCount;

    public const int kPlayer = 0;
    public const int kEnemy = 1;
    public const int kNeutral = 2;
    public const int kLoyaltyMod = 10;

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
        int netDamageInitiator = 0;
        int netDamageTarget = 0;

        if (hitRoll < (initiator.GetComponent<CharacterStatus>().getAccuracy() - target.GetComponent<CharacterStatus>().getEvasion()))
        {
            
            int tempDamage = initiator.GetComponent<CharacterStatus>().strength;
            int tempDefense = target.GetComponent<CharacterStatus>().defense;
            netDamageInitiator = (tempDamage - tempDefense);

            
            if (tempDamage > tempDefense)
            {
                target.GetComponent<CharacterStatus>().healthCurrent -= netDamageInitiator;
                Debug.Log(initiator + "Hit a " + hitRoll + " for " + netDamageInitiator);
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
            netDamageTarget = (tempDamage - tempDefense);
            if (tempDamage > tempDefense)
            { 
                initiator.GetComponent<CharacterStatus>().healthCurrent -= netDamageTarget;
                Debug.Log(target + "Hit a " + hitRoll + " for " + netDamageTarget);
            }
                else Debug.Log(target + "Hit a " + hitRoll + " for " + 0);
            if (initiator.GetComponent<CharacterStatus>().healthCurrent <= 0)
            {
                return;
            }
        }
        else Debug.Log(target + "Missed with a " + hitRoll);

        //Friendship Modifications
        //Loyalty

        //If retaliation damage is higher than the initiators damage, reduce friendship for the initiator and increase for the target.

        GameObject moveToEnemy;
        GameObject moveToAlly;
        
        if (netDamageTarget > netDamageInitiator)
        {
            for (int i = 0; i < this.gameObject.GetComponent<CharacterManager>().getCharacterInstanceListSize(); i++)
            {

                if (initiator == CharacterManager.instance.getCharacterInstance(i))
                {
                    int loyal = initiator.GetComponent<CharacterStatus>().loyalty;
                    int net = (int)(3 * (loyal / 100.000) * netDamageTarget);
                    initiator.GetComponent<CharacterStatus>().friendship -= net;
                    target.GetComponent<CharacterStatus>().friendship -= net;

                    if (initiator.GetComponent<CharacterStatus>().friendship <= 0)
                    {
                        moveToEnemy = initiator;
                    }
                    break;
                }

            }
            for (int i = 0; i < this.gameObject.GetComponent<EnemyManager>().getEnemyInstanceListSize(); i++)
            {

                if (initiator == EnemyManager.instance.getEnemyInstance(i))
                {
                    int loyal = initiator.GetComponent<CharacterStatus>().loyalty;
                    int net = (int)(3 * (loyal / 100.000) * netDamageTarget);
                    initiator.GetComponent<CharacterStatus>().friendship += net;
                    target.GetComponent<CharacterStatus>().friendship += net;

                    if (initiator.GetComponent<CharacterStatus>().friendship >= 0)
                    {
                        moveToAlly = initiator;
                    }
                    break;
                }
            }

        } else if (netDamageTarget < netDamageInitiator)
        {
            for (int i = 0; i < this.gameObject.GetComponent<CharacterManager>().getCharacterInstanceListSize(); i++)
            {

                if (initiator == CharacterManager.instance.getCharacterInstance(i))
                {
                    int loyal = initiator.GetComponent<CharacterStatus>().loyalty;
                    int net = (int)(3 * (loyal / 100.000) * netDamageInitiator);
                    initiator.GetComponent<CharacterStatus>().friendship += net;
                    target.GetComponent<CharacterStatus>().friendship += net;

                    if (target.GetComponent<CharacterStatus>().friendship >= 0)
                    {
                        moveToAlly = target;
                    }
                    break;
                }
            }
            for (int i = 0; i < this.gameObject.GetComponent<EnemyManager>().getEnemyInstanceListSize(); i++)
            {

                if (initiator == EnemyManager.instance.getEnemyInstance(i))
                {
                    int loyal = initiator.GetComponent<CharacterStatus>().loyalty;
                    int net = (int)(3 * (loyal / 100.000) * netDamageInitiator);
                    initiator.GetComponent<CharacterStatus>().friendship -= net;
                    target.GetComponent<CharacterStatus>().friendship += net;

                    if (target.GetComponent<CharacterStatus>().friendship <= 0)
                    {
                        moveToEnemy = target;
                    }
                    break;
                }
            }
            
        }
        if (moveToEnemy != null)
        {
            CharacterManager.instance.removeCharacterInstance(moveToEnemy);
            EnemyManager.instance.addCharacterInstance(moveToEnemy);
        }
        if (moveToAlly != null)
        {
            CharacterManager.instance.addCharacterInstance(moveToAlly);
            EnemyManager.instance.removeCharacterInstance(moveToAlly);
        }


    }
}
