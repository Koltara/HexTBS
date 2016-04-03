using UnityEngine;
using System.Collections;

public class CursorMovement : MonoBehaviour {
    public GameObject Hex;

    private float hexWidth;
    private float hexHeight;

    public TileBehaviour currentTile;

    public int tileX;
    public int tileY;


    // Use this for initialization
    void Start () {

        hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
        hexHeight = Hex.GetComponent<Renderer>().bounds.size.y;
        
        
        currentTile = (TileBehaviour)Hex.GetComponent("TileBehaviour");
        currentTile = GridManager.instance.normalizedBoard[new Point (0,0)];
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(currentTile.gridX, currentTile.gridY));


    }
	
	// Update is called once per frame
	void Update () {

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
        tileX = currentTile.gridX;
        tileY = currentTile.gridY;

        currentTile.HighlightCursor();
        if (!currentTile.containsEnemy())
            currentTile.UnlockTile();
        
    }
    void moveCursor(TileBehaviour tile)
    {
        transform.position = GridManager.instance.calcWorldCoord(new Vector2(tile.gridX, tile.gridY));
        return;
    }
}
