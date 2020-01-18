using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LayoutGridTest
{
	/// <summary>
	/// Interaction logic for WindowPlain.xaml
	/// </summary>
	public partial class WindowPlain : Window
	{
		public WindowPlain(Panel grid, bool useInfinitWidth = false, bool useInfiniteHeight = false)
		{
			InitializeComponent();

			// the default col and row are 1* when none are specified,
			// but we can make these auto, which will cause MeasureOverride for the children to get called with Infinity for width and/or height

			if (useInfinitWidth)
			{
				ColumnDefinition col = new ColumnDefinition();
				col.Width = new GridLength(0, GridUnitType.Auto);

				outerGrid.ColumnDefinitions.Add(col);
			}

			if (useInfiniteHeight)
			{
				RowDefinition row = new RowDefinition();
				row.Height = new GridLength(0, GridUnitType.Auto);

				outerGrid.RowDefinitions.Add(row);
			}

			outerGrid.Children.Add(grid);
		}
	}
}
