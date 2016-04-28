using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour {
    public GameObject Hex;

    private float hexWidth;
    private float hexHeight;

    public TileBehaviour currentTile;

    public int tileX;
    public int tileY;

    private float minXCameraRestraint;
    private float maxXCameraRestraint;
    private float minYCameraRestraint;
    private float maxYCameraRestraint;


    // Use this for initialization
    void Start () {

        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.y;
        
        
        currentTile = (TileBehaviour)Hex.GetComponent("TileBehaviour");
        currentTile = GridManager.instance.normalizedBoard[new Point (0,0)];
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(currentTile.gridX, currentTile.gridY));
        minXCameraRestraint = GridManager.instance.calcWorldCoord(new Vector2(GridManager.instance.normalizedBoard[new Point(8,0)].gridX, GridManager.instance.normalizedBoard[new Point(8, 0)].gridY)).x;
        maxXCameraRestraint = GridManager.instance.calcWorldCoord(new Vector2(GridManager.instance.normalizedBoard[new Point(GridManager.instance.gridWidthInHexes-8, 0)].gridX, GridManager.instance.normalizedBoard[new Point(GridManager.instance.gridWidthInHexes - 8, 0)].gridY)).x;
        minYCameraRestraint = GridManager.instance.calcWorldCoord(new Vector2(GridManager.instance.normalizedBoard[new Point(0, 6)].gridX, GridManager.instance.normalizedBoard[new Point(0, 6)].gridY)).y;
        maxYCameraRestraint = GridManager.instance.calcWorldCoord(new Vector2(GridManager.instance.normalizedBoard[new Point(0, GridManager.instance.gridHeightInHexes-6)].gridX, GridManager.instance.normalizedBoard[new Point(0, GridManager.instance.gridHeightInHexes-6)].gridY)).y;

    }
	
	// Update is called once per frame
	void Update () {

        if (GridManager.instance.pause)
            return;

        currentTile.RemoveHighlight();

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {

            if (GridManager.instance.normalizedBoard.ContainsKey(new Point(currentTile.gridX + currentTile.gridY % 2, currentTile.gridY + 1)))
            { 

                currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX + currentTile.gridY % 2, currentTile.gridY + 1)];
                moveCursor(currentTile);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {

            if (GridManager.instance.normalizedBoard.ContainsKey(new Point(currentTile.gridX - (currentTile.gridY - 1) % 2, currentTile.gridY - 1)))
            {
                currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX - (currentTile.gridY - 1) % 2, currentTile.gridY - 1)];
                moveCursor(currentTile);
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (GridManager.instance.normalizedBoard.ContainsKey(new Point(currentTile.gridX + 1, currentTile.gridY)))
            {
                currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX + 1, currentTile.gridY)];

                moveCursor(currentTile);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (GridManager.instance.normalizedBoard.ContainsKey(new Point(currentTile.gridX - 1, currentTile.gridY)))
            {
                currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX - 1, currentTile.gridY)];
                moveCursor(currentTile);
            }
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GridManager.instance.unitSelected = false;
            GridManager.instance.hideMovementRange();
            if (GridManager.instance.originTileTB != null)
                GridManager.instance.originTileTB = null;
            if (GridManager.instance.destTileTB != null)
            {
                GridManager.instance.destTileTB.RemoveHighlight();
                GridManager.instance.destTileTB = null;
                
            }
            
            GridManager.instance.generateAndShowPath();
        }

        Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -8);
        //Camera.main.transform.position = new Vector3(
        //    Mathf.Clamp(this.transform.position.x, minXCameraRestraint, maxXCameraRestraint),
        //    Mathf.Clamp(this.transform.position.y, minYCameraRestraint, maxYCameraRestraint),
        //    -8);
        tileX = currentTile.gridX;
        tileY = currentTile.gridY;

        currentTile.HighlightCursor();
        if (!currentTile.containsEnemy())
            currentTile.UnlockTile();
        else
        {

            GridManager.instance.HealthText.GetComponentInParent<Image>().color = Color.red;
            GridManager.instance.HealthText.text = "Unit Health: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().healthCurrent + "/" + currentTile.containedCharacter.GetComponent<CharacterStatus>().healthMax;
            GridManager.instance.AccuracyText.text = "Accuracy: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().getAccuracy();
            GridManager.instance.EvadeText.text = "Evade: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().getEvasion();
            GridManager.instance.AttackPowerText.text = "Attack Power: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().strength;
            GridManager.instance.DefenseText.text = "Defense: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().defense;

            GridManager.instance.NameText.text = currentTile.containedCharacter.GetComponent<CharacterStatus>().name;
            GridManager.instance.LevelText.text = "Level: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().currentLevel;
            GridManager.instance.ExpText.text = "Exp: " + currentTile.containedCharacter.GetComponent<CharacterStatus>().experience;

            GridManager.instance.DefenseBonusText.text = "Defense: " + currentTile.terrainType.defenseBonus;
            GridManager.instance.EvadeBonusText.text = "Evade: " + currentTile.terrainType.evadeBonus;
            GridManager.instance.TerrainNameText.text = "Terrain: " + currentTile.terrainType.name;

            int tempFriendship = Mathf.Abs(currentTile.containedCharacter.GetComponent<CharacterStatus>().friendship);
            
            if (tempFriendship >= 0 && tempFriendship < 15)
            {
                GridManager.instance.AllegianceText.text = "Allegiance: Unsatisfied with current faction.";
            }
            else if (tempFriendship >= 15 && tempFriendship < 49)
            {
                GridManager.instance.AllegianceText.text = "Allegiance: Beginning to question current faction.";
            }
            if (tempFriendship >= 50)
            {
                GridManager.instance.AllegianceText.text = "Allegiance: Satisfied with current faction";
            }
        }

    }
    void moveCursor(TileBehaviour tile)
    {
        transform.position = GridManager.instance.calcWorldCoord(new Vector2(tile.gridX, tile.gridY));
        return;
    }
}
