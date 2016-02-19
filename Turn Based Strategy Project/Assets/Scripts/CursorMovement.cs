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
        currentTile = GridManager.instance.normalizedBoard[GridManager.instance.originTileTB.tile.Location];
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(currentTile.gridX, currentTile.gridY));


    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            
            currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX + currentTile.gridY%2, currentTile.gridY + 1)];
            print(currentTile.tile.Location.X);
            print(currentTile.tile.Location.Y);
            moveCursor(currentTile);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {


            currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX - (currentTile.gridY - 1)%2, currentTile.gridY - 1)];
            print(currentTile.tile.Location.X);
            print(currentTile.tile.Location.Y);
            moveCursor(currentTile);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {

            currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX + 1, currentTile.gridY)];
            print(currentTile.tile.Location.X);
            print(currentTile.tile.Location.Y);
            moveCursor(currentTile);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {

            currentTile = GridManager.instance.normalizedBoard[new Point(currentTile.gridX - 1, currentTile.gridY)];
            print(currentTile.tile.Location.X);
            print(currentTile.tile.Location.Y);
            moveCursor(currentTile);
        }

        Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -2);
        tileX = currentTile.gridX;
        tileY = currentTile.gridY;
        
    }
    void moveCursor(TileBehaviour tile)
    {
        transform.position = GridManager.instance.calcWorldCoord(new Vector2(tile.gridX, tile.gridY));
        return;
    }
}
