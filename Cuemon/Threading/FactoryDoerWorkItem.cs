using System;

namespace Cuemon.Threading
{
    internal class FactoryDoerWorkItem<TResult> : DoerWorkItem<TResult>
    {
        internal FactoryDoerWorkItem(ISynchronization sync) : base(sync)
        {
        }

        public override void ProcessWork()
        {
            throw new NotImplementedException();
        }
    }
}