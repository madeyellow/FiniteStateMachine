namespace MadeYellow.FSM
{
    public interface IState
    {
        IState Parent { get; }

        IState CurrentChild { get; }

        void CheckTransitions();

        void Execute(in float deltaTime);

        void EnterState();

        void ExitState();

        void ChangeCurrentChild(IState state);
    }
}