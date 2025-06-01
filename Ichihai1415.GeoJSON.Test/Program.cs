using System.Text.Json;

namespace Ichihai1415.GeoJSON.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var json_init = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_Base>(File.ReadAllText(@"D:\Ichihai1415\data\map\JMA\geojson\AreaForecastLocalE_GIS_20190125_01.geojson"), GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);


            var dt1 = DateTime.Now;
            var json1 = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(File.ReadAllText(@"D:\Ichihai1415\data\map\JMA\geojson\AreaForecastLocalE_GIS_20190125_01.geojson"), GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);
            Console.WriteLine("1     ok  time:" + (DateTime.Now - dt1).TotalMilliseconds + "ms");

            var dt1_2 = DateTime.Now;
            var json1_2 = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(File.ReadAllText(@"D:\Ichihai1415\data\map\JMA\geojson\AreaForecastLocalE_GIS_20190125_1.geojson"), GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);
            Console.WriteLine("1_2   ok  time:" + (DateTime.Now - dt1_2).TotalMilliseconds + "ms");

            var dt2 = DateTime.Now;
            var json2 = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(File.ReadAllText(@"D:\Ichihai1415\data\map\JMA\geojson\AreaTsunami_GIS_20240520_01.geojson"), GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);
            Console.WriteLine("2     ok  time:" + (DateTime.Now - dt2).TotalMilliseconds + "ms");

            var dt2_2 = DateTime.Now;
            var json2_2 = JsonSerializer.Deserialize<GeoJSONScheme.GeoJSON_JMA_Map>(File.ReadAllText(@"D:\Ichihai1415\data\map\JMA\geojson\AreaTsunami_GIS_20240520_1.geojson"), GeoJSONHelper.ORIGINAL_GEOMETRY_SERIALIZER_OPTIONS_SAMPLE);
            Console.WriteLine("2_2   ok  time:" + (DateTime.Now - dt2_2).TotalMilliseconds + "ms");

            var name = json1.Features[2].Properties.Name;
            Console.WriteLine(name);
            Console.WriteLine("\n\nend");
        }
    }
}
