using UnityEngine;
using System.Collections;

public class EnemyMovementController: MonoBehaviour
{
    public TileBehaviour currentTB;
    public Tile currentTile;
    public static EnemyMovementController instance;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
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
        //       this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(destTile.gridX, destTile.gridY));
        Debug.Log("Character should have moved now.");
        //GridManager.instance.originTileTB = null;

    }
}

