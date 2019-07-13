using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurkeySpinner.Turkey
{
    public class SwipeHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        readonly ISubject<Unit> onPointerDown = new Subject<Unit>();
        readonly ISubject<Unit> onPointerUp = new Subject<Unit>();

        [SerializeField]
        float swipeSqrThreshold;

        public IObservable<float> OnSwipeAsObservable()
        {
            return onPointerDown.Select(_ => MousePositionAsObservable())
                .Switch()
                .Pairwise((latest, current) => current - latest)
                .Select(vector3 => vector3.sqrMagnitude < swipeSqrThreshold ? Vector3.zero : vector3)
                .Select(vector3 => vector3.x)
                .TakeUntil(onPointerUp)
                .Repeat();
        }

        IObservable<Vector3> MousePositionAsObservable()
        {
            return this.UpdateAsObservable()
                .Select(_ => Input.mousePosition);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.OnNext(Unit.Default);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp.OnNext(Unit.Default);
        }
    }
}
