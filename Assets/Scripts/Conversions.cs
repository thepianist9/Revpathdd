using UnityEngine;

namespace HistocachingII
{
    public static class Conversions
    {
        public static Vector2 GeoToUnityPosition(float latitude, float longitude, float refLatitude, float refLongitude)
        {
            // Reference: https://blog.anarks2.com/Geolocated-AR-In-Unity-ARFoundation/

            // Conversion factors
            float degreesLatitudeInMeters = 111132;
            float degreesLongitudeInMetersAtEquator = 111319.9f;

            // GPS position converted into unity coordinates
            var latOffset = (latitude - refLatitude) * degreesLatitudeInMeters;
            var lonOffset = (longitude - refLongitude) * degreesLongitudeInMetersAtEquator * Mathf.Cos(latitude * (Mathf.PI / 180));

            return new Vector2(latOffset, lonOffset);

            // Reference: http://wiki.gis.com/wiki/index.php/Decimal_degrees

            // var latOffset = (latitude - refLatitude) * 111.1f / distance;
            // var lonOffset = (longitude - refLongitude) * 111.1f / distance;
        }
    }
}
