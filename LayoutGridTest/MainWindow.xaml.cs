﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Motvin.LayoutGrid;
using SpreadsheetUtil;

namespace LayoutGridTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void btnRandAll_Click(object sender, RoutedEventArgs e)
		{
			RunRandManyChildren();
			RunRandSpans();
			RunRand2();
			RunRandStars();
		}

		private void btnRandManyChildren_Click(object sender, RoutedEventArgs e)
		{
			RunRandManyChildren();
		}

		private void btnRandSpans_Click(object sender, RoutedEventArgs e)
		{
			RunRandSpans();
		}

		private void btnRand2_Click(object sender, RoutedEventArgs e)
		{
			RunRand2();
		}

		private void btnRandStars_Click(object sender, RoutedEventArgs e)
		{
			RunRandStars();
		}

		private void btnPerf1_Click(object sender, RoutedEventArgs e)
		{
			RunPerformanceTest();
		}

		public static void RunRandManyChildren(bool useInfinitWidth = false, bool useInfiniteHeight = false)
		{
			int seed = 1;
			while (true)
			{
				if (seed == 15 || // span expands pixel sized col/row for Grid, it should not
					seed == 45 || // span expands pixel sized col/row for Grid, it should not
					seed == 87 || // span expands pixel sized col/row for Grid, it should not
					seed == 181 || // span expands pixel sized col/row for Grid, it should not
					seed == 382 || // span expands pixel sized col/row for Grid, it should not
					seed == 401 || // span expands pixel sized col/row for Grid, it should not
					seed == 538 || // Grid is wrong, we expand the last 2 cols evenly, but for some reason Grid does not
					seed == 654 || // span expands pixel sized col/row for Grid, it should not
					seed == 677 || // span expands pixel sized ccol/row for Grid, it should not
					seed == 807 || // span expands pixel sized col/row for Grid, it should not
					seed == 815 || // span expands pixel sized col/row for Grid, it should not
					seed == 898 || // span expands pixel sized col/row for Grid, it should not, also there are 2 star sized cols that aren't proportional in Grid for some reason - this must be a bug in Grid, not sure how
					seed == 905 || // span expands pixel sized col/row for Grid, it should not
					seed == 959 || // span expands pixel sized col/row for Grid, it should not
					seed == 960 || // span expands pixel sized col/row for Grid, it should not
					seed == 975 || // span expands pixel sized col/row for Grid, it should not
					seed == 993 || // span expands pixel sized col/row for Grid, it should not
					seed == 1087 || // span expands pixel sized col/row for Grid, it should not
					seed == 1155 || // span expands pixel sized col/row for Grid, it should not
									// next seed == 1289
					1 == 2
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed == 1183)
				{
					break;
				}

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGrid(g1, seed);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				WindowPlain win1 = new WindowPlain(g1, useInfinitWidth, useInfiniteHeight);
				win1.Title = "Grid 1 Many Children";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfinitWidth, useInfiniteHeight);
				win2.Title = "Test Grid 2 Many Children";
				win2.Show();

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
				}
				else
				{
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2);

						string fileName = @"e:\proj\SpreadsheetOut\LayoutGridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.ToString());
					}
					finally
					{
						wb.Close();
					}

					wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1);

						string fileName = @"e:\proj\SpreadsheetOut\GridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.ToString());
					}
					finally
					{
						wb.Close();
					}

					break;
				}
				seed++;
			}
		}

		public static void RunRandSpans(bool useInfinitWidth = false, bool useInfiniteHeight = false)
		{
			int seed = 1;
			while (true)
			{
				if (seed == 88 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed - this is because of resolving spans in a different order and possibly also not resolving the col width of children with rowspans before the width of any colspan children
					seed == 94 || // span order
					seed == 157 || // we are correct, Grid does not respect max size and goes past it for a colspan
					seed == 163 || // span order
					seed == 167 || // span order
					seed == 190 || // Grid does not evenly expand auto cols for a colspan for some reason
					seed == 245 || // span order
					seed == 255 || // span order
					seed == 324 || // this is an auto width that goes beyond the max, we constrain the desired/rendered size to max, but Grid does not, which chops off the border
					seed == 348 || // this is 0* treated like pixel sizing
					seed == 367 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed
					seed == 371 || // this is 0* treated like pixel sizing
					seed == 381 || // span order
					seed == 384 || // this is 0* treated like pixel sizing
					seed == 433 || // span order
					seed == 450 || // this is 0* treated like pixel sizing
					seed == 467 || // span order
					seed == 470 || // this is 0* treated like pixel sizing
					seed == 480 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max
					seed == 493 || // this is 0* treated like pixel sizing
					seed == 498 || // this is 0* treated like pixel sizing
					seed == 521 || // this is 0* treated like pixel sizing
					seed == 525 || // this is 0* treated like pixel sizing
					seed == 555 || // we are correct, Grid does not expand both cols evenly - not sure why
					seed == 574 || // this is 0 * treated like pixel sizing
					seed == 596 || // this is 0 * treated like pixel sizing
					seed == 597 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max
					seed == 599 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max
					seed == 689 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed
					seed == 729 || // Grid is wrong.  We use the extra space to not distribute more extra if not needed when overlapping spans.
					seed == 808 || // There is a rowspan(no colspan) with a higher cellgroup than the colspan that overlaps this col, the colspan is done first, although you could do the rowspan (only the single col width part of it) first - but it's probably not a big deal and we don't want to be calling measure more times than 1 per child when we don't have to
					seed == 835 || // span order
					seed == 856 || // we are correct in that we expand the auto cols equally, and Grid doesn't for some reason
					seed == 944 || // different ordering, but if we have spanextra and then we set a width/height without a span, then this should be subtracted from the extra (down to 0)???
					seed == 1029 || // we are correct, Grid does not respect max size and goes past it
					1 == 0
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed == 1030)
				{
					break;
				}

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGridSpans(g1, seed, true, out bool hasZeroStar);
				if (hasZeroStar)
				{
					seed++;
					continue;
				}

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				WindowPlain win1 = new WindowPlain(g1, useInfinitWidth, useInfiniteHeight);
				win1.Title = "Grid 1 Spans";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfinitWidth, useInfiniteHeight);
				win2.Title = "Test Grid 2 Spans";
				win2.Show();

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
				}
				else
				{
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2);

						string fileName = @"e:\proj\SpreadsheetOut\LayoutGridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(), ex.Message);
					}
					finally
					{
						if (wb != null)
						{
							wb.Close();
						}
					}

					wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1);

						string fileName = @"e:\proj\SpreadsheetOut\GridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(), ex.Message);
					}
					finally
					{
						if (wb != null)
						{
							wb.Close();
						}
					}

					break;
				}
				seed++;
			}
		}

		public static void RunRand2(bool useInfinitWidth = false, bool useInfiniteHeight = false)
		{
			int seed = 1;
			while (true)
			{
				if (
					seed == 49 || // span expands pixel sized col/row for Grid, it should not
					seed == 268 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					seed == 358 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					//seed == 425 || // Grid is correct and we are wrong.  We use the Min size as the constrained and this causes an uneven extra for span cols???
					seed == 674 || // this is strange in that Grid somehow increases a column from a spanned col (but there is a star col in the span) and also the increase is completely unnecessary
					seed == 668 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					seed == 1057 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					seed == 1457 || // we correctly distribute the extra evenly, where Grid does not
					1 == 2
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed == 1458)
				{
					break;
				}

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGrid2(g1, seed);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				WindowPlain win1 = new WindowPlain(g1, useInfinitWidth, useInfiniteHeight);
				win1.Title = "Grid 1 Rand 2";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfinitWidth, useInfiniteHeight);
				win2.Title = "Test Grid 2 Rand 2";
				win2.Show();

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
				}
				else
				{
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2);

						string fileName = @"e:\proj\SpreadsheetOut\LayoutGridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.ToString());
					}
					finally
					{
						wb.Close();
					}

					wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1);

						string fileName = @"e:\proj\SpreadsheetOut\GridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, ex.ToString());
					}
					finally
					{
						wb.Close();
					}

					break;
				}
				seed++;
			}
		}

		public static void RunRandStars(bool useInfinitWidth = false, bool useInfiniteHeight = false)
		{
			int seed = 1;
			while (true)
			{
				if (seed == 611 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 3216 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 4931 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 5266 || // Grid has a .01 difference in a star col, we are correct since we add up to the width
					seed == 10269 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 10577 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 10614 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols
					seed == 10721 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 11540 || // Grid is wrong when a star col has a max of 0, it doesn't proportionately space out the other star cols

					1 == 0
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed == 11541)
				{
					break;
				}

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGridStars(g1, seed);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				WindowPlain win1 = new WindowPlain(g1, useInfinitWidth, useInfiniteHeight);
				win1.Title = "Grid 1 Stars";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfinitWidth, useInfiniteHeight);
				win2.Title = "Test Grid 2 Stars";
				win2.Show();

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
				}
				else
				{
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2);

						string fileName = @"e:\proj\SpreadsheetOut\LayoutGridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(), ex.Message);
					}
					finally
					{
						if (wb != null)
						{
							wb.Close();
						}
					}

					wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1);

						string fileName = @"e:\proj\SpreadsheetOut\GridRand.xlsx";

						wb.SaveAs(fileName);

						ProcessStartInfo p = new ProcessStartInfo();
						p.UseShellExecute = true;
						p.FileName = fileName;
						p.Verb = "Open";
						Process.Start(p);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString(), ex.Message);
					}
					finally
					{
						if (wb != null)
						{
							wb.Close();
						}
					}

					break;
				}
				seed++;
			}
		}

		//??? compare the result of this with the grids actual width/height and flag any differences 
		public static double SumColWidthsOrRowHeights(LayoutGrid g, bool getCol, out double starCount)
		{
			double sumLength = 0;

			starCount = 0;

			if (getCol)
			{
				for (int i = 0; i < g.ColumnDefinitions.Count; i++)
				{
					sumLength += g.GetColWidth(i);

					ColumnDefinition c = g.ColumnDefinitions[i];
					if (c.Width.IsStar)
					{
						starCount += c.Width.Value;
					}
				}
			}
			else
			{
				for (int i = 0; i < g.RowDefinitions.Count; i++)
				{
					sumLength += g.GetRowHeight(i);

					RowDefinition c = g.RowDefinitions[i];
					if (c.Height.IsStar)
					{
						starCount += c.Height.Value;
					}
				}
			}

			return sumLength;
		}

		public static void RunPerformanceTest()
		{
			double winHeight = 500;
			double winWidth = 800;

			double gridMeasureNanoSecs = 0;
			double gridArrangeNanoSecs = 0;

			double layoutGridMeasureNanoSecs = 0;
			double layoutGridArrangeNanoSecs = 0;

			int gridMeasureCount = 0;
			int gridArrangeCount = 0;

			int layoutGridMeasureCount = 0;
			int layoutGridArrangeCount = 0;

			double gridConstructorNanoSecsTotal = 0;
			double layoutGridConstructorNanoSecsTotal = 0;

			double preMeasureNanoSecsTotal = 0;
			double measureNanoSecsTotal = 0;
			double childMeasureNanoSecsTotal = 0;
			double postMeasureNanoSecsTotal = 0;
			double shortMeasureNanoSecsTotal = 0;
			int shortMeasureCount = 0;

			double preArrangeNanoSecsTotal = 0;
			double arrangeNanoSecsTotal = 0;
			double childArrangeNanoSecsTotal = 0;
			double shortArrangeNanoSecsTotal = 0;
			int shortArrangeCount = 0;

			int seed = 1;
			int showCount = 0;
			while (true)
			{
				if (seed == 15 ||
					seed == 45 ||
					seed == 181 ||
					seed == 221 ||
					seed == 382 ||
					seed == 401 ||
					seed == 654 ||
					seed == 677 ||
					seed == 898 ||
					seed == 959 ||
					seed == 960 ||
					seed == 993 ||
					seed == 1087 ||
					seed == 1155
				)
				{
					seed++;
					continue; // these are cases where we are correct and the existing Grid is wrong
				}

				long startTicks;
				long endTicks;
				double ticks;
				double nanoSecs;

				PerfStatic.DoGCCollect();
				startTicks = Stopwatch.GetTimestamp();

				PerfGrid g1 = new PerfGrid();

				endTicks = Stopwatch.GetTimestamp();

				ticks = (double)(endTicks - startTicks);

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
				gridConstructorNanoSecsTotal += nanoSecs;

				PerfStatic.DoGCCollect();
				startTicks = Stopwatch.GetTimestamp();

				PerfLayoutGrid g2 = new PerfLayoutGrid();

				endTicks = Stopwatch.GetTimestamp();

				ticks = (double)(endTicks - startTicks);

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
				layoutGridConstructorNanoSecsTotal += nanoSecs;

				g1.Name = "grd";
				GridLog.SetupRandomGrid(g1, seed);

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				showCount++;

				PerfStatic.DoGCCollect();
				WindowPlain win2 = new WindowPlain(g2);
				win2.Show();

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.preMeasureTicks, Stopwatch.Frequency);
				preMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.measureTicks, Stopwatch.Frequency);
				measureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.childMeasureTicks, Stopwatch.Frequency);
				childMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.postMeasureTicks, Stopwatch.Frequency);
				postMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.shortMeasureTicks, Stopwatch.Frequency);
				shortMeasureNanoSecsTotal += nanoSecs;
				shortMeasureCount += g2.shortMeasureCount;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.preArrangeTicks, Stopwatch.Frequency);
				preArrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.arrangeTicks, Stopwatch.Frequency);
				arrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.childArrangeTicks, Stopwatch.Frequency);
				childArrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.shortArrangeTicks, Stopwatch.Frequency);
				shortArrangeNanoSecsTotal += nanoSecs;
				shortArrangeCount += g2.shortArrangeCount;

				win2.Width = winWidth;
				win2.Height = winHeight;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.preMeasureTicks, Stopwatch.Frequency);
				preMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.measureTicks, Stopwatch.Frequency);
				measureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.childMeasureTicks, Stopwatch.Frequency);
				childMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.postMeasureTicks, Stopwatch.Frequency);
				postMeasureNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.shortMeasureTicks, Stopwatch.Frequency);
				shortMeasureNanoSecsTotal += nanoSecs;
				shortMeasureCount += g2.shortMeasureCount;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.preArrangeTicks, Stopwatch.Frequency);
				preArrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.arrangeTicks, Stopwatch.Frequency);
				arrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.childArrangeTicks, Stopwatch.Frequency);
				childArrangeNanoSecsTotal += nanoSecs;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.shortArrangeTicks, Stopwatch.Frequency);
				shortArrangeNanoSecsTotal += nanoSecs;
				shortArrangeCount += g2.shortArrangeCount;

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
				gridConstructorNanoSecsTotal += nanoSecs;

				layoutGridMeasureNanoSecs += g2.measureNanoSecsTotal;
				layoutGridArrangeNanoSecs += g2.arrangeNanoSecsTotal;
				layoutGridMeasureCount += g2.measureCount;
				layoutGridArrangeCount += g2.arrangeCount;
				win2.Close();

				PerfStatic.DoGCCollect();
				WindowPlain win1 = new WindowPlain(g1);
				win1.Show();
				win1.Width = winWidth; // does this change the grid's width?
				win1.Height = winHeight;
				gridMeasureNanoSecs += g1.measureNanoSecsTotal;
				gridArrangeNanoSecs += g1.arrangeNanoSecsTotal;
				gridMeasureCount += g1.measureCount;
				gridArrangeCount += g1.arrangeCount;
				win1.Close();

				seed++;

				if (seed == 1155)
				{
					string contents = "";

					contents += "Grid1 Contructor Nano Secs=" + gridConstructorNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Contructor Nano Secs=" + layoutGridConstructorNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Measure Nano Secs=" + gridMeasureNanoSecs.ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Measure Nano Secs=" + layoutGridMeasureNanoSecs.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Arrange Nano Secs=" + gridArrangeNanoSecs.ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Arrange Nano Secs=" + layoutGridArrangeNanoSecs.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Measure Count=" + gridMeasureCount.ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Measure Count=" + layoutGridMeasureCount.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Arrange Count=" + gridArrangeCount.ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Arrange Count=" + layoutGridArrangeCount.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "showCount=" + showCount.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Constructor Nano Secs Avg=" + (gridConstructorNanoSecsTotal / Math.Max(showCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Constructor Nano Secs Avg=" + (layoutGridConstructorNanoSecsTotal / Math.Max(showCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Measure Nano Secs Avg=" + (gridMeasureNanoSecs / Math.Max(gridMeasureCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Measure Nano Secs Avg=" + (layoutGridMeasureNanoSecs / Math.Max(layoutGridMeasureCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					contents += "Grid1 Arrange Nano Secs Avg=" + (gridArrangeNanoSecs / Math.Max(gridArrangeCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += "Grid2 Arrange Nano Secs Avg=" + (layoutGridArrangeNanoSecs / Math.Max(layoutGridArrangeCount, 1)).ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					double nonChildMeasureNanoSecsTotal = preMeasureNanoSecsTotal + measureNanoSecsTotal + postMeasureNanoSecsTotal + shortMeasureNanoSecsTotal - childMeasureNanoSecsTotal;
					contents += "Pre-Measure Nano Secs=" + preMeasureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Measure Nano Secs=" + measureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Child-Measure Nano Secs=" + childMeasureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Non-Child-Measure Nano Secs=" + nonChildMeasureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Post-Measure Nano Secs=" + postMeasureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Short-Measure Nano Secs=" + shortMeasureNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Short-Measure Count=" + shortMeasureCount.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					double nonChildArrangeNanoSecsTotal = preArrangeNanoSecsTotal + arrangeNanoSecsTotal + shortArrangeNanoSecsTotal - childArrangeNanoSecsTotal;
					contents += "Pre-Arrange Nano Secs=" + preArrangeNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Arrange Nano Secs=" + arrangeNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Child-Arrange Nano Secs=" + childArrangeNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Non-Child-Arrange Nano Secs=" + nonChildArrangeNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Short-Arrange Nano Secs=" + shortArrangeNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
					contents += "Short-Arrange Count=" + shortArrangeCount.ToString("#,##0") + Environment.NewLine;
					contents += Environment.NewLine;

					double totalMeasureNanoSecs = preMeasureNanoSecsTotal + measureNanoSecsTotal + postMeasureNanoSecsTotal + shortMeasureNanoSecsTotal;
					contents += "Child-Measure %=" + (childMeasureNanoSecsTotal / totalMeasureNanoSecs).ToString("#,##0.0000%") + Environment.NewLine;

					double totalArrangeNanoSecs = preArrangeNanoSecsTotal + arrangeNanoSecsTotal + shortArrangeNanoSecsTotal;
					contents += "Child-Arrange %=" + (childArrangeNanoSecsTotal / totalArrangeNanoSecs).ToString("#,##0.0000%") + Environment.NewLine;

					File.WriteAllText(@"E:\Proj\SpreadsheetOut\GridPerf.txt", contents);

					//??? write out results to text file and open text file - run in release mode from cmd line or windows explorer
					// change the order to do win2 first and try again
					// make sure the resize actually calls measureoverride and arrangeoverride again - do the grids get resized?
					// another way to test speed is just do one Grid and then the other and measure the entire thing
					break;
				}
			}
		}

		private void btnTestComplex_Click(object sender, RoutedEventArgs e)
		{
			TestComplexLayout_LayoutGrid swin = new TestComplexLayout_LayoutGrid();
			swin.Show();

			TestComplexLayout_Grid win = new TestComplexLayout_Grid();
			win.Show();
		}
	}
}
