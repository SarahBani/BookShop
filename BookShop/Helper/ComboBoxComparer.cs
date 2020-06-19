using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;

namespace BookShop.Helper
{
    /// <summary>
    /// for comparing ComboBox controls based on their text
    /// </summary>
    public class ComboBoxComparer<T> : BaseCustomComparer where T : IComparable
    {

        #region Constructors

        public ComboBoxComparer(ListSortDirection direction = ListSortDirection.Ascending)
            : base(direction)
        {
        }

        #endregion /Constructors

        #region Methods

        public override int Compare(object x, object y)
        {
            T xValue = (T)Convert.ChangeType((x as ComboBox).Text, typeof(T));
            T yValue = (T)Convert.ChangeType((y as ComboBox).Text, typeof(T));
            return base.DirectionCompare * xValue.CompareTo(yValue);
        }

        #endregion /Methods

    }
}
