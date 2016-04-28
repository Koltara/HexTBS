using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EnemyManager : MonoBehaviour {

    List<GameObject> enemyList;
    List<GameObject> enemyInstanceList;
    public static EnemyManager instance;

    private int numActionableEnemies = 0;
    public int inspectorActionable = 0;
    // Use this for initialization
    void Start()
    {
        instance = this;

        setActionableEnemies();
    }
    public void setActionableEnemies()
    {
        for (int i = 0; i < enemyInstanceList.Count(); i++)
        {
            if (enemyInstanceList[i].GetComponent<CharacterStatus>().ableToMove)
                numActionableEnemies++;
        }
        inspectorActionable = numActionableEnemies;
    }
    // Update is called once per frame
    void Update()
    {
        if (enemyInstanceList.Count() == 0)
        {
            GridManager.instance.PhaseText.text = "Victory!";
            GridManager.instance.PhaseText.color = Color.blue;
            GridManager.instance.pause = true;

            

        }
        if (TurnManager.instance.getPhaseStatus() == TurnManager.kEnemy)
        {
            if (numActionableEnemies <= 0)
            {
                TurnManager.instance.nextPhase();

                return;
            }
            numActionableEnemies = 0;
            setActionableEnemies();
        }
        inspectorActionable = numActionableEnemies;
    }
    public int getEnemyListSize()
    {
        return enemyList.Count();
    }
    public int getEnemyInstanceListSize()
    {
        return enemyInstanceList.Count();
    }
    public void generateEnemyList()
    {
        enemyList = new List<GameObject>();
        enemyInstanceList = new List<GameObject>();
    }
    //Instantiated Enemy List
    public void addEnemyInstance(GameObject enemy)
    {
        enemyInstanceList.Add(enemy);
    }
    public void setEnemyInstance(GameObject enemy, int index)
    {

        if (enemyInstanceList.Count > index)
            enemyInstanceList[index] = enemy;
        else return;
    }
    public GameObject getEnemyInstance(int index)
    {
        if (enemyInstanceList.Count > index)
            return enemyInstanceList[index];
        else return null;
    }
    public void removeEnemyInstance(GameObject character)
    {
        enemyInstanceList.Remove(character);
    }

    //Reference Enemy List
    public void addEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }
    public void setEnemy(GameObject enemy, int index)
    {

        if (enemyList.Count > index)
            enemyList[index] = enemy;
        else return;
    }
    public GameObject getEnemy(int index)
    {
        if (enemyList.Count > index)
            return enemyList[index];
        else return null;
    }
}
