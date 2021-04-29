using System;
using UnityEngine;

public static class Conversions
{
    private const int EarthRadius = 6378137; //no seams with globe example
    private const float OriginShift = 2 * Mathf.PI * EarthRadius / 2;

    /// <summary>
    /// Converts WGS84 lat/lon to x/y meters in reference to a center point
    /// </summary>
    /// <param name="lat"> The latitude. </param>
    /// <param name="lon"> The longitude. </param>
    /// <param name="refPoint"> A <see cref="T:UnityEngine.Vector2d"/> center point to offset resultant xy, this is usually map's center mercator</param>
    /// <param name="scale"> Scale in meters. (default scale = 1) </param>
    /// <returns> A <see cref="T:UnityEngine.Vector2d"/> xy tile ID. </returns>
    /// <example>
    /// Converts a Lat/Lon of (37.7749, 122.4194) into Unity coordinates for a map centered at (10,10) and a scale of 2.5 meters for every 1 Unity unit 
    /// <code>
    /// var worldPosition = Conversions.GeoToWorldPosition(37.7749, 122.4194, new Vector2d(10, 10), (float)2.5);
    /// // worldPosition = ( 11369163.38585, 34069138.17805 )
    /// </code>
    /// </example>
    public static Vector2 GeoToWorldPosition(float lat, float lon, Vector2 refPoint, float scale = 1)
    {
        var posx = lon * OriginShift / 180;
        var posy = Mathf.Log(Mathf.Tan((90 + lat) * Mathf.PI / 360)) / (Mathf.PI / 180);
        posy = posy * OriginShift / 180;
        return new Vector2((posx - refPoint.x) * scale, (posy - refPoint.y) * scale);
    }
}
