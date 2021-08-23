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
		GameObject _locationMarkerTemplate;

		[SerializeField]
		GameObject _viewpointMarkerTemplate;

		List<GameObject> _spawnedHistocaches = new List<GameObject>();

		List<GameObject> _spawnedViewpoints = new List<GameObject>();

		ILocationProvider _locationProvider;

		private bool isLoaded;

		[Geocode]
		private List<Vector2d> _histocacheLocations = new List<Vector2d>();

		[Geocode]
		private List<Vector2d> _viewpointLocations = new List<Vector2d>();

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

			if (isLoaded)
				return;

			isLoaded = true;

			GetHistocacheCollection();
		}

		private void Update()
		{
			int count = _spawnedHistocaches.Count;
			for (int i = 0; i < count; ++i)
			{
				var spawnedObject = _spawnedHistocaches[i];
				var location = _histocacheLocations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}

			count = _spawnedViewpoints.Count;
			for (int i = 0; i < count; ++i)
			{
				var spawnedObject = _spawnedViewpoints[i];
				var location = _viewpointLocations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}

		private void GetHistocacheCollection()
        {
			DataManager.Instance.GetHistocacheCollection((Histocache[] histocacheCollection) =>
			{
				_histocacheLocations.Clear();
				_viewpointLocations.Clear();

				foreach (Histocache histocache in histocacheCollection)
				{
					_histocacheLocations.Add(new Vector2d(histocache.lat, histocache.@long));

					if (histocache.has_viewpoint_location)
						_viewpointLocations.Add(new Vector2d(histocache.viewpoint_lat, histocache.viewpoint_long));
				}

				SetMarkers();
			});
        }

		void SetMarkers()
		{
			int count = _histocacheLocations.Count;
			for (int i = 0; i < count; ++i)
			{
				var instance = Instantiate(_locationMarkerTemplate);
				instance.transform.localPosition = _map.GeoToWorldPosition(_histocacheLocations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedHistocaches.Add(instance);
			}

			count = _viewpointLocations.Count;
			for (int i = 0; i < count; ++i)
			{
				var instance = Instantiate(_viewpointMarkerTemplate);
				instance.transform.localPosition = _map.GeoToWorldPosition(_viewpointLocations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedViewpoints.Add(instance);
			}
		}
    }
}
