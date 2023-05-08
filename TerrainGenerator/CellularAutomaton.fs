module TerrainGenerator.CellularAutomaton

let rec private step stepCnt (neighborhoodChooser: int -> int -> 'a array2d -> 'a option seq) (transformer: 'a option seq -> 'a) (map: 'a[,]) =
    if stepCnt = 0 then
        map
    else
        Array2D.mapi (fun x y _ -> transformer (neighborhoodChooser x y map)) map
        |> step (stepCnt - 1) neighborhoodChooser transformer

let generateMap stepCnt neighborhoodChooser transformer normalizationFunc initMap =
    step stepCnt neighborhoodChooser transformer initMap
    |> normalizationFunc

let eightTilesNeighborhoodWithCell x y map =
    let mapHeight = Array2D.length1 map
    let mapWidth = Array2D.length2 map
    let neighborhood =
        Array2D.init 3 3 (fun _x _y ->
            let xInMap = x + _x - 1
            let yInMap = y + _y - 1
    
            if
                xInMap >= 0
                && xInMap < mapHeight
                && yInMap >= 0
                && yInMap < mapWidth
            then
                Some map.[xInMap, yInMap]
            else
                None)
    neighborhood
    |> Seq.cast<'a option>
    
let eightTilesNeighborhoodWithoutCell x y map =
    eightTilesNeighborhoodWithCell x y map
    |> Seq.removeAt 4

let chooseByAverage (neighbourhood: float option seq) =
    neighbourhood
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

let chooseByMajority neighborhood =
    neighborhood
    |> Seq.filter Option.isSome
    |> Seq.map Option.get
    |> Seq.fold majorityFolder []
    |> List.maxBy snd
    |> fst

let chooseByMajorityWithCorner cornerValue neighborhood =
    neighborhood
    |> Seq.map (Option.defaultValue cornerValue)
    |> Seq.fold majorityFolder []
    |> List.maxBy snd
    |> fst
