using UserTCQ.Engine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UserTCQ.Engine.Types
{
    public class Component
    {
        public GameObject gameObject;

        public bool active = true;

        public virtual void Start() { }
        public virtual void Update() 
        {
            UpdateCo();
        }
        public virtual void LateUpdate() { }
        public virtual void OnDestroy() { }

        List<IEnumerator> routines = new List<IEnumerator>();

        public IEnumerator StartCoroutine(IEnumerator routine)
        {
            routines.Add(routine);
            return routine;
        }

        public void StopCoroutine(IEnumerator routine)
        {
            routines.Remove(routine);
        }

        public IEnumerator WaitForSeconds(float time)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        public IEnumerator WaitForSecondsUnscaled(float time)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTimeUnscaled;
                yield return null;
            }
        }

        public IEnumerator WaitUntil(Func<bool> condition)
        {
            while (condition() != true)
            {
                yield return null;
            }
        }

        public void UpdateCo()
        {
            for (int i = 0; i < routines.Count; i++)
            {
                if (routines[i].Current is IEnumerator)
                    if (MoveNext((IEnumerator)routines[i].Current))
                        continue;
                if (!routines[i].MoveNext())
                    routines.Remove(routines[i]);
            }
        }

        bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator)
                if (MoveNext((IEnumerator)routine.Current))
                    return true;
            return routine.MoveNext();
        }

        public int Count
        {
            get { return routines.Count; }
        }

        public bool Running
        {
            get { return routines.Count > 0; }
        }
    }
}
