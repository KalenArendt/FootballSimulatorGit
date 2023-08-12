public class StateMachine<T>
{
   private T Owner;
   private State<T> CurrentState;
   private State<T> PreviousState;
   private State<T> GlobalState;

   public void Awake ()
   {
      CurrentState = null;
      PreviousState = null;
      GlobalState = null;
   }

   public void Configure (T owner, State<T> InitialState)
   {
      Owner = owner;
      ChangeState(InitialState);
   }

   public void Update ()
   {
      if (GlobalState != null)
      {
         GlobalState.Execute(Owner);
      }

      if (CurrentState != null)
      {
         CurrentState.Execute(Owner);

         foreach (Transition<T> transition in CurrentState.Transitions)
         {
            if (transition.Tcondition.Invoke() == transition.boolType)
            {
               ChangeState(transition.nextState);
            }
         }
      }
   }

   public void ChangeState (State<T> NewState)
   {
      PreviousState = CurrentState;
      if (CurrentState != null)
      {
         CurrentState.Exit(Owner);
      }

      CurrentState = NewState;
      if (CurrentState != null)
      {
         CurrentState.Enter(Owner);
      }
   }

   public void RevertToPreviousState ()
   {
      if (PreviousState != null)
      {
         ChangeState(PreviousState);
      }
   }

   public State<T> GetCurrentState ()
   {
      return CurrentState;
   }
}
