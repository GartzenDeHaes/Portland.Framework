using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Framework.ComponentModel
{
	public interface IPropertyStringQuery
	{
		bool PropertyHasNamed(string propertyName);
		string PropertyAsString(string propertyName);
	}
}
