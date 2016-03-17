using UnityEngine;
using System.Collections;

public class SimpleCharacterMovement : MonoBehaviour {
    public TileBehaviour currentTB;
    public Tile currentTile;

	// Use this for initialization
	void Start () {
        currentTB = null;
	}
	
	// Update is called once per frame
	void Update () {
        //if (currentTile != null)
        //{
        //    currentTB = GridManager.instance.board[currentTile.Location];
        //    currentTile = currentTB.tile;
        //}
        if (currentTB != null)
            this.gameObject.transform.position = GridManager.instance.calcWorldCoord(new Vector2(currentTB.gridX, currentTB.gridY));
    }
    public void MoveTo(TileBehaviour destTile)
    {
        currentTB = destTile;
        Debug.Log(destTile.gridX);
        Debug.Log(destTile.gridY);
        this.gameObject.transform.position = GridManager.instance.calcWorldCoord(new Vector2(destTile.gridX, destTile.gridY));
        Debug.Log("Character should have moved now.");
        //GridManager.instance.originTileTB = null;
        
    }
}
