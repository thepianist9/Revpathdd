/// Code modified from https://github.com/sinbad/UnityRecyclingListView

using System;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    /// <summary>
    /// You should subclass this to provide fast access to any data you need to populate
    /// this item on demand.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class RecyclingListViewItem : MonoBehaviour {

        private RecyclingListView parentList;
        public RecyclingListView ParentList {
            get => parentList;
        }

        private int currentRow = -1;
        public int CurrentRow {
            get => currentRow;
        }

        private RectTransform rectTransform;
        public RectTransform RectTransform {
            get {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        private AspectRatioFitter aspectRatioFitter;
        public AspectRatioFitter AspectRatioFitter {
            get {
                if (aspectRatioFitter == null)
                    aspectRatioFitter = GetComponent<AspectRatioFitter>();
                return aspectRatioFitter;
            }
        }

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        public void NotifyCurrentAssignment(RecyclingListView v, int row) {
            parentList = v;
            currentRow = row;
        }   
    }
}
