using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TurkeySpinner.Turkey
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;

        [SerializeField]
        float initialTorque;

        [SerializeField]
        SwipeHandler swipeHandler;

        [SerializeField]
        float swipePower;

        void Start()
        {
            this.FixedUpdateAsObservable()
                .First()
                .Subscribe(_ => AddTorque(initialTorque))
                .AddTo(this);

            swipeHandler.OnSwipeAsObservable()
                .Select(x => x * swipePower)
                .ObserveOn(Scheduler.MainThreadFixedUpdate)
                .Subscribe(AddTorque)
                .AddTo(this);
        }

        void AddTorque(float y)
        {
            Debug.Log(y);
            rb.angularVelocity = Vector3.zero;
            rb.AddTorque(0, y, 0, ForceMode.Impulse);
        }
    }
}
