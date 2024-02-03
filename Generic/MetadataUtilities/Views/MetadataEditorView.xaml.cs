﻿using KNARZhelper;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MetadataUtilities
{
    /// <summary>
    ///     Interaction logic for MetadataEditor.xaml
    /// </summary>
    public partial class MetadataEditorView : UserControl
    {
        private readonly bool _isSelectMode;

        public MetadataEditorView(bool isSelectMode = false)
        {
            _isSelectMode = isSelectMode;

            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during initializing Metadata Editor", true);
            }
        }

        private void DataGridCell_Selected(object sender, RoutedEventArgs e)
        {
            if (_isSelectMode)
            {
                return;
            }

            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() != typeof(DataGridCell))
            {
                return;
            }

            // Starts the Edit on the row;
            DataGrid grd = (DataGrid)sender;
            grd.BeginEdit(e);
        }

        private void ClearSearchBox(object sender, RoutedEventArgs e) => txtSearchBox.Clear();
    }
}