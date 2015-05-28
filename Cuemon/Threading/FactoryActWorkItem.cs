using System;

namespace Cuemon.Threading
{
    internal class FactoryActWorkItem : ActWorkItem
    {
        internal FactoryActWorkItem(ISynchronization sync) : base(sync)
        {
        }

        public override void ProcessWork()
        {
            throw new NotImplementedException();
        }
    }
}