using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sokoban.Model.GameDesk
{
    public class OneRoundQuest : IQuest
    {
        private string name;
        private string actualRoundXML;

        public OneRoundQuest(string name, string roundDefinition)
        {
            this.actualRoundXML = roundDefinition;
            this.name = name;
        }

        #region IQuest Members

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string ActualRoundXML
        {
            get
            {
                return actualRoundXML;
            }
        }

        public void MoveCurrentToNext()
        {
        }

        public bool IsLast()
        {
            return true;
        }

        #endregion
    }
}
