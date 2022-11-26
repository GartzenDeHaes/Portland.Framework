using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.Cinematic
{
	public class Shot
	{
		private List<IScreenDirection> _lines = new List<IScreenDirection>();


		/// <summary>
		/// SAY, STRING [.]
		/// STOP [.]
		/// WAIT NUM [.]
		/// LOOKAT TAG_OR_NAME [.] 
		/// GOTO TAG_OR_NAME [.]
		/// [.|e]
		/// </summary>
		public static Shot ParseScriptItems(string linesTxt)
		{
			Shot shot = new Shot();

			string[] lines = linesTxt.Split(new char[] { '.' });

			for (int x = 0; x < lines.Length; x++)
			{
				shot._lines.Add(ParseItem(lines[x]));
			}

			return shot;
		}

		private static IScreenDirection ParseItem(string line)
		{
			StringPart strCmd = new StringPart(line);
			StringPart strRem;

			int spacePos = strCmd.IndexOf(' ');
			if (spacePos < 0)
			{
				// WORD.
				if (strCmd.EndsWith('.'))
				{
					strCmd.Stop--;
				}

				strRem = new StringPart(String.Empty);
			}
			else
			{
				strRem = strCmd.Substring(spacePos + 1);
				strCmd.Stop = spacePos - 1;

				if (strCmd.EndsWith(',') || strCmd.EndsWith('.'))
				{
					strCmd.Stop--;
				}
			}

			int cmdHash = strCmd.GetHashCode();

			switch (cmdHash)
			{
				case -566243780:	// SAY
					break;
				case 980794899:	// WAIT
					break;
				case 1474212194:	// LOOKAT
					break;
				case 1302397397:  // GOTO
					break;
				case -2019834226: // STOP
					break;
				case 985304380:   // DEBUG
					break;
				default:
					break;
			}

			return null;
		}
	}
}
