using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TileBehaviour : MonoBehaviour
{

    public enum TerrainTypes
    {
        kNormal = 0,
        kForest = 1,
        kHill = 2,
        kWater = 3
    };
    public Tile tile;
    
    public Material OpaqueMaterial;
    public Material defaultMaterial;

    public int gridX;
    public int gridY;

    public TerrainType terrainType;

    public GameObject containedCharacter;
    private bool playerSpawnTile = false;
    private bool enemySpawnTile = false;
    private bool hasEnemy = false;

    

    //Slightly transparent orange
    public static Color orange = new Color(255f / 255f, 127f / 255f, 0, 127f / 255f);
    public static Color lightGreen = new Color(0f / 255f, 255f / 255f, 86f/255f, 255f / 255f);
    public static Color darkGreen = new Color(7f / 255f, 169f / 255f, 15f / 255f, 255f / 255f);

    public void setEnemy(bool enemy)
    {
        hasEnemy = enemy;
    }
    public bool containsEnemy()
    {
        return hasEnemy;
    }

    public bool getTileStatus()
    {
        return playerSpawnTile;
    }
    public void setPlayerSpawnStatus(bool spawn)
    {
        playerSpawnTile = spawn;
    }
    public bool getEnemySpawnTile()
    {
        return enemySpawnTile;
    }
    public void setEnemySpawnTile(bool spawn)
    {
        enemySpawnTile = spawn;
    }


    public GameObject getContainedCharacter()
    {
        return containedCharacter;
    }
    public void setContainedCharacter(GameObject character)
    {
        containedCharacter = character;
    }
    

    void Update()
    {
        
        gridX = tile.X + tile.Y / 2;
        gridY = tile.Y;
        //if (this.containedCharacter != null)
        //    this.containedCharacter.transform.position = this.transform.position;
        
    }

    public void changeColor(Color color)
    {
        //If transparency is not set already, set it to default value
        if (color.a == 1)
            color.a = 130f / 255f;
        GetComponent<Renderer>().material = OpaqueMaterial;
        GetComponent<Renderer>().material.color = color;
    }
    void Start()
    {
        
    }

    
    public void HighlightCursor()
    {
            //GridManager.instance.selectedTile = tile;

            //if (tile.Passable && this != GridManager.instance.destTileTB
            //    && this != GridManager.instance.originTileTB && !GridManager.instance.possibleMoves.Contains(this))
            //{
            //    changeColor(orange);
            //}
    }

    //changes back to fully transparent material
    public void RemoveHighlight()
    {
            //GridManager.instance.selectedTile = null;
            //if (tile.Passable && this != GridManager.instance.destTileTB
            //&& this != GridManager.instance.originTileTB && !GridManager.instance.possibleMoves.Contains(this))
            //{
            //    this.GetComponent<Renderer>().material = defaultMaterial;
            //    this.GetComponent<Renderer>().material.color = Color.white;
            //}
    }
    //called every frame when cursor is on this tile
    public void UnlockTile()
    {

        if (this.containedCharacter != null)
        {
            GridManager.instance.HealthText.text = "Unit Health: " + containedCharacter.GetComponent<CharacterStatus>().healthCurrent + "/" + containedCharacter.GetComponent<CharacterStatus>().healthMax;
            GridManager.instance.AccuracyText.text = "Accuracy: " + containedCharacter.GetComponent<CharacterStatus>().getAccuracy();
            GridManager.instance.EvadeText.text = "Evade: " + containedCharacter.GetComponent<CharacterStatus>().getEvasion();
            GridManager.instance.AttackPowerText.text = "Attack Power: " + (containedCharacter.GetComponent<CharacterStatus>().strength + containedCharacter.GetComponent<CharacterStatus>().strengthMod);
            GridManager.instance.DefenseText.text = "Defense: " + (containedCharacter.GetComponent<CharacterStatus>().defense + containedCharacter.GetComponent<CharacterStatus>().defenseMod);
            GridManager.instance.HealthText.GetComponentInParent<Image>().color = Color.blue;
        }
        else
        {
            GridManager.instance.HealthText.text = " - ";
            GridManager.instance.AccuracyText.text = " - ";
            GridManager.instance.EvadeText.text = " - ";
            GridManager.instance.AttackPowerText.text = " - ";
            GridManager.instance.DefenseText.text = " - ";
            GridManager.instance.HealthText.GetComponentInParent<Image>().color = Color.white;
        }
        //Toggle impassable
        if (Input.GetKeyUp(KeyCode.A))
        {
            
            if (this == GridManager.instance.destTileTB ||
                this == GridManager.instance.originTileTB)
                return;
            tile.Passable = !tile.Passable;
            if (!tile.Passable)
                changeColor(Color.gray);
            else
            {
                this.GetComponent<Renderer>().material = OpaqueMaterial;
                this.GetComponent<Renderer>().material.color = terrainType.tileColor;
            }
            
            GridManager.instance.generateAndShowPath();
        }
       
        if (Input.GetKeyUp(KeyCode.S))
        {
            
            if (GridManager.instance.originTileTB != null)
                GridManager.instance.unitSelected = true;

            //tile.Passable = true;
            if (this.containedCharacter != null)
            {
                //Space has a character
                if (GridManager.instance.unitSelected)
                {
                    
                    return;
                }
                if (!this.containedCharacter.GetComponent<CharacterStatus>().ableToMove)
                    return;
                GridManager.instance.unitSelected = true;

                TileBehaviour originTileTB = GridManager.instance.originTileTB;
                //if user clicks on origin tile or origin tile is not assigned yet
                if (GridManager.instance.unitSelected)
                    if (this == originTileTB || originTileTB == null)
                        GridManager.instance.originTileTB = this;

                //else
                //    destTileChanged();
                GridManager.instance.hideMovementRange();
                GridManager.instance.generateMovementRange(this, 5);
                GridManager.instance.generateAndShowPath();
            } else
            {
                //Space has no character
                if (GridManager.instance.possibleMoves.Contains(this) && GridManager.instance.unitSelected)
                {
                    TileBehaviour originTileTB = GridManager.instance.originTileTB;

                    
                    GridManager.instance.destTileTB = this;
                    originTileTB.containedCharacter.GetComponent<SimpleCharacterMovement>().currentTB = this;
                    originTileTB.containedCharacter.GetComponent<CharacterStatus>().ableToMove = false;
                    GridManager.instance.generateAndShowPath();
                    GridManager.instance.hideMovementRange();

                    foreach (Tile tile in this.tile.AllNeighbours)
                    {
                        TileBehaviour tileTB = GridManager.instance.board[tile.Location];
                        if (tileTB.hasEnemy)
                        {
                            TurnManager.instance.initiateCombat(this.containedCharacter, tileTB.containedCharacter);
                            Debug.Log(this.containedCharacter + "fighting " + tileTB.containedCharacter);
                        }
                    }


                }
                else
                {
                    GridManager.instance.destTileTB = null;
                    GridManager.instance.originTileTB = null;
                    GridManager.instance.hideMovementRange();
                    GridManager.instance.generateAndShowPath();
                    
                }
                GridManager.instance.unitSelected = false;
            }
        }
    }

    public void originTileChanged()
    {
        var originTileTB = GridManager.instance.originTileTB;
        //deselect origin tile if user clicks on current origin tile
        //if (this == originTileTB)
        //{
        //    GridManager.instance.originTileTB = null;
        //    GetComponent<Renderer>().material = defaultMaterial;
        //    return;
        //}
        //if origin tile is not specified already mark this tile as origin
        GridManager.instance.originTileTB = this;
        changeColor(Color.yellow);
    }

    public void destTileChanged()
    {
        var destTile = GridManager.instance.destTileTB;
        //deselect destination tile if user clicks on current destination tile
        if (this == destTile)
        {
            GridManager.instance.destTileTB = null;
            GetComponent<Renderer>().material.color = terrainType.tileColor;
            return;
        }
        //if there was other tile marked as destination, change its material to default (fully transparent) one
        if (destTile != null)
        {
            
            if (GridManager.instance.possibleMoves.Contains(destTile))
            {
                changeColor(Color.blue);
            } else destTile.GetComponent<Renderer>().material = OpaqueMaterial;
        }
        GridManager.instance.destTileTB = this;
        //changeColor(Color.blue);
    }
}
