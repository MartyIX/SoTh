using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban.View;
using Sokoban.Model;
using System.Windows;

namespace Sokoban.Presenter
{
    public abstract class BasePresenter<TView, TRepository>
        where TView : IBaseView
        where TRepository : IBaseRepository
    {
        protected TView view;
        protected TRepository repository;
        public virtual void InitializeView(Window window) { }
    }
}
