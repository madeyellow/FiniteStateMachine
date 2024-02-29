# What is it?

This package is a foundation that allows you to easily build an FSM in your Unity project using our building blocks. It features:

* FSM execution and state transition logic;
* Overrideable state enter/exit hook methods;
* State enter/exit UnityEvents;
* State execution duration; 

## How to install Finite State Machine in my Unity project?

Use the Unity Package Manager (in Unity’s top menu: **Window > Package Manager**), click the "+" icon, select **"Add package from git URL"** and type the URL of this repository:

```
https://github.com/madeyellow/FiniteStateMachine.git
```

![Installing package via PackageManager](https://unitology.ru/wp-content/webp-express/webp-images/uploads/image-5.png.webp)

## Getting started

Creating an FSM with use of this package is as simple as defining your state base, the states you need, and FSM itself. Like this:

```csharp
using MadeYellow.FSM;
using UnityEngine;
 
/// <summary>
/// Base for any state in our FSM, refrencing FSM
/// </summary>
public abstract class MyFiniteStateMachineStateBase : StateBase
{
    protected readonly MyFiniteStateMachine FiniteStateMachine;
 
    public MyFiniteStateMachineStateBase(MyFiniteStateMachine finiteStateMachine)
    {
        FiniteStateMachine = finiteStateMachine;
    }
}
 
/// <summary>
/// Example of some state
/// </summary>
public class MyState : MyFiniteStateMachineStateBase
{
    private float _time;
 
    public MyState(MyFiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
    {
    }
 
    protected override void ExecuteHandler(in float deltaTime)
    {
        // This is logic of your state. You may implement anything here.
        _time += deltaTime;
 
        Debug.Log($"{_time} has passed");
    }
}

/// <summary>
/// Your FSM
/// </summary>
public class MyFiniteStateMachine : FiniteStateMachineBase<MyFiniteStateMachineStateBase>
{
    public readonly MyState SomeState;
    public readonly MyState SomeOtherState;

    public MyFiniteStateMachine()
    {
        SomeState = new MyState(this);
        SomeOtherState = new MyState(this);

        ChangeState(SomeState);
    }
}
```

States define execution logic (e.g. how a character should move, fall, swim, etc.), and FSM gives you the ability to translate between those states. You may use your FSM in some MonoBehaviour like this:

```csharp
public class BasicCharacter : MonoBehaviour
{
    private MyFiniteStateMachine _fsm;

    private void Awake()
    {
        _fsm = new MyFiniteStateMachine();
    }

    private void FixedUpdate()
    {
        _fsm.Execute(Time.fixedDeltaTime);
    }
}
```

### Translating from one state to another

If you want to translate from state **SomeState** to state **SomeOtherState** you should override the *CheckTransitions()* method in the **MyState** class and use the *FiniteStateMachine.ChangeState()* method like this:

```csharp
public override void CheckTransitions()
{
    // Some condition to transition from this state. CheckTransitions() of CurrentState executes before ExecuteHandler() each time you call Execute() in your FSM
    if (_time > 1)
    {
        // A way to change state of FSM. FSM will use ExecuteHandler of SomeOtherState instead of this state
        FiniteStateMachine.ChangeState(FiniteStateMachine.SomeOtherState);
    }
}
```

### Current & previous states of FSM

If you want to know which state your FSM is now or was in before you may use the *CurrentState* and the *PreviousState* properties of your FSM:

```csharp
public class BasicCharacter : MonoBehaviour
{
    private MyFiniteStateMachine _fsm;

    private void SomeMethod()
    {
        var currentState = _fsm.CurrentState;
        var previousState = _fsm.PreviousState;
    }
}
```

You may even use it inside your state to define various logic based on the previous state:

```csharp
public class MyState : MyFiniteStateMachineStateBase
{
    private float _time;
 
    public MyState(MyFiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
    {
    }
 
    protected override void ExecuteHandler(in float deltaTime)
    {
        // Execute only if previous state was SomeState
        if (FiniteStateMachine.PreviousState == FiniteStateMachine.SomeState)
        {
                _time += deltaTime;
        }

        Debug.Log($"{_time} has passed");
    }
}
```

### FSM state change event

Each time your FSM changes its state it invokes an *OnCurrentStateChanged* UnityEvent. This may be useful if you want to perform some action on a change of state for some reason.

```csharp
public class BasicCharacter : MonoBehaviour
{
    private MyFiniteStateMachine _fsm;

    private void Awake()
    {
        _fsm = new MyFiniteStateMachine();
        _fsm.OnCurrentStateChanged.AddListener(OnStateChanged); // Subscribes to change of state in _fsm
    }

    private void OnStateChanged()
    {
        // Some logic
    }
}
```

### Enter/Exit state hook methods

You can add custom logic to your state when FSM enters or exits certain states. This can be useful to fetch & cache some data from FSM (like fetching velocity of character to determine how much damage you should apply on fall damage, etc.) or reset something inside your state before first *ExecuteHandler()* will be invoked.

There are two methods inside your state for it:

```csharp
protected override void StateEnteringHook()
{
    Debug.Log($"MyState entereted"); // This will be called when FSM enter your state
}
 
protected override void StateExitingHook()
{
    Debug.Log($"MyState exited"); // This will be called when FSM exit your state
}
```

### State enter/exit events

If you want other components to execute some logic when your FSM enters or exists in some specific state (e.g. show a character's fall animation in the animator when entering "air" state, or showing attack animation when entering "melee attack" state, etc.) you may use the state's *OnEnteredState* and *OnExitedState* UnityEvents.

```csharp
public class BasicCharacterAnimator : MonoBehaviour
{
    private MyFiniteStateMachine _fsm;

    private void Awake()
    {
        _fsm.SomeState.OnEnteredState.AddListener(OnSomeStateEntered);
    }

    private void OnSomeStateEntered() {}
}
```

### State execution duration 

If, for some reason, you'll need to know how long a certain state has been executing since last entering it, you may use the state's *ExecutionDuration* property, which tells how many in-game seconds this state is actually executing.

```csharp
public class MyState : MyFiniteStateMachineStateBase
{
    public MyState(MyFiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
    {
    }
 
    protected override void ExecuteHandler(in float deltaTime)
    {
        Debug.Log($"State is executing for {ExecutionDuration} seconds");
    }
}
```
