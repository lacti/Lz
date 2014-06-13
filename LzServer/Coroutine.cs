using System;
using System.Collections.Generic;
using System.Threading;

namespace LzServer
{
    internal class Coroutine
    {
        public delegate IEnumerable<int> LogicEntryDelegate();

        private readonly List<LogicEntry> _logicEntries = new List<LogicEntry>();
        private readonly List<LogicEntryDelegate> _newLogicEntries = new List<LogicEntryDelegate>();

        public void AddEntry(LogicEntryDelegate entry)
        {
            _newLogicEntries.Add(entry);
        }

        public void Start()
        {
            var thread = new Thread(EntryLoop) {IsBackground = true};
            thread.Start();
        }

        public void EntryLoop()
        {
            var prev = DateTime.Now;
            while (true)
            {
                var now = DateTime.Now;
                var delta = (now - prev).Milliseconds;

                foreach (var newOne in _newLogicEntries)
                {
                    var newEntry = new LogicEntry
                        {
                            Enumerator = newOne().GetEnumerator(),
                            SleepTime = 0
                        };
                    _logicEntries.Add(newEntry);
                }
                _newLogicEntries.Clear();

                var removals = new List<LogicEntry>();
                foreach (var each in _logicEntries)
                {
                    each.SleepTime -= delta;
                    if (each.SleepTime >= 0)
                        continue;

                    if (!each.Enumerator.MoveNext())
                        removals.Add(each);
                    else each.SleepTime = each.Enumerator.Current;
                }

                _logicEntries.RemoveAll(removals.Contains);

                prev = now;
                const int logicInterval = 16;
                Thread.Sleep(logicInterval);
            }
        }

        private class LogicEntry
        {
            public IEnumerator<int> Enumerator;
            public int SleepTime;
        }
    }
}