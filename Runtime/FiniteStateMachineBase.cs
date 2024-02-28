using UnityEngine;
using UnityEngine.Events;

namespace MadeYellow.FSM
{
    /// <summary>
    /// Abstract finite state machine (FSM) with basic state management.
    /// </summary>
    public abstract class FiniteStateMachineBase<TState> where TState : StateBase
    {
        [SerializeField]
        private FiniteStateMachineConfig _config;

        public TState CurrentState { get; private set; }
        public TState PreviousState { get; private set; }

        #region Events
        /// <summary>
        /// Triggers when <see cref="CurrentState"/> changes
        /// </summary>
        public readonly UnityEvent OnCurrentStateChanged;
        #endregion

        public FiniteStateMachineBase(FiniteStateMachineConfig config = null)
        {
            _config = config ?? new FiniteStateMachineConfig();

            OnCurrentStateChanged = new UnityEvent();
        }

        /// <summary>
        /// Switches <see cref="CurrentState"/> to a provided one
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(TState newState)
        {
            if (newState == null)
            {
                return; // TODO Replace with error
            }

            // Calculate if state will be changed or not
            var stateChanged = newState != CurrentState;

            // If state is changing OR it's allowed to call ExitState when changing to the same state
            if (stateChanged || _config.TriggerExitStateWhenEnteringSameState)
            {
                CurrentState?.ExitState();
            }

            // Cache current state as previous
            PreviousState = CurrentState;

            // Update current state
            CurrentState = newState;

            // If state is changing OR it's allowed to call EnterState when changing to the same state
            if (stateChanged || _config.TriggerEnterStateWhenEnteringSameState)
            {
                CurrentState?.EnterState();
            }

            // If state is changing OR it's allowed to trigger OnCurrentStateChanged when changing to the same state
            if (stateChanged || _config.TriggerCurrentStateChangedWhenEnteringSameState)
            {
                OnCurrentStateChanged.Invoke();
            }
        }

        /// <summary>
        /// Executes <see cref="CurrentState"/> of FSM (if it exists)
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Execute(in float deltaTime)
        {
            CurrentState?.CheckTransitions();

            CurrentState?.Execute(deltaTime);
        }

        [System.Serializable]
        public class FiniteStateMachineConfig
        {
            /// <summary>
            /// Determines if state's EnterState should be triggered if ChangeState don't changing CurrentState
            /// </summary>
            public bool TriggerEnterStateWhenEnteringSameState = false;

            /// <summary>
            /// Determines if state's ExitState should be triggered if ChangeState don't changing CurrentState
            /// </summary>
            public bool TriggerExitStateWhenEnteringSameState = false;

            /// <summary>
            /// Determines if FSM's OnCurrentStateChanged should be triggered if ChangeState don't changing CurrentState
            /// </summary>
            public bool TriggerCurrentStateChangedWhenEnteringSameState = false;
        }
    }
}
