using BookShop.Models;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BookShop.Commands;

namespace BookShop.ViewModels
{
    public class MainViewModel
    {

        #region Properties & Fields

        private ExtendedDataTable _booksDataTable;
        public ExtendedDataTable BooksDataTable
        {
            get
            {
                return this._booksDataTable;
            }
            set
            {
                this._booksDataTable = value;
            }
        }

        private readonly RelayCommand _selectFileCommand;
        public ICommand SelectFileCommand => this._selectFileCommand;

        private readonly RelayCommand _removeUnavailableCommand;
        public ICommand RemoveUnavailableCommand => this._removeUnavailableCommand;

        /// <summary>
        /// an ItemsSource for comboboxes
        /// </summary>
        private IEnumerable _comboBoxItemsSource;

        /// <summary>
        /// contains all prices in DataGrid price Column
        /// </summary>
        private IList<decimal> _priceList;

        /// <summary>
        /// a base color for the color of price cells 
        /// </summary>
        private Color _priceColor = Color.FromRgb(179, 201, 229);

        #endregion /Properties & Fields

        #region Constants

        private const string _error = "Error";
        private const string _fileFilter = "CSV files (*.csv)|*.csv";
        private const string _invalidFileFormat = "There csv file format is not valid!";

        #endregion /Constants

        #region Constructors

        public MainViewModel()
        {
            FillComboBoxItemsSource();
            this._selectFileCommand = new RelayCommand(OnSelectFile);
            this._removeUnavailableCommand = new RelayCommand(OnRemoveUnavailable);
            SetDataGrid();
        }

        #endregion /Constructors

        #region Methods

        /// <summary>
        /// Set an ItemsSource of BookBinding values to use it in comboboxes
        /// </summary>
        private void FillComboBoxItemsSource()
        {
            this._comboBoxItemsSource = Enum.GetValues(typeof(BookBinding)).Cast<object>()
                .Select(e => new
                {
                    Value = (int)e,
                    DisplayName = e.ToString()
                });
        }

        /// <summary>
        /// Set columns in DataGrid based on Book type properties & attributes
        /// </summary>
        private void SetDataGrid()
        {
            this._booksDataTable = new ExtendedDataTable();
            Type type = typeof(Book);
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var dataColumn = GetDataColumn(prop);
                this._booksDataTable.Columns.Add(dataColumn);
            }
            // var dataColumn2 = new DataColumn("PriceSort", typeof(decimal));
            // dataColumn2.Caption = "sdfsdf";
            //this._booksDataTable.Columns.Add(dataColumn2);            
        }

        /// <summary>
        /// Get a DataColumn for each property based on the property type & attribute
        /// </summary>
        /// <param name="prop">a property in the class</param>
        /// <returns></returns>
        private ExtendedDataColumn GetDataColumn(PropertyInfo prop)
        {
            ColumnType columnType = GetColumnType(prop);
            Type dataType = GetColumnDataType(columnType);
            var dataColumn = new ExtendedDataColumn(prop.Name, prop.PropertyType, dataType, columnType);
            dataColumn.Caption = GetPropertyDisplayName(prop);

            return dataColumn;
        }

        /// <summary>
        /// Get the column header which we want to be displayed in DataGrid for each property
        /// </summary>
        /// <param name="prop">a property in the class</param>
        /// <returns>
        /// If the property has a Display attribute, return it
        /// else return property name
        /// </returns>
        private string GetPropertyDisplayName(PropertyInfo prop)
        {
            var displayNameAttr = prop.GetCustomAttributes(typeof(DisplayAttribute), true)
                                         .Cast<DisplayAttribute>().SingleOrDefault();
            return (displayNameAttr?.Name ?? prop.Name);
        }

        /// <summary>
        /// Get the property name based on its Display attribute
        /// it searches in the class and finds the property with the specific Display attribute
        /// </summary>
        /// <typeparam name="T">type of class which property belongs to</typeparam>
        /// <param name="displayName">the display Name for the column header related to a property</param>
        /// <returns>the name of the property with the specific Display attribute</returns>
        private string GetPropertyName<T>(string displayName)
        {
            PropertyInfo prop = typeof(T).GetProperties()
                   .Where(q => q.GetCustomAttributes(typeof(DisplayAttribute), true)
                                    .Cast<DisplayAttribute>().Any(q => q.Name.Equals(displayName)))
                   .FirstOrDefault();
            return (prop?.Name ?? displayName);
        }

        /// <summary>
        /// Get the ColumnType attribute of the specific property
        /// It defines which column type we want to have in the DataGrid for this property
        /// </summary>
        /// <param name="prop">a property in the class</param>
        /// <returns>the ColumnType attribute of the specific property</returns>
        private ColumnType GetColumnType(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttributes(typeof(ColumnAttribute), true)
                                .Cast<ColumnAttribute>().SingleOrDefault();
            return (attr?.Type ?? ColumnType.Text);
        }

        /// <summary>
        /// Get the type that should be used in DataGrid for each ColumnType
        /// </summary>
        /// <param name="columnType">the customized ColumnType</param>
        /// <returns>the type related to the specific ColumnType</returns>
        private Type GetColumnDataType(ColumnType columnType)
        {
            return columnType switch
            {
                ColumnType.Text => typeof(string),
                ColumnType.Label => typeof(Label),
                ColumnType.ComboBox => typeof(ComboBox),
                ColumnType.CheckBox => typeof(bool),
                ColumnType.Button => typeof(Button),
                _ => typeof(string)
            };
        }

        /// <summary>
        /// Show a file dialog to user for choosing a csv file
        /// read the content of the file & close it ASAP
        /// send the content to fill the DataTable  
        /// </summary>
        /// <param name="parameter">null</param>
        private void OnSelectFile(object parameter)
        {
            ClearDataTable();
            var dialog = new OpenFileDialog()
            {
                Filter = _fileFilter
            };
            bool? result = dialog.ShowDialog();
            if (result.Value) // If user has selected any file
            {
                string fileName = dialog.FileName;
                string fullFileContent = string.Empty;

                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    // transfer all file content quickly
                    // and free the resource ASAP
                    // processes on content will be done after that
                    fullFileContent = reader.ReadToEnd();
                }
                FillDataTable(fullFileContent);
            }
        }

        /// <summary>
        /// clear the DataTable rows
        /// </summary>
        private void ClearDataTable()
        {
            this._booksDataTable.Clear();
            this._priceList = new List<decimal>();
        }

        /// <summary>
        /// fill the DataTable with the string file content
        /// parse and validate it
        /// extract the data from it & insert it into DataTable
        /// </summary>
        /// <param name="fullFileContent">the string file content from a csv file</param>
        private void FillDataTable(string fullFileContent)
        {
            try
            {
                fullFileContent = fullFileContent.TrimEnd(Environment.NewLine.ToCharArray());  // remove last new line character
                string[] fileLines = fullFileContent.Split(Environment.NewLine);
                string[] columnNames = (fileLines.Length > 0 ? fileLines[0].Split(';') : null);
                if (columnNames == null || columnNames.Length != this._booksDataTable.Columns.Count)
                {
                    throw new FileFormatException();
                }
                for (int i = 1; i < fileLines.Length; i++) // all rows except for header
                {
                    AddDataRow(fileLines[i].Split(';'), columnNames);
                }
                PaintPriceColumns();
            }
            catch (FileFormatException)
            {
                ShowError(_invalidFileFormat);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Add a row to DataTable from a string array and based on the specific columnNames
        /// </summary>
        /// <param name="rowValues">the string values extracted from the csv file row</param>
        /// <param name="columnNames">the column names extracted from the csv file first row</param>
        private void AddDataRow(string[] rowValues, string[] columnNames)
        {
            ExtendedDataRow dataRow = this._booksDataTable.GetNewRow();
            Type type = typeof(Book);
            int columnIndex = 0;
            foreach (string columnName in columnNames)
            {
                string propName = GetPropertyName<Book>(columnName);
                if (string.IsNullOrEmpty(propName))
                {
                    throw new FileFormatException();
                }
                string columnValue = rowValues[columnIndex++];
                PropertyInfo prop = type.GetProperty(propName);
                dataRow[propName] = GetColumnContent(prop, columnValue);
            }
            //dataRow["PriceSort"] = rowValues[2];
            SetDataRow(dataRow);
            this._booksDataTable.Rows.Add(dataRow);
        }

        /// <summary>
        /// Get the related content that we want to display in DataGrid cell for each property
        /// </summary>
        /// <param name="prop">a property in the class</param>
        /// <param name="columnValue">the value extracted from csv file</param>
        /// <returns></returns>
        private object GetColumnContent(PropertyInfo prop, string columnValue)
        {
            Type propType = prop.PropertyType;
            ColumnType columnType = GetColumnType(prop);

            return columnType switch
            {
                ColumnType.Text => Convert.ChangeType(columnValue, propType),
                ColumnType.Label => GetLabel(columnValue, propType),
                ColumnType.ComboBox => GetComboBox(columnValue),
                ColumnType.CheckBox => GetBoolean(columnValue),
                ColumnType.Button => GetButton(columnValue),
                _ => Convert.ChangeType(columnValue, propType),
            };
        }

        /// <summary>
        /// Get a Label with the specific content for displaying in DataGrid cell
        /// </summary>
        /// <param name="columnValue">the string value that should be displayed as content</param>
        /// <param name="propType">the type of the value</param>
        /// <returns>a Label</returns>
        private Label GetLabel(string columnValue, Type propType)
        {
            string propValue = Convert.ChangeType(columnValue.Replace(",", "."), propType).ToString();
            var label = new Label()
            {
                Content = propValue,
                Foreground = new SolidColorBrush(Colors.White),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9400D8")),
                HorizontalContentAlignment = HorizontalAlignment.Center
            };

            decimal price = decimal.Parse(propValue);
            this._priceList.Add(price);

            return label;
        }

        /// <summary>
        /// Get a ComboBox with default ItemsSource &
        /// the specific selected item for displaying in DataGrid cell
        /// </summary>
        /// <param name="columnValue">the selected item</param>
        /// <returns>a ComboBox</returns>
        private ComboBox GetComboBox(string columnValue)
        {
            var comboBox = new ComboBox()
            {
                ItemsSource = this._comboBoxItemsSource,
                DisplayMemberPath = "DisplayName",
                Text = columnValue
            };

            return comboBox;
        }

        /// <summary>
        /// Converts the specific string value to bool type for setting DataGrid cell checkbox  
        /// </summary>
        /// <param name="columnValue">the string value for boolean</param>
        /// <returns>a bool value related to the string value</returns>
        private bool GetBoolean(string columnValue)
        {
            return columnValue.ToLower() switch
            {
                "yes" => true,
                "no" => false,
                _ => false,
            };
        }

        /// <summary>
        /// Get a Button to display the specific selected value after clicking for displaying in DataGrid cell
        /// </summary>
        /// <param name="columnValue">the content we want to be displayed after clicking button</param>
        /// <returns>a Button</returns>
        private Button GetButton(string columnValue)
        {
            var button = new Button()
            {
                Content = "...",
                Tag = columnValue,
                FontWeight = FontWeights.Bold,
                Width = 30.0,
                Height = 25.0
            };
            button.Click += Button_Click;

            return button;
        }

        /// <summary>
        /// Display a MessageBox after clicking button        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            ShowMessage(button.Tag.ToString(), nameof(Book.Description));
        }

        /// <summary>
        /// Specify whether or not the dataRow should be displayed highlighted
        /// in xaml, the row background color is defined based on IsHighlighted property in dataRow
        /// rows with false InStock should be highlighted
        /// </summary>
        /// <param name="dataRow">the dataRow that should be highlighted or not</param>
        private void SetDataRow(ExtendedDataRow dataRow)
        {
            string inStockDisplayName = nameof(Book.InStock);
            dataRow.IsHighlighted = !bool.Parse(dataRow[inStockDisplayName].ToString());
        }

        /// <summary>
        /// for each prices that we have in DataGrid
        /// a color is specified based on its value in comparison with minimum & maximum prices
        /// </summary>
        private void PaintPriceColumns()
        {
            if (this._priceList.Count == 0)
            {
                return;
            }
            decimal minPrice = this._priceList.Min();
            decimal maxPrice = this._priceList.Max();
            decimal MaxPricesDiff = (maxPrice - minPrice);

            string priceColumnName = nameof(Book.Price);
            var rows = this._booksDataTable.Rows;

            for (int i = 0; i < this._priceList.Count; i++)
            {
                decimal price = this._priceList[i];
                byte newGreen = (byte)(this._priceColor.G - ((price - minPrice) / MaxPricesDiff * this._priceColor.G));
                Color color = Color.FromRgb(this._priceColor.R, newGreen, this._priceColor.B);

                DataRow dataRow = rows[i] as DataRow;
                (dataRow[priceColumnName] as Label).Background = new SolidColorBrush(color);
            }
        }

        /// <summary>
        /// Remove rows from DataGrid which are not available
        /// </summary>
        /// <param name="parameter">null</param>
        private void OnRemoveUnavailable(object parameter)
        {
            string inStockPropertyName = nameof(Book.InStock);
            int inStockColumnIndex = this._booksDataTable.Columns.IndexOf(inStockPropertyName);

            var rows = this._booksDataTable.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow dataRow = rows[i] as DataRow;
                if (dataRow != null)
                {
                    if (!bool.Parse(dataRow[inStockColumnIndex].ToString()))
                    {
                        dataRow.Delete();
                    }
                }
            }
            this._booksDataTable.AcceptChanges();
        }

        /// <summary>
        /// Display a simple message
        /// </summary>
        /// <param name="message">message text</param>
        /// <param name="caption">caption text</param>
        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }

        /// <summary>
        /// Display an error message
        /// </summary>
        /// <param name="message">error message text</param>
        public void ShowError(string message)
        {
            MessageBox.Show(message, _error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion /Methods

    }
}
