﻿using System.Collections;
using System.Collections.Generic;
using System;

public static class StaticData
{
    public const string Version = "1.0.0";

    public const float TimeLimit = (60f * 20f);

    public const int WorldWidth = 50;
    public const int WorldLenght = 50;

    public const float MinRockPercentageOnMap = 0.08f;
    public const float MaxRockPercentageOnMap = 0.12f;

    public const int MinOreAmountOnMap = 100;
    public const int MaxOreAmountOnMap = 150;

    public const int MinOreAmountInDeposit = 5;
    public const int MaxOreAmountInDeposit = 10;

    public const int MinCrystalsAmountOnMap = 130;
    public const int MaxCrystalsAmountOnMap = 160;

    public const int MinCrystalsAmountInDeposit = 6;
    public const int MaxCrystalsAmountInDeposit = 12;

    public const float MinCrystalsDistanceFromStartingArea = 15f;
    public const float MaxCrystalsDistanceFromStartingArea = 25f;

    public const int CrystalsAmountToGather = 100;

    public const float ScaffoldingConstructionTime = 2f;

    public const float StairsMovementCost = 3f;

    public const float NeedLevelToSeekService = 0.25f;
    public const float NeedLevelToDie = 0.99f;
   
    public static Dictionary<TileType, float> LoadTilesMovementCosts()
    {
        return new Dictionary<TileType, float>(){ {TileType.Empty, 0f},
                                                  {TileType.WalkableEmpty, 0.75f},
                                                  {TileType.Sand, 3f},
                                                  {TileType.Rock, 0.75f} };
    }

    public static Dictionary<int, ResourceInfo> LoadResources()
    {
        Dictionary<int, ResourceInfo> resources = new Dictionary<int, ResourceInfo>();

        resources.Add(0, new ResourceInfo() { Name = "s_crystals" });
        resources.Add(1, new ResourceInfo() { Name = "s_metal" });
        resources.Add(2, new ResourceInfo() { Name = "s_gas" });
        resources.Add(3, new ResourceInfo() { Name = "s_ore" });
        resources.Add(4, new ResourceInfo() { Name = "s_plants" });
        resources.Add(5, new ResourceInfo() { Name = "s_medicine" });
        resources.Add(6, new ResourceInfo() { Name = "s_food" });
        resources.Add(7, new ResourceInfo() { Name = "s_parts" });
        resources.Add(8, new ResourceInfo() { Name = "s_brain" });

        return resources;
    }

    public static void LoadNeeds(Character character)
    {
        if (character.IsRobot)
        {
            character.Needs = new Dictionary<string, float>()
            { {"Condition", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Condition", 0.0015f} };
        }
        else
        {
            character.Needs = new Dictionary<string, float>()
            { {"Health", 0f}, {"Hunger", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Health", 0.0015f}, {"Hunger", 0.004f} };
        }
    }

    public static List<BuildingPrototype> LoadPrototypes()
    {
        List<BuildingPrototype> prototypes = new List<BuildingPrototype>(24);
        BuildingPrototype bp;

        // -----------------------------------
        // SPACESHIP
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Spaceship";
        bp.DescriptionKey = "SpaceshipDesc";

        bp.CanBeBuiltOnRock = true;

        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(0,0,0),                          new TilePosition(2,0,0),
            new TilePosition(0,1,0), new TilePosition(1,1,0), new TilePosition(2,1,0),
            new TilePosition(0,2,0), new TilePosition(1,2,0), new TilePosition(2,2,0),
            new TilePosition(0,3,0), new TilePosition(1,3,0), new TilePosition(2,3,0),
            new TilePosition(0,4,0), new TilePosition(1,4,0), new TilePosition(2,4,0),
        };

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 0, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.MaxStorage = 24;
        bp.InitialStorage = new Dictionary<int, int>() { { 1, 12 },
                                                         { 5, 6 },
                                                         { 6, 6 }  };

        bp.CanBeDeconstructed = false;

        prototypes.Add(bp);


        // -----------------------------------
        // ORE DEPOSIT
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "OreDeposit";
        bp.DescriptionKey = "OreDepositDesc";

        bp.CanBeBuiltOnRock = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionWithoutScaffolding = true;

        bp.IsNaturalDeposit = true;

        bp.ProductionTime = 4f;
        bp.ProducedResources = new Dictionary<int, int>() { { 3, 1 } };
        bp.ProductionCyclesLimitMin = MinOreAmountInDeposit;
        bp.ProductionCyclesLimitMax = MaxOreAmountInDeposit;

        bp.NeedGrowthPerSecond = new Dictionary<string, float>() { { "Health", 0.02f },
                                                                   { "Condition", 0.01f } };

        prototypes.Add(bp);


        // -----------------------------------
        // CRYSTALS DEPOSIT
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "CrystalsDeposit";
        bp.DescriptionKey = "CrystalsDepositDesc";

        bp.CanBeBuiltOnSand = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionWithoutScaffolding = true;

        bp.IsNaturalDeposit = true;

        bp.ProductionTime = 4f;
        bp.ProducedResources = new Dictionary<int, int>() { { 0, 1 } };
        bp.ProductionCyclesLimitMin = MinCrystalsAmountInDeposit;
        bp.ProductionCyclesLimitMax = MaxCrystalsAmountInDeposit;

        bp.NeedGrowthPerSecond = new Dictionary<string, float>() { { "Health", 0.02f },
                                                                   { "Condition", 0.01f } };

        prototypes.Add(bp);


        // -----------------------------------
        // STAIRS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Stairs";
        bp.DescriptionKey = "StairsDesc";

        bp.CanBeBuiltOnSand = true;

        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(1,0,0), new TilePosition(2,0,0)
        };

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 0, 1);
        bp.NormalizedAccessTileRotation = Rotation.E;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(3, 0, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.W;

        bp.ConstructionTime = 1f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;

        bp.MousePivotPoint = new TilePosition(1, 0, 0);
       
        prototypes.Add(bp);


        // -----------------------------------
        // PLATFORM
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Platform";
        bp.DescriptionKey = "PlatformDesc";

        bp.CanBeBuiltOnSand = true;

        bp.MultipleBuildMode = true;

        bp.AllowRotation = false;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.MovementCost = 0.75f;
        bp.WalkableOnTop = true;
        bp.CanBeAccessedFromTop = true;
        bp.DisallowDiagonalMovement = true;
        bp.MovementCostOnTop = 2f;

        bp.ConstructionTime = 0.5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;

        bp.CanBeDeconstructed = false;

        prototypes.Add(bp);


        // -----------------------------------
        // SLAB
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Slab";
        bp.DescriptionKey = "SlabDesc";

        bp.CanBeBuiltOnSand = true;
        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.MultipleBuildMode = true;

        bp.AllowRotation = false;

        bp.MovementCost = 0.5f;
        bp.AllowToBuildOnTop = true;
        bp.HasAccessTile = false;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionTime = 0.5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;

        prototypes.Add(bp);


        // -----------------------------------
        // GREENHOUSE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Greenhouse";
        bp.DescriptionKey = "GreenhouseDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ProductionTime = 4f;
        bp.ProducedResources = new Dictionary<int, int>() { { 4, 3 } };

        bp.ConstructionTime = 6f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.HidesCharacter = true;

        prototypes.Add(bp);


        // -----------------------------------
        // STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Storage";
        bp.DescriptionKey = "StorageDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ConstructionTime = 6f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.MaxStorage = 18;

        prototypes.Add(bp);


        // -----------------------------------
        // HEALING CHAMBER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "HealingChamber";
        bp.DescriptionKey = "HealingChamberDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(1, 2, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.S;
        
        bp.ConsumedResources = new Dictionary<int, int>() { { 5, 1 } };
        bp.NeedFulfilled = "Health";
        bp.ServiceDuration = 6f;
        bp.NeedFulfillmentPerSecond = 1f / bp.ServiceDuration;

        bp.ConstructionTime = 6f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 3 } };

        bp.MousePivotPoint = new TilePosition(1, 1, 0);
        bp.StartingRotation = Rotation.S;

        bp.HidesCharacter = true;

        prototypes.Add(bp);


        // -----------------------------------
        // QUARTERS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Quarters";
        bp.DescriptionKey = "QuartersDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(3, 3);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ConstructionTime = 10f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 }, { 2, 1 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 3 } };

        bp.ConsumedResources = new Dictionary<int, int>() { { 6, 1 } };
        bp.NeedFulfilled = "Hunger";
        bp.ServiceDuration = 5f;
        bp.NeedFulfillmentPerSecond = 1f / bp.ServiceDuration;

        bp.HidesCharacter = true;

        prototypes.Add(bp);


        // -----------------------------------
        // METALWORKS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Metalworks";
        bp.DescriptionKey = "MetalworksDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.N;

        bp.ConstructionTime = 4f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 4f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 3, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 1, 4 } };

        bp.NeedGrowthPerSecond = new Dictionary<string, float>() { { "Health", 0.01f },
                                                                   { "Condition", 0.01f } };

        prototypes.Add(bp);


        // -----------------------------------
        // REFINERY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Refinery";
        bp.DescriptionKey = "RefineryDesc";

        bp.CanBeBuiltOnSand = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(2, 1, 0);
        bp.NormalizedAccessTileRotation = Rotation.W;

        bp.StartingRotation = Rotation.E;

        bp.ConstructionTime = 8f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 3 } };

        bp.ProductionTime = 6f;
        bp.ProducedResources = new Dictionary<int, int>() { { 2, 4 } };

        bp.NeedGrowthPerSecond = new Dictionary<string, float>() { { "Health", 0.01f },
                                                                   { "Condition", 0.005f } };

        prototypes.Add(bp);


        // -----------------------------------
        // LABORATORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Laboratory";
        bp.DescriptionKey = "LaboratoryDesc";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.S;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 6f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 }, { 2, 1 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 4f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 2, 1 }, { 4, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 5, 2 } };

        bp.NeedGrowthPerSecond = new Dictionary<string, float>() { { "Health", 0.01f },
                                                                   { "Condition", 0.005f } };

        prototypes.Add(bp);


        // -----------------------------------
        // FOOD SYNTHESIZER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "FoodSynthesizer";
        bp.DescriptionKey = "FoodSynthesizerDesc";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 3f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 4, 2 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 6, 2 } };

        prototypes.Add(bp);


        // -----------------------------------
        // ROBOT FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RobotFactory";
        bp.DescriptionKey = "RobotFactoryDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;
        
        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(3, 3);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 3, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(1, -1, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.W;

        bp.ConstructionTime = 10f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 4 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 4 } };

        bp.ProductionTime = 7f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 1, 1 }, { 7, 2 } };        
        bp.ProducesRobot = true;

        prototypes.Add(bp);


        // -----------------------------------
        // PARTS FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "PartsFactory";
        bp.DescriptionKey = "PartsFactoryDesc";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 4);

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(-1, 0, 0);
        bp.NormalizedAccessTileRotation = Rotation.E;

        bp.ConstructionTime = 10f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 }, { 2, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 3 } };

        bp.ProductionTime = 4f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 1, 1 }, { 2, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 7, 2 } };

        prototypes.Add(bp);


        // -----------------------------------
        // REPAIR STATION
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RepairStation";
        bp.DescriptionKey = "RepairStationDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.StartingRotation = Rotation.E;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 }, { 7, 1 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ConsumedResources = new Dictionary<int, int>() { { 7, 1 } };
        bp.NeedFulfilled = "Condition";
        bp.ServiceDuration = 4f;
        bp.NeedFulfillmentPerSecond = 1f / bp.ServiceDuration;

        bp.HidesCharacter = true;

        prototypes.Add(bp);


        // -----------------------------------
        // CRYSTAL STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "CrystalStorage";
        bp.DescriptionKey = "CrystalStorageDesc";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 }, { 7, 1 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.MaxStorage = 12;
        bp.RestrictedResources = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

        prototypes.Add(bp);

        return prototypes;
    }

    public static List<string> LoadNames()
    {
        return new List<string>()
        {
            "Agata",
            "Aleksandra",
            "Alicja",
            "Anna",
            "Antonina",
            "Bartosz",
            "Daniel",
            "Dominik",
            "Filip",
            "Jakub",
            "Joanna",
            "Karolina",
            "Katarzyna",
            "Konrad",
            "Konstanty",
            "Maciej",
            "Magdalena",
            "Małgorzata",
            "Marcin",
            "Marta",
            "Michał",
            "Monika",
            "Natalia",
            "Paulina",
            "Paweł",
            "Piotr",
            "Roman",
            "Ryszard",
            "Samuel",
            "Szymon",
            "Urszula"            
        };
    }

    public static string[] LoadLetters()
    {
        return new string[]{ "A", "B", "C", "D", "E", "F",
                             "G", "H", "I", "J", "K", "L",
                             "M", "N", "O", "P", "R", "S",
                             "T", "U", "W", "Y", "Z" };
    }

    public static string[] LoadDigits()
    {
        return new string[]{ "0", "1", "2", "3", "4",
                             "5", "6", "7", "8", "9" };
    }
}
