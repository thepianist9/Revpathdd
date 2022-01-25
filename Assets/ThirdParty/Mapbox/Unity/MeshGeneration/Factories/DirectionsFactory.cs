using UnityEngine;
using Mapbox.Directions;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using System.Collections;
using System.Runtime.CompilerServices;
using Mapbox.Unity;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Modifiers;


namespace HistocachingII
	{


		public class DirectionsFactory : MonoBehaviour
		{
			[SerializeField]
			AbstractMap _map;

			[SerializeField] private SpawnOnMap _spawnOnMap;

			[SerializeField]
			float _spawnScale = 100f;
			
			//Material modifier for Spawning route on map
			[SerializeField] MeshModifier[] MeshModifiers;
			[SerializeField] Material _material;

			//update frequency for route querying
			[SerializeField] [Range(1, 10)] private float UpdateFrequency = 2;

			//POIs Cache
			private List<Vector3> _cachedWaypoints;
			private bool _recalculateNext;

			private Directions _directions;
			private int _counter;

			GameObject _directionsGO;

			private void AwakeTour()
			{
				if (_map == null)
				{
					_map = FindObjectOfType<AbstractMap>();
				}

				_directions = MapboxAccess.Instance.Directions;
				_map.OnInitialized += Query;
				// _map.OnUpdated += Query;
			}



			protected virtual void OnDestroy()
			{
				_map.OnInitialized -= Query;
				_map.OnUpdated -= Query;
			}

			void Query()
			{
	        
				var count = _spawnOnMap.histocacheCollection.Count;
				var wp = new Vector2d[count];
				Debug.Log("Quering route");
				for (int i = 0; i < count; i++)
				{
					wp[i] = _cachedWaypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
				}
				var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
				_directionResource.Steps = true;
				_directions.Query(_directionResource, HandleDirectionsResponse);
			}

			public IEnumerator QueryTimer()
			{
				while (true)
				{ 
					if (_recalculateNext)
					{
						Query();
						_recalculateNext = false;
					}
					yield return new WaitForSeconds(UpdateFrequency);
					
				}
			}

			void HandleDirectionsResponse(DirectionsResponse response)
			{
				if (response == null || null == response.Routes || response.Routes.Count < 1)
				{
					return;
				}

				var meshData = new MeshData();
				var dat = new List<Vector3>();
				foreach (var point in response.Routes[0].Geometry)
				{
					dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
				}

				var feat = new VectorFeatureUnity();
				feat.Points.Add(dat);

				foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
				{
					mod.Run(feat, meshData, _map.WorldRelativeScale);
				}

				CreateGameObject(meshData);
			}

			
			GameObject CreateGameObject(MeshData data)
			{
				if (_directionsGO != null)
				{
					_directionsGO.Destroy();
				}
				_directionsGO = new GameObject("direction waypoint " + " entity");
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
				_directionsGO.AddComponent<MeshRenderer>().material = _material;
				return _directionsGO;
			}
			
			public void TourHandler(){
				Debug.Log("Tour initialised");
				AwakeTour();
				
				
				foreach (var modifier in MeshModifiers)
				{
					modifier.Initialize();
				}
				_cachedWaypoints = new List<Vector3>(_spawnOnMap.histocacheCollection.Count);
				foreach (var histocache in _spawnOnMap.histocacheCollection.Values)
				{
					_cachedWaypoints.Add(_map.GeoToWorldPosition(new Vector2d(histocache.lat, histocache.@long), true));
				}
				_recalculateNext = true;
	        
				StartCoroutine(QueryTimer());
				
		}
		

	}
}