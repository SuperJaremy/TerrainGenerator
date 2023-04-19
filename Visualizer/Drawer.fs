module Visualizer.Drawer

open System.Drawing
open Microsoft.FSharp.Core
open TerrainGenerator.Tiles

let tileToColor (tile: TerrainTile) =
    match tile with
    | TerrainTile.Water -> Color.Aqua
    | TerrainTile.Land biome ->
        match biome with
        | LandTile.Sand -> Color.SandyBrown
        | LandTile.Rock -> Color.DarkGray
        | LandTile.Tundra -> Color.DarkOliveGreen
        | LandTile.Forest -> Color.DarkGreen
        | LandTile.Field -> Color.LightGreen
        | LandTile.Snow -> Color.White

let draw map scale =
    let side = Array2D.length1 map
    let sideScaled = side * scale

    let bitmap =
        new Bitmap(sideScaled, sideScaled, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)

    let graphics = Graphics.FromImage(bitmap)

    map
    |> Array2D.iteri (fun x y value ->
        let pen = new SolidBrush(tileToColor value)
        let sclX, sclY = (x * scale), (y * scale)
        graphics.FillRectangle(pen, sclY, sclX, scale, scale))

    bitmap
