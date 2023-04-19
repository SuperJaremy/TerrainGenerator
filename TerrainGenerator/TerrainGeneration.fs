module TerrainGenerator.TerrainGeneration

open System

let generateTerrain n variance ratio scale clean seed =
    let sideSize = (pown 2 n) + 1
    let rng = Random(seed)
    //
    // let dsMap =
    //     DiamondSquare.generateMap n (variance * 2.0) (variance / 2.0) 0.0 variance variance seed DiamondSquare.normalizeByAbs
    //     |> CellularAutomaton.generateMap clean CellularAutomaton.chooseByAverage
    // // let dsMap1 =
    // DiamondSquare.generateMap n 0 0 0 0 variance (seed + 1) DiamondSquare.normalizeBySlide
    // |> CellularAutomaton.step CellularAutomaton.chooseByAverage
    //
    // let snMap =
    //     SimplexNoise.generateMap sideSize sideSize scale seed
    //     |> CellularAutomaton.generateMap clean CellularAutomaton.chooseByAverage
        
    let whiteNoiseMap = CellularAutomaton.generateWhiteNoise sideSize rng
    
    let landWaterMap =
        CellularAutomaton.generateDualWhiteNoise 0.4 sideSize rng
        |> Array2D.map (fun value ->
            match value with
            | 0 -> Tiles.TerrainTile.Water
            | _ -> Tiles.TerrainTile.Land Tiles.LandTile.Field)
        |> CellularAutomaton.generateMap clean (CellularAutomaton.chooseByMajority Tiles.TerrainTile.Water)
    // let landWaterMap =
    //     LandWaterSplice.landWater ratio LandWaterSplice.calcFactorMean dsMap
    
    let biomeMap = BiomeGeneration.generateBiomes landWaterMap whiteNoiseMap
    
    biomeMap
    |> CellularAutomaton.generateMap clean (CellularAutomaton.chooseByMajority (Tiles.TerrainTile.Land(Tiles.LandTile.Sand)))

    // let landWater =
    //     CellularAutomaton.generateWhiteNoise 0.4 129 rng
    //     |> Array2D.map (fun value ->
    //         match value with
    //         | 0 -> Tiles.TerrainTile.Water
    //         | _ -> Tiles.TerrainTile.Land Tiles.LandTile.Field)
            // |> Array2D.map float
    // let transformer = (CellularAutomaton.chooseByMajority Tiles.TerrainTile.Water)
    // let cellular = CellularAutomaton.generateMap clean transformer landWater
    // let cellular =
    //     CellularAutomaton.generateMap clean CellularAutomaton.chooseByAverage landWater

    // cellular
        // |> LandWaterSplice.landWater 0.6 LandWaterSplice.calcFactorMean
