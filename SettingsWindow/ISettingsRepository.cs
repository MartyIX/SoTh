using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Sokoban.Model
{
    public interface ISettingsRepository : IBaseRepository
    {
        event PropertyChangedEventHandler PropertyChanged;

        void Save();

        object this[string propertyName]
        {
            get;
            set;
        }
    }
}
