using System;
using UnityEngine;

namespace bsc
{
    public class Counter
    {
        public float startValue = 0;
        public float target = 0;
        public float rate = 1;
        public Action onFinishAction;

        public float value = 0;

        public bool IsDone => value == target;

        public Counter(float startValue, float endValue, float rate, Action onFinishAction)
        {
            this.startValue = startValue;
            this.target = endValue;
            this.rate = rate;
            this.onFinishAction = onFinishAction;
        }
        public Counter(float startValue, Action onFinishAction) => new Counter(startValue, 0, 1, onFinishAction);
        public Counter(float startValue) => new Counter(startValue, 0, 1, () => {});
        public Counter() => new Counter(0, 0, 1, () => {});

        public void Update(float deltaTime)
        {
            if(value != target)
            {
                value = Utils.Approach(value, target, rate * deltaTime * Application.targetFrameRate);
                if(value == target)
                {
                    onFinishAction?.Invoke();
                }
            }
        }

        public void Update() => Update(Time.fixedDeltaTime);

        public void Reset()
        {
            this.value = this.startValue;
        }

        public void Reconfigure(float newStartValue, Action onFinishAction = null)
        {
            this.startValue = newStartValue;
            this.onFinishAction = onFinishAction;
        }
        public void Reconfigure(float newStartValue) => Reconfigure(newStartValue, this.onFinishAction);

        public override string ToString()
        {
            return $"{value}";
        }
    }
}
