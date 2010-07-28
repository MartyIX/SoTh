using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using Sokoban.Lib;

namespace PluginOrdinary
{
	public class OrdinaryController : IGameVariant
	{
		private IPluginParent parent;
		private List<IGamePlugin> boxes = null;
		private IGamePlugin[,] aims = null;
		private IGamePlugin sokoban = null;

		public OrdinaryController(IPluginParent host)
		{
			parent = host;           
		}
	  
		public string Name
		{
			get { return "Ordinary"; }
		}

		public string Description
		{
			get { return "Controller for standard Sokoban game variant"; }
		}

		public string Author
		{
			get { return "Martin Vseticka"; }
		}

		public string Version
		{
			get { return "1.00"; }
		}

		public string CreatedForHostVersion
		{
			get { return "2.00"; }
		}


		public void Load(string appPath)
		{
			boxes = new List<IGamePlugin>();

			foreach (IGamePlugin gp in parent.AllPlugins)
			{
				if (gp.Name == "Box")
				{
					boxes.Add(gp);
				}

				if (gp.Name == "Sokoban")
				{
					sokoban = gp;
				}
			}

			aims = parent.GetFixedTiles();
		}

		public void Unload()
		{
			
		}

		public void CheckRound(long time)
		{
			
		}

		public void CheckRound(long time, string messageType, object data)
		{
			if (messageType == "BoxMoved")
			{
				bool isFinished = true;

				foreach (IGamePlugin b in boxes)
				{
					IGamePlugin aim = aims[b.PosX - 1, b.PosY - 1];

					if (aim == null)
					{
						isFinished = false;
						break;
					}
				}

				if (isFinished)
				{
					parent.MakePlan("Ordinary", time + ((IMovableElement)sokoban).Speed, null, EventType.gameWon);
				}
			}
		}

	}
}
