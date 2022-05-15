
using System;
using UnityEngine;
using Mapbox.Directions;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;

using Mapbox.Unity;
using Mapbox.Unity.Location;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;
using TMPro;



namespace HistocachingII
	{


		public class DirectionsFactory : MonoBehaviour
		{
			
			private static DirectionsFactory _Instance; 
			public static DirectionsFactory Instance { get { return _Instance; } }
			[SerializeField]
			AbstractMap _map;

			
			[SerializeField] private SpawnOnMap _spawnOnMap;



			[SerializeField]
			float _spawnScale = 10f;
			
			//Material modifier for Spawning route on map
			[SerializeField] MeshModifier[] MeshModifiers;
			[SerializeField] Material _material;
			private DirectionsResponse _directionsResponse;
			private List<Vector3> dat = new List<Vector3>();
			private VectorFeatureUnity feat = new VectorFeatureUnity();
			
			

			//update frequency for route querying
			[SerializeField] [Range(1, 10)] private float UpdateFrequency = 2;

			//POIs Cache
			private List<Vector3> _cachedWaypoints;
			private bool _recalculateNext;

			private Directions _directions;
			private int _counter;

			GameObject _directionsGO;
			ILocationProvider _locationProvider;

			private void Awake()
			{
				if (_Instance == null)
				{
					_Instance = this;
				}
			}

			private void AwakeTour()
			{
				
				if (_map == null)
				{
					_map = FindObjectOfType<AbstractMap>();
					
				}
				_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				_directions = MapboxAccess.Instance.Directions;
				_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
				
				// _map.OnUpdated += Query;
			}
			
			void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
			{
				_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
				_locationProvider.OnLocationUpdated += Query;
			}

			


			void Query(Mapbox.Unity.Location.Location location)
			{

				Vector2d currentLoc = location.LatitudeLongitude; 
				
				var count = _cachedWaypoints.Count;
				var wp = new Vector2d[count+1];
				Debug.Log("Quering route");
				for (int i = 0; i < count; i++)
				{
					wp[i] = _cachedWaypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
				}

				wp[count] = currentLoc;
				
				var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
				_directionResource.Steps = true;
				_directions.Query(_directionResource, HandleDirectionsResponse);
			}

			void HandleDirectionsResponse(DirectionsResponse response)
			{
				GameObject instructions = GameObject.Find("DirectionsText");
				TextMeshProUGUI directionsText = instructions.GetComponentInChildren<TextMeshProUGUI>();
				_directionsResponse = response;
				directionsText.text = "";

				if (response == null || null == response.Routes || response.Routes.Count < 1)
				{
					return;
				}

				var meshData = new MeshData();
				foreach (var point in response.Routes[0].Geometry)
				{
					dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
				}

				foreach (var step in response.Routes[0].Legs[0].Steps)
				{
					directionsText.text += step.Maneuver.Instruction + "\n";
				}

				feat.Points.Add(dat);

				foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
				{
					mod.Run(feat, meshData, _map.WorldRelativeScale);
				}

				CreateGameObject(meshData);
			}

			private void Update()
			{
				if (_directionsGO != null)
				{
					dat.Clear();
					
					foreach (var point in _directionsResponse.Routes[0].Geometry)
					{
						dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
					}

					var meshData = new MeshData();
					feat = new VectorFeatureUnity();
					feat.Points.Add(dat);
					
					foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
					{
						mod.Run(feat, meshData, _map.WorldRelativeScale);
					}

					CreateGameObject(meshData);

				}
			}


			GameObject CreateGameObject(MeshData data)
			{
				if (_directionsGO != null)
				{
					_directionsGO.Destroy();
				}
				_directionsGO = new GameObject("direction waypoint " + " entity");
				_directionsGO.transform.parent = _map.transform;
				var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
				mesh.subMeshCount = data.Triangles.Count;

				mesh.SetVertices(data.Vertices);
				_counter = data.Triangles.Count;
				for (int i = 0; i < _counter; i++)
				{
					var triangle = data.Triangles[i];
					mesh.SetTriangles(triangle, i);
				}

				_counter = data.UV.Count;
				for (int i = 0; i < _counter; i++)
				{
					var uv = data.UV[i];
					mesh.SetUVs(i, uv);
				}

				mesh.RecalculateNormals();
				_directionsGO.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_directionsGO.AddComponent<MeshRenderer>().material = _material;
				return _directionsGO;
			}

			public void DestroyDirections()
			{
				if (_directionsGO)
				{
					_directionsGO.gameObject.SetActive(false);
				}
				_locationProvider.OnLocationUpdated -= Query;
				_locationProvider = null;
				
			}
			
			public void TourHandler(List<TourPOI> tourPois)
			{

				if (_directionsGO)
				{
					Destroy(_directionsGO);
				}
				else
				{
					Debug.Log("Tour initialised");
					AwakeTour();
				
				
					foreach (var modifier in MeshModifiers)
					{
						modifier.Initialize();
					}
				
					_cachedWaypoints = new List<Vector3>(_spawnOnMap.histocacheCollection.Count);
					foreach (var tourhisto in tourPois)
					{
						
							_cachedWaypoints.Add(_map.GeoToWorldPosition(
								new Vector2d(tourhisto.lat, tourhisto.@long),
								true));
						
							_recalculateNext = true;
					}
				}

			}
		}
}