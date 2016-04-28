using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterManager : MonoBehaviour {

    List<GameObject> characterList;
    List<GameObject> characterInstanceList;
    public static CharacterManager instance;
    private int numActionableCharacters = 0;
    public int inspectorActionable = 0;
	// Use this for initialization
	void Start () {
        instance = this;
        setActionableCharacters();
	}
    public void setActionableCharacters()
    {
        for (int i = 0; i < characterInstanceList.Count(); i++)
        {
            if (characterInstanceList[i].GetComponent<CharacterStatus>().ableToMove)
                numActionableCharacters++;
        }
        inspectorActionable = numActionableCharacters;
    }
	
	// Update is called once per frame
	void Update () {
        if (characterInstanceList.Count() == 0)
        {
            GridManager.instance.PhaseText.text = "Game Over";
            GridManager.instance.PhaseText.color = Color.red;
            GridManager.instance.pause = true;

            
        }
        if (TurnManager.instance.getPhaseStatus() == TurnManager.kPlayer)
        {
            if (numActionableCharacters <= 0)
            {
                TurnManager.instance.nextPhase();
                
                return;
            }
            numActionableCharacters = 0;
            setActionableCharacters();
        }
        inspectorActionable = numActionableCharacters;
	}
    public int getCharacterListSize()
    {
        return characterList.Count();
    }
    public int getCharacterInstanceListSize()
    {
        return characterInstanceList.Count();
    }
    public void generateCharacterList()
    {
        characterList = new List<GameObject>();
        characterInstanceList = new List<GameObject>();
    }
    //Instantiated Character List
    public void addCharacterInstance(GameObject character)
    {
        characterInstanceList.Add(character);
    }
    public void removeCharacterInstance(GameObject character)
    {
        characterInstanceList.Remove(character);
    }
    public void setCharacterInstance(GameObject character, int index)
    {

        if (characterInstanceList.Count > index)
            characterInstanceList[index] = character;
        else return;
    }
    public GameObject getCharacterInstance(int index)
    {
        if (characterInstanceList.Count > index)
            return characterInstanceList[index];
        else return null;
    }

    //Reference Character List
    public void addCharacter(GameObject character)
    {
        characterList.Add(character);
    }
    public void setCharacter(GameObject character, int index)
    {

        if (characterList.Count > index)
            characterList[index] = character;
        else return;
    }
    public GameObject getCharacter(int index)
    {
        if (characterList.Count > index)
            return characterList[index];
        else return null;
    }
}
