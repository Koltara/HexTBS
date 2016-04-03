using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class EnemyManager : MonoBehaviour {

    List<GameObject> enemyList;
    List<GameObject> enemyInstanceList;
    public static EnemyManager instance;
    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public int getEnemyListSize()
    {
        return enemyList.Count();
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
