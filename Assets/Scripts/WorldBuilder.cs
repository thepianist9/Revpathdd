using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public LocationService locationService;

    public GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        locationService.LocationChangedEvent.AddListener(OnLocationChanged);
    }

    void Destroy()
    {
        locationService.LocationChangedEvent.RemoveListener(OnLocationChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLocationChanged(float altitude, float latitude, float longitude, double timestamp)
    {
        locationService.LocationChangedEvent.RemoveListener(OnLocationChanged);

        Vector2 position = Conversions.GeoToWorldPosition(Constants.OTHER_LAT, Constants.OTHER_LONG, new Vector2(Constants.PIBU_LAT, Constants.PIBU_LONG), 2.5f);

        Instantiate(marker, new Vector3(position.x, 0, -position.y), Quaternion.identity);
    }
}
