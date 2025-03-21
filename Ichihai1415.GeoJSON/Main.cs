using System.Text.Json;
using System.Text.Json.Serialization;
using static Ichihai1415.GeoJSON.GeoJSONScheme;

namespace Ichihai1415.GeoJSON
{
    /// <summary>
    /// メインのクラスです。
    /// </summary>
    public class GeoJSONHelper
    {
        /// <summary>
        /// Geometry独自クラスへの読み込みに必要です。書き込みは未実装です。
        /// </summary>
        public class OriginalGeometryConverter : JsonConverter<OriginalGeometry?>
        {
            /// <inheritdoc/>
            public override OriginalGeometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var jsonDoc = JsonDocument.ParseValue(ref reader);
                var root = jsonDoc.RootElement;
                if (root.TryGetProperty("type", out JsonElement type))
                    if (root.TryGetProperty("coordinates", out JsonElement coordinates))//lineSt:[[ ]],multiLineSt,[[[ ]]],poly:[[[ ]]],multiPoly:[[[[ ]]]]
                        return Coordinates2OriginalGeometry(coordinates, type.ToString());
                    else
                        throw new JsonException("JSONの解析に失敗しました。", new ArgumentException("geometryのcoordinatesの取得に失敗しました。"));
                else
                    throw new JsonException("JSONの解析に失敗しました。", new ArgumentException("geometryのtypeの取得に失敗しました。"));
            }

            /// <summary>
            /// オブジェクトから各座標を取得します。
            /// </summary>
            /// <param name="singleObject">1つのオブジェクト(<c>[[x1,y1],[x2,y2,], ...]</c>)</param>
            /// <returns>座標の配列</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry.Point[] GetPoints(JsonElement singleObject)
            {
                var pointsList = new List<OriginalGeometry.Point>();
                foreach (var point in singleObject.EnumerateArray())
                {
                    if (point.GetArrayLength() == 2)
                    {
                        var lon = point[0].GetDouble();
                        var lat = point[1].GetDouble();
                        pointsList.Add(new OriginalGeometry.Point { Lat = (float)lat, Lon = (float)lon });
                    }
                    else
                        throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                }
                return [.. pointsList];
            }

            /// <summary>
            /// coordinatesを<see cref="OriginalGeometry"/>に変換します。
            /// </summary>
            /// <param name="coordinates">coordinatesのジャグ配列(<see cref="JsonElement"/>)をそのまま</param>
            /// <param name="type">種類(<c>Polygon</c>/<c>MultiPolygon</c>/<c>LineString</c>/<c>MultiLineString</c>) ※指定ミス注意</param>
            /// <returns>変換された値</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry Coordinates2OriginalGeometry(JsonElement coordinates, string type)
            {
                if (type == "Polygon" || type == "LineString")
                    return Coordinates2OriginalGeometry([coordinates], type);
                else if (type == "MultiPolygon" || type == "MultiLineString")
                    return Coordinates2OriginalGeometry([.. coordinates.EnumerateArray()], type);
                else
                    throw new JsonException("JSONの解析に失敗しました。", new Exception("typeが不正です。"));
            }

            /// <summary>
            /// coordinatesを<see cref="OriginalGeometry"/>に変換します。
            /// </summary>
            /// <remarks>通常は<see cref="Coordinates2OriginalGeometry(JsonElement, string)"/>を利用してください。</remarks>
            /// <param name="coordinatesList">coordinatesのジャグ配列をリスト化したもの(<see cref="Coordinates2OriginalGeometry(JsonElement, string)"/>を参照)</param>
            /// <param name="type">種類(<c>Polygon</c>/<c>MultiPolygon</c>/<c>LineString</c>/<c>MultiLineString</c>) ※指定ミス注意</param>
            /// <returns>変換された値</returns>
            /// <exception cref="JsonException">変換に失敗したとき</exception>
            private static OriginalGeometry Coordinates2OriginalGeometry(JsonElement[] coordinatesList, string type)
            {
                if (type != "Polygon" && type != "MultiPolygon" && type != "LineString" && type != "MultiLineString")
                    throw new JsonException("JSONの解析に失敗しました。", new Exception("typeが不正です。"));
                var polygonsPointsList = new List<List<OriginalGeometry.Point[]>>();
                foreach (var coordinates in coordinatesList)
                {
                    var polygonsPoints = new List<OriginalGeometry.Point[]>();//polygon:mainはpolygonsPoints[0]、subはあるときpolygonsPoints[1]として line:polygonsPoints[0]のみ
                    if (coordinates.ValueKind != JsonValueKind.Array)
                        throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                    if (type == "Polygon" || type == "MultiPolygon")
                        foreach (var element_pts in coordinates.EnumerateArray())
                        {
                            if (element_pts.ValueKind != JsonValueKind.Array)
                                throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                            var points = GetPoints(element_pts);
                            polygonsPoints.Add(points);
                        }
                    else//LineString/MultiLineString
                    {
                        if (coordinates.ValueKind != JsonValueKind.Array)
                            throw new JsonException("JSONの解析に失敗しました。", new Exception("構造が想定外です。"));
                        var points = GetPoints(coordinates);
                        polygonsPoints.Add(points);
                    }
                    polygonsPointsList.Add(polygonsPoints);
                }

                return new OriginalGeometry
                {
                    Type = type,
                    Coordinates = new OriginalGeometry.OriginalCoordinates
                    {
                        Objects = polygonsPointsList.Select(p => new OriginalGeometry.OriginalCoordinates.SingleObject
                        {
                            MainPoints = p[0],
                            HolePoints = p.Count == 2 ? p[1] : null
                        }).ToArray()
                    }
                };
            }

            /// <inheritdoc/>
            /// <remarks>書き込みは未実装です。</remarks>
            public override void Write(Utf8JsonWriter writer, OriginalGeometry? value, JsonSerializerOptions options)
            {
                throw new NotImplementedException("Writing is not yet implemented.");
            }
        }


    }
}
