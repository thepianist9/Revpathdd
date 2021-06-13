using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace HistocachingII
{
    public class SpawnOnMap : MonoBehaviour
    {
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		List<GameObject> _spawnedObjects = new List<GameObject>();

		ILocationProvider _locationProvider;

		private bool isLoading;

		[Geocode]
		private List<Vector2d> _locations = new List<Vector2d>();

		private NetworkManager networkManager = new NetworkManager();

		private void Awake()
		{
			// Prevent double initialization of the map. 
			_map.InitializeOnStart = false;
		}

		protected virtual IEnumerator Start()
		{
			yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}

		void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_map.Initialize(location.LatitudeLongitude, _map.AbsoluteZoom);

			GetPOICollection();
		}

		// private void Update()
		// {
		// 	int count = _spawnedObjects.Count;
		// 	for (int i = 0; i < count; ++i)
		// 	{
		// 		var spawnedObject = _spawnedObjects[i];
		// 		var location = _locations[i];
		// 		spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
		// 		spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
		// 	}
		// }

		void GetPOICollection()
        {
            if (isLoading)
                return;

            isLoading = true;

            _locations.Clear();

            StartCoroutine(networkManager.GetPOICollection((POI[] poiCollection) =>
            {
                // isLoading = false;

                for (int i = 0; i < poiCollection?.Length; ++i)
                {
                    POI poi = poiCollection[i];

                    _locations.Add(new Vector2d(poi.lat, poi.@long));
                }

				SetMarkers();
            }));
        }

		void SetMarkers()
		{
			int count = _locations.Count;
			for (int i = 0; i < count; ++i)
			{
				var instance = Instantiate(_markerPrefab);
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}
		}
    }
}
