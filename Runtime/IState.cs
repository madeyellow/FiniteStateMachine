using UnityEngine.Events;

namespace MadeYellow.FSM
{
    public interface IState
    {
        IState Parent { get; }

        IState CurrentChild { get; }

        float ExecutionDuration { get; }

        void CheckTransitions();

        void Execute(in float deltaTime);

        void EnterState();

        void ExitState();

        void ChangeCurrentChild(IState state);

        UnityEvent OnEnteredState { get; }

        UnityEvent OnExitedState { get; }
    }
}