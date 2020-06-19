using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace BookShop.Helper
{
    /// <summary>
    /// for comparing Label controls based on their content
    /// </summary>
    public class LabelComparer<T> : BaseCustomComparer where T : IComparable
    {

        #region Constructors

        public LabelComparer(ListSortDirection direction = ListSortDirection.Ascending)
            : base(direction)
        {
        }

        #endregion /Constructors

        #region Methods

        public override int Compare(object x, object y)
        {
            T xValue = (T)Convert.ChangeType((x as Label).Content.ToString(), typeof(T));
            T yValue = (T)Convert.ChangeType((y as Label).Content.ToString(), typeof(T));
            return base.DirectionCompare * xValue.CompareTo(yValue);
        }

        #endregion /Methods

    }
}
