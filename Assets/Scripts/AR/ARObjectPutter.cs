using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TurkeySpinner.AR
{
    public class ARObjectPutter : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        ARPlaneManager arPlaneManager;

        void Start()
        {

            arPlaneManager.PlanesAddedAsObservable()
                .SelectMany(x => x)
                .Take(1)
                .Subscribe(Put)
                .AddTo(this);
        }

        void Put(ARPlane plane)
        {
            Instantiate(prefab, plane.center, Quaternion.identity);
        }
    }

    public static class ARPlaneManagerExtensions
    {
        public static IObservable<List<ARPlane>> PlanesAddedAsObservable(this ARPlaneManager arPlaneManager)
        {
            return arPlaneManager.PlanesChangedAsObservable()
                .Select(x => x.added)
                .Where(x => x != null && x.Any());
        }

        public static IObservable<ARPlanesChangedEventArgs> PlanesChangedAsObservable(this ARPlaneManager arPlaneManager)
        {
            return Observable.FromEvent<ARPlanesChangedEventArgs>(
                h => arPlaneManager.planesChanged += h,
                h => arPlaneManager.planesChanged -= h);
        }
    }
}
