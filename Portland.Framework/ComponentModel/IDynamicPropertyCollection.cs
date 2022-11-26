using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.ComponentModel
{
	public interface IDynamicPropertyCollection<K, V>
	{
		void SetProperty(K name, V value);
		V GetProperty(K name, V value);
	}
}
