// Implementation written by Tanner Holladay for CS 3500, September 2020.
// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    /// <summary>
    ///     (s1,t1) is an ordered pair of strings
    ///     t1 depends on s1; s1 must be evaluated before t1
    ///     A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    ///     (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    ///     Recall that sets never contain duplicates.  If an attempt is made to add an element to a
    ///     set, and the element is already in the set, the set remains unchanged.
    ///     Given a DependencyGraph DG:
    ///     (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///     (The set of things that depend on s)
    ///     (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///     (The set of things that s depends on)
    /// </summary>
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    /// dependents("a") = {"b", "c"}
    /// dependents("b") = {"d"}
    /// dependents("c") = {}
    /// dependents("d") = {"d"}
    /// dependees("a") = {}
    /// dependees("b") = {"a"}
    /// dependees("c") = {"a"}
    /// dependees("d") = {"b", "d"}
    public class DependencyGraph
    {
        /// <summary>
        ///     The dictionary that stores dependees as keys and the dependents inside
        /// </summary>
        private readonly Dictionary<string, HashSet<string>> _dependees;

        private readonly Dictionary<string, HashSet<string>> _dependents;

        /// <summary>
        ///     Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            _dependees = new Dictionary<string, HashSet<string>>();
            _dependents = new Dictionary<string, HashSet<string>>();
            Size = 0;
        }


        /// <summary>
        ///     The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size { get; private set; }


        /// <summary>
        ///     The size of dependees(s).
        ///     This property is an example of an indexer.  If dg is a DependencyGraph, you would
        ///     invoke it like this:
        ///     dg["a"]
        ///     It should return the size of dependees("a")
        /// </summary>
        public int this[string s] => _dependents.ContainsKey(s) ? _dependents[s].Count : 0;


        /// <summary>
        ///     Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return _dependees.ContainsKey(s);
        }


        /// <summary>
        ///     Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return _dependents.ContainsKey(s);
        }


        /// <summary>
        ///     Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            return _dependees.ContainsKey(s) ? _dependees[s] : new HashSet<string>();
        }

        /// <summary>
        ///     Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return _dependents.ContainsKey(s) ? _dependents[s] : new HashSet<string>();
        }


        /// <summary>
        ///     <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///     <para>This should be thought of as:</para>
        ///     t depends on s
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>
        /// ///
        public void AddDependency(string s, string t)
        {
            if (!_dependees.ContainsKey(s)) _dependees[s] = new HashSet<string>();
            if (!_dependees[s].Add(t)) return;

            if (!_dependents.ContainsKey(t)) _dependents[t] = new HashSet<string>();
            _dependents[t].Add(s);

            Size++;
        }


        /// <summary>
        ///     Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (!_dependees.ContainsKey(s) || !_dependents.ContainsKey(t)) return;

            if (_dependees[s].Count > 1) _dependees[s].Remove(t);
            else _dependees.Remove(s);

            if (_dependents[t].Count > 1) _dependents[t].Remove(s);
            else _dependents.Remove(t);

            Size--;
        }


        /// <summary>
        ///     Removes all existing ordered pairs of the form (s,r).  Then, for each
        ///     t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (_dependees.ContainsKey(s))
            {
                foreach (string r in new HashSet<string>(_dependees[s])) RemoveDependency(s, r);
                _dependees.Remove(s);
            }

            if (newDependents is null) return;

            foreach (string t in newDependents) AddDependency(s, t);
        }


        /// <summary>
        ///     Removes all existing ordered pairs of the form (r,s).  Then, for each
        ///     t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (_dependents.ContainsKey(s))
            {
                foreach (string r in new HashSet<string>(_dependents[s])) RemoveDependency(r, s);
                _dependents.Remove(s);
            }

            if (newDependees is null) return;

            foreach (string t in newDependees) AddDependency(t, s);
        }
    }
}