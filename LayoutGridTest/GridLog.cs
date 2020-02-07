using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Motvin.LayoutGrid;
using SpreadsheetUtil;
using System.Diagnostics.CodeAnalysis;

namespace LayoutGridTest
{
	public static class GridLog
	{
		const string dblFmt = "###0.##";
		const string dblFmt6 = "###0.######";

		public class SortChildData : IComparer<ChildData>
		{
			public int Compare(ChildData x, ChildData y)
			{
				int cmp = x.col - y.col;

				if (cmp == 0)
				{
					cmp = x.row - y.row;
				}

				if (cmp == 0)
				{
					cmp = x.zIndex - y.zIndex;
				}

				if (cmp == 0)
				{
					cmp = x.childIndex - y.childIndex;
				}

				return cmp;
			}
		}

		public class ChildData
		{
			public int col;
			public int row;
			public int colSpan;
			public int rowSpan;
			public string type;
			public string name;
			public string content;
			public UIElement child;
			public int zIndex; // if zIndex = 0, then use child index to determine z order
			public int childIndex;
		}

		public static void SetupRandomGrid(Grid g, int seed, bool useInnerGrids)
		{
			Random rand = new Random(seed);

			int colCnt = rand.Next(1, 10);
			int rowCnt = rand.Next(1, 10);

			g.ColumnDefinitions.Clear();
			for (int i = 0; i < colCnt; i++)
			{
				ColumnDefinition c = new ColumnDefinition();

				int unitTypeInt = rand.Next(0, 8);

				if (unitTypeInt == 0)
				{
					GridLength len = new GridLength((double)rand.Next(0, 500), GridUnitType.Pixel);
					c.Width = len;
				}
				else if (unitTypeInt >= 7)
				{
					GridLength len = new GridLength(rand.Next(0, 100), GridUnitType.Star);
					c.Width = len;
				}
				else
				{
					GridLength len = new GridLength(rand.Next(0, 100), GridUnitType.Auto);
					c.Width = len;
				}

				if (rand.Next(0, 4) == 4)
				{
					c.MinWidth = rand.Next(0, 300);
				}

				if (rand.Next(0, 4) == 4)
				{
					c.MaxWidth = rand.Next(0, 800);
				}

				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			for (int i = 0; i < rowCnt; i++)
			{
				RowDefinition c = new RowDefinition();

				int unitTypeInt = rand.Next(0, 8);

				if (unitTypeInt == 0)
				{
					GridLength len = new GridLength((double)rand.Next(0, 500), GridUnitType.Pixel);
					c.Height = len;
				}
				else if (unitTypeInt >= 7)
				{
					GridLength len = new GridLength(rand.Next(0, 100), GridUnitType.Star);
					c.Height = len;
				}
				else
				{
					GridLength len = new GridLength(rand.Next(0, 100), GridUnitType.Auto);
					c.Height = len;
				}

				if (rand.Next(0, 4) == 4)
				{
					c.MinHeight = rand.Next(0, 300);
				}

				if (rand.Next(0, 4) == 4)
				{
					c.MaxHeight = rand.Next(0, 800);
				}

				g.RowDefinitions.Add(c);
			}

			int childCnt = rand.Next(0, 100);
			for (int i = 0; i < childCnt; i++)
			{
				int col = rand.Next(0, colCnt - 1);
				int row = rand.Next(0, rowCnt - 1);

				UIElement t;
				if (useInnerGrids)
				{
					if (rand.Next(0, 10) == 0)
					{
						Grid g2 = new Grid();
						SetupRandomGrid(g2, seed + 1, false);
						t = g2;
					}
					else
					{
						Button b = new Button();
						b.Name = "btn" + i.ToString("0");
						b.Content = "Button " + b.Name + new string('X', rand.Next(0, 10));

						t = b;
					}
				}
				else
				{
					Button b = new Button();
					b.Name = "btn" + i.ToString("0");
					b.Content = "Button " + b.Name + new string('X', rand.Next(0, 10));

					t = b;
				}

				int colSpan = 1;

				if (rand.Next(0, 10) == 0)
				{
					colSpan = rand.Next(2, 10);
				}

				int rowSpan = 1;

				if (rand.Next(0, 10) == 0)
				{
					rowSpan = rand.Next(2, 10);
				}

				Grid.SetColumn(t, col);
				Grid.SetRow(t, row);

				Grid.SetColumnSpan(t, colSpan);
				Grid.SetRowSpan(t, rowSpan);
				g.Children.Add(t);
			}
		}

		public static void SetupRandomGrid2(Grid g, int seed)
		{
			Random rand = new Random(seed);

			int colCnt = rand.NextInc(1, 5);
			int rowCnt = rand.NextInc(1, 5);

			g.ColumnDefinitions.Clear();
			for (int i = 0; i < colCnt; i++)
			{
				ColumnDefinition c = new ColumnDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				if (unitTypeInt == 0)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Width = len;
				}
				else if (unitTypeInt >= 7)
				{
					GridLength len = new GridLength(rand.NextInc(1, 100), GridUnitType.Star);
					c.Width = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Width = len;
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MinWidth = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MaxWidth = rand.NextInc(0, 300);
				}

				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			for (int i = 0; i < rowCnt; i++)
			{
				RowDefinition c = new RowDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				if (unitTypeInt == 0)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Height = len;
				}
				else if (unitTypeInt >= 7)
				{
					GridLength len = new GridLength(rand.NextInc(1, 100), GridUnitType.Star);
					c.Height = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Height = len;
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MinHeight = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MaxHeight = rand.NextInc(0, 300);
				}

				g.RowDefinitions.Add(c);
			}

			HashSet<long> colAndRowSet = new HashSet<long>();
			int childTryCnt = rand.NextInc(1, ((colCnt + 1) * rowCnt));
			int childCnt = 0;
			for (int i = 0; i < childTryCnt; i++)
			{
				int col = rand.NextInc(0, colCnt - 1);
				int row = rand.NextInc(0, rowCnt - 1);

				if (colAndRowSet.Add(CombineIntsIntoLong(col, row)) || rand.NextInc(1, 20) == 1)
				{
					int colSpan = 1;

					if (rand.NextInc(0, 20) == 0)
					{
						colSpan = rand.NextInc(2, 10);
						colSpan = Math.Min(colSpan, colCnt - col);
					}

					int rowSpan = 1;

					if (rand.NextInc(0, 20) == 0)
					{
						rowSpan = rand.NextInc(2, 10);
						rowSpan = Math.Min(rowSpan, rowCnt - row);
					}

					Button b = new Button();
					b.Name = "btn" + childCnt.ToString("0");
					b.Content = "Button " + b.Name + new string('X', rand.NextInc(0, 10));

					Grid.SetColumn(b, col);
					Grid.SetRow(b, row);

					Grid.SetColumnSpan(b, colSpan);
					Grid.SetRowSpan(b, rowSpan);
					g.Children.Add(b);
					childCnt++;
				}
			}
		}

		public static void SetupRandomGridStars(Grid g, int seed, bool allowZeroMax = true)
		{
			Random rand = new Random(seed);

			int colCnt = rand.NextInc(1, 5);
			int rowCnt = rand.NextInc(1, 5);

			g.ColumnDefinitions.Clear();
			for (int i = 0; i < colCnt; i++)
			{
				ColumnDefinition c = new ColumnDefinition();

				int unitTypeInt = rand.NextInc(0, 20);

				if (unitTypeInt <= 3)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Width = len;
				}
				else if (unitTypeInt >= 10)
				{
					GridLength len = new GridLength(rand.NextInc(1, 50), GridUnitType.Star);
					c.Width = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Width = len;
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MinWidth = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					if (allowZeroMax)
					{
						c.MaxWidth = rand.NextInc(0, 300);
					}
					else
					{
						c.MaxWidth = rand.NextInc(1, 300);
					}
				}

				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			for (int i = 0; i < rowCnt; i++)
			{
				RowDefinition c = new RowDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				if (unitTypeInt <= 3)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Height = len;
				}
				else if (unitTypeInt >= 10)
				{
					GridLength len = new GridLength(rand.NextInc(1, 50), GridUnitType.Star);
					c.Height = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Height = len;
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MinHeight = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MaxHeight = rand.NextInc(0, 300);
				}

				g.RowDefinitions.Add(c);
			}

			HashSet<long> colAndRowSet = new HashSet<long>();
			int childTryCnt = rand.NextInc(1, ((colCnt + 1) * rowCnt));
			int childCnt = 0;
			for (int i = 0; i < childTryCnt; i++)
			{
				int col = rand.NextInc(0, colCnt - 1);
				int row = rand.NextInc(0, rowCnt - 1);

				if (colAndRowSet.Add(CombineIntsIntoLong(col, row)) || rand.NextInc(1, 20) == 1)
				{
					int colSpan = 1;

					//if (rand.NextInc(0, 20) == 0)
					//{
					//	colSpan = rand.NextInc(2, 10);
					//	colSpan = Math.Min(colSpan, colCnt - col);
					//}

					int rowSpan = 1;

					//if (rand.NextInc(0, 20) == 0)
					//{
					//	rowSpan = rand.NextInc(2, 10);
					//	rowSpan = Math.Min(rowSpan, rowCnt - row);
					//}

					Button b = new Button();
					b.Name = "btn" + childCnt.ToString("0");
					b.Content = "Button " + b.Name + new string('X', rand.NextInc(0, 10));

					Grid.SetColumn(b, col);
					Grid.SetRow(b, row);

					Grid.SetColumnSpan(b, colSpan);
					Grid.SetRowSpan(b, rowSpan);
					g.Children.Add(b);
					childCnt++;
				}
			}
		}

		public static void SetupRandomGridDifferentControls(Grid g, int seed, bool allowZeroMax = true)
		{
			Random rand = new Random(seed);

			int colCnt = rand.NextInc(1, 7);
			int rowCnt = rand.NextInc(1, 10);

			g.ColumnDefinitions.Clear();
			for (int i = 0; i < colCnt; i++)
			{
				ColumnDefinition c = new ColumnDefinition();

				int unitTypeInt = rand.NextInc(0, 20);

				if (unitTypeInt <= 3)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Width = len;
				}
				else if (unitTypeInt >= 10)
				{
					GridLength len = new GridLength(rand.NextInc(1, 50), GridUnitType.Star);
					c.Width = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Width = len;
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MinWidth = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					if (allowZeroMax)
					{
						c.MaxWidth = rand.NextInc(0, 300);
					}
					else
					{
						c.MaxWidth = rand.NextInc(1, 300);
					}
				}

				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			for (int i = 0; i < rowCnt; i++)
			{
				RowDefinition c = new RowDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				if (unitTypeInt <= 3)
				{
					GridLength len = new GridLength((double)rand.NextInc(0, 500), GridUnitType.Pixel);
					c.Height = len;
				}
				else if (unitTypeInt >= 10)
				{
					GridLength len = new GridLength(rand.NextInc(1, 50), GridUnitType.Star);
					c.Height = len;
				}
				else
				{
					GridLength len = new GridLength(rand.NextInc(0, 100), GridUnitType.Auto);
					c.Height = len;
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MinHeight = rand.NextInc(0, 100);
				}

				if (rand.NextInc(0, 4) >= 3)
				{
					c.MaxHeight = rand.NextInc(0, 300);
				}

				g.RowDefinitions.Add(c);
			}

			HashSet<long> colAndRowSet = new HashSet<long>();
			int childTryCnt = rand.NextInc(1, ((colCnt + 4) * rowCnt));
			int childCnt = 0;
			for (int i = 0; i < childTryCnt; i++)
			{
				int col = rand.NextInc(0, colCnt - 1);
				int row = rand.NextInc(0, rowCnt - 1);

				if (colAndRowSet.Add(CombineIntsIntoLong(col, row)) || rand.NextInc(1, 20) == 1)
				{
					int colSpan = 1;

					if (rand.NextInc(0, 20) == 0)
					{
						colSpan = rand.NextInc(2, 10);
						colSpan = Math.Min(colSpan, colCnt - col);
					}

					int rowSpan = 1;

					if (rand.NextInc(0, 20) == 0)
					{
						rowSpan = rand.NextInc(2, 10);
						rowSpan = Math.Min(rowSpan, rowCnt - row);
					}

					int controlTypeRand = rand.NextInc(0, 20);
					UIElement n;
					if (controlTypeRand <= 4)
					{
						Button b = new Button();
						n = b;
						b.Name = "btn" + childCnt.ToString("0");
						b.Content = "Button " + b.Name + new string('X', rand.NextInc(0, 20));
					}
					else if (controlTypeRand <= 9)
					{
						Label b = new Label();
						n = b;
						b.Name = "lbl" + childCnt.ToString("0");
						b.Content = "Label " + b.Name + new string('X', rand.NextInc(0, 20));
					}
					else if (controlTypeRand <= 14)
					{
						TextBox b = new TextBox();
						n = b;
						b.Name = "txt" + childCnt.ToString("0");
						b.Text = "Text " + b.Name + new string('X', rand.NextInc(0, 20));
					}
					else
					{
						ComboBox b = new ComboBox();
						n = b;
						b.Name = "cbo" + childCnt.ToString("0");
						b.Text = "Combo " + b.Name + new string('X', rand.NextInc(0, 20));
					}

					Grid.SetColumn(n, col);
					Grid.SetRow(n, row);

					Grid.SetColumnSpan(n, colSpan);
					Grid.SetRowSpan(n, rowSpan);
					g.Children.Add(n);
					childCnt++;
					//if (rand.NextInc(0, 200) == 0)
					//{
					//	g.Children.Add(null); //??? can't be null, so why do we even check for null - maybe can be null for databound children
					//	childCnt++;
					//}
				}
			}
		}

		public static void SetupRandomGridSpans(Grid g, int seed, bool putChildrenOnSeparateRows, out bool hasZeroStars)
		{
			hasZeroStars = false;

			Random rand = new Random(seed);

			int colCnt = rand.NextInc(1, 5);
			int rowCnt = rand.NextInc(1, 8);

			g.ColumnDefinitions.Clear();
			for (int i = 0; i < colCnt; i++)
			{
				ColumnDefinition c = new ColumnDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				// don't use pixel cols because these aren't done the same
				//if (unitTypeInt == 0)
				//{
				//	GridLength len = new GridLength((double)rand.NextInc(0, 200), GridUnitType.Pixel);
				//	c.Width = len;
				//}
				//else
				if (unitTypeInt >= 7)
				{
					int stars = rand.NextInc(0, 10);
					if (stars == 0)
					{
						hasZeroStars = true;
					}
					GridLength len = new GridLength(stars, GridUnitType.Star);
					c.Width = len;
				}
				else
				{
					GridLength len = new GridLength(1, GridUnitType.Auto);
					c.Width = len;
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MinWidth = rand.NextInc(0, 50);
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MaxWidth = rand.NextInc(0, 800);
				}

				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			for (int i = 0; i < rowCnt; i++)
			{
				RowDefinition c = new RowDefinition();

				int unitTypeInt = rand.NextInc(0, 8);

				// don't use pixel cols because these aren't done the same
				//if (unitTypeInt == 0)
				//{
				//	GridLength len = new GridLength((double)rand.NextInc(0, 200), GridUnitType.Pixel);
				//	c.Height = len;
				//}
				//else
				if (unitTypeInt >= 7)
				{
					int stars = rand.NextInc(0, 10);
					if (stars == 0)
					{
						hasZeroStars = true;
					}
					GridLength len = new GridLength(stars, GridUnitType.Star);
					c.Height = len;
				}
				else
				{
					GridLength len = new GridLength(1, GridUnitType.Auto);
					c.Height = len;
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MinHeight = rand.NextInc(0, 50);
				}

				if (rand.NextInc(0, 4) == 4)
				{
					c.MaxHeight = rand.NextInc(0, 800);
				}

				g.RowDefinitions.Add(c);
			}

			HashSet<long> childCellSet = new HashSet<long>();
			HashSet<int> childRowSet = new HashSet<int>();

			int childCnt;
			if (putChildrenOnSeparateRows)
			{
				childCnt = rowCnt;
			}
			else
			{
				childCnt = rand.NextInc(0, 10);
			}

			for (int i = 0; i < childCnt;)
			{
				int col = 0;
				int row = 0;

				if (putChildrenOnSeparateRows)
				{
					col = rand.NextInc(0, colCnt - 1);
					row = i;
				}
				else
				{
					for (int j = 0; j <= 10; j++)
					{
						// try not to put children in the same cell
						col = rand.NextInc(0, colCnt - 1);

						for (int k = 0; k <= 10; k++)
						{
							// try not to put children in the same row
							row = rand.NextInc(0, rowCnt - 1);
							if (!childRowSet.Contains(row))
							{
								break;
							}
						}

						long key = CombineIntsIntoLong(col, row);
						if (!childCellSet.Contains(key))
						{
							break;
						}
					}
				}

				Button b = new Button();
				b.Name = "btn" + i.ToString("0");
				b.Content = "Button " + b.Name + new string('X', rand.NextInc(0, 10));

				//??? also randomly insert other controls and grids set with this function

				int colSpan = 1;

				if (rand.NextInc(0, 1) == 0)
				{
					colSpan = rand.NextInc(2, 8);
					if (colSpan >= 6)
					{
						colSpan = 2;
					}
					colSpan = Math.Min(colSpan, colCnt - col);
				}

				int rowSpan = 1;

				if (rand.NextInc(0, 1) == 0)
				{
					rowSpan = rand.NextInc(2, 8);
					if (rowSpan >= 6)
					{
						rowSpan = 2;
					}
					rowSpan = Math.Min(rowSpan, rowCnt - row);
				}

				i += rowSpan;

				Grid.SetColumn(b, col);
				Grid.SetRow(b, row);

				Grid.SetColumnSpan(b, colSpan);
				Grid.SetRowSpan(b, rowSpan);
				g.Children.Add(b);
			}
		}

		// extension method for Random that makes the Next inclusive
		public static int NextInc(this Random rand, int minValue, int maxValueInclusive)
		{
			return rand.Next(minValue, maxValueInclusive + 1);
		}

		public static void CopyGridSetup(Grid grd, LayoutGrid g)
		{
			g.ColumnDefinitions.Clear();
			foreach (ColumnDefinition c2 in grd.ColumnDefinitions)
			{
				ColumnDefinition c = new ColumnDefinition();
				c.Width = c2.Width;
				c.MinWidth = c2.MinWidth;
				c.MaxWidth = c2.MaxWidth;
				g.ColumnDefinitions.Add(c);
			}

			g.RowDefinitions.Clear();
			foreach (RowDefinition c2 in grd.RowDefinitions)
			{
				RowDefinition c = new RowDefinition();
				c.Height = c2.Height;
				c.MinHeight = c2.MinHeight;
				c.MaxHeight = c2.MaxHeight;
				g.RowDefinitions.Add(c);
			}

			foreach (UIElement child in grd.Children)
			{
				if (child == null)
				{
					g.Children.Add(null);
				}
				else
				{
					if (child is Button)
					{
						Button b2 = child as Button;
						Button b = new Button();
						b.Name = b2.Name;
						b.Content = b2.Content;

						Grid.SetColumn(b, Grid.GetColumn(b2));
						Grid.SetRow(b, Grid.GetRow(b2));

						Grid.SetColumnSpan(b, Grid.GetColumnSpan(b2));
						Grid.SetRowSpan(b, Grid.GetRowSpan(b2));
						g.Children.Add(b);
					}
					else if (child is Label)
					{
						Label l2 = child as Label;
						Label l = new Label();
						l.Name = l2.Name;
						l.Content = l2.Content;

						Grid.SetColumn(l, Grid.GetColumn(l2));
						Grid.SetRow(l, Grid.GetRow(l2));

						Grid.SetColumnSpan(l, Grid.GetColumnSpan(l2));
						Grid.SetRowSpan(l, Grid.GetRowSpan(l2));
						g.Children.Add(l);
					}
					else if (child is TextBox)
					{
						TextBox t2 = child as TextBox;
						TextBox t = new TextBox();
						t.Name = t2.Name;
						t.Text = t2.Text;

						Grid.SetColumn(t, Grid.GetColumn(t2));
						Grid.SetRow(t, Grid.GetRow(t2));

						Grid.SetColumnSpan(t, Grid.GetColumnSpan(t2));
						Grid.SetRowSpan(t, Grid.GetRowSpan(t2));
						g.Children.Add(t);
					}
					else if (child is ComboBox)
					{
						ComboBox c2 = child as ComboBox;
						ComboBox c = new ComboBox();
						c.Name = c2.Name;
						c.Text = c2.Text;

						Grid.SetColumn(c, Grid.GetColumn(c2));
						Grid.SetRow(c, Grid.GetRow(c2));

						Grid.SetColumnSpan(c, Grid.GetColumnSpan(c2));
						Grid.SetRowSpan(c, Grid.GetRowSpan(c2));
						g.Children.Add(c);
					}
				}
			}
		}

		private static void AddNameAndSizeToString(StringBuilder sb, string name, Size renderSize, int intID)
		{
			string gridName;
			if (string.IsNullOrEmpty(name))
			{
				gridName = "grid";
			}
			else
			{
				gridName = name;
			}
			string gridNamePlusSize = gridName + ' ' + renderSize.Width.ToString(dblFmt) + ',' + renderSize.Height.ToString(dblFmt) + " ; " + intID.ToString("0");
			sb.AppendLine(gridNamePlusSize);
		}

		private static void AddColsStoString(StringBuilder sb, List<ColumnDefinition> colDefs, Grid g, LayoutGrid g2)
		{
			string minStr;
			string maxStr;
			string minMaxStr;
			int cnt;
			cnt = colDefs.Count;
			sb.AppendLine("Cols:");
			if (cnt == 0)
			{
				sb.AppendLine("1*");
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					ColumnDefinition c = colDefs[i];

					if (c.MinWidth > 0)
					{
						minStr = c.MinWidth.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(c.MaxWidth))
					{
						maxStr = c.MaxWidth.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr;
					if (g != null)
					{
						actualLengthStr = " ; " + c.ActualWidth.ToString(dblFmt);
					}
					else
					{
						actualLengthStr = " ; " + g2.GetColWidth(i).ToString(dblFmt);
					}

					string str;
					if (c.Width.IsStar)
					{
						str = c.Width.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr;
					}
					else if (c.Width.IsAuto)
					{
						str = "A " + minMaxStr + actualLengthStr;
					}
					else // absolute
					{
						str = "P" + c.Width.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr;
					}
					sb.AppendLine(str);
				}
			}
		}

		private static void AddRowsToString(StringBuilder sb, List<RowDefinition> rowDefs, Grid g, LayoutGrid g2)
		{
			string minStr;
			string maxStr;
			string minMaxStr;
			int cnt;
			sb.AppendLine("Rows:");
			cnt = rowDefs.Count;
			if (cnt == 0)
			{
				sb.AppendLine("1*");
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					RowDefinition r = rowDefs[i];

					if (r.MinHeight > 0)
					{
						minStr = r.MinHeight.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(r.MaxHeight))
					{
						maxStr = r.MaxHeight.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr;
					if (g != null)
					{
						actualLengthStr = " ; " + r.ActualHeight.ToString(dblFmt);
					}
					else
					{
						actualLengthStr = " ; " + g2.GetRowHeight(i).ToString(dblFmt);
					}

					string str;
					if (r.Height.IsStar)
					{
						str = r.Height.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr;
					}
					else if (r.Height.IsAuto)
					{
						str = "A " + minMaxStr + actualLengthStr;
					}
					else // absolute
					{
						str = "P" + r.Height.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr;
					}
					sb.AppendLine(str);
				}
			}
		}

		private static List<T> AddChildrenToString<T>(StringBuilder sb, UIElementCollection children) where T : class
		{
			List<T> childGridList = new List<T>();

			sb.AppendLine("Children:");
			List<ChildData> childList = new List<ChildData>();
			for (int i = 0; i < children.Count; i++)
			{
				UIElement child = children[i];
				if (child != null)
				{
					T gChild = child as T;
					if (gChild != null)
					{
						childGridList.Add(gChild);
					}

					ChildData c = new ChildData();
					childList.Add(c);

					c.col = (int)child.GetValue(Grid.ColumnProperty);
					c.row = (int)child.GetValue(Grid.RowProperty);
					c.colSpan = (int)child.GetValue(Grid.ColumnSpanProperty);
					c.rowSpan = (int)child.GetValue(Grid.RowSpanProperty);

					ContentControl conCtrl = child as ContentControl;
					if (conCtrl != null)
					{
						c.content = conCtrl.Content.ToString();
					}
					c.type = child.GetType().Name;
					c.child = child;
					c.name = child.GetValue(FrameworkElement.NameProperty) as string;
					c.zIndex = (int)child.GetValue(Panel.ZIndexProperty);
					c.childIndex = i;
				}
			}
			childList.Sort(new SortChildData());

			foreach (ChildData c in childList)
			{
				string contentStr;
				if (c.content != null)
				{
					contentStr = " [" + c.content + ']';
				}
				else
				{
					contentStr = "";
				}
				sb.AppendLine(c.name + contentStr + " ; col=" + c.col.ToString("N0") + "; row=" + c.row.ToString("N0") +
					(c.colSpan > 1 ? " ; colSpan=" + c.colSpan.ToString("N0") : "") + (c.rowSpan > 1 ? "; rowSpan=" + c.rowSpan.ToString("N0") : "") +
					"; DesiredSize=" + c.child.DesiredSize.Width.ToString(dblFmt6) + "," + c.child.DesiredSize.Height.ToString(dblFmt6) +
					"; RenderSize=" + c.child.RenderSize.Width.ToString(dblFmt6) + "," + c.child.RenderSize.Height.ToString(dblFmt6)
					);
			}

			return childGridList;
		}

		public static StringBuilder CreateGridString(Grid g, int seed)
		{
			StringBuilder sb = new StringBuilder();

			AddNameAndSizeToString(sb, g.Name, g.RenderSize, seed);

			AddColsStoString(sb, new List<ColumnDefinition>(g.ColumnDefinitions), g, null);
			AddRowsToString(sb, new List<RowDefinition>(g.RowDefinitions), g, null);
			List<Grid> childGridList = AddChildrenToString<Grid>(sb, g.Children);

			if (childGridList.Count > 0)
			{
				sb.AppendLine("Child Grids:");

				foreach (Grid grd in childGridList)
				{
					StringBuilder sbChild = CreateGridString(grd, seed);

					sb.Append(sbChild);
					sb.AppendLine();
				}
			}

			return sb;
		}

		public static StringBuilder CreateGridString(LayoutGrid g, int seed)
		{
			StringBuilder sb = new StringBuilder();

			AddNameAndSizeToString(sb, g.Name, g.RenderSize, seed);

			AddColsStoString(sb, new List<ColumnDefinition>(g.ColumnDefinitions), null, g);
			AddRowsToString(sb, new List<RowDefinition>(g.RowDefinitions), null, g);
			List<LayoutGrid> childGridList = AddChildrenToString<LayoutGrid>(sb, g.Children);

			if (childGridList.Count > 0)
			{
				sb.AppendLine("Child Grids:");

				foreach (LayoutGrid grd in childGridList)
				{
					StringBuilder sbChild = CreateGridString(grd, seed);

					sb.Append(sbChild);
					sb.AppendLine();
				}
			}

			return sb;
		}

		public static void WriteGridToExcelCreateWorksheet(Workbook wb, Grid g, int decimalPlaces, bool showChildGrids = true)
		{
			string gridName;
			if (string.IsNullOrEmpty(g.Name))
			{
				gridName = "grid";
			}
			else
			{
				gridName = g.Name;
			}
			string gridNamePlusSize = gridName;

			Worksheet ws = wb.AddWorksheet();
			ws.SetWorksheetNameSafe(gridNamePlusSize);

			WriteGridToExcel(ws, g, decimalPlaces, 0, 0, showChildGrids);
		}

		public static int WriteGridToExcel(Worksheet ws, Grid g, int decimalPlaces, int startCol, int startRow, bool showChildGrids = true, bool showChildSize = true)
		{
			int row = startRow;
			int col = startCol;

			string gridName;
			if (string.IsNullOrEmpty(g.Name))
			{
				gridName = "grid";
			}
			else
			{
				gridName = g.Name;
			}
			
			string dblFmt = CreateDoubleFormatStr(decimalPlaces);

			string gridNamePlusSize = gridName + " " + g.ActualWidth.ToString(dblFmt) + ',' + g.ActualHeight.ToString(dblFmt);
			ws.SetValue(row, col++, gridNamePlusSize);

			string minStr;
			string maxStr;
			string minMaxStr;
			int cnt;
			cnt = g.ColumnDefinitions.Count;
			if (cnt == 0)
			{
				ws.SetValue(row, col++, "1*"); //??? how can we get the actual length if this isn't a real col def
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					ColumnDefinition c = g.ColumnDefinitions[i];

					if (c.MinWidth > 0)
					{
						minStr = c.MinWidth.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(c.MaxWidth))
					{
						maxStr = c.MaxWidth.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr = " ; " + c.ActualWidth.ToString(dblFmt);

					if (c.Width.IsStar)
					{
						ws.SetValue(row, col++, c.Width.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr);
					}
					else if (c.Width.IsAuto)
					{
						ws.SetValue(row, col++, "A " + minMaxStr + actualLengthStr);
					}
					else // absolute
					{
						ws.SetValue(row, col++, "P" + c.Width.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr);
					}
				}
			}
			int maxCol = col - 1;

			col = startCol;
			row++;

			cnt = g.RowDefinitions.Count;
			if (cnt == 0)
			{
				ws.SetValue(row++, col, "1*"); //??? how can we get the actual length if this isn't a real col def
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					RowDefinition r = g.RowDefinitions[i];

					if (r.MinHeight > 0)
					{
						minStr = r.MinHeight.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(r.MaxHeight))
					{
						maxStr = r.MaxHeight.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr = " ; " + r.ActualHeight.ToString(dblFmt);

					if (r.Height.IsStar)
					{
						ws.SetValue(row++, col, r.Height.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr);
					}
					else if (r.Height.IsAuto)
					{
						ws.SetValue(row++, col, "A " + minMaxStr + actualLengthStr);
					}
					else // absolute
					{
						ws.SetValue(row++, col, "P" + r.Height.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr);
					}
				}
			}
			int maxRow = row - 1;

			Dictionary<long, string> colAndRowToChildrenStringDict = new Dictionary<long, string>();

			List<Grid> childGridList = new List<Grid>();

			for (int i = 0; i < g.Children.Count; i++)
			{
				UIElement child = g.Children[i];
				if (child != null)
				{
					Grid gChild = child as Grid;
					if (gChild != null)
					{
						childGridList.Add(gChild);
					}

					col = (int)child.GetValue(Grid.ColumnProperty);
					row = (int)child.GetValue(Grid.RowProperty);
					int colSpan = (int)child.GetValue(Grid.ColumnSpanProperty);
					int rowSpan = (int)child.GetValue(Grid.RowSpanProperty);
					long n = CombineIntsIntoLong(col, row);

					string additionalStr;
					if (colAndRowToChildrenStringDict.TryGetValue(n, out string existingStr))
					{
						additionalStr = " ; ";
					}
					else
					{
						additionalStr = "";
						existingStr = "";
					}

					string name = child.GetValue(FrameworkElement.NameProperty) as string;
					if (!string.IsNullOrEmpty(name))
					{
						additionalStr += name;
					}

					if (colSpan > 1)
					{
						additionalStr += " colSpan=" + colSpan.ToString("N0");
					}

					if (rowSpan > 1)
					{
						additionalStr += " rowSpan=" + rowSpan.ToString("N0");
					}

					if (showChildSize)
					{
						additionalStr += " ; DesiredSize=" + child.DesiredSize.Width.ToString("0.##") + ',' + child.DesiredSize.Height.ToString("0.##");
						additionalStr += " ; RenderSize=" + child.RenderSize.Width.ToString("0.##") + ',' + child.RenderSize.Height.ToString("0.##");
					}

					colAndRowToChildrenStringDict[n] = existingStr + additionalStr;
				}
			}

			foreach (KeyValuePair<long, string> keyVal in colAndRowToChildrenStringDict)
			{
				GetIntsFromLong(keyVal.Key, out col, out row);
				ws.SetValue(row + 1 + startRow, col + 1 + startCol, keyVal.Value);
				// add wraptext???, but after autosizing - do the same for other grid function
			}

			if (showChildGrids)
			{
				foreach (Grid grd in childGridList)
				{
					maxRow = WriteGridToExcel(ws, grd, decimalPlaces, startCol + 1, maxRow + 2, showChildGrids);
				}
			}
			ws.AutoSizeColumns(startCol, maxCol);

			return maxRow;
		}

		public static int WriteGridToExcelCreateWorksheet(Workbook wb, LayoutGrid g, int decimalPlaces, bool showChildGrids = true)
		{
			string gridName;
			if (string.IsNullOrEmpty(g.Name))
			{
				gridName = "grid";
			}
			else
			{
				gridName = g.Name;
			}

			string gridNamePlusSize = gridName;

			Worksheet ws = wb.AddWorksheet();
			ws.SetWorksheetNameSafe(gridNamePlusSize);

			return WriteGridToExcel(ws, g, decimalPlaces, 0, 0, showChildGrids);
		}

		public static string CreateDoubleFormatStr(int decimalPlaces)
		{
			string fmt =  "###0." + new string('#', decimalPlaces);
			return fmt;
		}

		public static int WriteGridToExcel(Worksheet ws, LayoutGrid g, int decimalPlaces, int startCol, int startRow, bool showChildGrids = true, bool showChildSize = true, bool showCellGroup = true)
		{
			int row = startRow;
			int col = startCol;

			string gridName;
			if (string.IsNullOrEmpty(g.Name))
			{
				gridName = "grid";
			}
			else
			{
				gridName = g.Name;
			}
			
			string dblFmt = CreateDoubleFormatStr(decimalPlaces);

			string gridNamePlusSize = gridName + " " + g.ActualWidth.ToString(dblFmt) + ',' + g.ActualHeight.ToString(dblFmt);
			ws.SetValue(row, col++, gridNamePlusSize);

			string minStr;
			string maxStr;
			string minMaxStr;
			int cnt;
			cnt = g.ColumnDefinitions.Count;
			if (cnt == 0)
			{
				string actualLengthStr = " ; " + g.GetColWidth(0).ToString(dblFmt);
				ws.SetValue(row, col++, "1* " + actualLengthStr);
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					ColumnDefinition c = g.ColumnDefinitions[i];

					if (c.MinWidth > 0)
					{
						minStr = c.MinWidth.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(c.MaxWidth))
					{
						maxStr = c.MaxWidth.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr = " ; " + g.GetColWidth(i).ToString(dblFmt);

					if (c.Width.IsStar)
					{
						ws.SetValue(row, col++, c.Width.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr);
					}
					else if (c.Width.IsAuto)
					{
						ws.SetValue(row, col++, "A " + minMaxStr + actualLengthStr);
					}
					else // absolute
					{
						ws.SetValue(row, col++, "P" + c.Width.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr);
					}
				}
			}
			int maxCol = col - 1;

			col = startCol;
			row++;

			cnt = g.RowDefinitions.Count;
			if (cnt == 0)
			{
				string actualLengthStr = " ; " + g.GetRowHeight(0).ToString(dblFmt);
				ws.SetValue(row++, col, "1* " + actualLengthStr);
			}
			else
			{
				for (int i = 0; i < cnt; i++)
				{
					RowDefinition r = g.RowDefinitions[i];

					if (r.MinHeight > 0)
					{
						minStr = r.MinHeight.ToString(dblFmt); //??? what if this is infinite?
					}
					else
					{
						minStr = "";
					}

					if (!double.IsPositiveInfinity(r.MaxHeight))
					{
						maxStr = r.MaxHeight.ToString(dblFmt);
					}
					else
					{
						maxStr = "";
					}

					if (minStr.Length > 0 || maxStr.Length > 0)
					{
						minMaxStr = minStr + '|' + maxStr;
					}
					else
					{
						minMaxStr = "";
					}

					string actualLengthStr = " ; " + g.GetRowHeight(i).ToString(dblFmt);

					if (r.Height.IsStar)
					{
						ws.SetValue(row++, col, r.Height.Value.ToString(dblFmt) + "* " + minMaxStr + actualLengthStr);
					}
					else if (r.Height.IsAuto)
					{
						ws.SetValue(row++, col, "A " + minMaxStr + actualLengthStr);
					}
					else // absolute
					{
						ws.SetValue(row++, col, "P" + r.Height.Value.ToString(dblFmt) + ' ' + minMaxStr + actualLengthStr);
					}
				}
			}
			int maxRow = row - 1;

			Dictionary<long, string> colAndRowToChildrenStringDict = new Dictionary<long, string>();

			List<LayoutGrid> childGridList = new List<LayoutGrid>();

			for (int i = 0; i < g.Children.Count; i++)
			{
				UIElement child = g.Children[i];
				if (child != null)
				{
					LayoutGrid gChild = child as LayoutGrid;
					if (gChild != null)
					{
						childGridList.Add(gChild);
					}

					col = (int)child.GetValue(Grid.ColumnProperty);
					row = (int)child.GetValue(Grid.RowProperty);
					int colSpan = (int)child.GetValue(Grid.ColumnSpanProperty);
					int rowSpan = (int)child.GetValue(Grid.RowSpanProperty);
					long n = CombineIntsIntoLong(col, row);

					string additionalStr;
					if (colAndRowToChildrenStringDict.TryGetValue(n, out string existingStr))
					{
						additionalStr = " ; ";
					}
					else
					{
						additionalStr = "";
						existingStr = "";
					}

					string name = child.GetValue(FrameworkElement.NameProperty) as string;
					if (!string.IsNullOrEmpty(name))
					{
						additionalStr += name;
					}

					if (colSpan > 1)
					{
						additionalStr += " colSpan=" + colSpan.ToString("N0");
					}

					if (rowSpan > 1)
					{
						additionalStr += " rowSpan=" + rowSpan.ToString("N0");
					}

					if (showChildSize)
					{
						additionalStr += " ; DesiredSize=" + child.DesiredSize.Width.ToString("0.##") + ',' + child.DesiredSize.Height.ToString("0.##");
						additionalStr += " ; RenderSize=" + child.RenderSize.Width.ToString("0.##") + ',' + child.RenderSize.Height.ToString("0.##");
					}

					if (showCellGroup)
					{
					#if DEBUG
						int cellGroup = g.GetChildCellGroup(child);
						additionalStr += " ; grp=" + cellGroup.ToString("N0");
					#endif
					}

					colAndRowToChildrenStringDict[n] = existingStr + additionalStr;
				}
			}

			foreach (KeyValuePair<long, string> keyVal in colAndRowToChildrenStringDict)
			{
				GetIntsFromLong(keyVal.Key, out col, out row);
				ws.SetValue(row + 1 + startRow, col + 1 + startCol, keyVal.Value);
			}

			if (showChildGrids)
			{
				foreach (LayoutGrid grd in childGridList)
				{
					maxRow = WriteGridToExcel(ws, grd, decimalPlaces, startCol + 1, maxRow + 2, showChildGrids);
				}
			}
			ws.AutoSizeColumns(startCol, maxCol);

			return maxRow;
		}

		public static long CombineIntsIntoLong(int high, int low)
		{
			return ((long)high << 32) | (long)low;
		}

		public static void GetIntsFromLong(long n, out int high, out int low)
		{
			high = (int)(n >> 32);
			low = (int)(n & 0x00000ffffL);
		}
	}
}

