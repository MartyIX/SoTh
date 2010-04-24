using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.Model.PluginInterface;
using System.Windows.Input;

namespace PluginSokoban
{
    public class Sokoban : MovableEssentials, IGamePlugin, IMovable, IControllableByUserInput
    {

        public Sokoban(IPluginParent host) : base(host)
        {

        }

        private IPluginParent parent;

        #region IPlugin Members

        public string Name
        {
            get { return "Sokoban"; }
        }

        public string Description
        {
            get { return "Oficial Sokoban implementation"; }
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
            get { return "1.00"; }
        }

        public void Load()
        {
            
        }

        public void Unload()
        {
            
        }

        public IPluginParent Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        #endregion

        #region IControllableByUserInput Members

        public void OnKeyDown(Key key)
        {
            throw new NotImplementedException();
        }

        public void OnKeyUp(Key key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
