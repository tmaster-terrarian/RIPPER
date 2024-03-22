using System;
using UnityEngine;

namespace bsc
{
    public class Ticker
    {
        public float startValue = 0;
        public float target = 0;
        public float rate = 1;
        public Action onFinishAction;

        public float value = 0;

        public bool isDone {
            get {
                return value == target;
            }
        }

        public Ticker(float startValue, float endValue, float rate, Action onFinishAction)
        {
            this.startValue = startValue;
            this.target = endValue;
            this.rate = rate;
            this.onFinishAction = onFinishAction;
        }
        public Ticker(float startValue, Action onFinishAction) => new Ticker(startValue, 0, 1, onFinishAction);
        public Ticker(float startValue) => new Ticker(startValue, 0, 1, () => {});
        public Ticker() => new Ticker(0, 0, 1, () => {});

        public void Update(float deltaTime)
        {
            if(value != target)
            {
                value = Utils.Approach(value, target, rate * deltaTime * Application.targetFrameRate);
                if(value == target)
                {
                    onFinishAction();
                }
            }
        }
        public void Update() => Update(Time.fixedDeltaTime);

        public void Reset()
        {
            this.value = this.startValue;
        }

        public void Reconfigure(float newStartValue, Action onFinishAction)
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
