using System;
using Seven_Bridges.Controls;
using System.Collections.Generic;
using System.Windows;

namespace Seven_Bridges
{
    /// <summary>Linked list of limited length designed to behave like a stack. Keeps last 'maxHistoryLength' actions performed on a GraphCanvas.</summary>
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
                ActionHistory.Last.Value.Undo();
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
        public abstract void Undo();  // Might be used in the future
    }


    class AddVertex_Action : Action
    {
        public readonly Vertex AddedVertex;

        public AddVertex_Action(Vertex vertex)
        {
            AddedVertex = vertex;
        }

        public override void Undo() { }
    }

    class AddEdge_Action : Action
    {
        public readonly Edge AddedEdge;

        public AddEdge_Action(Edge edge)
        {
            AddedEdge = edge;
        }

        public override void Undo() { }
    }

    class DeleteVertex_Action : Action
    {
        public readonly Vertex DeletedVertex;

        public DeleteVertex_Action(Vertex vertex)
        {
            DeletedVertex = vertex;
        }

        public override void Undo() { }
    }

    class DeleteEdge_Action : Action
    {
        public readonly Edge DeletedEdge;

        public DeleteEdge_Action(Edge edge)
        {
            DeletedEdge = edge;
        }

        public override void Undo() { }
    }

    class ChangeEdgeWeight_Action : Action
    {
        public readonly Edge Edge;
        public readonly float PreviousWeight;

        public ChangeEdgeWeight_Action(Edge edge, float prevWeight)
        {
            Edge = edge;
            PreviousWeight = prevWeight;
        }

        public override void Undo() { }
    }

    class ChangeVertexName_Action : Action
    {
        public readonly Vertex Vertex;
        public readonly string PreviousName;

        public ChangeVertexName_Action(Vertex vertex, string previousName)
        {
            Vertex = vertex;
            PreviousName = previousName;
        }

        public override void Undo() { }
    }
}
