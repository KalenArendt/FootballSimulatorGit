using System.Collections.Generic;

public abstract class State<T>
{
   public List<Transition<T>> Transitions = new List<Transition<T>> { };

   public abstract void Enter (T entity);

   public abstract void Execute (T entity);

   public abstract void Exit (T entity);
}
