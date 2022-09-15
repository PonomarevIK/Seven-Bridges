using Seven_Bridges.Controls;
using System;
using System.Collections.Generic;

namespace Seven_Bridges
{
    class UndoStack
    {
        private int maxHistoryLength;
        private LinkedList<Action> ActionHistory;

        public UndoStack(int maxHistoryLength = 20)
        {
            this.maxHistoryLength = maxHistoryLength;
            ActionHistory = new LinkedList<Action>();
        }

        public void Push(Action action)
        {
            ActionHistory.AddLast(action);
            if (ActionHistory.Count > maxHistoryLength)
            {
                ActionHistory.RemoveFirst();
            }
        }

        public void Pop()
        {
            if (ActionHistory.Count > 0)
            {
                ActionHistory.Last.Value.Undo();
                ActionHistory.RemoveLast();
            }
        }
    }



    abstract class Action
    {
        public abstract void Undo();
    }


    class AddVertex_Action : Action
    {
        Vertex addedVertex;
        public AddVertex_Action(Vertex vertex)
        {
            addedVertex = vertex;
        }

        public override void Undo()
        {
            addedVertex.Delete();
        }
    }

    class AddEdge_Action : Action
    {
        Edge addedEdge;
        public AddEdge_Action(Edge edge)
        {
            addedEdge = edge;
        }

        public override void Undo()
        {
            addedEdge.Delete();
        }
    }
}
