using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;



public class TurnManager : MonoBehaviour {
    public int inspectorPhase = 0;
    private int phaseStatus;
    private int turnCount;

    public static const int kPlayer = 0;
    public static const int kEnemy = 1;
    public static const int kNeutral = 2;

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
                    CharacterManager.instance.getCharacterInstance(j).GetComponent<CharacterStatus>().ableToMove = true;
                }
                break;
            case kEnemy:
                break;
            case kNeutral:
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
}
