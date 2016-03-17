using UnityEngine;
using System.Collections;

public class SimpleCharacterMovement : MonoBehaviour {
    public Tile currentTile;
    public TileBehaviour currentTB;

    public Tile destinationTile;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //if (currentTile != null)
        //{
        //    currentTB = GridManager.instance.board[currentTile.Location];
        //    currentTile = currentTB.tile;
        //}
	}
    public void MoveTo(TileBehaviour destTile)
    {
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(destTile.tile.Location.X, destTile.tile.Location.Y));
        currentTile = destTile.tile;
        currentTB = destTile;
    }
}
