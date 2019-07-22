using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TurkeySpinner.AR
{
    public class ARObjectPutter : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        ARPlaneManager arPlaneManager;

        [SerializeField]
        ARRaycastManager arRaycastManager;

        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        async void Start()
        {
            var cancellation = this.GetCancellationTokenOnDestroy();
            var plane = await arPlaneManager.PlanesAddedAsObservable()
                .SelectMany(x => x)
                .Take(1)
                .ToUniTask(cancellation);

            var gameObject = Put(plane);

            TouchRaycastAndRelocationAsync(arRaycastManager, gameObject, cancellation).Forget();
        }

        GameObject Put(ARPlane plane)
        {
            return Instantiate(prefab, plane.center, Quaternion.identity);
        }

        static async UniTask TouchRaycastAndRelocationAsync(ARRaycastManager arRaycastManager, GameObject gameObject, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.Yield();
                cancellationToken.ThrowIfCancellationRequested();

                if (Input.touchCount != 2)
                    continue;
                var touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Moved)
                    continue;

                if (!arRaycastManager.Raycast(touch.position, hits, TrackableType.Planes)) continue;
                var pose = hits[0].pose;
                gameObject.transform.position = pose.position;
            }
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
