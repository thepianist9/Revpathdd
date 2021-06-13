using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HistocachingII
{
    public class CompassObject : MonoBehaviour
    {
        public TMP_Text _tmpText;

        private Camera m_MainCamera;

		private StateManager SM;

		private int counter = 0, counter1 = 0;

		/// <summary>
		/// The rate at which the transform's rotation tries catch up to the provided heading.  
		/// </summary>
		[SerializeField]
		[Tooltip("The rate at which the transform's rotation tries catch up to the provided heading. ")]
		float _rotationFollowFactor = 1;

		/// <summary>
		/// <para>Set this to true if you'd like to adjust the sign of the rotation angle.</para>
		/// <para>eg angle passed in 63.5, angle that should be used for rotation: -63.5.</para>
		/// <para>This might be needed when rotating the map and not objects on the map.</para>
		/// </summary>
		[SerializeField]
		[Tooltip("Set this to true if you'd like to adjust the sign of the rotation angle. eg angle passed in 63.5, angle that should be used for rotation: -63.5.")]
		bool _useNegativeAngle;

		Quaternion _targetRotation;

		/// <summary>
		/// The location provider.
		/// This is public so you change which concrete <see cref="ILocationProvider"/> to use at runtime.  
		/// </summary>
		ILocationProvider _locationProvider;
		public ILocationProvider LocationProvider
		{
			private get
			{
				if (_locationProvider == null)
				{
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
				}
				return _locationProvider;
			}
			set
			{
				if (_locationProvider != null)
				{
					_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
				}
				_locationProvider = value;
				_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			}
		}

		Vector3 _targetPosition;

		void Awake()
		{
			SM = StateManager.Instance;
		}

		void Start()
		{
            m_MainCamera = Camera.main;

			LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}

		void OnDestroy()
		{
			if (LocationProvider != null)
			{
				LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			}
		}

		void LocationProvider_OnLocationUpdated(Location location)
		{
			float rotationAngle = location.DeviceOrientation;

			if (_useNegativeAngle) { rotationAngle *= -1f; }

			_tmpText.text = "Wawawawaw: " + ++counter1 + "\n"
				+ "m_MainCamera.transform.localEulerAngles.x: " + m_MainCamera.transform.localEulerAngles.x + "\n"
				+ "Input.compass.trueHeading: " + Input.compass.trueHeading + "\n"
				+ "location.DeviceOrientation: " + location.DeviceOrientation + "\n"
				+ "rotationAngle: " + rotationAngle + "\n";

			_tmpText.text += "state: " + SM.state + "\n";

			rotationAngle += m_MainCamera.transform.localEulerAngles.y;
			if (rotationAngle < 0) { rotationAngle += 360; }
			if (rotationAngle >= 360) { rotationAngle -= 360; }

			_targetRotation = Quaternion.Euler(getNewEulerAngles(rotationAngle));

			_tmpText.text += "m_MainCamera.transform.localEulerAngles.y: " + m_MainCamera.transform.localEulerAngles.y + "\n"
				+ "rotationAngle: " + rotationAngle + "\n"
				+ "_targetRotation: " + _targetRotation + "\n";
		}

		private Vector3 getNewEulerAngles(float newAngle)
		{
			var localRotation = transform.localRotation;
			var currentEuler = localRotation.eulerAngles;
			var euler = Mapbox.Unity.Constants.Math.Vector3Zero;

			euler.y = newAngle;
			euler.x = currentEuler.x;
			euler.z = currentEuler.z;

			return euler;
		}

		void Update()
		{
			Vector3 targetPosition = m_MainCamera.transform.position;
			targetPosition.y -= 1.8f;
			transform.position = targetPosition;

			transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotationFollowFactor);
		}
    }
}
