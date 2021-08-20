// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace HistocachingII
// {
//     public class Data : MonoBehaviour
//     {
//         public List<POI> histocacheCollection = new List<POI>();

//         void Awake()
//         {
//             StartCoroutine(NetworkManager.GetPOICollection((POI[] histocacheCollection) =>
//             {
//                 for (int i = 0; i < histocacheCollection?.Length; ++i)
//                 {
//                     POI histocache = histocacheCollection[i];

//                     this.histocacheCollection.Add(histocache);
//                 }
//             }));
//         }
//     }
// }
