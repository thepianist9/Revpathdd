using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HistocachingII
{
	public class QuadTreeCameraMovement : MonoBehaviour
	{
		[SerializeField]
		[Range(1, 20)]
		public float _panSpeed = 1.0f;

		[SerializeField]
		float _zoomSpeed = 0.25f;

		[SerializeField]
		public Camera _referenceCamera;

		public ScreenManager m_ScreenManager;

		[SerializeField]
		AbstractMap _mapManager;

		ILocationProvider _locationProvider;

		[SerializeField]
		bool _useDegreeMethod;

		private Vector3 _origin;
		private Vector3 _mousePosition;
		private Vector3 _mousePositionPrevious;
		private bool _shouldDrag;
		private bool _isInitialized = false;
		private Plane _groundPlane = new Plane(Vector3.up, 0);
		private bool _dragStartedOnUI = false;
		private StateManager SM;

		private bool m_IsTouchReset = true;

		void Awake()
		{
			SM = StateManager.Instance;

			if (null == _referenceCamera)
			{
				_referenceCamera = GetComponent<Camera>();
				if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
			}
			_mapManager.OnInitialized += () =>
			{
				_isInitialized = true;
			};
		}

		void Start()
		{
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
		}

		public void Update()
		{
			if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
			{
				_dragStartedOnUI = true;
			}

			if (Input.GetMouseButtonUp(0))
			{
				_dragStartedOnUI = false;
			}

			if (m_ScreenManager.IsStickyMyLocation())
			{
				float interpolation = 2f * Time.deltaTime;

				Vector2d startLatLong = _mapManager.CenterLatitudeLongitude;
				Vector2d endLatlong = _locationProvider.CurrentLocation.LatitudeLongitude;
				Vector3 startPosition = _mapManager.GeoToWorldPosition(startLatLong, false);
				Vector3 endPosition = _mapManager.GeoToWorldPosition(endLatlong, false);

				Vector3 interpolatedPosition = startPosition;
				interpolatedPosition.x = Mathf.Lerp(startPosition.x, endPosition.x, interpolation);
				interpolatedPosition.y = Mathf.Lerp(startPosition.y, endPosition.y, interpolation);
				interpolatedPosition.z = Mathf.Lerp(startPosition.z, endPosition.z, interpolation);

				var interpolatedLatLong = _mapManager.WorldToGeoPosition(interpolatedPosition);

				_mapManager.UpdateMap(interpolatedLatLong, _mapManager.Zoom);
			}
		}

		private void LateUpdate()
		{
			if (!_isInitialized) { return; }

			if (SM.state == State.Map)
			{
				if (!_dragStartedOnUI)
				{
					if (Input.touchSupported && Input.touchCount > 0)
						HandleTouch();
					else
						m_IsTouchReset = true;
				}
			}
		}

		void HandleTouch()
		{
			float zoomFactor = 0.0f;
			//pinch to zoom.
			switch (Input.touchCount)
			{
				case 1:
					if (m_IsTouchReset)
						PanMapUsingTouch();
					break;
				case 2:
					{
						m_IsTouchReset = false;

						// Store both touches.
						Touch touchZero = Input.GetTouch(0);
						Touch touchOne = Input.GetTouch(1);

						// Find the position in the previous frame of each touch.
						Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
						Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

						// Find the magnitude of the vector (the distance) between the touches in each frame.
						float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
						float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

						// Find the difference in the distances between each frame.
						zoomFactor = 0.01f * (touchDeltaMag - prevTouchDeltaMag);
					}
					ZoomMapUsingTouch(zoomFactor);
					break;
				default:
					break;
			}
		}

		void ZoomMapUsingTouch(float zoomFactor)
		{
			var zoom = Mathf.Max(0.0f, Mathf.Min(_mapManager.Zoom + zoomFactor * _zoomSpeed, 21.0f));
			if (Math.Abs(zoom - _mapManager.Zoom) > 0.0f)
			{
				_mapManager.UpdateMap(_mapManager.CenterLatitudeLongitude, zoom);
			}
		}

		void PanMapUsingTouch()
		{
			if (Input.GetMouseButtonUp(1))
			{
				var mousePosScreen = Input.mousePosition;
				//assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
				//http://answers.unity3d.com/answers/599100/view.html
				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				var pos = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				var latlongDelta = _mapManager.WorldToGeoPosition(pos);
				Debug.Log("Latitude: " + latlongDelta.x + " Longitude: " + latlongDelta.y);
			}

			if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				var mousePosScreen = Input.mousePosition;
				//assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
				//http://answers.unity3d.com/answers/599100/view.html
				mousePosScreen.z = _referenceCamera.transform.localPosition.y;
				_mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);

				if (_shouldDrag == false)
				{
					_shouldDrag = true;
					_origin = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
				}
			}
			else
			{
				_shouldDrag = false;
			}

			if (_shouldDrag == true)
			{
				m_ScreenManager.SetStickyMyLocation(false);

				var changeFromPreviousPosition = _mousePositionPrevious - _mousePosition;
				if (Mathf.Abs(changeFromPreviousPosition.x) > 0.0f || Mathf.Abs(changeFromPreviousPosition.y) > 0.0f)
				{
					_mousePositionPrevious = _mousePosition;
					var offset = _origin - _mousePosition;

					if (Mathf.Abs(offset.x) > 0.0f || Mathf.Abs(offset.z) > 0.0f)
					{
						if (null != _mapManager)
						{
							float factor = _panSpeed * Conversions.GetTileScaleInMeters((float)0, _mapManager.AbsoluteZoom) / _mapManager.UnityTileSize;
							var latlongDelta = Conversions.MetersToLatLon(new Vector2d(offset.x * factor, offset.z * factor));
							var newLatLong = _mapManager.CenterLatitudeLongitude + latlongDelta;

							_mapManager.UpdateMap(newLatLong, _mapManager.Zoom);
						}
					}
					_origin = _mousePosition;
				}
				else
				{
					if (EventSystem.current.IsPointerOverGameObject())
					{
						return;
					}
					_mousePositionPrevious = _mousePosition;
					_origin = _mousePosition;
				}
			}
		}
	}
}