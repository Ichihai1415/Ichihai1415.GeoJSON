# Ichihai1415.GeoJSON

![GitHub License](https://img.shields.io/github/license/Ichihai1415/Ichihai1415.GeoJSON)
![NuGet Version](https://img.shields.io/nuget/v/Ichihai1415.GeoJSON)
![NuGet Downloads](https://img.shields.io/nuget/dt/Ichihai1415.GeoJSON)
![GitHub last commit](https://img.shields.io/github/last-commit/Ichihai1415/Ichihai1415.GeoJSON)
![GitHub Release Date](https://img.shields.io/github/release-date/Ichihai1415/Ichihai1415.GeoJSON)
![GitHub issues](https://img.shields.io/github/issues/Ichihai1415/Ichihai1415.GeoJSON)

GeoJSON read support library for map drawing, etc.

This library is provided only in `.NET9`.

> [!NOTE]
> code comments are written in Japanese. I used DeepL partially in this README.

## Installation

- Search for `Ichihai1415.GeoJSON` in the NuGet Package Manager

- PM> `NuGet\Install-Package Ichihai1415.GeoJSON`

- \> `dotnet add package Ichihai1415.GeoJSON`

## How to use

```csharp
using Ichihai1415.GeoJSON;
using System.Text.Json;

var geojsonString = "";//enter geojson string here
var geojson = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_Base>(geojsonString, GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);
```

- `GeoJSONScheme.GeoJSON_Base`: The base class of GeoJSON. If `"Type": "GeometryCollection"`, use `GeoJSON_Base_OnlyGeometry` instead.

> [!TIP]
> `Properties` is an empty class. You can create your own GeoJSON scheme by inheriting from `GeoJSONScheme.GeoJSON_Base`. An example is `GeoJSON_JMA_Map`.

- `GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE`: The minimum `JsonSerializerOptions` that must be specified for loading. The code is as follows: 
```csharp
public static readonly JsonSerializerOptions ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE = new() { Converters = { new OriginalGeometryConverter() } };
```


## Ichihai1415.GeoJSON.Test

debug/test for library. If you want to use this, you must change the path.

output example:

```
1     ok  time:24.3561ms
1_2   ok  time:179.6224ms
2     ok  time:10.5185ms
2_2   ok  time:125.8494ms
```

## Contribution

If you find a bug or see an improvement, please create an Issue or Pull Request, or contact me ([X @ProjectS31415_1](https://x.com/ProjectS31415_1)).

## Versions

### v1.0.0

2025/03/21

- Initial release
