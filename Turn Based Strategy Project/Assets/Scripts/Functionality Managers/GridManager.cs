﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

public class GridManager : MonoBehaviour {

    
    public GameObject Hex;
    public GameObject Line;
    public GameObject Cursor;
    public GameObject PlayerChar;
    public GameObject EnemyChar;
    GameObject playerCharInstance;

    public CursorMovement cursorScript;


    public int gridWidthInHexes;
    public int gridHeightInHexes;

    
    public Tile selectedTile = null;
    public TileBehaviour originTileTB = null;
    public TileBehaviour destTileTB = null;

    public static GridManager instance = null;
    public Dictionary<Point, TileBehaviour> board;
    public Dictionary<Point, TileBehaviour> normalizedBoard;

    public Boolean unitSelected = false;
    public Text HealthText;
    public Text AttackPowerText;
    public Text DefenseText;
    public Text AccuracyText;
    public Text EvadeText;



    List<GameObject> path;
    public List<TileBehaviour> possibleMoves;

    //void Awake()
    //{
    //    instance = this;
    //    setSizes();
    //    createGrid();
    //    generateAndShowPath();
    //}

    //Hexagon tile width and height in game world
    private float hexWidth;
    private float hexHeight;


    

    //Method to initialise Hexagon width and height
    void setSizes()
    {
        //renderer component attached to the Hex prefab is used to get the current width and height
        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.y;
    }

    //Method to calculate the position of the first hexagon tile
    //The center of the hex grid is (0,0,0)
    Vector3 calcInitPos()
    {
        Vector3 initPos;
        //the initial position will be in the left upper corner
        initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, gridHeightInHexes / 2f * hexHeight - hexHeight / 2, 0);

        return initPos;
    }

    //method used to convert hex grid coordinates to game world coordinates
    public Vector3 calcWorldCoord(Vector2 gridPos)
    {
        //Position of the first hex tile
        Vector3 initPos = calcInitPos();
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = initPos.x + offset + gridPos.x * hexWidth;
        //Every new line is offset in z direction by 3/4 of the hexagon height
        float y = initPos.y - gridPos.y * hexHeight * 0.75f;
        return new Vector3(x, y, 0);
    }
    
    //Finally the method which initialises and positions all the tiles
    void createGrid()
    {
        Vector2 gridSize = new Vector2(gridWidthInHexes, gridHeightInHexes);

        //Game object which is the parent of all the hex tiles
        GameObject hexGridGO = new GameObject("HexGrid2");

        

        board = new Dictionary<Point, TileBehaviour>();
        normalizedBoard = new Dictionary<Point, TileBehaviour>();

        for (float y = 0; y < gridSize.y; y++)
        {
            
            for (float x = 0; x < gridSize.x; x++)
            {
                //GameObject assigned to Hex public variable is cloned
                GameObject hex = (GameObject)Instantiate(Hex);
                //Current position in grid
                Vector2 gridPos = new Vector2(x, y);
                hex.transform.position = calcWorldCoord(gridPos);
                hex.transform.parent = hexGridGO.transform;

                var tb = (TileBehaviour)hex.GetComponent("TileBehaviour");
                tb.tile = new Tile((int)x - (int)(y / 2), (int)y);

                if (x % 3 == 0 && x != 0)
                    tb.terrainType = new TerrainType((int)TileBehaviour.TerrainTypes.kForest);
                else tb.terrainType = new TerrainType((int)TileBehaviour.TerrainTypes.kNormal);

                if (x % 4 == 0 && y == 0)
                    tb.setPlayerSpawnStatus(true);
                if (x % 5 == 0 && y == 5)
                    tb.setEnemySpawnTile(true);

                board.Add(tb.tile.Location, tb);

                normalizedBoard.Add(new Point ((int)x, (int)y), tb);

                tb.GetComponent<Renderer>().material = tb.OpaqueMaterial;
                tb.GetComponent<Renderer>().material.color = tb.terrainType.tileColor;
                tb.setContainedCharacter(null);

                if (x == 0 && y == 0)
                {
                    tb.GetComponent<Renderer>().material = tb.OpaqueMaterial;
                    //Color red = Color.red;
                    //red.a = 158f / 255f;
                    tb.GetComponent<Renderer>().material.color = tb.terrainType.tileColor;
                    originTileTB = tb;
                }
            }
        }

        
        Cursor.transform.position = calcWorldCoord(new Vector2(0, 0));

        bool equalLineLengths = (gridSize.x + 0.5) * hexWidth <= gridSize.x;
        foreach (TileBehaviour tb in board.Values)
            tb.tile.FindNeighbours(board, gridSize, equalLineLengths);
    }
   
    double calcDistance(Tile tile)
    {
        Tile destTile = destTileTB.tile;

        float deltaX = Mathf.Abs(destTile.X - tile.X);
        float deltaY = Mathf.Abs(destTile.Y - tile.Y);
        int z1 = -(tile.X + tile.Y);
        int z2 = -(destTile.X + destTile.Y);
        float deltaZ = Mathf.Abs(z2 - z1);

        return Mathf.Max(deltaX, deltaY, deltaZ);
    }
    private void DrawPath(IEnumerable<Tile> path)
    {
        if (this.path == null)
            this.path = new List<GameObject>();
        //Destroy game objects which used to indicate the path
        this.path.ForEach(Destroy);
        this.path.Clear();

        //Lines game object is used to hold all the "Line" game objects indicating the path
        GameObject lines = GameObject.Find("Lines");
        if (lines == null)
            lines = new GameObject("Lines");
        if (path == null)
            return;
        //foreach (Tile tile in path)
        //{
        //    var line = (GameObject)Instantiate(Line);
        //    //calcWorldCoord method uses squiggly axis coordinates so we add y / 2 to convert x coordinate from straight axis coordinate system
        //    Vector2 gridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
        //    line.transform.position = calcWorldCoord(gridPos);
        //    this.path.Add(line);
        //    line.transform.parent = lines.transform;
        //}
    }
    public void generateAndShowPath()
    {
        //Don't do anything if origin or destination is not defined yet
        if (originTileTB == null || destTileTB == null)
        {
            DrawPath(new List<Tile>());
            return;
        }
        //We assume that the distance between any two adjacent tiles is 1
        
        Func<TileBehaviour, TileBehaviour, double> distance = (node1, node2) => node2.terrainType.cost;

        var path = PathFinder.FindPath(originTileTB.tile, destTileTB.tile);
        DrawPath(path);
        //if (destTileTB != null)
        //{
        //    if (originTileTB.containedCharacter != null)
        //    {
                destTileTB.setContainedCharacter(originTileTB.getContainedCharacter());
                originTileTB.setContainedCharacter(null);
                destTileTB.getContainedCharacter().GetComponent<SimpleCharacterMovement>().MoveTo(destTileTB);
                this.originTileTB = null;
                this.destTileTB = null;
        //    }
        //}
    }

    //The grid should be generated on game start
    void Start()
    {
         
        instance = this;
        setSizes();
        Instantiate(Cursor);
        
        cursorScript = Cursor.GetComponent<CursorMovement>();
        createGrid();
        spawnCharacters();

        originTileTB = null;
        destTileTB = null;

        generateAndShowPath();
    }
    void spawnCharacters()
    {
        int characterListPosition = 0;
        int enemyListPosition = 0;

        this.GetComponent<CharacterManager>().generateCharacterList();
        this.GetComponent<EnemyManager>().generateEnemyList();

        populateCharacterList(3);
        populateEnemyList(4);
        Debug.Log("Character Name: " + this.GetComponent<CharacterManager>().getCharacter(0).GetComponent<CharacterStatus>().characterName);
        Debug.Log("Character Name: " + this.GetComponent<CharacterManager>().getCharacter(1).GetComponent<CharacterStatus>().characterName);

        //Iterate through the gameboard, spawning characters from the character list at player spawn tiles
        for (int i = 0; i < gridWidthInHexes; i++)
            for (int j = 0; j < gridHeightInHexes; j++)
            {
                //If we've spawned every character
                if (characterListPosition >= this.GetComponent<CharacterManager>().getCharacterListSize())
                    break;
                //Store character to spawn
                GameObject tempCharacter = this.GetComponent<CharacterManager>().getCharacter(characterListPosition);
                
                //Store current tile
                TileBehaviour tempTB = normalizedBoard[new Point(i,j)];

                if (tempTB.getTileStatus())
                {
                    GameObject charInstance;
                    if (characterListPosition < this.GetComponent<CharacterManager>().getCharacterListSize())
                    {
                        Debug.Log("Generated Character at: " + characterListPosition);
                        charInstance = Instantiate(tempCharacter);
                        tempTB.setContainedCharacter(charInstance);

                        charInstance.GetComponent<SimpleCharacterMovement>().currentTB = tempTB;
                        charInstance.transform.position = calcWorldCoord(new Vector2(i, j));
                        charInstance.name = tempCharacter.GetComponent<CharacterStatus>().characterName;

                        this.GetComponent<CharacterManager>().addCharacterInstance(charInstance);
                        characterListPosition++;
                    }
                }

            }
        //Once more for enemy spawns
        for (int i = 0; i < gridWidthInHexes; i++)
            for (int j = 0; j < gridHeightInHexes; j++)
            {
                //If we've spawned every character
                if (enemyListPosition >= this.GetComponent<EnemyManager>().getEnemyListSize())
                    break;
                //Store character to spawn
                GameObject tempEnemy = this.GetComponent<EnemyManager>().getEnemy(enemyListPosition);
                //Store current tile
                TileBehaviour tempTB = normalizedBoard[new Point(i, j)];

                if (tempTB.getEnemySpawnTile())
                {
                    GameObject enemyInstance;
                    if (enemyListPosition < this.GetComponent<EnemyManager>().getEnemyListSize())
                    {
                        
                        enemyInstance = Instantiate(tempEnemy);
                        tempTB.setContainedCharacter(enemyInstance);
                        tempTB.tile.Passable = false;
                        tempTB.setEnemy(true);

                        enemyInstance.GetComponent<EnemyMovementController>().currentTB = tempTB;
                        enemyInstance.transform.position = calcWorldCoord(new Vector2(i, j));

                        this.GetComponent<EnemyManager>().addEnemyInstance(enemyInstance);
                        enemyListPosition++;
                    }
                }

            }
    }
    void populateCharacterList(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject tempChar = PlayerChar;

            tempChar.GetComponent<CharacterStatus>().characterName = "";
            tempChar.GetComponent<CharacterStatus>().characterName = "Player Unit " + (i+1);

            tempChar.GetComponent<CharacterStatus>().agility = 5;
            tempChar.GetComponent<CharacterStatus>().agilityMod = 0;
            tempChar.GetComponent<CharacterStatus>().agilityRate = 45;

            tempChar.GetComponent<CharacterStatus>().strength = 5;
            tempChar.GetComponent<CharacterStatus>().strengthMod = 0;
            tempChar.GetComponent<CharacterStatus>().strengthRate = 45;

            tempChar.GetComponent<CharacterStatus>().skill = 5;
            tempChar.GetComponent<CharacterStatus>().skillMod = 0;
            tempChar.GetComponent<CharacterStatus>().skillRate = 45;

            tempChar.GetComponent<CharacterStatus>().defense = 4;
            tempChar.GetComponent<CharacterStatus>().defenseMod = 0;
            tempChar.GetComponent<CharacterStatus>().defenseRate = 45;

            tempChar.GetComponent<CharacterStatus>().healthMax = 15;
            tempChar.GetComponent<CharacterStatus>().healthMaxMod = 0;
            tempChar.GetComponent<CharacterStatus>().healthRate = 45;

            tempChar.GetComponent<CharacterStatus>().healthCurrent = 15;
            tempChar.GetComponent<CharacterStatus>().healthCurrentMod = 0;

            tempChar.GetComponent<CharacterStatus>().moveDistance = 5;
            tempChar.GetComponent<CharacterStatus>().moveDistanceMod = 0;

            tempChar.GetComponent<CharacterStatus>().currentLevel = 1;
            tempChar.GetComponent<CharacterStatus>().levelCap = 20;
            tempChar.GetComponent<CharacterStatus>().experience = 0;

            tempChar.GetComponent<CharacterStatus>().loyalty = 50;
            tempChar.GetComponent<CharacterStatus>().courage = 50;
            tempChar.GetComponent<CharacterStatus>().greed = 50;
            tempChar.GetComponent<CharacterStatus>().friendship = 50;
            tempChar.GetComponent<CharacterStatus>().patience = 50;


            this.GetComponent<CharacterManager>().addCharacter(tempChar);

            
        }
    }
    void populateEnemyList(int size)
    {
        for (int i = 0; i < size; i++)
        {
            //GameObject tempCharInstance;
            EnemyChar.GetComponent<CharacterStatus>().characterName = "Enemy Unit " + (i + 1);

            EnemyChar.GetComponent<CharacterStatus>().agility = 5;
            EnemyChar.GetComponent<CharacterStatus>().agilityMod = 0;
            EnemyChar.GetComponent<CharacterStatus>().agilityRate = 45;

            EnemyChar.GetComponent<CharacterStatus>().strength = 5;
            EnemyChar.GetComponent<CharacterStatus>().strengthMod = 0;
            EnemyChar.GetComponent<CharacterStatus>().strengthRate = 45;

            EnemyChar.GetComponent<CharacterStatus>().skill = 5;
            EnemyChar.GetComponent<CharacterStatus>().skillMod = 0;
            EnemyChar.GetComponent<CharacterStatus>().skillRate = 45;

            EnemyChar.GetComponent<CharacterStatus>().healthMax = 15;
            EnemyChar.GetComponent<CharacterStatus>().healthMaxMod = 0;
            EnemyChar.GetComponent<CharacterStatus>().healthRate = 45;

            EnemyChar.GetComponent<CharacterStatus>().defense = 3;
            EnemyChar.GetComponent<CharacterStatus>().defenseMod = 0;
            EnemyChar.GetComponent<CharacterStatus>().defenseRate = 45;

            EnemyChar.GetComponent<CharacterStatus>().healthCurrent = 15;
            EnemyChar.GetComponent<CharacterStatus>().healthCurrentMod = 0;

            EnemyChar.GetComponent<CharacterStatus>().moveDistance = 5;
            EnemyChar.GetComponent<CharacterStatus>().moveDistanceMod = 0;

            EnemyChar.GetComponent<CharacterStatus>().currentLevel = 1;
            EnemyChar.GetComponent<CharacterStatus>().levelCap = 20;
            EnemyChar.GetComponent<CharacterStatus>().experience = 0;

            EnemyChar.GetComponent<CharacterStatus>().loyalty = 50;
            EnemyChar.GetComponent<CharacterStatus>().courage = 50;
            EnemyChar.GetComponent<CharacterStatus>().greed = 50;
            EnemyChar.GetComponent<CharacterStatus>().friendship = 50;
            EnemyChar.GetComponent<CharacterStatus>().patience = 50;


            this.GetComponent<EnemyManager>().addEnemy(EnemyChar);


        }
    }
    public void generateMovementRange(TileBehaviour startTile, int movementRange)
    {
        if (this.possibleMoves == null)
            this.possibleMoves = new List<TileBehaviour>();

        if (movementRange < 0)
        {
            return;
        }
        if (startTile.tile.Passable)
        {
            startTile.changeColor(Color.blue);
            possibleMoves.Add(startTile);
        }

        foreach(Tile tile in startTile.tile.Neighbours)
        {
            //if (!possibleMoves.Contains(board[tile.Location]))
            TileBehaviour tileTB = board[tile.Location];
            generateMovementRange(tileTB, movementRange - tileTB.terrainType.cost);
        }

    }
    public void hideMovementRange()
    {
        if (possibleMoves != null)
        {
            foreach (TileBehaviour tb in possibleMoves)
            {
                if (tb.tile.Passable) 
                {
                    tb.GetComponent<Renderer>().material = tb.OpaqueMaterial;
                    tb.GetComponent<Renderer>().material.color = tb.terrainType.tileColor;
                }
            }
            possibleMoves.Clear();
        }
        
    }
}
    
