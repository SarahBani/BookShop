using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BookShop.Helper
{
    public abstract class BaseCustomComparer : IComparer
    {

        #region Properties & Fields

        protected ListSortDirection _direction;

        protected int DirectionCompare => (this._direction == ListSortDirection.Ascending ? 1 : -1);

        #endregion /Properties & Fields

        #region Constructors

        public BaseCustomComparer(ListSortDirection direction)
        {
            this._direction = direction;
        }

        #endregion /Constructors

        #region Methods

        public abstract int Compare(object x, object y);

        #endregion /Methods

    }
}
