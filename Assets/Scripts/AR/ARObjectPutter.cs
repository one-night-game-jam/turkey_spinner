using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
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
        ARRaycastManager raycastManager;

        void Start()
        {
            var hit = this.UpdateAsObservable()
                .Select(_ => raycastManager.RaycastHitAsObservable(CameraCenterRay(Camera.main)))
                .Switch()
                .Select(hits => hits.First().pose.position)
                .Publish();
            hit.Take(1)
                .Select(Spawn)
                .Select(g => hit.Select(x => (gameObject: g, position: x)))
                .Switch()
                .TakeUntil(this.ObserveEveryValueChanged(_ => Input.GetMouseButton(0)).Where(b => b))
                .Subscribe(t => t.gameObject.transform.position = t.position)
                .AddTo(this);
            hit.Connect();
        }

        static Ray CameraCenterRay(Camera camera)
        {
            return camera.ScreenPointToRay(new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0));
        }

        GameObject Spawn(Vector3 position)
        {
            return Instantiate(prefab, position, Quaternion.identity);
        }
    }

    public static class ARRaycastManagerExtensions
    {
        static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
        public static IObservable<List<ARRaycastHit>> RaycastHitAsObservable(this ARRaycastManager raycastManager, Ray ray)
        {
            return Observable.Create<List<ARRaycastHit>>(observer =>
                {
                    if (raycastManager.Raycast(ray, hits, TrackableType.PlaneWithinInfinity))
                    {
                        observer.OnNext(hits);
                    }
                    return Disposable.Empty;
                });
        }
    }
}
