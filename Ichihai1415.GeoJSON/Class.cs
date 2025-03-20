using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ichihai1415.GeoJSON
{
    public class Class
    {

        /// <summary>
        /// 気象庁GISデータのGeoJSONの地図用クラス
        /// </summary>
        public class GeoJSON_JMA_Map
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
            public required C_Feature[] Features { get; set; }

            /// <summary>
            /// 地理要素
            /// </summary>
            public class C_Feature
            {
                /// <summary>
                /// 常にFeature
                /// </summary>
                [JsonPropertyName("type")]
                public required string Type { get; set; }

                /// <summary>
                /// 地物(nullの可能性あり)
                /// </summary>
                /// <remarks>自作クラス(<see cref="OriginalGeometry"/>?)に格納します。nullの場合のfeature例: <c>{"type":"Feature","geometry":null,"properties":{"code":"","name":"鷹島(甑島南方)","namekana":""}}</c></remarks>
                [JsonPropertyName("geometry")]
                public OriginalGeometry? Geometry { get; set; }

                [JsonPropertyName("properties")]
                public required C_Properties Properties { get; set; }
            }
            /*//旧クラス
            public class C_Geometry
            {
                /// <summary>
                /// Polygon/MultiPolygon
                /// </summary>
                [JsonPropertyName("type")]
                public required string Type { get; set; }

                /// <summary>
                /// 座標の配列
                /// </summary>
                /// <remarks><c>"type":"Polygon"</c>の場合、<c>double</c>(<c>double[外側(,内側)][点配列][(経度,緯度)配列]</c>)、<c>"type":"MultiPolygon"</c>の場合、<c>double[]</c>(<c>double[ポリゴン配列][外側(,内側)][点配列][(経度,緯度)配列]</c>)</remarks>
                [JsonPropertyName("coordinates")]
                public required OriginalGeometry Coordinates { get; set; }
            }
            */
            /// <summary>
            /// 地物の詳細
            /// </summary>
            public class C_Properties
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

                /// <summary>
                /// 標高(y座標)
                /// </summary>
                /// <remarks>必須ではない</remarks>
                //public float? height { get; set; }//軽量化のためOFF
            }
        }

    }
}
