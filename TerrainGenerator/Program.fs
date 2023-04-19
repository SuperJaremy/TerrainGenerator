// For more information see https://aka.ms/fsharp-console-apps

open System
open TerrainGenerator

printfn "%A" (CellularAutomaton.generateDualWhiteNoise 0.5 5 (Random()))

