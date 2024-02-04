﻿using KNARZhelper;
using System;
using System.Windows.Controls;

namespace MetadataUtilities.Views
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MergeDialogView : UserControl
    {
        public MergeDialogView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during initializing Metadata Editor", true);
            }
        }
    }
}