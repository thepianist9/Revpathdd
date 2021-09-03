using UnityEngine;

namespace HistocachingII
{
    public static class HistocacheConversions
    {
        public const float DEGREES_LATITUDE_IN_METERS = 111132f;
        public const float DEGREES_LONGITUDE_IN_METERS_AT_EQUATOR = 111319.9f;

        // Scale 1 means 1 Unity unit equals to 1 meter in real world
        // Scale 10 means 1 Unity unit equals to 10 meter in real world
        public static Vector2 GeoToUnityPosition(float latitude, float longitude, float refLatitude, float refLongitude, float scale = 1f)
        {
            // Reference: https://blog.anarks2.com/Geolocated-AR-In-Unity-ARFoundation/
            // Reference: http://wiki.gis.com/wiki/index.php/Decimal_degrees

            // GPS position converted into unity coordinates
            var latOffset = (latitude - refLatitude) * DEGREES_LATITUDE_IN_METERS / scale;
            var lonOffset = (longitude - refLongitude) * DEGREES_LONGITUDE_IN_METERS_AT_EQUATOR * Mathf.Cos(latitude * (Mathf.PI / 180)) / scale;

            return new Vector2(latOffset, lonOffset);
        }
    }
}
