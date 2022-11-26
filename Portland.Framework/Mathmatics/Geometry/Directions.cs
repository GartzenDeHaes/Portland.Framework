using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Specifies directions along thee axes
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	[Flags]
   public enum Directions
   {
      None = 0,
      Left = 1,
      Right = 2,
      Down = 4,
      Up = 8,
      Back = 16,
      Forward = 32,
      XAxis = Left | Right,
      YAxis = Down | Up,
      ZAxis = Back | Forward,
      All = Left | Right | Down | Up | Back | Forward
   }

   public static class DirectionsExtensions
   {
      public static bool HasFlag(this Directions directions, Directions flag)
      {
         return (directions & flag) == flag;
      }

      public static Directions AddFlag(this Directions directions, Directions flag)
      {
         return directions | flag;
      }

      public static Directions RemoveFlag(this Directions directions, Directions flag)
      {
         return directions & ~flag;
      }
   }
}
