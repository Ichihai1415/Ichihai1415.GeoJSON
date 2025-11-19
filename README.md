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

### GeoJSON loading

```csharp
using Ichihai1415.GeoJSON;

var geojsonString = "";//enter geojson string here
var geojson = GeoJSONHelper.Deserialize<GeoJSONScheme.GeoJSON_Base>(geojsonString);
```

- `GeoJSONScheme.GeoJSON_Base`: The base class of GeoJSON. If `"Type": "GeometryCollection"`, use `GeoJSON_Base_OnlyGeometry` instead.

> [!TIP]
> `Properties` is an empty class. You can create your own GeoJSON scheme by inheriting from `GeoJSONScheme.GeoJSON_Base`. An example is `GeoJSON_JMA_Map`.

- `GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE`: The minimum `JsonSerializerOptions` that must be specified for loading. The code is as follows: 
```csharp
public static readonly JsonSerializerOptions ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE = new() { Converters = { new OriginalGeometryConverter() } };
```

### Inheriting samples

The samples is for Japan, so it is written here in Japanese. If garbled characters are occurring, please look at GitHub, etc.

- `GeoJSON_JMA_Map`: 気象庁GISデータ。地図・津波予報区も含め利用可能。
- `GeoJSON_JMA_FaultDL`: 気象庁断層データ([震央分布での内部データ](https://www.jma.go.jp/bosai/hypo/const/faultDL.geojson))

### Drawing supports



## Ichihai1415.GeoJSON.Test

debug/test for library. If you want to use this, you must change the path.

## Contribution

If you find a bug or see an improvement, please create an Issue or Pull Request, or contact me ([X @ProjectS31415_1](https://x.com/ProjectS31415_1)).

## Versions

- Change: Upgraded to .NET10.0 .  
- Add: Drawing supports. Additional items will be added in the future. See `Drawing supports` above for details.

### v1.0.3

2025/06/22

- Add: Some properties in `GeoJSON_JMA_Map.C_Properties_JMA_Map`.

### v1.0.2

2025/06/21

- Fix: Some typos.
- Fix: A problem where occurs exception when loading some JMA GIS data. Changed property of `GeoJSON_JMA_Map.C_Properties_JMA_Map` from `required` to `null`.
- Add: `GeoJSONHelper.Desirialize`. You can omit the `option` specification in `JsonSerializer.Deserialize` (`GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE` will be set automatically).

### v1.0.1

2025/06/01

- Fix: A problem where property contents could not be accessed when inherited. Fixed the inheritance process, so if you used inheritance in a previous version, you need to fix it.
- Add: Inheriting sample "GeoJSON_JMA_FaultDL". See `Inheriting samples` above for details.

### v1.0.0

2025/03/21

- Initial release
