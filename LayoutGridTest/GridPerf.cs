using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Motvin.LayoutGrid;

namespace LayoutGridTest
{
	public class PerfGrid : Grid
	{
		public double measureNanoSecsTotal = 0;
		public double arrangeNanoSecsTotal = 0;

		public int measureCount = 0;
		public int arrangeCount = 0;

		protected override Size MeasureOverride(Size availableSize)
		{
			long startTicks = Stopwatch.GetTimestamp();

			Size sz = base.MeasureOverride(availableSize);

			long endTicks = Stopwatch.GetTimestamp();

			double ticks = (double)(endTicks - startTicks);

			double nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
			measureNanoSecsTotal += nanoSecs;

			measureCount++;

			return sz;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			long startTicks = Stopwatch.GetTimestamp();

			Size sz = base.ArrangeOverride(finalSize);

			long endTicks = Stopwatch.GetTimestamp();

			double ticks = (double)(endTicks - startTicks);

			double nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
			arrangeNanoSecsTotal += nanoSecs;

			arrangeCount++;

			return sz;
		}
	}

	public class PerfSectionGrid : LayoutGrid
	{
		public double measureNanoSecsTotal = 0;
		public double arrangeNanoSecsTotal = 0;

		public int measureCount = 0;
		public int arrangeCount = 0;

		protected override Size MeasureOverride(Size availableSize)
		{
			long startTicks = Stopwatch.GetTimestamp();

			Size sz = base.MeasureOverride(availableSize);

			long endTicks = Stopwatch.GetTimestamp();

			double ticks = (double)(endTicks - startTicks);

			double nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
			measureNanoSecsTotal += nanoSecs;

			measureCount++;

			return sz;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			long startTicks = Stopwatch.GetTimestamp();

			Size sz = base.ArrangeOverride(finalSize);

			long endTicks = Stopwatch.GetTimestamp();

			double ticks = (double)(endTicks - startTicks);

			double nanoSecs = PerfStatic.GetNanoSecondsFromTicks(ticks, Stopwatch.Frequency);
			arrangeNanoSecsTotal += nanoSecs;

			arrangeCount++;

			return sz;
		}
	}

	public static class PerfStatic
	{
		public static double GetNanoSecondsFromTicks(double ticks, long ticksPerSec)
		{
			double nanoSecPerTick = (1_000_000_000.0) / ticksPerSec;
			return nanoSecPerTick * ticks;
		}

		public static void DoGCCollect()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}
