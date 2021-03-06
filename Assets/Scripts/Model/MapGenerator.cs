﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator
{    
    int rockPercentage = 45;
        
    int smoothingIterations = 4;

    // Generator liczb pseudolosowych
    System.Random prng;

    int[,] tiles;
    int xSize;
    int ySize;

    int border = 3;

    int sand = 0;
    int rock = 1;

    // Statek kosmiczny i postaci
    int startingAreaXSize;
    int startingAreaYSize;

    int startingAreaMinimumDistanceToBorder = 5;

    int checkedAreaOffset = 3;
    int minRockTilesInCheckedArea = (5*8);

    public Tile[,,] Tiles { get; protected set; }
    public int StartingAreaX { get; protected set; }
    public int StartingAreaY { get; protected set; }

    public Dictionary<TilePosition, int> Ore { get; protected set; }
    public Dictionary<TilePosition, int> Crystals { get; protected set; }

    public Tile[,,] GenerateMap(int xSize, int ySize, 
                                int startingAreaXSize, int startingAreaYSize)
    {     
        this.xSize = xSize;
        this.ySize = ySize;
        this.startingAreaXSize = startingAreaXSize;
        this.startingAreaYSize = startingAreaYSize;

        float minFinalRockPercentage = StaticData.MinRockPercentageOnMap;
        float maxFinalRockPercentage = StaticData.MaxRockPercentageOnMap;

        int attempts = 50;        
        for (int a = 0; a < attempts; a++)
        {
            int seed = (System.DateTime.Now.GetHashCode() + attempts).GetHashCode();
            prng = new System.Random(seed.GetHashCode());

            TryGenerateMap();

            float finalRockPercentage = GetRockPercentage();

            if (finalRockPercentage < minFinalRockPercentage
                || finalRockPercentage > maxFinalRockPercentage)
            {
                //Debug.Log("Nieudana próba, nr: " + (a + 1) + ", %: " + finalRockPercentage);
                continue;
            }
            else
            {
                break;
            }
        }

        Tiles = GetGeneratedTiles();

        GenerateResources();

        return Tiles;
    }

    void TryGenerateMap()
    {
        tiles = new int[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (x < border || y < border
                    || x >= xSize - border || y >= ySize - border)
                {
                    tiles[x, y] = sand;
                    continue;
                }

                int randomNumber = prng.Next(0, 100);

                if (randomNumber > rockPercentage)
                {
                    tiles[x, y] = sand;
                }
                else
                {
                    tiles[x, y] = rock;
                }
            }
        }

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        GetRandomStartingArea();
        PlaceStartingArea();
    }

    void SmoothMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int neighbourRockTiles = GetRockNeighboursNumber(x, y);

                if (neighbourRockTiles > 4)
                {
                    tiles[x, y] = rock;
                }
                else if (neighbourRockTiles < 4)
                {
                    tiles[x, y] = sand;
                }
            }
        }
    }

    int GetRockNeighboursNumber(int x, int y)
    {
        int result = 0;
       
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < xSize 
                    && neighbourY >= 0 && neighbourY < ySize)
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        result += tiles[neighbourX, neighbourY];
                    }
                }
            }
        }

        return result;
    }

    Tile[,,] GetGeneratedTiles()
    {
        Dictionary<TileType, float> movementCosts = StaticData.LoadTilesMovementCosts();
                
        Tile[,,] newMap = new Tile[xSize, ySize, 2];

        Tile newTile = null;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int value = tiles[x, y];

                if (value == sand)
                {
                    newTile = new Tile(x, y, 0, TileType.Sand, movementCosts[TileType.Sand]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 1] = newTile;
                }
                else if (value == rock)
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock, movementCosts[TileType.Rock]);
                    newMap[x, y, 1] = newTile;
                }                                
                else
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 1] = newTile;
                }
            }
        }

        return newMap;
    }

    void GetRandomStartingArea()
    {
        int attempts = 50;
        
        int minRandomX = startingAreaMinimumDistanceToBorder;
        int maxRandomX = xSize - 1 - startingAreaMinimumDistanceToBorder - startingAreaXSize;

        int minRandomY = startingAreaMinimumDistanceToBorder;
        int maxRandomY = ySize - 1 - startingAreaMinimumDistanceToBorder - startingAreaYSize;

        for (int i = 0; i < attempts; i++)
        {            
            int randomX = prng.Next(minRandomX, maxRandomX);           
            int randomY = prng.Next(minRandomY, maxRandomY);

            int rockTilesNumber = 0;

            int firstCheckedX = Math.Max(randomX - checkedAreaOffset, 0);
            int lastCheckedX = Math.Min(randomX + startingAreaXSize + checkedAreaOffset, xSize);
            int firstCheckedY = Math.Max(randomY - checkedAreaOffset, 0);
            int lastCheckedY = Math.Min(randomY + startingAreaYSize + checkedAreaOffset, ySize);

            for (int x = firstCheckedX; x <= lastCheckedX; x++)
            {
                for (int y = firstCheckedY; y <= lastCheckedY; y++)
                {
                    if (tiles[x, y] == rock) rockTilesNumber++;
                }
            }

            if (rockTilesNumber >= minRockTilesInCheckedArea)
            {
                StartingAreaX = randomX;
                StartingAreaY = randomY;
                return;
            }
            else
            {
                continue;
            }
        }

        // Nie powiodło się - bierzemy środek
        StartingAreaX = (xSize - startingAreaXSize) / 2;
        StartingAreaY = (ySize - startingAreaYSize) / 2;
    }

    void PlaceStartingArea()
    {
        int endX = StartingAreaX + startingAreaXSize - 1;
        int endY = StartingAreaY + startingAreaYSize - 1;

        for (int x = StartingAreaX; x <= endX; x++)
        {
            for (int y = StartingAreaY; y <= endY; y++)
            {
                if(y == StartingAreaY && (x == StartingAreaX || x == endX)
                   || y == endY && (x == StartingAreaX || x == endX))
                {
                    continue;
                }
                else
                {
                    tiles[x, y] = 1;
                }
            }
        }
    }
    
    void GenerateResources()
    {        
        int oreAmountToPlace = prng.Next(StaticData.MinOreAmountOnMap, 
                                         StaticData.MaxOreAmountOnMap);
        int crystalsAmountToPlace = prng.Next(StaticData.MinCrystalsAmountOnMap, 
                                              StaticData.MaxCrystalsAmountOnMap);
        
        Ore = new Dictionary<TilePosition, int>();
        Crystals = new Dictionary<TilePosition, int>();

        // Ore

        int orePlaced = 0;

        int attempts = 500;
        for (int i = 0; i < attempts; i++)
        {
            TilePosition randomTile = GetRandomTile();
            if (CheckTileValue(randomTile, rock) 
                && Ore.ContainsKey(randomTile) == false
                && IsTileInStartingArea(randomTile) == false
                && IsAccessible(randomTile))
            {                
                int randomDepositSize = prng.Next(StaticData.MinOreAmountInDeposit,
                                                  StaticData.MaxOreAmountInDeposit);

                Ore.Add(randomTile, randomDepositSize);
                orePlaced += randomDepositSize;

                if (orePlaced >= oreAmountToPlace)
                {
                    break;
                }
            }
        }

        // Crystals

        int numberOfCrystalSites = 4;
        int minNumberOfCrystalsPerSite = 3;
        int maxNumberOfCrystalsPerSite = 6;
        int minSiteRadius = 2;
        int maxSiteRadius = 4;

        int crystalsPlaced = 0;

        TilePosition startingPosition = new TilePosition(StartingAreaX + (startingAreaXSize / 2),
                                                         StartingAreaY + (startingAreaYSize / 2),
                                                         rock);

        float minCrystalsDistanceFromStartingArea = StaticData.MinCrystalsDistanceFromStartingArea;
        float maxCrystalsDistanceFromStartingArea = StaticData.MaxCrystalsDistanceFromStartingArea;

        for (int s = 0; s < numberOfCrystalSites; s++)
        {
            TilePosition sitePosition;
            while (true)
            {
                sitePosition = GetRandomTileInRadius(startingPosition, 
                                                     minCrystalsDistanceFromStartingArea, 
                                                     maxCrystalsDistanceFromStartingArea);

                if (CheckTileValue(sitePosition, sand))
                {
                    break;
                }
            }

            int numberOfCrystalsForThisSite = prng.Next(minNumberOfCrystalsPerSite, 
                                                        maxNumberOfCrystalsPerSite);

            attempts = 500;
            for (int i = 0; i < attempts; i++)
            {
                TilePosition crystalsPosition = GetRandomTileInRadius(sitePosition, minSiteRadius, maxSiteRadius);
                if (CheckTileValue(crystalsPosition, sand)
                    && Crystals.ContainsKey(crystalsPosition) == false
                    && IsAccessible(crystalsPosition))
                {
                    int randomDepositSize = prng.Next(StaticData.MinCrystalsAmountInDeposit,
                                                      StaticData.MaxCrystalsAmountInDeposit);

                    Crystals.Add(crystalsPosition, randomDepositSize);
                    crystalsPlaced += randomDepositSize;

                    if (crystalsPlaced >= crystalsAmountToPlace)
                    {
                        return;
                    }

                    numberOfCrystalsForThisSite--;
                    if (numberOfCrystalsForThisSite <= 0)
                    {
                        break;
                    }
                }
            }            
        }
    }

    TilePosition GetRandomTile()
    {
        int x = prng.Next(0, xSize - 1);
        int y = prng.Next(0, ySize - 1);
        if (tiles[x,y] == rock)
        {
            return new TilePosition(x, y, 1);
        }
        else
        {
            return new TilePosition(x, y, 0);
        }
    }

    TilePosition GetRandomTileInRadius(TilePosition center, float minRadius, float maxRadius)
    {        
        while (true)
        {
            TilePosition randomPos = GetRandomTile();

            float dist = Mathf.Sqrt(Mathf.Pow(randomPos.X - center.X, 2) +
                                     Mathf.Pow(randomPos.Y - center.Y, 2));

            if (dist > minRadius && dist < maxRadius)
            {
                return randomPos;   
            }
        }       
    }

    bool CheckTileValue(TilePosition position, int value)
    {
        int valueAtPosition = tiles[position.X, position.Y];
        return (value == valueAtPosition);
    }

    bool IsTileInStartingArea(TilePosition position)
    {
        if (position.X >= StartingAreaX && position.X <= (StartingAreaX + startingAreaXSize - 1)
            && position.Y >= StartingAreaY && position.Y <= (StartingAreaY + startingAreaYSize - 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int CountRocksOnMap()
    {
        int result = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y] == rock) result++;
            }
        }
        return result;
    }    

    float GetRockPercentage()
    {
        //Debug.Log(CountRocksOnMap());
        float result = ((float)CountRocksOnMap() / (xSize * ySize));
        return result;
    }

    bool IsAccessible(TilePosition tilePosition)
    {
        bool result =
            (IsAccessibleFromNeighbour(tilePosition, GetNorthNeighbourPosition(tilePosition))
            || IsAccessibleFromNeighbour(tilePosition, GetEastNeighbourPosition(tilePosition))
            || IsAccessibleFromNeighbour(tilePosition, GetSouthNeighbourPosition(tilePosition))
            || IsAccessibleFromNeighbour(tilePosition, GetWestNeighbourPosition(tilePosition)));
        return result;
    }

    TilePosition GetNorthNeighbourPosition(TilePosition tilePosition)
    {
        return new TilePosition(tilePosition.X, tilePosition.Y + 1, tilePosition.Height);
    }

    TilePosition GetSouthNeighbourPosition(TilePosition tilePosition)
    {
        return new TilePosition(tilePosition.X, tilePosition.Y - 1, tilePosition.Height);
    }

    TilePosition GetEastNeighbourPosition(TilePosition tilePosition)
    {
        return new TilePosition(tilePosition.X + 1, tilePosition.Y, tilePosition.Height);
    }

    TilePosition GetWestNeighbourPosition(TilePosition tilePosition)
    {
        return new TilePosition(tilePosition.X - 1, tilePosition.Y, tilePosition.Height);
    }

    bool IsPositionInBounds(TilePosition postion)
    {
        return (postion.X < xSize && postion.Y < ySize
                && postion.X >= 0 && postion.Y >= 0);
    }

    bool IsAccessibleFromNeighbour(TilePosition checkedTile, TilePosition neighbour)
    {
        if (IsPositionInBounds(neighbour) == false) return false;

        else if (neighbour.Height != checkedTile.Height) return false;

        else if (Crystals.ContainsKey(neighbour)
                || Ore.ContainsKey(neighbour)) return false;

        else return true;
    }
}