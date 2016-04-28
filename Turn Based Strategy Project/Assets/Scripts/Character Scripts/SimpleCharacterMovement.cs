using UnityEngine;
using System.Collections;

public class SimpleCharacterMovement : MonoBehaviour {
    public TileBehaviour currentTB;
    public Tile currentTile;
    public static SimpleCharacterMovement instance;

	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        //if (currentTile != null)
        //{
        //    currentTB = GridManager.instance.board[currentTile.Location];
        //    currentTile = currentTB.tile;
        //}

        //Enemy AI if allied with enemy
        if (this.gameObject.GetComponent<CharacterStatus>().allegiance == 1)
        {
            if (TurnManager.instance.getPhaseStatus() == TurnManager.kEnemy)
            {
                if (this.GetComponent<CharacterStatus>().ableToMove)
                {
                    foreach (Tile tile in this.currentTB.tile.Neighbours)
                    {
                        TileBehaviour tb = GridManager.instance.board[tile.Location];
                        for (int i = 0; i < CharacterManager.instance.getCharacterInstanceListSize(); i++)
                        {
                            if (CharacterManager.instance.getCharacterInstance(i) == tb.containedCharacter)
                            {
                                TurnManager.instance.initiateCombat(this.gameObject, tb.containedCharacter);
                                Debug.Log("Fighting " + CharacterManager.instance.getCharacterInstance(i));
                                this.GetComponent<CharacterStatus>().ableToMove = false;
                                return;
                                
                            }
                        }
                    }
                    this.GetComponent<CharacterStatus>().ableToMove = false;
                    return;
                }
            }
        }

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
