using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cuemon.Threading;

namespace Cuemon.Data.Entity
{
    internal class OpenListWorkItem : SortedDoerWorkItem<int, Entity>
	{
		public OpenListWorkItem(int sortOrder, CountdownEvent synchronization) : base(sortOrder, synchronization)
		{
		}

		public override void ProcessWork()
		{
			EntityDataAdapter adapter = this.Data["adapter"] as EntityDataAdapter;
			Type entitiesEntityType = this.Data["entitiesEntityType"] as Type;
			object[] compoundPrimaryKey = this.Data["compoundPrimaryKey"] as object[];

            Entity resolvedEntity = BusinessEntityUtility.CreateDescendantOrSelfEntity<Entity>(entitiesEntityType, adapter, compoundPrimaryKey);
			this.Result = resolvedEntity;
		}
	}
}
