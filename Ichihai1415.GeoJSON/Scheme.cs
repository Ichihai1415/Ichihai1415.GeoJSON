using System.Text.Json.Serialization;

namespace Ichihai1415.GeoJSON
{
    /// <summary>
    /// 参照用クラスのクラス
    /// </summary>
    public class GeoJSONScheme
    {
        /// <summary>
        /// GeoJSONの格納クラスの基本クラス(通常使用用)
        /// </summary>
        /// <remarks>type:FeatureCollectionを前提としています。GeometryCollectionの場合<see cref="GeoJSON_Base_OnlyGeometry"/>を参照してください。</remarks>
        public class GeoJSON_Base : GeoJSON_Base<GeoJSON_Base.C_Properties_Empty>
        {
            /// <summary>
            /// 空の地物の詳細クラス(不要な場合これを使用)
            /// </summary>
            public class C_Properties_Empty
            {

            }
        }

        /// <summary>
        /// GeoJSONの格納クラスの基本クラス(継承用)
        /// </summary>
        public class GeoJSON_Base<TFeaturesProperties>
        {
            /// <summary>
            /// 常にFeatureCollection
            /// </summary>
            [JsonPropertyName("type")]
            public required string Type { get; set; }

            /// <summary>
            /// 地理要素の配列
            /// </summary>
            [JsonPropertyName("features")]
            public virtual required C_Feature<TFeaturesProperties>[] Features { get; set; }

            /// <summary>
            /// 地理要素
            /// </summary>
            public class C_Feature<TProperties>
            {
                /// <summary>
                /// 常にFeature
                /// </summary>
                [JsonPropertyName("type")]
                public required string Type { get; set; }

                /// <summary>
                /// 地物(nullの可能性あり)
                /// </summary>
                /// <remarks>自作クラス(<see cref="OriginalGeometry"/>?)に格納します。</remarks>
                //nullの場合のfeature例: {"type":"Feature","geometry":null,"properties":{"code":"","name":"鷹島(甑島南方)","namekana":""}}
                [JsonPropertyName("geometry")]
                public OriginalGeometry? Geometry { get; set; }

                /// <summary>
                /// 地物のプロパティ
                /// </summary>
                [JsonPropertyName("properties")]
                public TProperties? Properties { get; set; }

            }

            /// <summary>
            /// 空の地物の詳細クラス(不要な場合これを使用)
            /// </summary>
            public class C_Properties
            {

            }
        }

        /// <summary>
        /// GeoJSONの格納クラスの基本クラス
        /// </summary>
        /// <remarks>type:GeometryCollectionを前提としています。FeatureCollectionの場合<see cref="GeoJSON_Base{TFeatures}"/>を参照してください。</remarks>
        public class GeoJSON_Base_OnlyGeometry
        {
            /// <summary>
            /// 常にGeometryCollection
            /// </summary>
            [JsonPropertyName("type")]
            public required string Type { get; set; }

            /// <summary>
            /// 地物の配列
            /// </summary>
            [JsonPropertyName("geometries")]
            public required OriginalGeometry?[] Geometries { get; set; }
        }

        /// <summary>
        /// 気象庁GISデータのGeoJSON変換データの地図用クラス
        /// </summary>
        /// <remarks>FeatureCollectionを前提としています。</remarks>
        public class GeoJSON_JMA_Map : GeoJSON_Base<GeoJSON_JMA_Map.C_Properties_JMA_Map>
        {
            /// <inheritdoc/>
            [JsonPropertyName("features")]
            public override required C_Feature<C_Properties_JMA_Map>[] Features { get; set; }

            /// <summary>
            /// 地物の詳細
            /// </summary>
            public class C_Properties_JMA_Map
            {
                /// <summary>
                /// 気象庁コード
                /// </summary>
                [JsonPropertyName("code")]
                public required string Code { get; set; }

                /// <summary>
                /// 名称
                /// </summary>
                [JsonPropertyName("name")]
                public required string Name { get; set; }

                /// <summary>
                /// 名称(かな)
                /// </summary>
                [JsonPropertyName("namekana")]
                public required string Namekana { get; set; }
            }
        }

        /// <summary>
        /// 独自クラスのGeometry
        /// </summary>
        public class OriginalGeometry
        {
            /// <summary>
            /// Polygon/MultiPolygon
            /// </summary>
            public required string Type { get; set; }

            /// <summary>
            /// 独自クラスのcoordinates
            /// </summary>
            public required OriginalCoordinates Coordinates { get; set; }

            /// <summary>
            /// 独自クラスのcoordinates
            /// </summary>
            public class OriginalCoordinates
            {
                /// <summary>
                /// 1オブジェクトの配列(Geometryではない)
                /// </summary>
                public required SingleObject[] Objects { get; set; }

                /// <summary>
                ///  1オブジェクト(Geometryではない)
                /// </summary>
                public class SingleObject
                {
                    /// <summary>
                    /// 通常の座標の配列
                    /// </summary>
                    public required Point[] MainPoints { get; set; }

                    /// <summary>
                    /// Polygonの中空き用の座標の配列
                    /// </summary>
                    public Point[]? HolePoints { get; set; }
                }
            }

            /// <summary>
            /// 座標
            /// </summary>
            public class Point
            {
                /// <summary>
                /// 経度(x座標)
                /// </summary>
                public required float Lon { get; set; }

                /// <summary>
                /// 緯度(y座標)
                /// </summary>
                public required float Lat { get; set; }
                /*
                /// <summary>
                /// 標高(y座標)
                /// </summary>
                /// <remarks>必須ではない</remarks>
                public float? height { get; set; }//軽量化のためOFF*/
            }
        }

    }
}
