module Visualizer.Drawer

open System.Drawing
open Microsoft.FSharp.Core
open TerrainGenerator.Tiles

let tileToColor (tile: TerrainTile) =
    match tile with
    | TerrainTile.Water -> Color.Aqua
    | TerrainTile.Land biome ->
        match biome with
        | LandTile.Snow -> Color.White
        | LandTile.Tundra -> Color.DarkSeaGreen
        | LandTile.Bare -> Color.LightGray
        | LandTile.Scorched -> Color.Gray
        | LandTile.Taiga -> Color.OliveDrab
        | LandTile.Shrubland -> Color.DarkSeaGreen
        | LandTile.Temperate_Desert -> Color.PaleGoldenrod
        | LandTile.Temperate_Rain_Forest -> Color.ForestGreen
        | LandTile.Temperate_Deciduous_Forest -> Color.Olive
        | LandTile.Grassland -> Color.GreenYellow
        | LandTile.Tropical_Rain_Forest -> Color.DarkSlateGray
        | LandTile.Tropical_Seasonal_Forest -> Color.SeaGreen
        | LandTile.Subtropical_Desert -> Color.Goldenrod
        
let debugToColor x =
    match x with
    | _ when x = 0 -> Color.Blue
    | _ when x = 1 -> Color.Yellow
    | _ when x = 2 -> Color.Green
    | _ when x = 3 -> Color.Gray
    | _ when x = 4 -> Color.Purple
    | _ when x = 5 -> Color.White
    | _ -> Color.Red
    
let toBlackAndWhite x =
    let x = x * 255.0 |> round |> int
    Color.FromArgb(x, x, x)
    
let fromRGBMaps x =
    let r, g, b = x
    Color.FromArgb((r * 255.0 |> round |> int), (g * 255.0 |> round |> int), (b * 255.0 |> round |> int))

let draw colorPicker map scale =
    let side = Array2D.length1 map
    let sideScaled = side * scale

    let bitmap =
        new Bitmap(sideScaled, sideScaled, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

    let graphics = Graphics.FromImage(bitmap)

    map
    |> Array2D.iteri (fun x y value ->
        let pen = new SolidBrush(colorPicker value)
        let sclX, sclY = (x * scale), (y * scale)
        graphics.FillRectangle(pen, sclY, sclX, scale, scale))

    bitmap
