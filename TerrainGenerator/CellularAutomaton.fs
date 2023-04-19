module TerrainGenerator.CellularAutomaton

open System
open Microsoft.FSharp.Core

let private excludeCenter (x: seq<'a>) =
    x |> Seq.except ([ (Seq.item ((Seq.length x) / 2) x) ] |> Seq.ofList)

let chooseByAverage (cell: Option<float>[,]) =
    cell
    |> Seq.cast<Option<float>>
    |> excludeCenter
    |> Seq.filter Option.isSome
    |> Seq.map Option.get
    |> Seq.average

let chooseByMajority cornerValue (cell: Option<'a>[,]) =
    let ls = List.empty

    cell
    |> Seq.cast
    |> Seq.fold
        (fun state value ->
            let a =
                match value with
                | None -> cornerValue
                | Some x -> x

            let chooser (tag, _) = tag = a

            if List.exists chooser state then
                let _, cnt = List.find chooser state
                (List.filter (chooser >> not) state) @ [ (a, (cnt + 1)) ]
            else
                state @ [ (a, 1) ])
        ls
    |> List.maxBy snd
    |> fst

let private step (transformer: Option<'a>[,] -> 'a) (map: 'a[,]) =
    let mapHeight = Array2D.length1 map
    let mapWidth = Array2D.length2 map

    // Array2D.mapi
    //     (fun x y _ ->
    //         let cell =
    //             Array2D.init 3 3 (fun _x _y ->
    //                 let xInMap = x + _x - 1
    //                 let yInMap = y + _y - 1
    //
    //                 if
    //                     xInMap >= 0
    //                     && xInMap < mapHeight
    //                     && yInMap >= 0
    //                     && yInMap < mapWidth
    //                 then
    //                     Some map.[xInMap, yInMap]
    //                 else
    //                     None)
    //
    //         transformer cell)
    //     map
    let m =
        Array2D.init (mapHeight + 2) (mapWidth + 2) (fun x y ->
            if x = 0 || y = 0 || x >= mapHeight || y >= mapWidth then
                None
            else
                Some map.[(x - 1), (y - 1)])

    Array2D.mapi (fun x y _ -> transformer m.[x .. (x + 2), y .. (y + 2)]) map

let rec generateMap stepCnt transformer initMap =
    if stepCnt = 0 then
        initMap
    else
        // initMap |> step transformer |> generateMap (stepCnt - 1) transformer
        let afterTransform = step transformer initMap
        generateMap (stepCnt - 1) transformer afterTransform

let generateDualWhiteNoise (ratio: float) sideSize (rng: Random) =
    let size = pown sideSize 2
    let zeroes = floor (ratio * (size |> float)) |> int

    let values =
        List.init size (fun x -> if x < zeroes then 0 else 1)
        |> List.sortBy (fun _ -> rng.Next(1, size))

    Array2D.init sideSize sideSize (fun x y -> List.item (x * sideSize + y) values)

let generateWhiteNoise sideSize (rng: Random) =
    let size = pown sideSize 2
    let values = List.init size (fun _ -> rng.NextDouble())

    Array2D.init sideSize sideSize (fun x y -> List.item (x * sideSize + y) values)
