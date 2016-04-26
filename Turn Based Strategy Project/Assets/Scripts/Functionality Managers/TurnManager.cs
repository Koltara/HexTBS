

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
                StartCoroutine(ShowPhaseTransition(kPlayer));
                StartCoroutine(activateUnits(phaseStatus));
                break;
            case kEnemy:
                StartCoroutine(ShowPhaseTransition(kEnemy));

                StartCoroutine(activateUnits(phaseStatus));
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
    IEnumerator activateUnits(int phaseStatus)
    {
        yield return new WaitForSeconds(0);
        if (phaseStatus == kPlayer)
        {
            for (int j = 0; j < CharacterManager.instance.getCharacterInstanceListSize(); j++)
            {
                if (CharacterManager.instance.getCharacterInstance(j) != null)
                    CharacterManager.instance.getCharacterInstance(j).GetComponent<CharacterStatus>().ableToMove = true;
            }
            CharacterManager.instance.setActionableCharacters();
        } else if (phaseStatus == kEnemy)
        {
            for (int j = 0; j < EnemyManager.instance.getEnemyInstanceListSize(); j++)
            {
                if (EnemyManager.instance.getEnemyInstance(j) != null)
                    EnemyManager.instance.getEnemyInstance(j).GetComponent<CharacterStatus>().ableToMove = true;
            }
            EnemyManager.instance.setActionableEnemies();
        }
        
    }
    
	
	// Update is called once per frame
	void Update () {
        inspectorPhase = phaseStatus;
	}
    IEnumerator ShowPhaseTransition(int newPhase)
    {
        switch (newPhase)
        {
            case kPlayer:
                GridManager.instance.PhaseText.gameObject.SetActive(true);
                GridManager.instance.PhaseText.text = "Player Phase";
                yield return new WaitForSeconds(2);
                GridManager.instance.PhaseText.text = "";
                GridManager.instance.PhaseText.gameObject.SetActive(false);
                break;
            case kEnemy:
                GridManager.instance.PhaseText.gameObject.SetActive(true);
                GridManager.instance.PhaseText.text = "Enemy Phase";
                yield return new WaitForSeconds(2);
                GridManager.instance.PhaseText.text = "";
                GridManager.instance.PhaseText.gameObject.SetActive(false);
                break;
        }
    }
    public void initiateCombat(GameObject initiator, GameObject target)
    {
        //Initiator Attacks
        int hitRoll = (int)(UnityEngine.Random.value * 100);
        int netDamageInitiator = 0;
        int netDamageTarget = 0;

        

        if (hitRoll < (initiator.GetComponent<CharacterStatus>().getAccuracy() - target.GetComponent<CharacterStatus>().getEvasion()))
        {

            int tempDamage = initiator.GetComponent<CharacterStatus>().strength + initiator.GetComponent<CharacterStatus>().strengthMod;
            int tempDefense = target.GetComponent<CharacterStatus>().defense + target.GetComponent<CharacterStatus>().defenseMod;
            netDamageInitiator = (tempDamage - tempDefense);

            
            if (tempDamage > tempDefense)
            {
                target.GetComponent<CharacterStatus>().healthCurrent -= netDamageInitiator;
                
                target.GetComponent<CharacterStatus>().InitiateCombatText("-" + netDamageInitiator);
                Debug.Log(initiator + "Hit a " + hitRoll + " for " + netDamageInitiator);
            }
            else target.GetComponent<CharacterStatus>().InitiateCombatText("-" + 0);
            if (target.GetComponent<CharacterStatus>().healthCurrent <= 0)
            {
                return;
            }

        }
        else target.GetComponent<CharacterStatus>().InitiateCombatText("Miss");

        //Target Retaliation
        hitRoll = (int)(UnityEngine.Random.value * 100);
        if (hitRoll < (target.GetComponent<CharacterStatus>().getAccuracy() - initiator.GetComponent<CharacterStatus>().getEvasion()))
        {
            int tempDamage = target.GetComponent<CharacterStatus>().strength + target.GetComponent<CharacterStatus>().strengthMod;
            int tempDefense = initiator.GetComponent<CharacterStatus>().defense + initiator.GetComponent<CharacterStatus>().defenseMod;
            netDamageTarget = (tempDamage - tempDefense);
            if (tempDamage > tempDefense)
            { 
                initiator.GetComponent<CharacterStatus>().healthCurrent -= netDamageTarget;
                initiator.GetComponent<CharacterStatus>().InitiateCombatText("-" + netDamageTarget);
            }
            else initiator.GetComponent<CharacterStatus>().InitiateCombatText("-" + 0);
            if (initiator.GetComponent<CharacterStatus>().healthCurrent <= 0)
            {
                return;
            }
        }
        else initiator.GetComponent<CharacterStatus>().InitiateCombatText("Miss");

        //Friendship Modifications
        //Loyalty

        //If retaliation damage is higher than the initiators damage, reduce friendship for the initiator and increase for the target.

        GameObject moveToEnemy = null;
        GameObject moveToAlly = null;
        
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
            EnemyManager.instance.addEnemyInstance(moveToEnemy);
        }
        if (moveToAlly != null)
        {
            CharacterManager.instance.addCharacterInstance(moveToAlly);
            EnemyManager.instance.removeEnemyInstance(moveToAlly);
        }


    }
}
