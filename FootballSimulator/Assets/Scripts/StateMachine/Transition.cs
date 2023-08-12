public class Transition<T>
{
   public delegate bool condition ();
   public condition Tcondition;
   public bool boolType;
   public State<T> nextState;

   public Transition (condition _condition, bool isTrue, State<T> _nextState)
   {
      Tcondition = _condition;

      boolType = isTrue;

      nextState = _nextState;
   }
}
