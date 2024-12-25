using UnityEngine;

namespace ITKombat
{
    public class HitFelixStateMachine : MonoBehaviour
    {
        public string customName;

        private HitFelixState mainStateType;

        public HitFelixState CurrentState { get; private set; }
        private HitFelixState nextState;

        private void Awake()
        {
            if (mainStateType == null)
            {
                Debug.LogWarning("mainStateType is not initialized. Setting default state...");
                InitializeDefaultState(); // Inisialisasi default jika tidak diatur di Editor
            }

            if (mainStateType == null)
            {
                Debug.LogError("mainStateType is null! Cannot set initial state.");
                return;
            }

            // Initialize CurrentState
            SetState(mainStateType);
        }

        private void InitializeDefaultState()
        {
            if (customName == "Kombat")
            {
                mainStateType = new HitIdleCombatState();
            }
            else
            {
                Debug.LogError("Unknown customName: " + customName);
            }
        }

        void Update()
        {
            if (nextState != null)
            {
                SetState(nextState);
            }

            if (CurrentState != null)
                CurrentState.OnUpdate();
        }

        public void SetState(HitFelixState _newState)
        {
            nextState = null;
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }
            CurrentState = _newState;

            if (CurrentState != null)
            {
                CurrentState.OnEnter(this);
            }
            else
            {
                Debug.LogError("Failed to set new state. CurrentState is null!");
            }
        }

        public void SetNextState(HitFelixState _newState)
        {
            if (_newState != null)
            {
                nextState = _newState;
            }
        }

        private void LateUpdate()
        {
            if (CurrentState != null)
                CurrentState.OnLateUpdate();
        }

        private void FixedUpdate()
        {
            if (CurrentState != null)
                CurrentState.OnFixedUpdate();
        }

        public void SetNextStateToMain()
        {
            nextState = mainStateType;
        }

        private void OnValidate()
        {
            if (mainStateType == null)
            {
                InitializeDefaultState();
            }
        }
    }
}