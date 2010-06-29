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
        NameValueCollection Settings { get; }
        void Save(string key, string value);
        event PropertyChangedEventHandler PropertyChanged;        
    }
}
