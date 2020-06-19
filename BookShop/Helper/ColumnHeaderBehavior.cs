using BookShop.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BookShop.Helper
{
    public class ColumnHeaderBehavior : Behavior<DataGrid>
    {

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.Sorting += OnSorting;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
            AssociatedObject.Sorting -= OnSorting;
        }

        /// <summary>
        /// Make propertly content for columns that consist controls
        /// </summary>
        /// <param name="sender">DataGrid as object</param>
        /// <param name="e">DataGridAutoGeneratingColumnEventArgs</param>
        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            ExtendedDataColumn column = GetColumn(sender, e.PropertyName);
            e.Column.Header = column.Caption;

            switch (column.ColumnType)
            {
                case ColumnType.Label:
                    SetContentColumn(e);
                    break;
                case ColumnType.ComboBox:
                    SetContentColumn(e);
                    break;
                case ColumnType.Button:
                    SetContentColumn(e);
                    break;
                case ColumnType.Text:
                case ColumnType.CheckBox:
                default:
                    break;
            }
        }

        /// <summary>
        /// Custom Sort for DataGrid
        /// Although the DataGrid has sorting capability for columns
        /// it didn't work correctly for columns containg controls 
        /// & I insisted to have sort perticularly based on Labels contents 
        /// so I added this event handler
        /// </summary>
        /// <param name="sender">DataGrid as object</param>
        /// <param name="e">DataGridSortingEventArgs</param>
        protected void OnSorting(object sender, DataGridSortingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null) return;
            ExtendedDataTable dataTable = (dataGrid.ItemsSource as DataView).Table as ExtendedDataTable;

            DataGridColumn column = e.Column;
            ExtendedDataColumn extendedDataColumn = dataTable.Columns[column.SortMemberPath] as ExtendedDataColumn;
            column.SortDirection = GetSortDirection(column);

            DataView dataView = dataGrid.ItemsSource as DataView;
            Type comparerType;
            BaseCustomComparer comparer;

            switch (extendedDataColumn.ColumnType)
            {
                case ColumnType.Label:
                    // sort by decimal values in content of the label      
                    dataView.Sort = string.Empty;
                    Type contentType = extendedDataColumn.ContentType;
                    comparerType = typeof(LabelComparer<>).MakeGenericType(contentType);
                    comparer = Activator.CreateInstance(comparerType, column.SortDirection.Value) as BaseCustomComparer;
                    dataTable.ApplySort(column.SortMemberPath, comparer);
                    break;
                case ColumnType.ComboBox:
                    dataView.Sort = string.Empty;
                    comparer = new ComboBoxComparer<string>(column.SortDirection.Value);
                    dataTable.ApplySort(column.SortMemberPath, comparer);
                    break;
                case ColumnType.Button:
                    // not allow to sort by button
                    break;
                case ColumnType.CheckBox:
                case ColumnType.Text:
                default:
                    dataView.Sort = GetSort(column);
                    break;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Extract the ExtendedDataColumn from the DataGrid Source by propertyName
        /// </summary>
        /// <param name="sender">DataGrid as object</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns></returns>
        private ExtendedDataColumn GetColumn(object sender, string propertyName)
        {
            var dataTable = ((sender as DataGrid).ItemsSource as DataView).Table;
            return dataTable.Columns[propertyName] as ExtendedDataColumn;
        }

        /// <summary>
        /// Set TemplateColumn for column which have control content and not simple text
        /// </summary>
        /// <param name="e">DataGridAutoGeneratingColumnEventArgs</param>
        private void SetContentColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            var template = new DataTemplate();
            var contentFactory = new FrameworkElementFactory(typeof(ContentControl));

            template.VisualTree = contentFactory;
            contentFactory.SetBinding(ContentControl.ContentProperty, new Binding(e.PropertyName));

            var templateColumn = new DataGridTemplateColumn()
            {
                Header = e.Column.Header,
                CellTemplate = template,
                SortMemberPath = e.Column.SortMemberPath
            };
            e.Column = templateColumn;
        }

        /// <summary>
        /// Specify the SortDirection for this column
        /// </summary>
        /// <param name="column">column of DataGrid</param>
        /// <returns>
        /// if DataGrid has been sorted by this column, reverse the previous sort direction 
        /// if DataGrid has not been sorted by this column, return Ascending
        /// </returns>
        private ListSortDirection GetSortDirection(DataGridColumn column)
        {
            return column.SortDirection switch
            {
                ListSortDirection.Ascending => ListSortDirection.Descending,
                _ => ListSortDirection.Ascending
            };
        }

        /// <summary>
        /// Get simple Sort string based for specific column
        /// </summary>
        /// <param name="column">DataGridColumn</param>
        /// <returns>a sort in string format</returns>
        private string GetSort(DataGridColumn column) {
            string sort = column.SortDirection.Value == ListSortDirection.Ascending ? "ASC" : "DESC";
             return $"{column.SortMemberPath} {sort}";
        }

    }
}
