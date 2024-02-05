﻿using KNARZhelper;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MetadataUtilities
{
    /// <summary>
    ///     Interaction logic for MergeRuleEditorView.xaml
    /// </summary>
    public partial class MergeRuleEditorView : UserControl
    {
        public MergeRuleEditorView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during initializing Merge Rule Editor", true);
            }
        }

        private void ClearSearchBox(object sender, RoutedEventArgs e) => txtSearchBox.Clear();
    }
}