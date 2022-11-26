using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.ComponentModel;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.AI.Semantics
{
	public class SemanticEvent : IDynamicPropertyCollection<string, string>
	{
		public string Frame;		
		public string Goal;		
		public string Act;
		public string AgentTag;	
		public string AgentName; 
		public string DoTag;
		public string DoName;
		public string IoTag;
		public string IoName;
		public string LocationTag;
		public string LocationName;

		public string GetProperty(string name, string value)
		{
			switch (name)
			{
				case "Frame": return Frame;
				case "Goal": return Goal;
				case "Act": return Act;
				case "AgentTag": return AgentTag;
				case "AgentName": return AgentName;
				case "DoTag": return DoTag;
				case "DoName": return DoName;
				case "IoTag": return IoTag;
				case "IoName": return IoName;
				case "LocationTag": return LocationTag;
				case "LocationName": return LocationName;
			}
			throw new ArgumentException("GetProperty unknown key " + name + " value " + value);
		}

		public void SetProperty(string name, string value)
		{
			switch (name)
			{
				case "Frame": Frame = value; break;
				case "Goal": Goal = value; break;
				case "Act": Act = value; break;
				case "AgentTag": AgentTag = value; break;
				case "AgentName": AgentName = value; break;
				case "DoTag": DoTag = value; break;
				case "DoName": DoName = value; break;
				case "IoTag": IoTag = value; break;
				case "IoName": IoName = value; break;
				case "LocationTag": LocationTag = value; break;
				case "LocationName": LocationName = value; break;
				default:
					throw new ArgumentException("SetProperty unknown key " + name + " value " + value);
			}
		}

		public void Reset()
		{
			Frame = String.Empty;
			Goal = String.Empty;
			Act = String.Empty;
			AgentTag = String.Empty;
			AgentName = String.Empty;
			DoTag = String.Empty;
			DoName = String.Empty;
			IoTag = String.Empty;
			IoName = String.Empty;
			LocationTag = String.Empty;
			LocationName = String.Empty;
		}
	}
}
