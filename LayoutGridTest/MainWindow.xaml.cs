using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.IO;

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
			bool useInfinite = chkInfinite.IsChecked??true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked.Value;
			RunRandManyChildren(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);
			RunRandSpans(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);
			RunRand2(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);
			RunRandStars(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);
			RunRandStarsNoZeroMax(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);
			RunRandDifferentControls(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnRandManyChildren_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRandManyChildren(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnRandSpans_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRandSpans(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnRand2_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRand2(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnRandStars_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRandStars(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnRandStars2_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRandStarsNoZeroMax(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnDifferentControls_Click(object sender, RoutedEventArgs e)
		{
			bool useInfinite = chkInfinite.IsChecked ?? true;
			bool useInfiniteMix = chkInfiniteMix.IsChecked ?? true;
			bool useInnerGrids = chkInnerGrids.IsChecked ?? true;
			RunRandDifferentControls(lblStatus, useInfinite, useInfiniteMix, useInnerGrids);

			lblStatus.Content = "Done Running Tests";
		}

		private void btnPerf1_Click(object sender, RoutedEventArgs e)
		{
			RunPerformanceTest();

			lblStatus.Content = "Done Running Performance Tests";
		}

		public static void GetRandInfinite(int seed, bool useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight)
		{
			useInfiniteWidth = false;
			useInfiniteHeight = false;

			if (useInfinite)
			{
				Random rand = new Random(seed);

				if (rand.NextInc(1, 2) == 1)
				{
					useInfiniteWidth = true;
				}

				if (rand.NextInc(1, 2) == 1)
				{
					useInfiniteHeight = true;
				}
			}
		}

		public static int RunRandManyChildren(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}

			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Many Children: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

				if (seed == 15 || // span expands pixel sized col/row for Grid, it should not
					seed == 45 || // span expands pixel sized col/row for Grid, it should not
					(seed == 60 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					seed == 87 || // span expands pixel sized col/row for Grid, it should not
					seed == 181 || // span expands pixel sized col/row for Grid, it should not
					(seed == 288 && useInfinite) || // span expands pixel sized col/row for Grid, it should not
					seed == 382 || // span expands pixel sized col/row for Grid, it should not
					seed == 401 || // span expands pixel sized col/row for Grid, it should not
					seed == 538 || // Grid is wrong, we expand the last 2 cols evenly, but for some reason Grid does not
					(seed == 610 && useInfinite) || // span order
					seed == 654 || // span expands pixel sized col/row for Grid, it should not
					seed == 677 || // span expands pixel sized ccol/row for Grid, it should not
					(seed == 731 && useInfinite) || // span expands pixel sized ccol/row for Grid, it should not
					seed == 807 || // span expands pixel sized col/row for Grid, it should not
					seed == 815 || // span expands pixel sized col/row for Grid, it should not
					seed == 898 || // span expands pixel sized col/row for Grid, it should not, also there are 2 star sized cols that aren't proportional in Grid for some reason - this must be a bug in Grid, not sure how
					seed == 905 || // span expands pixel sized col/row for Grid, it should not
					seed == 959 || // span expands pixel sized col/row for Grid, it should not
					seed == 960 || // span expands pixel sized col/row for Grid, it should not
					seed == 975 || // span expands pixel sized col/row for Grid, it should not
					seed == 993 || // span expands pixel sized col/row for Grid, it should not
					(seed == 1060 && useInfinite) || // span order
					seed == 1087 || // span expands pixel sized col/row for Grid, it should not
					seed == 1155 || // span expands pixel sized col/row for Grid, it should not
									// next seed == 1289
					1 == 2
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed >= 1183)
				{
					break;
				}

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGrid(g1, seed, useInnerGrids);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				WindowPlain win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Many Children";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win2.Title = "Test Grid 2 Many Children";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
					testCount++;
				}
				else
				{
					int decPlaces = 2;
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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
			return testCount;
		}

		public static int RunRandSpans(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}

			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Spans: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

				if (
					(seed == 5 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 12 && useInfinite) || // We expand a row after the min, not sure this is correct??? - could expand more evenly like Grid does
					(seed == 25 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 36 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 70 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					seed == 88 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed - this is because of resolving spans in a different order and possibly also not resolving the col width of children with rowspans before the width of any colspan children
					seed == 94 || // span order
					(seed == 100 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 117 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 125 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 135 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 139 && useInfinite) || // different ways of distributing span
					(seed == 142 && useInfinite) || // different ways of distributing span
					(seed == 155 && useInfinite) || // Grid is wrong, we expand the last 2 rows evenly, but for some reason Grid does not
					(seed == 156 && useInfinite) || // different ways of distributing span
					seed == 157 || // we are correct, Grid does not respect max size and goes past it for a colspan???
					(seed == 159 && useInfinite) || // different ways of distributing span - it's interesting that the span is completely distributed into the star row instead of any auto rows - this is probably on purpose - not sure what the reasoning is behind it
					seed == 163 || // span order
					seed == 167 || // span order
					seed == 190 || // Grid does not evenly expand auto cols for a colspan for some reason
					seed == 245 || // span order
					seed == 255 || // span order
					seed == 324 || // this is an auto width that goes beyond the max, we constrain the desired/rendered size to max, but Grid does not, which chops off the border???
					seed == 348 || // this is 0* treated like pixel sizing
					seed == 367 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed
					seed == 371 || // this is 0* treated like pixel sizing
					seed == 381 || // span order
					seed == 384 || // this is 0* treated like pixel sizing
					seed == 433 || // span order
					seed == 450 || // this is 0* treated like pixel sizing
					seed == 467 || // span order
					seed == 470 || // this is 0* treated like pixel sizing
					seed == 480 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max???
					seed == 493 || // this is 0* treated like pixel sizing
					seed == 498 || // this is 0* treated like pixel sizing
					seed == 521 || // this is 0* treated like pixel sizing
					seed == 525 || // this is 0* treated like pixel sizing
					seed == 555 || // we are correct, Grid does not expand both cols evenly - not sure why
					seed == 574 || // this is 0 * treated like pixel sizing
					seed == 596 || // this is 0 * treated like pixel sizing
					seed == 597 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max???
					seed == 599 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max???
					seed == 689 || // Grid is wrong, it distributes extra space to a col in a colspan when that extra space is not needed
					seed == 729 || // Grid is wrong.  We use the extra space to not distribute more extra if not needed when overlapping spans.
					seed == 808 || // There is a rowspan(no colspan) with a higher cellgroup than the colspan that overlaps this col, the colspan is done first, although you could do the rowspan (only the single col width part of it) first - but it's probably not a big deal and we don't want to be calling measure more times than 1 per child when we don't have to
					seed == 835 || // span order
					seed == 856 || // we are correct in that we expand the auto cols equally, and Grid doesn't for some reason
					seed == 944 || // different ordering, but if we have spanextra and then we set a width/height without a span, then this should be subtracted from the extra (down to 0)???
					seed == 1029 || // we are correct, Grid does not respect max size and goes past it???
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

				if (seed > 159 && useInfinite)
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

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				WindowPlain win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Spans";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win2.Title = "Test Grid 2 Spans";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
					testCount++;
				}
				else
				{
					Workbook wb = null;
					int decPlaces = 2;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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
			return testCount;
		}

		public static int RunRand2(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}
			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Rand 2: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

				if (
					seed == 49 || // span expands pixel sized col/row for Grid, it should not
					seed == 268 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					seed == 358 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					seed == 674 || // this is strange in that Grid somehow increases a column from a spanned col (but there is a star col in the span) and also the increase is completely unnecessary
					seed == 668 || // we distribute to the auto and star row (which is effectively auto) evenly, where Grid does not
					(seed == 859 && useInfinite) || // different ways of distributing span
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

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				WindowPlain win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Rand 2";
				win1.Show();

				WindowPlain win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win2.Title = "Test Grid 2 Rand 2";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				StringBuilder sb1 = GridLog.CreateGridString(g1, seed);
				StringBuilder sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					win1.Close();
					win2.Close();
					testCount++;
				}
				else
				{
					int decPlaces = 2;
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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
			return testCount;
		}

		//[SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public static void DoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
				new DispatcherOperationCallback(ExitFrame), frame);
			Dispatcher.PushFrame(frame);
		}

		public static object ExitFrame(object f)
		{
			((DispatcherFrame)f).Continue = false;

			return null;
		}

		public static int RunRandStars(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}
			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Stars: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

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

				StringBuilder sb1;
				StringBuilder sb2;
				WindowPlain win1;
				WindowPlain win2;

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGridStars(g1, seed);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Stars";
				win1.Show();

				win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false;
				win2.Title = "Test Grid 2 Stars";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				sb1 = GridLog.CreateGridString(g1, seed);
				sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal))
				{
					testCount++;
					win1.Close();
					win2.Close();
				}
				else
				{
					int decPlaces = 2;
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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

			return testCount;
		}

		public static int RunRandStarsNoZeroMax(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}
			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Stars 2: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

				if (
					seed == 5266 || // Grid has a .01 difference in a star col, we are correct since we add up to the width
					seed == 5764 || // Grid has a .01 difference in a star col, we are correct since we add up to the width, Grid uses 99.2249999999999 instead of 99.225
					seed == 10721 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 12877 || // Grid has a .01 difference in a star col, we are correct since we add up to the width, Grid uses 51.7249999999999
					seed == 16724 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 19375 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 22903 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 23142 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 23611 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 25171 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					1 == 0
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed > 25171)
				{
					break;
				}

				StringBuilder sb1;
				StringBuilder sb2;
				WindowPlain win1;
				WindowPlain win2;

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGridStars(g1, seed, false);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Stars";
				win1.Show();

				win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false;
				win2.Title = "Test Grid 2 Stars";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				sb1 = GridLog.CreateGridString(g1, seed);
				sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				double starCountCol;
				double starCountRow;
				double g2Width = SumColWidthsOrRowHeights(g2, true, out starCountCol);
				double g2Height = SumColWidthsOrRowHeights(g2, false, out starCountRow);

				if (string.Equals(s1, s2, StringComparison.Ordinal)
					//&& (starCountCol == 0 || g2Width == g2.ActualWidth) && (starCountRow == 0 || g2Height == g2.ActualHeight)
					)
				{
					testCount++;
					win1.Close();
					win2.Close();
				}
				else
				{
					int decPlaces = 14;
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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

			return testCount;
		}

		public static int RunRandDifferentControls(Label lblStatus, bool useInfinite, bool useInfiniteMix, bool useInnerGrids)
		{
			int testCount = 0;
			int seed = 1;
			if (useInfiniteMix)
			{
				useInfinite = true;
			}
			while (true)
			{
				if ((testCount % 500) == 0)
				{
					// need to do this to free the memory held by Show and then Close windows
					lblStatus.Content = "Different Controls: test = " + testCount.ToString("N0");
					DoEvents();
					PerfStatic.DoGCCollect();
				}

				if (
					(seed == 235 && useInfinite) || // Grid expands spans beyond max col widths
					(seed == 604 && useInfinite) || // different span distributions
					(seed == 1171 && useInfinite) || // different span distributions
					(seed == 1603 && useInfinite) || // different span distributions
					(seed == 2306 && useInfinite) || // Grid expands pixel col
					(seed == 2830 && useInfinite) || // we respect max, Grid goes over max
					(seed == 3483 && useInfinite) || // we respect max, Grid goes over max
					(seed == 3542 && useInfinite) || // different span distributions
					(seed == 3655 && useInfinite) || // Grid expands pixel
					seed == 760 || // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					seed == 794 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max???
					seed == 901 || // Grid is wrong.  Some Auto cols have controls that have desired width and rendered width > max???
					seed == 1504 || // off by .01
					seed == 1966 || // off by .01
					seed == 3788 || // off by .01
					seed == 3830 || // Grid limits DesiredSize width for star children to min for some reason although RenderSize is the same for both - there is a strange thing where when resizing this screen down lbl20XX.... suddenly gets chopped for Grid, which visually looks very wrong - should highlight this as a thing that is wrong with Grid
					seed == 3994 || // Grid has span that expands pixel col
					seed == 4219 ||  // Grid is very wrong.  For some reason there is a min col and a col with no min/max and one col with a max and yet it can't figure out the correct ratios
					1 == 0
				)
				{
					seed++;
					continue; // these are cases where we are correct and Grid is wrong
				}

				if (seed > 4219)
				{
					break;
				}

				StringBuilder sb1;
				StringBuilder sb2;
				WindowPlain win1;
				WindowPlain win2;

				Grid g1 = new Grid();

				g1.Name = "grd";
				GridLog.SetupRandomGridDifferentControls(g1, seed, false);

				LayoutGrid g2 = new LayoutGrid();

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				GetRandInfinite(seed, useInfinite, out bool useInfiniteWidth, out bool useInfiniteHeight);

				win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
				//win1.ShowInTaskbar = false; // showing and closing windows seems to be faster with this set to false
				win1.Title = "Grid 1 Diff";
				win1.Show();

				win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
				//win2.ShowInTaskbar = false;
				win2.Title = "Test Grid 2 Diff";
				win2.Show();

				if (useInfiniteMix)
				{
					GetRandInfinite(seed, useInfinite, out useInfiniteWidth, out useInfiniteHeight);

					win1.Close();
					win2.Close();

					win1.outerGrid.Children.Remove(g1);
					win2.outerGrid.Children.Remove(g2);

					win1 = new WindowPlain(g1, useInfiniteWidth, useInfiniteHeight);
					win1.Show();

					win2 = new WindowPlain(g2, useInfiniteWidth, useInfiniteHeight);
					win2.Show();
				}

				sb1 = GridLog.CreateGridString(g1, seed);
				sb2 = GridLog.CreateGridString(g2, seed);

				string s1 = sb1.ToString();
				string s2 = sb2.ToString();

				if (string.Equals(s1, s2, StringComparison.Ordinal)
					)
				{
					testCount++;
					win1.Close();
					win2.Close();
				}
				else
				{
					int decPlaces = 14;
					Workbook wb = null;
					try
					{
						wb = new Workbook(spWorkbookFileFormat.OfficeOpenXML);

						GridLog.WriteGridToExcelCreateWorksheet(wb, g2, decPlaces);

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

						GridLog.WriteGridToExcelCreateWorksheet(wb, g1, decPlaces);

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

			return testCount;
		}

		//??? compare the result of this with the grid's actual width/height and flag any differences 
		public static double SumColWidthsOrRowHeights(LayoutGrid g, bool getCol, out double starCountNoMax)
		{
			double sumLength = 0;

			starCountNoMax = 0;

			if (getCol)
			{
				for (int i = 0; i < g.ColumnDefinitions.Count; i++)
				{
					sumLength += g.GetColWidth(i);

					ColumnDefinition c = g.ColumnDefinitions[i];
					if (c.Width.IsStar && double.IsPositiveInfinity(c.MaxWidth))
					{
						starCountNoMax += c.Width.Value;
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
						starCountNoMax += c.Height.Value;
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
			double childSortNanoSecsTotal = 0;
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

				if (seed % 100 == 0)
				{
					DoEvents();
					PerfStatic.DoGCCollect();
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
				GridLog.SetupRandomGrid(g1, seed, false);

				GridLog.CopyGridSetup(g1, g2);
				g2.Name = "grd";

				showCount++;

				PerfStatic.DoGCCollect();
				WindowPlain win2 = new WindowPlain(g2);
				//win2.ShowInTaskbar = false;
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

				nanoSecs = PerfStatic.GetNanoSecondsFromTicks(g2.childSortTicks, Stopwatch.Frequency);
				childSortNanoSecsTotal += nanoSecs;
				
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
				//win1.ShowInTaskbar = false;
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
					contents += "Child Sort Nano Secs=" + childSortNanoSecsTotal.ToString("#,##0") + Environment.NewLine;
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
			int testCount = 1;
			TestComplexLayout_LayoutGrid lgWin = new TestComplexLayout_LayoutGrid();
			lgWin.Width = 800;
			lgWin.Height = 500;
			lgWin.Show();

			TestComplexLayout_Grid win = new TestComplexLayout_Grid();
			win.Width = 800;
			win.Height = 500;
			win.Show();

			StringBuilder sb1 = GridLog.CreateGridString(win.gridx, 1);
			StringBuilder sb2 = GridLog.CreateGridString(lgWin.gridx, 1);

			string s1 = sb1.ToString();
			string s2 = sb2.ToString();

			if (!string.Equals(s1, s2, StringComparison.Ordinal)
				)
			{
				testCount++;
			}
		}

		private void btnGCCollect_Click(object sender, RoutedEventArgs e)
		{
			PerfStatic.DoGCCollect();
		}

		private void btnInner_Click(object sender, RoutedEventArgs e)
		{
			int testCount = 1;
			TestInner_LayoutGrid lgWin = new TestInner_LayoutGrid();
			lgWin.Width = 800;
			lgWin.Height = 500;
			lgWin.Show();

			TestInner_Grid win = new TestInner_Grid();
			win.Width = 800;
			win.Height = 500;
			win.Show();

			StringBuilder sb1 = GridLog.CreateGridString(win.gridx, 1);
			StringBuilder sb2 = GridLog.CreateGridString(lgWin.gridx, 1);

			string s1 = sb1.ToString();
			string s2 = sb2.ToString();

			if (!string.Equals(s1, s2, StringComparison.Ordinal)
				)
			{
				testCount++;
			}
		}
	}
}
