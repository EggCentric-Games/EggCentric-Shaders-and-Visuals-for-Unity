using System;
using System.Collections.Generic;
using UnityEngine;

namespace EggCentric.Sights
{
    public class Sight : MonoBehaviour
    {
        [SerializeField] protected MeshRenderer lensRenderer;

        private Magnifier _magnifier;
        private MagnificationConfig _magnificationConfig;

        public void SetConfig(MagnificationConfig magnificationConfig)
        {
            _magnificationConfig = magnificationConfig;
            _magnifier.SetConfig(magnificationConfig);
        }

        public void AdjustMagnification(float adjustment) => _magnifier.HandleInput(adjustment);

        protected virtual void SetMagnification(float magnification) => lensRenderer.material.SetFloat("_Magnification", magnification);

        protected virtual void OnEnable()
        {
            if (_magnifier == null)
                _magnifier = new Magnifier(_magnificationConfig);

            _magnifier.OnMagnificationChanged += UpdateMagnification;
        }

        protected virtual void OnDisable() => _magnifier.OnMagnificationChanged -= UpdateMagnification;

        private void UpdateMagnification() => SetMagnification(_magnifier.CurrentMagnification);
    }

    public class Magnifier
    {
        public float CurrentMagnification => _magnificationStrategy.CurrentMagnification;

        private MagnificationStrategy _magnificationStrategy;

        public event Action OnMagnificationChanged;

        public Magnifier(MagnificationConfig config = null)
        {
            SetConfig(config);
        }

        public void SetConfig(MagnificationConfig config)
        {
            if (config == null)
            {
                Debug.LogWarning($"Received invalid magnification config. Reverting");
                return;
            }

            if(_magnificationStrategy != null)
                _magnificationStrategy.OnMagnificationChanged -= OnMagnificationChanged;

            _magnificationStrategy = config.CreateStrategy();
            _magnificationStrategy.OnMagnificationChanged += OnMagnificationChanged;
        }

        public void HandleInput(float input)
        {
            _magnificationStrategy.HandleInput(input);
        }
    }

    public abstract class MagnificationStrategy
    {
        public float CurrentMagnification => currentMagnification;

        protected float currentMagnification;

        public event Action OnMagnificationChanged;

        public void HandleInput(float input)
        {
            float newValue = GetNewValue(input);
            UpdateValue(newValue);
        }

        protected void UpdateValue(float newValue)
        {
            if (currentMagnification == newValue)
                return;

            currentMagnification = newValue;
            OnMagnificationChanged?.Invoke();
        }

        protected abstract float GetNewValue(float input);
    }

    public class StaticMagnification : MagnificationStrategy
    {
        public StaticMagnification(float magnification = 1f)
        {
            currentMagnification = magnification;
        }

        public StaticMagnification(StaticMagnificationConfig config) : this(config.Magnification)
        {

        }

        protected override float GetNewValue(float input) => currentMagnification;
    }

    public class RangedMagnification : MagnificationStrategy
    {
        private float _minValue = 1f;
        private float _maxValue = 1f;

        public RangedMagnification(float minValue = 1f, float maxValue = 1f)
        {
            _minValue = minValue;
            _maxValue = maxValue;

            currentMagnification = minValue;
        }

        public RangedMagnification(Vector2 magnificationRange) : this(magnificationRange.x, magnificationRange.y)
        {

        }

        public RangedMagnification(RangedMagnificationConfig config) : this(config.MagnificationRange)
        {

        }

        protected override float GetNewValue(float input) => Mathf.Clamp(currentMagnification * (1 + input), _minValue, _maxValue);
    }

    public class StepMagnification : MagnificationStrategy
    {
        private IReadOnlyList<float> _magnifications;
        private int _currentMagnification;

        public StepMagnification(IReadOnlyList<float> magnifications)
        {
            if (magnifications == null || magnifications.Count <= 0)
            {
                Debug.LogError($"Available magnification count must be greater than 0!");
                magnifications = new float[] { 1f };
            }

            _magnifications = magnifications;

            currentMagnification = _magnifications[0];
        }

        public StepMagnification(StepMagnificationConfig config) : this(config.AvailableMagnifications)
        {
        }

        protected override float GetNewValue(float input)
        {
            int nextId = GetNextId(input);
            _currentMagnification = nextId;

            return _magnifications[nextId];
        }

        private int GetNextId(float input) => (_currentMagnification + Math.Sign(input) + _magnifications.Count) % _magnifications.Count;
    }
}