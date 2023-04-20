// For more information see https://aka.ms/fsharp-console-apps

open TerrainGenerator
open Visualizer

let n = 11
let sideSize = (pown 2 n) + 1
let variance = 256
let ratio = 1
let scale = 0.001f
let clear = 3
let seed = 26552


// seq {0..10} |> Array.ofSeq |> Array.Parallel.iter (fun x ->
//     let terrain = TerrainGeneration.generateTerrain 9 1024 0.7 0.005f x 102737
//     let res = Drawer.draw terrain 10
//     let path = sprintf "C:\Users\SJar\RiderProjects\TerrainGenerator\ss%d.png" x
//     res.Save(path)
//     )

// for i in 0..10 do
//     let terrain = TerrainGeneration.generateTerrain 10 1024 0.7 0.005f i 52802
//     let res = Drawer.draw terrain 10
//     let path = sprintf "C:\Users\SJar\RiderProjects\TerrainGenerator\ss%d.png" i
//     res.Save(path)

// let terrain = TerrainGeneration.generateTerrain n variance ratio scale clear seed
//
// let dsMap =
//      DiamondSquare.generateMap n 0 0 0 0 variance (seed + 1) DiamondSquare.normalizeBySlide
//      |> Array2D.map (BiomeGeneration.toTableIndexMapper Tiles.TerrainTable false)
//      
// let dsMapRes = Drawer.draw Drawer.debugToColor dsMap 10
//      
// let snMap =
//      SimplexNoise.generateMap sideSize sideSize scale seed
//      |> Array2D.map (BiomeGeneration.toTableIndexMapper Tiles.TerrainTable true)
//      
// let snMapRes = Drawer.draw Drawer.debugToColor snMap 10
// let res = Drawer.draw Drawer.tileToColor terrain 10
//
// dsMapRes.Save("C:\Users\SJar\RiderProjects\TerrainGenerator\ds.png")
// snMapRes.Save("C:\Users\SJar\RiderProjects\TerrainGenerator\sn.png")
// res.Save("C:\Users\SJar\RiderProjects\TerrainGenerator\ss.png")

let tiles = Array2D.create 129 129 (Tiles.TerrainTile.Land Tiles.LandTile.Grassland)

let river = RiverGenerator.generateRiver 65 65 RiverGenerator.logProbability seed tiles

let riverRes = Drawer.draw Drawer.tileToColor river 10

riverRes.Save("C:\Users\SJar\RiderProjects\TerrainGenerator\\river.png")
