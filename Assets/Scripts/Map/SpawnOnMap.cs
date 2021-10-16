using Mapbox.Utils;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HistocachingII
{
    public class SpawnOnMap : MonoBehaviour
    {
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		Camera mapCamera;

		[SerializeField]
		GameObject _histocacheTemplate;

		[SerializeField]
		GameObject _viewpointTemplate;

		ILocationProvider _locationProvider;

        private Dictionary<string, Histocache> histocacheCollection = new Dictionary<string, Histocache>();

		[Geocode]
		private Dictionary<string, Vector2d> _histocacheLocations = new Dictionary<string, Vector2d>();

		[Geocode]
		private Dictionary<string, Vector2d> _viewpointLocations = new Dictionary<string, Vector2d>();

		private Dictionary<GameObject, string> _spawnedHistocaches = new Dictionary<GameObject, string>();

		private Dictionary<GameObject, string> _spawnedViewpoints = new Dictionary<GameObject, string>();

		private Vector3 defaultSpawnScale, selectedSpawnScale;

		private GameObject selectedGameObject = null;
		private string selectedId = null;

		private bool isInteractable;

		// public Text m_DetailText;

		public GameObject m_Detail;

		private RectTransform m_DetailRectTransform;

		public Button m_DetailBtn;
        public Text m_DetailBtnLabel;

        public Toggle m_LanguageToggle;

        public Documents documents;

		private void Awake()
		{
			// Prevent double initialization of the map. 
			_map.InitializeOnStart = false;
		}

		protected virtual IEnumerator Start()
		{
			defaultSpawnScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			selectedSpawnScale = 1.5f * defaultSpawnScale;

			yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

			m_DetailRectTransform = m_Detail.GetComponent<RectTransform>();
			m_DetailRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.85f * Screen.width);
		}

		void Destroy()
		{
			foreach (KeyValuePair<GameObject, string> kvp in _spawnedHistocaches)
			{
				Destroy(kvp.Key);
			}

			foreach (KeyValuePair<GameObject, string> kvp in _spawnedViewpoints)
			{
				Destroy(kvp.Key);
			}

			_spawnedHistocaches.Clear();
			_spawnedViewpoints.Clear();
		}

		void LocationProvider_OnLocationUpdated(Mapbox.Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_map.Initialize(location.LatitudeLongitude, _map.AbsoluteZoom);

			GetHistocacheCollection(() => SetMarkers());
		}

		void Update()
		{
			if (StateManager.Instance.state == State.Map)
			{
				// Bit shift the index of the layer (6) to get a bit mask
				// This would cast rays only against colliders in layer 6.
				int layerMask = 1 << 6;

				// We check if we have more than one touch happening.
				// We also check if the first touches phase is Ended (that the finger was lifted)
				if (Input.touchCount > 0)
				{
					Touch touch = Input.GetTouch(0);

					if (touch.phase == TouchPhase.Began)
					{
						isInteractable = !EventSystem.current.IsPointerOverGameObject(touch.fingerId);

						if (isInteractable)	UnsetSelected();
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						if (isInteractable)
						{
							Ray ray = mapCamera.ScreenPointToRay(touch.position);

							// We now raycast with this information. If we have hit something we can process it.
							RaycastHit hit;
							if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) && hit.collider != null)
							{
								// We should have hit something with a physics collider!
								GameObject touchedObject = hit.transform.gameObject;
								// touchedObject should be the object we touched.
								string id;
								if (_spawnedHistocaches.TryGetValue(touchedObject, out id) || _spawnedViewpoints.TryGetValue(touchedObject, out id))
								{
									SetSelected(touchedObject, id, mapCamera.WorldToScreenPoint(hit.transform.position));
								}
								else
								{
									UnsetSelected();
								}
							}
							else
							{
								UnsetSelected();
							}
						}
					}
				}
			}

			foreach (KeyValuePair<GameObject, string> kvp in _spawnedHistocaches)
			{
				kvp.Key.transform.localPosition = _map.GeoToWorldPosition(_histocacheLocations[kvp.Value], true);
				kvp.Key.transform.localScale = defaultSpawnScale;
			}

			foreach (KeyValuePair<GameObject, string> kvp in _spawnedViewpoints)
			{
				kvp.Key.transform.localPosition = _map.GeoToWorldPosition(_viewpointLocations[kvp.Value], true);
				kvp.Key.transform.localScale = defaultSpawnScale;
			}

			if (selectedGameObject != null)
			{
				selectedGameObject.transform.localScale = selectedSpawnScale;

				// m_DetailBtn.GetComponent<RectTransform>().anchoredPosition = selectedGameObject.transform.localPosition;
				// m_DetailText.transform.localPosition = _map.GeoToWorldPosition(_viewpointLocations[_spawnedViewpoints[selectedGameObject]], true);
			}
		}

		private void SetMarkers()
		{
			foreach (Histocache histocache in histocacheCollection.Values)
			{
				if (!histocache.has_histocache_location)
					continue;

				var marker = Instantiate(_histocacheTemplate);
				marker.transform.localPosition = _map.GeoToWorldPosition(_histocacheLocations[histocache._id], true);
				marker.transform.localScale = defaultSpawnScale;
				_spawnedHistocaches.Add(marker, histocache._id);

				if (!histocache.has_viewpoint_location)
					continue;

				marker = Instantiate(_viewpointTemplate);
				marker.transform.localPosition = _map.GeoToWorldPosition(_viewpointLocations[histocache._id], true);
				marker.transform.localScale = defaultSpawnScale;
				_spawnedViewpoints.Add(marker, histocache._id);
			}
		}

		private void GetHistocacheCollection(Action callback)
        {
			DataManager.Instance.GetHistocacheCollection((bool success, Histocache[] histocacheCollection) =>
			{
				if (success)
				{
					this.histocacheCollection.Clear();

					_histocacheLocations.Clear();
					_viewpointLocations.Clear();

					foreach (Histocache histocache in histocacheCollection)
					{
						this.histocacheCollection[histocache._id] = histocache;

						if (!histocache.has_histocache_location)
							continue;

						_histocacheLocations[histocache._id] = new Vector2d(histocache.lat, histocache.@long);

						if (!histocache.has_viewpoint_location)
							continue;

						_viewpointLocations[histocache._id] = new Vector2d(histocache.viewpoint_lat, histocache.viewpoint_long);
					}
				}

				callback();
			});
        }

        private void GetHistocache(string id, Action<Histocache> callback)
        {
			if (histocacheCollection.TryGetValue(id, out Histocache histocache))
			{
				if (string.IsNullOrWhiteSpace(histocache.image_url))			
				{
					Debug.Log("SpawnOnMap::GetHistocache " + id);

					DataManager.Instance.GetHistocache(id, (bool success, Histocache h) =>
					{
						Debug.Log("SpawnOnMap::GetHistocache receive " + success + " | " + h);

						if (success && h != null)
						{
							histocache.image_url = h.image_url;
							histocache.image_aspect_ratio = h.image_aspect_ratio;
							histocache.title_de = h.title_de;
							histocache.title_en = h.title_en;
							histocache.description_de = h.description_de;
							histocache.description_en = h.description_en;
							histocache.caption_de = h.caption_de;
							histocache.caption_en = h.caption_en;

							histocache.viewpoint_image_url = h.viewpoint_image_url;
							histocache.viewpoint_image_aspect_ratio = h.viewpoint_image_aspect_ratio;
							histocache.viewpoint_image_height = h.viewpoint_image_height;
							histocache.viewpoint_image_offset = h.viewpoint_image_offset;
                            histocache.viewpoint_image_vertical_offset = h.viewpoint_image_vertical_offset;

							histocache.is_displayed_on_table = h.is_displayed_on_table;

							histocache.add_info_url = h.add_info_url;

							histocache.documents = h.documents;

							callback(histocache);
						}
					});
				}
				else
				{
					callback(histocache);
				}
			}
        }

		private void SetSelected(GameObject selectedGameObject, string selectedId, Vector3 touchPosition)
		{
			if (selectedGameObject.Equals(this.selectedGameObject))
				return;

			this.selectedGameObject = selectedGameObject;

			if (selectedId.Equals(this.selectedId))
				return;

			this.selectedId = selectedId;

			GetHistocache(selectedId, (Histocache histocache) =>
			{
				SetDetailTitle(m_LanguageToggle.isOn ? histocache.title_en : histocache.title_de);

				m_DetailBtn.onClick.RemoveAllListeners();
				m_DetailBtn.onClick.AddListener(() => OnHistocache(histocache._id));

				Vector3 position = m_DetailRectTransform.anchoredPosition;
				position.x = touchPosition.x;
				position.y = touchPosition.y;
			
				m_DetailRectTransform.anchoredPosition = position;

				m_Detail.SetActive(true);
			});
		}

		public void UnsetSelected()
		{
			if (selectedGameObject == null && selectedId == null)
				return;
			
			selectedGameObject = null;
			selectedId = null;

			m_Detail.SetActive(false);
		}

		private void SetDetailTitle(string title)
        {
            string[] texts = title.Split('(');

			m_DetailBtnLabel.text = texts[0];
        }

        private void OnHistocache(string histocacheId)
        {
			documents.gameObject.SetActive(true);
            documents.Show(m_LanguageToggle.isOn ? 1 : 0, histocacheCollection[histocacheId]);
        }
    }
}
