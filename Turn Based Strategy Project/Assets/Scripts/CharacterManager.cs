using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterManager : MonoBehaviour {

    List<GameObject> characterList;
    public static CharacterManager instance;
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void generateCharacterList()
    {
        characterList = new List<GameObject>();
    }
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
