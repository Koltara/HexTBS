using UnityEngine;
using System.Collections;

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

    //Slightly transparent orange
    Color orange = new Color(255f / 255f, 127f / 255f, 0, 127f / 255f);
    

    void Update()
    {
        gridX = tile.X + tile.Y / 2;
        gridY = tile.Y;
    }

    void changeColor(Color color)
    {
        //If transparency is not set already, set it to default value
        if (color.a == 1)
            color.a = 130f / 255f;
        GetComponent<Renderer>().material = OpaqueMaterial;
        GetComponent<Renderer>().material.color = color;
    }

    
    public void HighlightCursor()
    {
        GridManager.instance.selectedTile = tile;

            if (tile.Passable && this != GridManager.instance.destTileTB
                && this != GridManager.instance.originTileTB)
            {
                changeColor(orange);
            }
    }

    //changes back to fully transparent material
    public void RemoveHighlight()
    {
        GridManager.instance.selectedTile = null;
            if (tile.Passable && this != GridManager.instance.destTileTB
            && this != GridManager.instance.originTileTB)
            {
                this.GetComponent<Renderer>().material = defaultMaterial;
                this.GetComponent<Renderer>().material.color = Color.white;
            }
    }
    //called every frame when cursor is on this tile
    public void UnlockTile()
    {
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
                changeColor(orange);

            GridManager.instance.generateAndShowPath();
        }
        //Replace Origin/Destination Tile
        if (Input.GetKeyUp(KeyCode.S))
        {
            tile.Passable = true;

            TileBehaviour originTileTB = GridManager.instance.originTileTB;
            //if user clicks on origin tile or origin tile is not assigned yet
            if (this == originTileTB || originTileTB == null)
                originTileChanged();
            else
                destTileChanged();

            GridManager.instance.generateAndShowPath();
        }
    }

    public void originTileChanged()
    {
        var originTileTB = GridManager.instance.originTileTB;
        //deselect origin tile if user clicks on current origin tile
        if (this == originTileTB)
        {
            GridManager.instance.originTileTB = null;
            GetComponent<Renderer>().material = defaultMaterial;
            return;
        }
        //if origin tile is not specified already mark this tile as origin
        GridManager.instance.originTileTB = this;
        changeColor(Color.red);
    }

    public void destTileChanged()
    {
        var destTile = GridManager.instance.destTileTB;
        //deselect destination tile if user clicks on current destination tile
        if (this == destTile)
        {
            GridManager.instance.destTileTB = null;
            GetComponent<Renderer>().material.color = orange;
            return;
        }
        //if there was other tile marked as destination, change its material to default (fully transparent) one
        if (destTile != null)
            destTile.GetComponent<Renderer>().material = defaultMaterial;
        GridManager.instance.destTileTB = this;
        changeColor(Color.blue);
    }
}
