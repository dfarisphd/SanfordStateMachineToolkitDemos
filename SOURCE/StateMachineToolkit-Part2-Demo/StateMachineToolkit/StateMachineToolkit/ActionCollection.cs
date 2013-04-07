using System;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Summary description for ActionCollection.
	/// </summary>
    public class ActionCollection : ICollection
    {
        private ArrayList actions = new ArrayList();

        public ActionCollection()
        {
        }

        public void Add(ActionHandler action)
        {
            actions.Add(action);
        }

        public void Remove(ActionHandler action)
        {
            actions.Remove(action);
        }

        #region ICollection Members

        public bool IsSynchronized
        {
            get
            {                
                return false;
            }
        }

        public int Count
        {
            get
            {
                return actions.Count;
            }
        }

        public void CopyTo(Array array, int index)
        {
            actions.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return actions.GetEnumerator();
        }

        #endregion
    }
}
