﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }
    public World World;

    public GameObject DebugGroundTilePrefab;
    public GameObject DebugRockTilePrefab;
    public GameObject CharacterPrefab;

    public GameObject TilesParent;
    public GameObject BuildingsParent;
    public GameObject CharactersParent;
    public GameObject PreviewsParent;
    public GameObject ConstructionSitesParent;

    public GameObject AccessArrowPrefab;

    public GameObject DebugBuildingPreviewPrefab;

    public float LevelHeightOffset;

    // Tablice zamiast słowników - dzięki temu można ustalić ich zawartość z poziomu edytora
    [Space(10)]
    public ResourceDisplayInfo[] resourceDisplay;
    [Space(10)]
    public NaturalDepositDisplayInfo[] depositDisplay;
    [Space(10)]
    public BuildingDisplayInfo[] buildingDisplay;
    [Space(10)]

    public PlatformTopsPrefabs PlatformTopsPrefabs;

    List<GameObject> previewObjects = new List<GameObject>();
    GameObject accessArrow;
    GameObject secondAccessArrow;

    Dictionary<ConstructionSite, GameObject> constructionSitesDisplay;

    void OnEnable()
    {
        if (Instance != null) { Debug.LogError("Dwie instancje klasy GameManager!"); }
        Instance = this;

        int worldSize = 50;

        World = new World(worldSize, worldSize);

        constructionSitesDisplay = new Dictionary<ConstructionSite, GameObject>();

        GenerateDisplayForTiles();

        for (int x = 3; x < 4; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                World.CreateNewCharacter(new TilePosition(x, y, 0));
            }
        }

        World.InstantBuild(new TilePosition(0, 2, 0), Rotation.N, World.GetBuildingPrototype("Spaceship"));
        // World.InstantBuild(new TilePosition(5, 5, 0), Rotation.S, World.GetBuildingPrototype("Debug4"));
    }

    void Update ()
    {
        World.UpdateModel(Time.deltaTime);
    }

    public CharacterDisplayObject GenerateDisplayForCharacter(Character c)
    {
        GameObject gameObject = GameObject.Instantiate(
            CharacterPrefab,
            new Vector3(c.CurrentTile.X, 0, c.CurrentTile.Y),
            Quaternion.identity,
            CharactersParent.transform
            );

        CharacterDisplayObject displayObject = gameObject.GetComponentInChildren<CharacterDisplayObject>();

        displayObject.AssignCharacter(c);
        c.AssignDisplayObject(displayObject);

        return displayObject;
    }

    public SelectableDisplayObject ShowBuilding(Building building, TilePosition positionForDisplay)
    {
        GameObject model = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == building.Type)
            {
                model = buildingDisplay[i].Model;
                break;
            }
        }
        if (model == null)
        {
            Debug.Log("Nie znaleziono modelu dla budynku: " + building.Type);
            model = DebugBuildingPreviewPrefab;
        }

        GameObject gameObject = GameObject.Instantiate(
            model,
            new Vector3(positionForDisplay.X,
                        positionForDisplay.Height * LevelHeightOffset,
                        positionForDisplay.Y),
            Quaternion.identity,
            BuildingsParent.transform
            );

        RotateGameObject(gameObject, building.Rotation);

        SelectableDisplayObject selectableDisplayObject = gameObject.GetComponentInChildren<SelectableDisplayObject>();

        selectableDisplayObject.AssignModelObject(building);
        building.AssignDisplayObject(selectableDisplayObject);

        return selectableDisplayObject;
    }

    public ConstructionSiteDisplayObject ShowConstructionSite(ConstructionSite newSite)
    {
        Building building = newSite.Building;

        GameObject siteObjectPrefab = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == building.Type)
            {
                siteObjectPrefab = buildingDisplay[i].ConstructionSiteObject;
                break;
            }
        }
        if (siteObjectPrefab == null)
        {
            Debug.Log("Nie znaleziono placu konstrukcyjnego dla budynku: " + building.Type);
            return null;
        }

        GameObject gameObject = GameObject.Instantiate(
            siteObjectPrefab,
            new Vector3(building.Tiles[0].Position.X,
                        building.Tiles[0].Position.Height * LevelHeightOffset,
                        building.Tiles[0].Position.Y),
            Quaternion.identity,
            ConstructionSitesParent.transform
            );

        RotateGameObject(gameObject, building.Rotation);

        bool builtOnSecondLevel = (building.Tiles[0].Position.Height > 0);

        ConstructionSiteDisplayObject selectableObject = gameObject.GetComponentInChildren<ConstructionSiteDisplayObject>();

        selectableObject.AssignConstructionSite(newSite, builtOnSecondLevel);
        building.AssignDisplayObject(selectableObject);

        return selectableObject;
    }

    public void RemoveConstructionSiteDisplay(ConstructionSite siteToRemove)
    {
        siteToRemove.Building.DisplayObject.transform.gameObject.SetActive(false);
        siteToRemove.Building.DisplayObject.ModelObject = null;
        siteToRemove.Building.DisplayObject = null;
    }

    public void ShowPreview(TilePosition position, Rotation rotation, BuildingPrototype prototype)
    {
        bool isPositionValid = World.IsValidBuildingPosition(position, rotation, prototype);

        GameObject model = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == prototype.ModelName)
            {
                if (isPositionValid)
                {
                    model = buildingDisplay[i].PreviewModel;
                }
                else
                {
                    model = buildingDisplay[i].InvalidPreviewModel;
                }
                break;
            }
        }
        if (model == null)
        {
            model = DebugBuildingPreviewPrefab;
        }

        GameObject preview = SimplePool.Spawn(
            model, 
            new Vector3(position.X, position.Height * LevelHeightOffset, position.Y), 
            Quaternion.identity);
       
        if (isPositionValid && prototype.HasAccessTile)
        {
            TilePosition arrowPosition = prototype.NormalizedAccessTilePosition;

            accessArrow = SimplePool.Spawn(
                AccessArrowPrefab,
                new Vector3(position.X + arrowPosition.X,
                            (position.Height + arrowPosition.Height) * LevelHeightOffset,
                            position.Y + arrowPosition.Y),
                Quaternion.identity);

            RotateGameObject(accessArrow, prototype.NormalizedAccessTileRotation);

            accessArrow.transform.SetParent(preview.transform);            
        }

        if (isPositionValid && prototype.HasSecondAccessTile)
        {
            TilePosition arrowPosition = prototype.NormalizedSecondAccessTilePosition;

            secondAccessArrow = SimplePool.Spawn(
                AccessArrowPrefab,
                new Vector3(position.X + arrowPosition.X,
                            (position.Height + arrowPosition.Height) * LevelHeightOffset,
                            position.Y + arrowPosition.Y),
                Quaternion.identity);

            RotateGameObject(secondAccessArrow, prototype.NormalizedSecondAccessTileRotation);

            secondAccessArrow.transform.SetParent(preview.transform);
        }

        RotateGameObject(preview, rotation);

        preview.transform.SetParent(PreviewsParent.transform);
        previewObjects.Add(preview);
    }

    public void HidePreviews()
    {
        for (int i = 0; i < previewObjects.Count; i++)
        {
            SimplePool.Despawn(previewObjects[i]);
        }
        if (accessArrow != null)
        {
            accessArrow.transform.parent = null;
            accessArrow.transform.localScale = new Vector3(1f, 1f, 1f);
            SimplePool.Despawn(accessArrow);
            accessArrow = null;
        }
        if (secondAccessArrow != null)
        {
            secondAccessArrow.transform.parent = null;
            secondAccessArrow.transform.localScale = new Vector3(1f, 1f, 1f);
            SimplePool.Despawn(secondAccessArrow);
            secondAccessArrow = null;
        }
        previewObjects.Clear();        
    }

    public ResourceDisplayInfo GetResourceDisplayInfo(int resourceID)
    {
        string resourceName;
        if (World.ResourcesInfo.ContainsKey(resourceID))
        {
            resourceName = World.ResourcesInfo[resourceID].Name;
        }
        else
        {
            return null;
        }
        
        ResourceDisplayInfo result = null;
        for (int i = 0; i < resourceDisplay.Length; i++)
        {
            if (resourceDisplay[i] != null && resourceDisplay[i].ResourceName == resourceName)
            {
                result = resourceDisplay[i];
                break;
            }
        }
        return result;
    }

    public void RemoveDisplayForBuilding(Building building)
    {
        building.DisplayObject.transform.gameObject.SetActive(false);
        building.DisplayObject.ModelObject = null;
        building.DisplayObject = null;
    }

    void GenerateDisplayForTiles()
    {
        for (int height = 0; height < World.Height; height++)
        {
            for (int x = 0; x < World.XSize; x++)
            {
                for (int y = 0; y < World.YSize; y++)
                {
                    if (World.Tiles[x, y, height].Type == TileType.Dirt)
                    {
                        GameObject.Instantiate(
                            DebugGroundTilePrefab,
                            new Vector3(x, height * LevelHeightOffset, y),
                            Quaternion.identity,
                            TilesParent.transform);
                    }
                    else if (World.Tiles[x, y, height].Type == TileType.Rock)
                    {
                        GameObject.Instantiate(
                            DebugRockTilePrefab,
                            new Vector3(x, height * LevelHeightOffset, y),
                            Quaternion.identity,
                            TilesParent.transform);
                    }
                }   
            }
        }
    }

    void RotateGameObject(GameObject displayObject, Rotation rotation)
    {
        if (rotation == Rotation.N)
        {
            return;
        }
        else if (rotation == Rotation.E)
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(0, 0, 1),
                Quaternion.Euler(new Vector3(0, 90, 0))
                );
        }
        else if (rotation == Rotation.S)
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(1, 0, 1),
                Quaternion.Euler(new Vector3(0, 180, 0))
                );
        }
        else
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(1, 0, 0),
                Quaternion.Euler(new Vector3(0, 270, 0))
                );
        }
    }
}