module TerrainGenerator.TerrainGeneration

open System

type TerrainGen =
    static member generateTerrain
        (
            generator: Tiles.TerrainTile[,],
            [<System.ParamArray>] postGens: (Tiles.TerrainTile[,] -> Tiles.TerrainTile[,])[]
        ) =
        Array.fold (fun state post -> post state) generator postGens


let generateTerrainWith2DTableAndElevationMapBasedRiversFromSnowToWater
    elevationMap
    moistureMap
    mapTable
    riverCnt
    seed
    clean
    =
    let biomeGen =
        BiomeGeneration.generateBiomesFrom2DTable elevationMap moistureMap Util.to2DTableIndexMapper mapTable

    let snowCond _ _ elem =
        elem = Tiles.TerrainTile.Land(Tiles.LandTile.Snow)

    let riverGen =
        RiverGenerator.generateRivers
            (RiverGenerator.elevationMapRiverGenerator
                elevationMap
                (RiverGenerator.minFlowChooser RiverGenerator.isWater))
            riverCnt
            snowCond
            seed

    let cleaner =
        CellularAutomaton.generateMap
            clean
            CellularAutomaton.eightTilesNeighborhoodWithoutCell
            CellularAutomaton.chooseByMajority
            Util.doNotNormalize

    TerrainGen.generateTerrain (biomeGen, cleaner, riverGen)

let generateTerrainWithLandWaterDivisionAndProbabilityRiverGenerator
    landWaterMap
    waterToLandRatio
    biomeSegment
    biomeMap
    riverCnt
    seed
    clean
    =
    let landWater =
        LandWaterSplice.landWater waterToLandRatio LandWaterSplice.calcFactorMed landWaterMap

    let gen =
        BiomeGeneration.generateBiomesFromLandWaterMap landWater biomeSegment biomeMap

    let prg =
        RiverGenerator.probabilityRiverGenerator RiverGenerator.logProbability (Random(seed))

    let points =
        Util.foldiArray2D
            (fun x y state elem ->
                match elem with
                | Tiles.TerrainTile.Land _ -> (x, y) :: state
                | _ -> state)
            []
            landWater
        |> Util.shuffleList seed
        |> List.take riverCnt

    let randCond x y _ = List.contains (x, y) points
    let riverGen = RiverGenerator.generateRivers prg riverCnt randCond seed

    let cleaner =
        CellularAutomaton.generateMap
            clean
            CellularAutomaton.eightTilesNeighborhoodWithoutCell
            CellularAutomaton.chooseByMajority
            Util.doNotNormalize

    TerrainGen.generateTerrain (gen, cleaner, riverGen)

let generateTerrainWithBiomeSegmentAndProbabilityRiverGenerator biomeSegment biomeMap riverCnt seed clean =
    let gen = BiomeGeneration.generateBiomesFromBiomeSegment biomeSegment biomeMap

    let prg =
        RiverGenerator.probabilityRiverGenerator RiverGenerator.logProbability (Random(seed))

    let points =
        Util.foldiArray2D
            (fun x y state elem ->
                match elem with
                | Tiles.TerrainTile.Land _ -> (x, y) :: state
                | _ -> state)
            []
            gen
        |> Util.shuffleList seed
        |> List.take riverCnt

    let randCond x y _ = List.contains (x, y) points
    let riverGen = RiverGenerator.generateRivers prg riverCnt randCond seed

    let cleaner =
        CellularAutomaton.generateMap
            clean
            CellularAutomaton.eightTilesNeighborhoodWithoutCell
            CellularAutomaton.chooseByMajority
            Util.doNotNormalize

    TerrainGen.generateTerrain (gen, cleaner, riverGen)
