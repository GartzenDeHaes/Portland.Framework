using System;
using System.Collections.Generic;

namespace Portland.CheckedEvents
{
	public sealed class Notify : Command
	{
	}

	public sealed class Notify<T> : Command<T>
	{
	}

	public sealed class Notify<T1, T2> : Command<T1, T2>
	{
	}

	public sealed class Notify<T1, T2, T3> : Command<T1, T2, T3>
	{
	}

	public sealed class Notify<T1, T2, T3, T4> : Command<T1, T2, T3, T4>
	{
	}

	public sealed class Notify<T1, T2, T3, T4, T5> : Command<T1, T2, T3, T4, T5>
	{
	}

	public sealed class Notify<T1, T2, T3, T4, T5, T6> : Command<T1, T2, T3, T4, T5, T6>
	{
	}
}
