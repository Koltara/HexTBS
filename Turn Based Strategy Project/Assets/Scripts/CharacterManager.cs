using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterManager : MonoBehaviour {

    List<GameObject> characterList;
    List<GameObject> characterInstanceList;
    public static CharacterManager instance;
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public int getCharacterListSize()
    {
        return characterList.Count();
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
