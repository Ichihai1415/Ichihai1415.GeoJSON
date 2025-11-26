using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
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
        /// Geometry独自クラスへの読み込みに指定する必要がある最低限の<see cref="JsonSerializerOptions"/>
        /// </summary>
        public static readonly JsonSerializerOptions ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE = new() { Converters = { new OriginalGeometryConverter() } };

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
                        Objects = [.. polygonsPointsList.Select(p => new OriginalGeometry.OriginalCoordinates.SingleObject
                        {
                            MainPoints = p[0],
                            HolePoints = p.Count == 2 ? p[1] : null
                        })]
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

        /// <summary>
        /// <see cref="JsonSerializerOptions"/>に<see cref="ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE"/>を自動的に指定してJSONをデシリアライズします。
        /// </summary>
        /// <typeparam name="TValue">デシリアライズする型。</typeparam>
        /// <param name="json">JSON文字列</param>
        /// <returns><typeparamref name="TValue"/>にデシリアライズされたJSON</returns>
        public static TValue? Deserialize<TValue>(string json) => JsonSerializer.Deserialize<TValue>(json, ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);

        /// <summary>
        /// <see cref="JsonSerializerOptions"/>に<see cref="ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE"/>を自動的に指定してJSONをデシリアライズします。
        /// </summary>
        /// <remarks><see cref="JsonSerializer.Deserialize{T}(string, JsonSerializerOptions?)"/>と同じです。</remarks>
        /// <typeparam name="TValue">デシリアライズする型。</typeparam>
        /// <param name="json">JSON文字列</param>
        /// <param name="options">JSONシリアライズのオプション</param>
        /// <returns><typeparamref name="TValue"/>にデシリアライズされたJSON</returns>
        public static TValue? Deserialize<TValue>(string json, JsonSerializerOptions options) => JsonSerializer.Deserialize<TValue>(json, options);
    }

    /// <summary>
    /// マップ描画用クラスです。
    /// </summary>
    public class MapDrawer
    {
        /// <summary>
        /// System.Drawing.Common用
        /// </summary>
        [SupportedOSPlatform("windows")]
        public class DrawingCommon
        {
            /// <summary>
            /// 気象庁GISデータで地図を描画します。
            /// </summary>
            /// <param name="g">Encapsulates a GDI+ drawing surface.</param>
            /// <param name="mapData">地図データ</param>
            /// <param name="config">描画設定</param>
            /// <exception cref="Exception"></exception>
            public static void DrawMap(Graphics g, GeoJSON_JMA_Map mapData, DrawConfig config)
            {
                if (mapData == null) throw new Exception("地図データが読み込まれていません。");
                var colorConfig = config.Colors.DeepCopy() ?? new();
                if (colorConfig.CodeFillColors.Count == 0) Console.WriteLine("塗り替え配色データがありません。");
                using var gp = new GraphicsPath();
                foreach (var feature in mapData.Features)
                {
                    if (feature.Geometry == null) continue;
                    foreach (var singleObject in feature.Geometry.Coordinates.Objects)
                    {
                        gp.StartFigure();
                        var points = singleObject.MainPoints.Select(coordinate => new PointF((coordinate.Lon - config.LonSta) * (float)config.Zoom, (config.LatEnd - coordinate.Lat) * (float)config.Zoom));
                        if (points.Count() > 2)
                        {
                            gp.AddPolygon(points.ToArray());
                            if (colorConfig.CodeFillColors.TryGetValue(int.Parse(feature.Properties?.GetCode() == "" ? "-1" : feature.Properties?.GetCode() ?? "-1"), out var color))
                                g.FillPolygon(new SolidBrush(color), points.ToArray());
                            else
                                g.FillPolygon(new SolidBrush(colorConfig.DefaultFillColor), points.ToArray());
                        }
                    }
                }
                //g.FillPath(new SolidBrush(Color.FromArgb(100, 100, 150)), gp);//単色用
                var lineWidth = Math.Max(1f, (float)config.Zoom / 216f);
                g.DrawPath(new Pen(colorConfig.LineColor, lineWidth) { LineJoin = LineJoin.Round }, gp);
            }

            /// <summary>
            /// FeatureCollectionの基本クラスのデータで地図を描画します。Featureは使用しません。
            /// </summary>
            /// <param name="g">Encapsulates a GDI+ drawing surface.</param>
            /// <param name="mapData">地図データ</param>
            /// <param name="config">描画設定</param>
            public static void DrawMap_OnlyGeometry(Graphics g, GeoJSON_Base mapData, DrawConfig config) => DrawMap_OnlyGeometry(g, [.. mapData.Features.Select(x => x.Geometry)], config);

            /// <summary>
            /// GeometryCollectionの基本クラスのデータで地図を描画します。
            /// </summary>
            /// <param name="g">Encapsulates a GDI+ drawing surface.</param>
            /// <param name="mapData">地図データ</param>
            /// <param name="config">描画設定</param>
            public static void DrawMap_OnlyGeometry(Graphics g, GeoJSON_Base_OnlyGeometry mapData, DrawConfig config) => DrawMap_OnlyGeometry(g, mapData.Geometries, config);

            /// <summary>
            /// FeatureCollectionの基本クラスのデータで地図を描画します。
            /// </summary>
            /// <param name="g">Encapsulates a GDI+ drawing surface.</param>
            /// <param name="geometries">独自Geometry配列</param>
            /// <param name="config">描画設定</param>
            /// <exception cref="Exception"></exception>
            public static void DrawMap_OnlyGeometry(Graphics g, OriginalGeometry?[] geometries, DrawConfig config)
            {
                if (geometries == null) throw new Exception("地図データが読み込まれていません。");
                var colorConfig = config.Colors.DeepCopy() ?? new();
                if (colorConfig.CodeFillColors.Count > 0) Console.WriteLine("塗り替え配色データがありますが、ここでは無効です。");
                using var gp = new GraphicsPath();
                foreach (var geometry in geometries)
                {
                    if (geometry == null) continue;
                    foreach (var singleObject in geometry.Coordinates.Objects)
                    {
                        gp.StartFigure();
                        var points = singleObject.MainPoints.Select(coordinate => new PointF((coordinate.Lon - config.LonSta) * (float)config.Zoom, (config.LatEnd - coordinate.Lat) * (float)config.Zoom));
                        if (points.Count() > 2) gp.AddPolygon(points.ToArray());
                    }
                }
                g.FillPath(new SolidBrush(Color.FromArgb(100, 100, 150)), gp);//単色用
                var lineWidth = Math.Max(1f, (float)config.Zoom / 216f);
                g.DrawPath(new Pen(colorConfig.LineColor, lineWidth) { LineJoin = LineJoin.Round }, gp);
            }
        }

        /// <summary>
        /// 描画設定
        /// </summary>
        public class DrawConfig
        {
            /// <summary>
            /// 初期化コンストラクト
            /// </summary>
            public DrawConfig() { }

            /// <summary>
            /// [動画用]描画開始日時
            /// </summary>
            public DateTime? StartTime { get; init; }

            /// <summary>
            /// [動画用]描画終了日時
            /// </summary>
            public DateTime? EndTime { get; init; }

            /// <summary>
            /// [動画用]描画間隔
            /// </summary>
            public TimeSpan? DrawSpan { get; init; }

            /// <summary>
            /// サイズ
            /// </summary>
            public required C_Size Size { get; init; }

            /// <summary>
            /// 緯度の始点
            /// </summary>
            public required float LatSta { get; init; }

            /// <summary>
            /// 緯度の終点
            /// </summary>
            public required float LatEnd { get; init; }

            /// <summary>
            /// 経度の始点
            /// </summary>
            public required float LonSta { get; init; }

            /// <summary>
            /// 経度の終点
            /// </summary>
            public required float LonEnd { get; init; }

            /// <summary>
            /// ズーム率
            /// </summary>
            //いるかわからないが再計算回避策
            private float _zoom = -1;

            /// <summary>
            /// ズーム率
            /// </summary>
            public float Zoom
            {
                get
                {
                    if (_zoom == -1)
                        _zoom = Size.Height / (LatEnd - LatSta);
                    return _zoom;
                }
            }

            /// <summary>
            /// ズーム率（幅, 高さ）
            /// </summary>
            private (float zw, float zh) _zoomWH = (-1, -1);

            /// <summary>
            /// ズーム率（幅, 高さ）
            /// </summary>
            public (float zw, float zh) ZoomWH
            {
                get
                {
                    if (_zoomWH.zw == -1)
                        _zoomWH = (Size.Width / (LonEnd - LonSta), Size.Height / (LatEnd - LatSta));
                    return _zoomWH;
                }
            }

            /// <summary>
            /// サイズを取得します。
            /// </summary>
            /// <returns><see cref="System.Drawing.Size"/>でのサイズ</returns>
            public Size GetDrawSize() => Size.ToDrawingSize();

            /// <summary>
            /// 配色
            /// </summary>
            public required C_Colors Colors { get; set; }

            /// <summary>
            /// ディープコピーします。
            /// </summary>
            /// <returns>ディープコピーされたオブジェクト</returns>
            public DrawConfig DeepCopy() => new()
            {
                StartTime = StartTime,
                EndTime = EndTime,
                DrawSpan = DrawSpan,
                Size = Size.DeepCopy(),
                LatSta = LatSta,
                LatEnd = LatEnd,
                LonSta = LonSta,
                LonEnd = LonEnd,
                _zoom = _zoom,
                _zoomWH = _zoomWH,
                Colors = Colors.DeepCopy()
            };

            /// <summary>
            /// 配色を指定しディープコピーします。
            /// </summary>
            /// <param name="color">配色</param>
            /// <returns>ディープコピーされたオブジェクト</returns>
            public DrawConfig DeepCopy(C_Colors color) => new()
            {
                StartTime = StartTime,
                EndTime = EndTime,
                DrawSpan = DrawSpan,
                Size = Size.DeepCopy(),
                LatSta = LatSta,
                LatEnd = LatEnd,
                LonSta = LonSta,
                LonEnd = LonEnd,
                _zoom = _zoom,
                _zoomWH = _zoomWH,
                Colors = color
            };

            /// <summary>
            /// サイズ
            /// </summary>
            public class C_Size
            {
                /// <summary>
                /// 初期化コンストラクト（高さ指定、16:9）
                /// </summary>
                /// <param name="height">高さ</param>
                public C_Size(int height)
                {
                    Width = height * 16 / 9;
                    Height = height;
                }

                /// <summary>
                /// 初期化コンストラクト（高さ、比率指定）
                /// </summary>
                /// <param name="height">高さ</param>
                /// <param name="ratio">比率（幅/高さ）</param>
                public C_Size(int height, double ratio)
                {
                    Width = (int)(height * ratio);
                    Height = height;
                }

                /// <summary>
                /// 初期化コンストラクト
                /// </summary>
                /// <param name="height">高さ</param>
                /// <param name="ratio">比（幅, 高さ）</param>
                public C_Size(int height, (int rw, int rh) ratio)
                {
                    Width = height * ratio.rw / ratio.rh;
                    Height = height;
                }

                /// <summary>
                /// 初期化コンストラクト（幅、高さ指定）
                /// </summary>
                /// <param name="width">幅</param>
                /// <param name="height">高さ</param>
                public C_Size(int width, int height)
                {
                    Width = width;
                    Height = height;
                }

                /// <summary>
                /// 幅
                /// </summary>
                public int Width { get; }

                /// <summary>
                /// 高さ
                /// </summary>
                public int Height { get; }

                /// <summary>
                /// サイズを取得します。
                /// </summary>
                /// <returns><see cref="System.Drawing.Size"/>でのサイズ</returns>
                public Size ToDrawingSize() => new(Width, Height);

                /// <summary>
                /// ディープコピーします。
                /// </summary>
                /// <returns>ディープコピーされたオブジェクト</returns>
                public C_Size DeepCopy() => new(Width, Height);
            }

            /// <summary>
            /// 配色
            /// </summary>
            public class C_Colors
            {
                /// <summary>
                /// 境界線色
                /// </summary>
                public Color LineColor { get; init; } = Color.White;

                /// <summary>
                /// 背景色
                /// </summary>
                public Color BackgroundColor { get; init; } = Color.FromArgb(20, 40, 60);

                /// <summary>
                /// 通常塗りつぶし色
                /// </summary>
                public Color DefaultFillColor { get; init; } = Color.FromArgb(100, 100, 150);

                /// <summary>
                /// コードと塗りつぶし色のペア
                /// </summary>
                public Dictionary<int, Color> CodeFillColors { get; set; } = [];

                /// <summary>
                /// ディープコピーします。
                /// </summary>
                /// <returns>ディープコピーされたオブジェクト</returns>
                public C_Colors DeepCopy() => new()
                {
                    LineColor = LineColor,
                    BackgroundColor = BackgroundColor,
                    DefaultFillColor = DefaultFillColor,
                    CodeFillColors = CodeFillColors.ToDictionary(x => x.Key, x => x.Value)//SolidBrushのDeepCopy怪しいけど保留//こっちではColorに変更
                };
            }
        }
    }
}
