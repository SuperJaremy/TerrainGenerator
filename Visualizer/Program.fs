// For more information see https://aka.ms/fsharp-console-apps

open TerrainGenerator
open Visualizer

seq {0..10} |> Array.ofSeq |> Array.Parallel.iter (fun x ->
    let terrain = TerrainGeneration.generateTerrain 9 1024 0.7 0.005f x 102737
    let res = Drawer.draw terrain 10
    let path = sprintf "C:\Users\SJar\RiderProjects\TerrainGenerator\ss%d.png" x
    res.Save(path)
    )

// for i in 0..10 do
//     let terrain = TerrainGeneration.generateTerrain 10 1024 0.7 0.005f i 52802
//     let res = Drawer.draw terrain 10
//     let path = sprintf "C:\Users\SJar\RiderProjects\TerrainGenerator\ss%d.png" i
//     res.Save(path)