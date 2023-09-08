﻿using LeafletAPI.Models;

namespace LeafletAPI;

// All the code in this file is included in all platforms.
public partial class MapBuilder
{
    private string _htmlHeader = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">";
    private Models.Map map;

    private string _htmlBody = "<body>";

    internal L_Polyline buildingShape { get; private set; }

    public MapBuilder()
    {
        map = new Models.Map();
        PrepareStructure();
    }

    private void PrepareStructure()
    {
        ComposeHtmlHeader();
    }

    private void ComposeHtmlHeader()
    {
        _htmlHeader += AddStylesheetPath();
        _htmlHeader += AddScriptPath();
        _htmlHeader += AddHeaderStyles();
    }

    private string AddHeaderStyles()
    {
        return """
                <style>
                html, body {
                    height: 100%;
                    margin: 0;
                }

                #map {
                    width: 100%;
                    height: 100%;
                }

                .tooltipclass {
                    background: transparent;
                    border: transparent;
                    box-shadow: 0 0px 0px rgba(0,0,0,0);
                }

                .imgclass {
                    display: block;
                    max-width: 400px;
                    max-height: 400px;
                    width: auto;
                    height: auto;
                }
                </style>
            """;
    }

    private string AddScriptPath()
    {
        return """
            <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" integrity="sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=" crossorigin="" />
            """;
    }

    private string AddStylesheetPath()
    {
        return """
            <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js" integrity="sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=" crossorigin=""></script>
            """;
    }

    /// <summary>
    /// Creates new map level with provided name.
    /// </summary>
    /// <param name="levelName"></param>
    /// <exception cref="ArgumentException"> if <paramref name="levelName"/> is null or empty</exception>
    public MapBuilder AddLevel(string levelName)
    {
        if (string.IsNullOrEmpty(levelName))
        {
            throw new ArgumentException($"'{nameof(levelName)}' cannot be null or empty.", nameof(levelName));
        }

        map.layers.Add(new L_Layer(levelName));
        return this;
    }

    /// <summary>
    /// Creates named room from given shape and associates it with
    /// <paramref name="level"/>
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="roomShape"></param>
    /// <param name="level">Optional, last level created before the method call or the one with provided name</param>
    public MapBuilder AddRoom(string roomName, float[,] roomShape, MapObjectStyle roomStyle, string level = "")
    {
        if (map.layers.Count < 1)
            throw new InvalidOperationException($"Room cannot be instantiated withouot creating level, call {nameof(AddLevel)}");

        L_Layer roomLevel;
        if (level == "")
            roomLevel = map.layers[^1];
        else
            roomLevel = map.layers.Find(layer => layer.Name == level);

        if (roomLevel == null) throw new ArgumentException($"Level named {level} was not found, did you create it first using {nameof(AddLevel)}?");


        roomLevel.polygons.Add(new L_Polygon(roomName, roomShape, roomStyle));
        return this;
    }


    /// <summary>
    /// Defines outer shape of building, which will be inserted on every level
    /// </summary>
    /// <param name="borders"></param>
    /// <param name="borderStyle">Style of building walls</param>
    public MapBuilder SetBuildingShape(Dictionary<string, float[,]> borders, MapObjectStyle borderStyle)
    {
        buildingShape = new L_Polyline("border", borderStyle, borders);
        return this;
    }

    public HtmlWebViewSource Build()
    {
        _htmlBody += "<script>"; // Open script tag
        ParseStyles();
        //ParsePolylines();
        //ParsePolygons();
        //ParseLayers();
        _htmlBody += "</script>"; // Close script tag

        var x = new HtmlWebViewSource
        {
            Html = _htmlHeader + "</head>" + _htmlBody + "</body></html>"
        };
        return x;
    }

    private void ParseStyles()
    {
        L_Layer anyLevel;
        L_StyledObject anyPolygon;

        try { anyLevel = map.layers.First(); } catch (System.Exception ex) { throw new InvalidOperationException($"{ex.Message}"); }
        try { anyPolygon = anyLevel.polygons.First(); } catch (System.Exception) { throw new InvalidOperationException($"You did not create any level features such as rooms, use {nameof(AddRoom)} or nameof(AddFeature) first."); }
        
        List<MapObjectStyle> createdStyles = anyPolygon.Style.GetInstances();

        string htmlStyles = "";

        foreach (MapObjectStyle uniqueStyle in createdStyles)
        {
            htmlStyles += uniqueStyle.ToHtmlStyle() + "\n";
        }

        _htmlBody += htmlStyles;
    }

    private void ParseLayers()
    {
        throw new NotImplementedException();
    }

    private void ParsePolylines()
    {
        throw new NotImplementedException();
    }

    private void ParsePolygons()
    {
        throw new NotImplementedException();
    }

    public void ExportToJson()
    {
        throw new NotImplementedException();
    }

    public void mockBody()
    {
        _htmlBody = tmp_bodybuilder();
    }
}