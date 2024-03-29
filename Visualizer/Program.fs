﻿// Usage example

open System
open TerrainGenerator
open Visualizer

let n = 8
let sideSize = (pown 2 n) + 1
let variance = 0.01
let ratio = 0.7
let scale = 0.01f
let clean = 3
let seed = 63281
let rng = Random(seed)
let d1 = variance * rng.NextDouble() - (variance / 2.)
let d2 = variance * rng.NextDouble() - (variance / 2.)
let d3 = variance * rng.NextDouble() - (variance / 2.)
let d4 = variance * rng.NextDouble() - (variance / 2.)

let rivers = 11

let dsMap =
    DiamondSquare.generateMap n d1 d2 d3 d4 variance seed Util.normalizeBySlide
    |> CellularAutomaton.generateMap
        clean
        CellularAutomaton.eightTilesNeighborhoodWithoutCell
        CellularAutomaton.chooseByAverage
        Util.doNotNormalize

let snMap =
    SimplexNoise.generateMap sideSize sideSize scale seed SimplexNoise.normalizeByZeroAndTwoFiftyFive

let terrainOne =
    TerrainGeneration.generateTerrainWithBiomeSegmentAndProbabilityRiverGenerator
        Tiles.biomeSegment
        snMap
        rivers
        seed
        clean

let terrainTwo =
    TerrainGeneration.generateTerrainWithLandWaterDivisionAndProbabilityRiverGenerator
        dsMap
        ratio
        Tiles.landSegment
        snMap
        rivers
        seed
        clean

let terrainThree =
    TerrainGeneration.generateTerrainWith2DTableAndElevationMapBasedRiversFromSnowToWater
        dsMap
        snMap
        Tiles.TerrainTable
        rivers
        seed
        clean

(Drawer.draw Drawer.tileToColor terrainOne 10)
    .Save("C:\Users\SJar\RiderProjects\TerrainGenerator\ss_one.png")

(Drawer.draw Drawer.tileToColor terrainTwo 10)
    .Save("C:\Users\SJar\RiderProjects\TerrainGenerator\ss_two.png")

(Drawer.draw Drawer.tileToColor terrainThree 10)
    .Save("C:\Users\SJar\RiderProjects\TerrainGenerator\ss_three.png")
