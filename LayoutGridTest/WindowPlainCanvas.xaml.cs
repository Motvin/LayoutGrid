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
	/// Interaction logic for WindowPlainCanvas.xaml
	/// </summary>
	public partial class WindowPlainCanvas : Window
	{
		public WindowPlainCanvas(Panel grid)
		{
			InitializeComponent();

			this.outerCanvas.Children.Add(grid);
		}
	}
}
