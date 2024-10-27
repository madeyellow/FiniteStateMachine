using UnityEngine.Events;

namespace MadeYellow.FSM
{
    /// <summary>
    /// Abstract state of <see cref="FiniteStateMachineBase"/>
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// Determines if this state is executing or not
        /// </summary>
        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Shows how long this state is executing since <see cref="OnEnteredState"/> event
        /// </summary>
        public float ExecutionDuration { get; private set; }

        #region События
        /// <summary>
        /// Triggers after <see cref="StateEnteringHook"/> when <see cref="EnterState"/> called
        /// </summary>
        public readonly UnityEvent OnEnteredState;

        /// <summary>
        /// Triggers after <see cref="StateExitingHook"/> when <see cref="ExitState"/> called
        /// </summary>
        public readonly UnityEvent OnExitedState;
        #endregion

        public StateBase()
        {
            OnEnteredState = new UnityEvent();
            OnExitedState = new UnityEvent();
        }

        /// <summary>
        /// Hook method.
        /// You may override it to check if this state should transit to another state
        /// </summary>
        public virtual void CheckTransitions() { }

        /// <summary>
        /// Exeution of the state's logic
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Execute(in float deltaTime)
        {
            if (!IsExecuting)
            {
                return;
            }

            

            ExecuteHandler(deltaTime);

            ExecutionDuration += deltaTime;
        }

        /// <summary>
        /// The state's logic
        /// </summary>
        /// <param name="deltaTime"></param>
        protected abstract void ExecuteHandler(in float deltaTime);

        /// <summary>
        /// This nethod is intended to be used only by the <see cref="FiniteStateMachineBase"/>.
        /// Don't invoke it manually!
        /// </summary>
        public void EnterState()
        {
            StateEnteringHook();

            ExecutionDuration = 0f;

            IsExecuting = true;

            OnEnteredState.Invoke();
        }

        /// <summary>
        /// This nethod is intended to be used only by the <see cref="FiniteStateMachineBase"/>.
        /// Don't invoke it manually!
        /// </summary>
        public void ExitState()
        {
            StateExitingHook();

            IsExecuting = false;

            OnExitedState.Invoke();
        }

        /// <summary>
        /// Hook method.
        /// You may override it to perform some actions at the begining of the <see cref="EnterState"/> before all meters will be reset.
        /// During the execution of this method <see cref="ExecutionDuration"/> isn't yet reset
        /// </summary>
        protected virtual void StateEnteringHook() { }

        /// <summary>
        /// Hook method.
        /// You may override it to perform some actions at the begining of the <see cref="ExitState"/>.
        /// </summary>
        protected virtual void StateExitingHook() { }
    }
}