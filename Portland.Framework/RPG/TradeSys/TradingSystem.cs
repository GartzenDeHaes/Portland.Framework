using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG.Economics
{
	// Order book

	//public enum RouteSurfaceType
	//{
	//	None = 0,
	//	Path = 1,
	//	DirtRoad = 2,
	//	Road = 3,
	//	Rail = 4,
	//}

	//public enum LandType
	//{
	//	Water = 0,
	//	Plains = 1,
	//	Hills = 2,
	//	Mountains = 3,
	//	Swamp = 4,
	//}

	//public struct GridCoord
	//{
	//	public int x; 
	//	public int y;
	//}

	//public struct GridCell
	//{
	//	public GridCoord Coord;
	//	public LandType LandType;
	//}

	//public enum AgentState
	//{
	//	Idle = 0,
	//	InRoute = 1,
	//}

	//public struct Agent
	//{
	//	public int AgentId;
	//	public AgentState CurrentState;
	//	public int CurrentCityId;
	//	public int CurrentRouteId;
	//	public int StateRemaingTime;
	//	public ItemCollection Inventory;
	//}

	//public struct City
	//{
	//	public int CityId;
	//	public string Name;
	//}

	//public struct Route
	//{
	//	public int RouteId;
	//	public int SrcCityId;
	//	public int DestCityId;
	//	public int Distance;							// Walking distance in time steps
	//	public RouteSurfaceType SurfaceType;
	//	public float WeightFactor;					// Reduce speed by weight, such as uphill
	//}
}
