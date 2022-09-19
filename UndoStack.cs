using System;
using Seven_Bridges.Controls;
using System.Collections.Generic;

namespace Seven_Bridges
{
    /// <summary>Linked list of limited length that behaves like a stack. Keeps last 'maxHistoryLength' actions performed on a canvas.</summary>
    class UndoStack
    {
        private int maxHistoryLength;
        private LinkedList<Action> ActionHistory;

        public int Count => ActionHistory.Count;

        public UndoStack(int maxHistoryLength = 20)
        {
            this.maxHistoryLength = maxHistoryLength;
            ActionHistory = new LinkedList<Action>();
        }

        /// <summary>Push new action on top of the stack</summary>
        public void Push(Action action)
        {
            ActionHistory.AddLast(action);
            if (ActionHistory.Count > maxHistoryLength)
            {
                ActionHistory.RemoveFirst();
            }
        }

        /// <summary>Remove and return the last action from the stack</summary>
        public Action Pop()
        {
            if (ActionHistory.Count > 0)
            {
                //ActionHistory.Last.Value.Undo();
                Action lastAction = ActionHistory.Last.Value;
                ActionHistory.RemoveLast();
                return lastAction;
            }
            return null;
        }

        /// <summary>Return the last action without removing it</summary>
        public Action Peek()
        {
            return ActionHistory.Last?.Value;
        }
    }



    abstract class Action
    {
        public abstract void Undo();
    }


    class AddVertex_Action : Action
    {
        public readonly Vertex addedVertex;
        public AddVertex_Action(Vertex vertex)
        {
            addedVertex = vertex;
        }

        public override void Undo()
        {
            //addedVertex.Delete();
        }
    }

    class AddEdge_Action : Action
    {
        public readonly Edge addedEdge;
        public AddEdge_Action(Edge edge)
        {
            addedEdge = edge;
        }

        public override void Undo()
        {
            //addedEdge.Delete();
            var ss = new Stack<int>();
        }
    }
}
