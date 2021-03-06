﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour
{

    //speed in meters per second
    public float speed = 0.0025F;
    public float rotationSpeed = 0.004F;
    //distance between character and tile position when we assume we reached it and start looking for the next. Explained in detail later on
    public static float MinNextTileDist = 0.07f;

    public static CharacterMovement instance = null;
    //position of the tile we are heading to
    Vector3 curTilePos;
    Tile curTile;
    List<Tile> path;
    public bool IsMoving { get; private set; }
    Transform myTransform;

    void Awake()
    {
        //singleton pattern here is used just for the sake of simplicity. Messenger (http://goo.gl/3Okkh) should be used in cases when this script is attached to more than one character
        instance = this;
        IsMoving = false;
    }

    void Start()
    {

        //all the animations by default should loop
        this.transform.position = GridManager.instance.calcWorldCoord(new Vector2(0, 0));

        //caching the transform for better performance
        myTransform = transform;
    }

    //gets tile position in world space
    Vector3 calcTilePos(Tile tile)
    {
        //y / 2 is added to convert coordinates from straight axis coordinate system to squiggly axis system
        Vector2 tileGridPos = new Vector2(tile.X + tile.Y / 2, tile.Y);
        Vector3 tilePos = GridManager.instance.calcWorldCoord(tileGridPos);
        //y coordinate is disregarded
        tilePos.y = myTransform.position.y;
        return tilePos;
    }

    //method argument is a list of tiles we got from the path finding algorithm
    public void StartMoving(List<Tile> path)
    {
        if (path.Count == 0)
            return;
        //the first tile we need to reach is actually in the end of the list just before the one the character is currently on
        curTile = path[path.Count - 2];
        curTilePos = calcTilePos(curTile);
        IsMoving = true;
        this.path = path;
    }

    //Method used to switch destination and origin tiles after the destination is reached
    void switchOriginAndDestinationTiles()
    {
        GridManager GM = GridManager.instance;
        Material originMaterial = GM.originTileTB.GetComponent<Renderer>().material;
        GM.originTileTB.GetComponent<Renderer>().material = GM.destTileTB.defaultMaterial;
        GM.originTileTB = GM.destTileTB;
        GM.originTileTB.GetComponent<Renderer>().material = originMaterial;
        GM.destTileTB = null;
        GM.generateAndShowPath();
    }

    void Update()
    {
        if (!IsMoving)
            return;
        if ((curTilePos - myTransform.position).sqrMagnitude < MinNextTileDist * MinNextTileDist)
        {
            //if we reached the destination tile
            if (path.IndexOf(curTile) == 0)
            {
                IsMoving = false;
                switchOriginAndDestinationTiles();
                return;
            }
            //curTile becomes the next one
            curTile = path[path.IndexOf(curTile) - 1];
            curTilePos = calcTilePos(curTile);
        }

        MoveTowards(curTilePos);
    }

    void MoveTowards(Vector3 position)
    {
        //mevement direction
        Vector2 dir = position - myTransform.position;

        // Rotate towards the target
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
            Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);

        Vector2 forwardDir = myTransform.forward;
        forwardDir = forwardDir * speed;

        float speedModifier = Vector3.Dot(dir.normalized, myTransform.forward);
        forwardDir *= speedModifier;
        if (speedModifier > 0.95f)
        {
            this.GetComponent<Prime31.CharacterController2D>().move(forwardDir);
            
        }
        
    }

}
