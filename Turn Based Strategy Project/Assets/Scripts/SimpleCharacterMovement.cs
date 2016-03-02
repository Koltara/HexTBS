using UnityEngine;
using System.Collections;

public class SimpleCharacterMovement : MonoBehaviour {
    public Tile currentTile;
    public TileBehaviour currentTB;

    public Tile destinationTile;

	// Use this for initialization
	void Start () {
        if (currentTile != null)
        {
            currentTB = GridManager.instance.board[currentTile.Location];
            currentTile = currentTB.tile;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (currentTile != null)
        {
            currentTB = GridManager.instance.board[currentTile.Location];
            currentTile = currentTB.tile;
        }
	}
    public void MoveTo(TileBehaviour destTile)
    {
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(destTile.gridX, destTile.gridY));
        currentTile = destTile.tile;
        currentTB = destTile;
    }
}
