using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.CheckedEvents
{
	public class Marshallable
	{
		public static Func<bool> RequiresMarshalling = () => false;
		public static Action<Action> SendToMarshaller = (sub) => { sub(); };

		public Marshallable()
		{
		}
	}
}
