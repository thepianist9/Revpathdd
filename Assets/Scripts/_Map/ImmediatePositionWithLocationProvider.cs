﻿using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;

namespace HistocachingII
{
	public class ImmediatePositionWithLocationProvider : MonoBehaviour
	{
		public FollowTarget m_MapCameraFollowTarget;

		bool _isInitialized;

		ILocationProvider _locationProvider;
		ILocationProvider LocationProvider
		{
			get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}

				return _locationProvider;
			}
		}

		Vector3 _targetPosition;

		void Start()
		{
			LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
		}

		void LateUpdate()
		{
			if (_isInitialized)
			{
				var map = LocationProviderFactory.Instance.mapManager;
				transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);

				if (m_MapCameraFollowTarget.enabled)
					map.UpdateMap(_locationProvider.CurrentLocation.LatitudeLongitude, map.Zoom);
			}
		}
	}
}