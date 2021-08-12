/// Code modified from https://github.com/sinbad/UnityRecyclingListView

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    /// <summary>
    /// RecyclingListView uses a Unity UI ScrollRect to provide an efficiently scrolling list.
    /// The key feature is that it only allocates just enough child items needed for the
    /// visible part of the view, and recycles them as the list is scrolled, saving memory
    /// and layout cost.
    ///
    /// There are limitations:
    ///   * Child items must be a fixed height
    ///   * Only one type of child item is supported
    ///   * Only vertical scrolling is virtualised. Horizontal scrolling is still supported but
    ///     there is no support for grid view style layouts 
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class RecyclingListView : MonoBehaviour
    {
        [Tooltip("Prefab for all the header view objects in the list")]
        public RecyclingListViewItem HeaderPrefab;
        [Tooltip("Prefab for all the row view objects in the list")]
        public RecyclingListViewItem ChildPrefab;
        [Tooltip("The amount of vertical padding to add between items")]
        public float RowPadding = 30f;
        [Tooltip("Minimum height to pre-allocate list items for. Use to prevent allocations on resizing.")]
        public float PreAllocHeight = 0;

        /// <summary>
        /// Set the vertical normalized scroll position. 0 is bottom, 1 is top (as with ScrollRect) 
        /// </summary>
        public float VerticalNormalizedPosition {
            get => scrollRect.verticalNormalizedPosition;
            set => scrollRect.verticalNormalizedPosition = value;
        }

        protected List<float> cumulativeHeights = new List<float>();
        
        protected int headerCount;
        protected int rowCount;
        // /// <summary>
        // /// Get / set the number of rows in the list. If changed, will cause a rebuild of
        // /// the contents of the list. Call Refresh() instead to update contents without changing
        // /// length.
        // /// </summary>
        // public int RowCount {
        //     get => rowCount;
        //     set {
        //         if (rowCount != value) {
        //             rowCount = value;
        //             // avoid triggering double refresh due to scroll change from height change
        //             ignoreScrollChange = true;
        //             UpdateContentHeight();
        //             ignoreScrollChange = false;
        //             ReorganiseContent(true);
        //         }
        //     }
        // }

        public void SetCount(int headerCount, int rowCount)
        {
            this.headerCount = headerCount;
            this.rowCount = rowCount;

            cumulativeHeights.Clear();

            float height = 0;
            cumulativeHeights.Add(height);

            if (headerCount > 0)
            {
                for (int i = 1; i <= headerCount + rowCount; ++i)
                {
                    height += RowPadding;
                    height += IsHeaderCallback(i - 1) ? HeaderHeight() : RowHeight();
                    cumulativeHeights.Add(height);
                }
            }
            else
            {
                for (int i = 1; i <= rowCount; ++i)
                {
                    cumulativeHeights.Add(i * (RowPadding + RowHeight()));
                }
            }

            // avoid triggering double refresh due to scroll change from height change
            ignoreScrollChange = true;
            UpdateContentHeight();
            ignoreScrollChange = false;
            ReorganiseContent(true);
        }

        public delegate bool IsHeader(int index);

        public IsHeader IsHeaderCallback;

        /// <summary>
        /// Delegate which users should implement to populate their custom RecyclingListViewItem
        /// instances when they're needed by the list.
        /// </summary>
        /// <param name="item">The header item being populated. These are recycled as the list scrolls.</param>
        /// <param name="rowIndex">The overall row index of the list item being populated</param>
        public delegate void HeaderItemDelegate(RecyclingListViewItem item, int rowIndex);

        /// <summary>
        /// Delegate which users should implement to populate their custom RecyclingListViewItem
        /// instances when they're needed by the list.
        /// </summary>
        /// <param name="item">The child item being populated. These are recycled as the list scrolls.</param>
        /// <param name="rowIndex">The overall row index of the list item being populated</param>
        public delegate void ItemDelegate(RecyclingListViewItem item, int rowIndex);

        /// <summary>
        /// Set the delegate which will be called back to populate items. You must provide this at runtime.
        /// </summary>
        public ItemDelegate HeaderItemCallback;

        /// <summary>
        /// Set the delegate which will be called back to populate items. You must provide this at runtime.
        /// </summary>
        public ItemDelegate ItemCallback; 
        
        protected ScrollRect scrollRect;
        // circular buffer of header items which are reused
        protected RecyclingListViewItem[] headerItems;
        // circular buffer of child items which are reused
        protected RecyclingListViewItem[] childItems;
        // the current start index of the circular buffer
        protected int headerBufferStart = 0;
        // the current start index of the circular buffer
        protected int childBufferStart = 0;
        // the index into source data which childBufferStart refers to 
        protected int sourceDataRowStart; 

        protected bool ignoreScrollChange = false;
        protected float previousBuildHeight = 0;
        protected const int rowsAboveBelow = 1;
        
        protected virtual void Awake() {
            scrollRect = GetComponent<ScrollRect>();
        }
        
        protected virtual bool CheckChildItems() {
            float vpHeight = ViewportHeight();
            float buildHeight = Mathf.Max(vpHeight, PreAllocHeight);
            bool rebuild = childItems == null || buildHeight > previousBuildHeight;
            if (rebuild) {
                // create a fixed number of children, we'll re-use them when scrolling
                // figure out how many we need, round up
                int childCount = Mathf.RoundToInt(0.5f + buildHeight / RowHeight());
                childCount += rowsAboveBelow * 2; // X before, X after

                if (HeaderPrefab)
                {
                    if (headerItems == null) 
                        headerItems = new RecyclingListViewItem[childCount];
                    else if (childCount > headerItems.Length)
                        Array.Resize(ref headerItems, childCount);

                    for (int i = 0; i < headerItems.Length; ++i) {
                        if (headerItems[i] == null) {
                            headerItems[i] = Instantiate(HeaderPrefab);
                        }
                        headerItems[i].RectTransform.SetParent(scrollRect.content, false);
                        headerItems[i].gameObject.SetActive(false);
                    }
                }

                if (childItems == null) 
                    childItems = new RecyclingListViewItem[childCount];
                else if (childCount > childItems.Length)
                    Array.Resize(ref childItems, childCount);

                for (int i = 0; i < childItems.Length; ++i) {
                    if (childItems[i] == null) {
                        childItems[i] = Instantiate(ChildPrefab);
                    }
                    childItems[i].RectTransform.SetParent(scrollRect.content, false);
                    childItems[i].gameObject.SetActive(false);
                }

                previousBuildHeight = buildHeight;
            }

            return rebuild;
        }


        protected virtual void OnEnable() {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
            ignoreScrollChange = false;
        }

        protected virtual void OnDisable() {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }

        protected virtual void OnScrollChanged(Vector2 normalisedPos) {
            // This is called when scroll bar is moved *and* when viewport changes size
            if (!ignoreScrollChange) {
                ReorganiseContent(false);
            }
        }

        protected virtual void ReorganiseContent(bool clearContents) {

            if (clearContents) {
                scrollRect.StopMovement();
                scrollRect.verticalNormalizedPosition = 1; // 1 == top
            }
            
            bool childrenChanged = CheckChildItems();
            bool populateAll = childrenChanged || clearContents;
            
            // Figure out which is the first virtual slot visible
            float ymin = scrollRect.content.localPosition.y;

            // round down to find first visible
            // int firstVisibleIndex = (int)(ymin / RowHeight());

            int firstVisibleIndex = 0;

            for (int i = 0; i < cumulativeHeights.Count; ++i)
            {
                // TODO check whether need to use < || Mathf.Approximately
                if (ymin <= cumulativeHeights[i])
                {
                    firstVisibleIndex = i;
                    break;
                }
            }

            // we always want to start our buffer before
            // int newRowStart = firstVisibleIndex - rowsAboveBelow;
            int newRowStart = firstVisibleIndex - rowsAboveBelow;

            // If we've moved too far to be able to reuse anything, same as init case
            // int diff = newRowStart - sourceDataRowStart;
            // if (populateAll || Mathf.Abs(diff) >= childItems.Length) {
                
                sourceDataRowStart = newRowStart;
                headerBufferStart = 0;
                childBufferStart = 0;
                int headerIdx = newRowStart;
                foreach (var item in headerItems) {
                    UpdateHeader(item, headerIdx++);
                }
                int rowIdx = newRowStart;
                foreach (var item in childItems) {
                    UpdateChild(item, rowIdx++);
                }
            // } else if (diff != 0) {
            //     // we scrolled forwards or backwards within the tolerance that we can re-use some of what we have
            //     // Move our window so that we just re-use from back and place in front
            //     // children which were already there and contain correct data won't need changing
            //     int newBufferStart = (childBufferStart + diff) % childItems.Length;

            //     if (diff < 0) {
            //         // window moved backwards
            //         for (int i = 1; i <= -diff; ++i) {
            //             int bufi = WrapChildIndex(childBufferStart - i);
            //             int headerIdx = sourceDataRowStart - i;
            //             UpdateHeader(headerItems[bufi], headerIdx++);
            //             int rowIdx = sourceDataRowStart - i;
            //             UpdateChild(childItems[bufi], rowIdx);
            //         }
            //     } else {
            //         // window moved forwards
            //         int prevLastBufIdx = childBufferStart + childItems.Length - 1;
            //         int prevLastRowIdx = sourceDataRowStart + childItems.Length - 1;
            //         for (int i = 1; i <= diff; ++i) {
            //             int bufi = WrapChildIndex(prevLastBufIdx + i);
            //             int headerIdx = prevLastRowIdx + i;
            //             UpdateChild(headerItems[bufi], headerIdx);
            //             int rowIdx = prevLastRowIdx + i;
            //             UpdateChild(childItems[bufi], rowIdx);
            //         }
            //     }

            //     sourceDataRowStart = newRowStart;
            //     childBufferStart = newBufferStart;
            // }
            
        }

        // private int WrapChildIndex(int idx) {
        //     while (idx < 0)
        //         idx += childItems.Length;
            
        //     return idx % childItems.Length;
        // }

        private float HeaderHeight() {
            return HeaderPrefab ? (scrollRect.content.rect.width - HeaderPrefab.RectTransform.offsetMin.x + HeaderPrefab.RectTransform.offsetMax.x) / HeaderPrefab.AspectRatioFitter.aspectRatio : 0;
        }

        private float RowHeight() {
            // return RowPadding + ChildPrefab.RectTransform.rect.height;
            return RowPadding + (scrollRect.content.rect.width - ChildPrefab.RectTransform.offsetMin.x + ChildPrefab.RectTransform.offsetMax.x) / ChildPrefab.AspectRatioFitter.aspectRatio;
        }

        private float ViewportHeight() {
            return scrollRect.viewport.rect.height;
        }

        protected virtual void UpdateHeader(RecyclingListViewItem child, int rowIdx) {
            if (!IsHeaderCallback(rowIdx))
                return;                    

            if (rowIdx < 0 || rowIdx >= headerCount + rowCount) {
                // Out of range of data, can happen
                child.gameObject.SetActive(false);
            } else {
                if (HeaderItemCallback == null) {
                    Debug.Log("RecyclingListView is missing a HeaderItemCallback, cannot function", this);
                    return;
                }
                
                // Move to correct location
                var childRect = HeaderPrefab.RectTransform.rect;
                Vector2 pivot = HeaderPrefab.RectTransform.pivot;
                float ytoppos = cumulativeHeights[rowIdx];
                // float ypos = ytoppos + (1f - pivot.y) * childRect.height;
                float ypos = ytoppos + (1f - pivot.y) * HeaderHeight();
                // float xpos = 0 + pivot.x * childRect.width;
                float xpos = HeaderPrefab.RectTransform.offsetMin.x + pivot.x * childRect.width;
                child.RectTransform.anchoredPosition = new Vector2(xpos, -ypos);
                // child.NotifyCurrentAssignment(this, rowIdx);

                // Populate data
                HeaderItemCallback(child, rowIdx);

                child.NotifyCurrentAssignment(this, rowIdx);
                child.gameObject.SetActive(true);
            }
        }

        protected virtual void UpdateChild(RecyclingListViewItem child, int rowIdx) {
            if (IsHeaderCallback(rowIdx))
                return;

            if (rowIdx < 0 || rowIdx >= headerCount + rowCount) {
                // Out of range of data, can happen
                child.gameObject.SetActive(false);
            } else {
                if (ItemCallback == null) {
                    Debug.Log("RecyclingListView is missing an ItemCallback, cannot function", this);
                    return;
                }
                
                // Move to correct location
                var childRect = ChildPrefab.RectTransform.rect;
                Vector2 pivot = ChildPrefab.RectTransform.pivot;
                // float ytoppos = RowHeight() * rowIdx;
                float ytoppos = cumulativeHeights[rowIdx];
                // float ypos = ytoppos + (1f - pivot.y) * childRect.height;
                float ypos = ytoppos + (1f - pivot.y) * RowHeight();
                // float xpos = 0 + pivot.x * childRect.width;
                float xpos = ChildPrefab.RectTransform.offsetMin.x + pivot.x * childRect.width;
                child.RectTransform.anchoredPosition = new Vector2(xpos, -ypos);
                // child.NotifyCurrentAssignment(this, rowIdx);

                // Populate data
                ItemCallback(child, rowIdx);

                child.NotifyCurrentAssignment(this, rowIdx);
                child.gameObject.SetActive(true);
            }
        }
        
        protected virtual void UpdateContentHeight() {
            // float height = ChildPrefab.RectTransform.rect.height * rowCount + (rowCount-1) * RowPadding;

            float height = cumulativeHeights[headerCount + rowCount];

            // apparently 'sizeDelta' is the way to set w / h 
            var sz = scrollRect.content.sizeDelta;
            scrollRect.content.sizeDelta = new Vector2(sz.x, height);
        }

        public virtual void DisableAllChildren() {
            if (headerItems != null) {
                for (int i = 0; i < headerItems.Length; ++i) {
                    headerItems[i].gameObject.SetActive(false);
                }
            }
            if (childItems != null) {
                for (int i = 0; i < childItems.Length; ++i) {
                    childItems[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
