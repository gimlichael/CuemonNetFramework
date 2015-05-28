using System;

namespace Cuemon.Threading
{
    internal class FactorySortedWorkItem<TKey, TResult> : SortedDoerWorkItem<TKey, TResult>
    {
        internal FactorySortedWorkItem(TKey sortOrder, ISynchronization sync) : base(sortOrder, sync)
        {
        }

        public override void ProcessWork()
        {
            throw new NotImplementedException();
        }
    }
}