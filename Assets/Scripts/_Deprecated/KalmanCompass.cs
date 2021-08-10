using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HistocachingII
{
	/************************************************
	 * Kalman filter, source code from Mapbox
	 * (deprecated version), originally for GPS
	 ***********************************************/

    public class KalmanCompass
    {
		private float _minAccuracy = 1;
		private float _qMetresPerSecond;
		private double _timeStampMilliseconds;
		private float _heading;
		private float _variance; // P matrix.  Negative means object uninitialised.  NB: units irrelevant, as long as same units used throughout


		public KalmanCompass(float Q_metres_per_second)
		{
			_qMetresPerSecond = Q_metres_per_second;
			_variance = -1;
		}


		public double TimeStamp { get { return _timeStampMilliseconds; } }

		public float Heading { get { return _heading; } }

		public float Accuracy { get { return (float)Math.Sqrt(_variance); } }



		public void SetState(float heading, float accuracy, double TimeStamp_milliseconds)
		{
			_heading = heading;
			_variance = accuracy * accuracy;
			_timeStampMilliseconds = TimeStamp_milliseconds;
		}



		/// <summary>
		/// Kalman filter processing for heading
		/// </summary>
		/// <param name="heading_measurement">new measurement of heading</param>
		/// <param name="accuracy">measurement of 1 standard deviation error in metres</param>
		/// <param name="TimeStamp_milliseconds">time of measurement</param>
		/// <returns>new state</returns>
		public void Process(float heading_measurement, float accuracy, double TimeStamp_milliseconds)
		{
			if (accuracy < _minAccuracy)
			{
				accuracy = _minAccuracy;
			}

			if (_variance < 0)
			{
				// if variance < 0, object is unitialised, so initialise with current values
				_timeStampMilliseconds = TimeStamp_milliseconds;
				_heading = heading_measurement; _variance = accuracy * accuracy;
			}
			else
			{
				// else apply Kalman filter methodology

				double TimeInc_milliseconds = TimeStamp_milliseconds - TimeStamp_milliseconds;
				if (TimeInc_milliseconds > 0)
				{
					// time has moved on, so the uncertainty in the current position increases
					_variance += (float) TimeInc_milliseconds * _qMetresPerSecond * _qMetresPerSecond / 1000;
					_timeStampMilliseconds = TimeStamp_milliseconds;
					// TO DO: USE VELOCITY INFORMATION HERE TO GET A BETTER ESTIMATE OF CURRENT POSITION
				}

				// Kalman gain matrix K = Covarariance * Inverse(Covariance + MeasurementVariance)
				// NB: because K is dimensionless, it doesn't matter that variance has different units to lat and lng
				float K = _variance / (_variance + accuracy * accuracy);
				// apply K
				_heading += K * (heading_measurement - _heading);
				// new Covarariance  matrix is (IdentityMatrix - K) * Covarariance 
				_variance = (1 - K) * _variance;
			}
		}
    }
}
