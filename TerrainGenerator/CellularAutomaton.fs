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

let private majorityFolder state value =
     let chooser (tag, _) = tag = value

     if List.exists chooser state then
        let _, cnt = List.find chooser state
        (List.filter (chooser >> not) state) @ [ (value, (cnt + 1)) ]
     else
         state @ [ (value, 1) ]

let chooseByMajority (cell: Option<'a>[,]) =
    cell
    |> Seq.cast
    |> Seq.filter Option.isSome
    |> Seq.map Option.get
    |> Seq.fold majorityFolder []
    |> List.maxBy snd
    |> fst

let chooseByMajorityWithCorner cornerValue (cell: Option<'a>[,]) =
    cell
    |> Seq.cast
    |> Seq.map (Option.defaultValue cornerValue)
    |> Seq.fold majorityFolder []
    |> List.maxBy snd
    |> fst

let private step (transformer: Option<'a>[,] -> 'a) (map: 'a[,]) =
    let mapHeight = Array2D.length1 map
    let mapWidth = Array2D.length2 map

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
        step transformer initMap
        |> generateMap (stepCnt - 1) transformer
