#define CollectPerformanceStats

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;

#if CollectPerformanceStats
using System.Diagnostics;
#endif

namespace Motvin.LayoutGrid
{
	public class LayoutGrid : Panel//, IAddChild // Panel implements IAddChild, so I'm not sure why it is implemented here??? - maybe it uses explicit interface ...?
	{
		//public static readonly DependencyProperty ShowGridLinesProperty;

		//??? i'm not sure that these properties affect both arrange and measure
		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached("Column", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached("Row", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty RowSpanProperty = DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public static readonly DependencyProperty IsSharedSizeScopeProperty = DependencyProperty.RegisterAttached("IsSharedSizeScope", typeof(bool), typeof(LayoutGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnDefCollection ColumnDefinitions { get; } = new ColumnDefCollection();

		// probably need to make these dependency properties that affect arrange and measure???
		public double ColumnSpacing { get; set; }
		public double RowSpacing { get; set; }

		public bool ShowGridLines { get; set; } //??? is this needed?

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RowDefCollection RowDefinitions { get; } = new RowDefCollection();

		protected override int VisualChildrenCount { get { return base.VisualChildrenCount; } } //??? maybe need to add in grid lines renderer if this is used, otherwise we don't have to override this at all

		//???protected override IEnumerator LogicalChildren { get; } // not sure why this is needed?

		[AttachedPropertyBrowsableForChildren]
		public static int GetColumn(UIElement element)
		{
			return (int)element.GetValue(LayoutGrid.ColumnProperty); //??? is it correct to cast to an int?
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetGridColumn(UIElement element)
		{
			return (int)element.GetValue(Grid.ColumnProperty);
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetColumnSpan(UIElement element)
		{
			return (int)element.GetValue(LayoutGrid.ColumnSpanProperty); //??? is it correct to cast to an int?
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetGridColumnSpan(UIElement element)
		{
			return (int)element.GetValue(Grid.ColumnSpanProperty);
		}

		public static bool GetIsSharedSizeScope(UIElement element)
		{
			return (bool)element.GetValue(LayoutGrid.IsSharedSizeScopeProperty); //??? is it correct to cast to a bool?
		}
		//
		// Summary:
		//     Gets the value of the System.Windows.Controls.Grid.Row attached property from
		//     a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element from which to read the property value.
		//
		// Returns:
		//     The value of the System.Windows.Controls.Grid.Row attached property.
		[AttachedPropertyBrowsableForChildren]
		public static int GetRow(UIElement element)
		{
			return (int)element.GetValue(LayoutGrid.RowProperty); //??? is it correct to cast to an int?
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetGridRow(UIElement element)
		{
			return (int)element.GetValue(Grid.RowProperty);
		}

		//
		// Summary:
		//     Gets the value of the System.Windows.Controls.Grid.RowSpan attached property
		//     from a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element from which to read the property value.
		//
		// Returns:
		//     The value of the System.Windows.Controls.Grid.RowSpan attached property.
		[AttachedPropertyBrowsableForChildren]
		public static int GetRowSpan(UIElement element)
		{
			return (int)element.GetValue(LayoutGrid.RowSpanProperty); //??? is it correct to cast to an int?
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetGridRowSpan(UIElement element)
		{
			return (int)element.GetValue(Grid.RowSpanProperty); //??? is it correct to cast to an int?
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.Grid.Column attached property to
		//     a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element on which to set the System.Windows.Controls.Grid.Column attached
		//     property.
		//
		//   value:
		//     The property value to set.
		public static void SetColumn(UIElement element, int value)
		{
			element.SetValue(LayoutGrid.ColumnProperty, value);
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.Grid.ColumnSpan attached property
		//     to a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element on which to set the System.Windows.Controls.Grid.ColumnSpan attached
		//     property.
		//
		//   value:
		//     The property value to set.
		public static void SetColumnSpan(UIElement element, int value)
		{
			element.SetValue(LayoutGrid.ColumnSpanProperty, value);
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.Grid.IsSharedSizeScope attached
		//     property to a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element on which to set the System.Windows.Controls.Grid.IsSharedSizeScope
		//     attached property.
		//
		//   value:
		//     The property value to set.
		public static void SetIsSharedSizeScope(UIElement element, bool value)
		{
			element.SetValue(LayoutGrid.IsSharedSizeScopeProperty, value);
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.Grid.Row attached property to a
		//     given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element on which to set the attached property.
		//
		//   value:
		//     The property value to set.
		public static void SetRow(UIElement element, int value)
		{
			element.SetValue(LayoutGrid.RowProperty, value);
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.Grid.RowSpan attached property
		//     to a given System.Windows.UIElement.
		//
		// Parameters:
		//   element:
		//     The element on which to set the System.Windows.Controls.Grid.RowSpan attached
		//     property.
		//
		//   value:
		//     The property value to set.
		public static void SetRowSpan(UIElement element, int value)
		{
			element.SetValue(LayoutGrid.RowSpanProperty, value);
		}

		//
		// Summary:
		//     Returns true if System.Windows.Controls.ColumnDefinitionCollection associated
		//     with this instance of System.Windows.Controls.Grid is not empty.
		//
		// Returns:
		//     true if System.Windows.Controls.ColumnDefinitionCollection associated with this
		//     instance of System.Windows.Controls.Grid is not empty; otherwise, false.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeColumnDefinitions()
		{
			return ColumnDefinitions.Count != 0;
		}

		//
		// Summary:
		//     Returns true if System.Windows.Controls.RowDefinitionCollection associated with
		//     this instance of System.Windows.Controls.Grid is not empty.
		//
		// Returns:
		//     true if System.Windows.Controls.RowDefinitionCollection associated with this
		//     instance of System.Windows.Controls.Grid is not empty; otherwise, false.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeRowDefinitions()
		{
			return RowDefinitions.Count != 0;
		}

		//??? maybe grid needs to implement these
		// the base version supports zindex, but maybe need to override this to display the gridlines
		//protected override Visual GetVisualChild(int index)
		//{
		//	return base.GetVisualChild(index);
		//	//return Children[index];
		//	//???return Children[(Children.Count - 1) - index];
		//}

		//??? maybe grid needs to implement these
		//protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		//{

		//}

		private const ushort ChildFlag_SpanHasAuto = 1; // this flag must reflect the unitTypes (not effectiveUnitTypes)
		private const ushort ChildFlag_SpanHasStar = 1 << 1; // this flag must reflect the unitTypes (not effectiveUnitTypes)
		private const ushort ChildFlag_SpanHasPixel = 1 << 2; // don't actually use this for anything
		private const ushort ChildFlag_SpanHasAutoNoStar = 1 << 3; // this flag must reflect the effectiveUnitTypes
		private const ushort ChildFlag_IsLastOfSameSpan = 1 << 4;

		private const ushort ChildFlag_SpanHasAutoNoStar_Reset = (ushort)0xffff ^ ChildFlag_SpanHasAutoNoStar;

		[Flags]
		public enum LayoutGridUnitType : ushort
		{
			Auto = ChildFlag_SpanHasAuto,
			Star = ChildFlag_SpanHasStar,
			Pixel = ChildFlag_SpanHasPixel
		}

		private struct GridColRowInfo
		{
			public double constrainedPixelLength; // this can be set again in MeasureOverride and ArrangeOverride
			public double spanExtraLengthOrPosition; // this has a dual purpose as spanExtraLength (within MeasureOverride) and position (within ArrangeOverride)

			public double minPixelLength;
			public double maxPixelLength;

			//??? remove this, or if more efficient keep it and remove stars
			public double starLengthPercent; // this is the star length provided by the user and then later converted to total star length percent (i.e. if 1 originally and total stars = 4, then this becomes 1/4 = .25)

			public double stars;

			public LayoutGridUnitType unitType;
			public LayoutGridUnitType effectiveUnitType; // star unit types can become auto if the available width or height is infinite - this can be set again in MeasureOverride - probably we can remove this and just make the logic slightly more complicated???

			public bool isResolved; // probably this bool will be 4 bytes and the entire struct will be 8 * 6 = 48 bytes
		}

		internal struct ChildInfo
		{
			// this struct takes 32 bytes (if compiled 64 bits), which is on an 8 byte boundary
			// it takes 28 bytes (if compiled 32 bits), which is on a 4 byte boundary
			// therefore it shouldn't require any padding when this struct is used in an array

			public UIElement child; // 4 or 8 bytes
			public int col; // 4 bytes
			public int row; // 4 bytes
			public int colSpan; // 4 bytes
			public int rowSpan; // 4 bytes

			public int cellGroup; // 4 bytes

			public ushort colFlags; // 2 bytes
			public ushort rowFlags; // 2 bytes
		}

		private struct StarMinMax
		{
			public int index; // the col or row index
			public double minOrMaxPerStar; // this is minPixelLength or maxPixelLength divided by # of stars for the col/row
		}

		private class CompareStarMinMaxByPerStar : IComparer<StarMinMax>
		{
			public int Compare(StarMinMax x, StarMinMax y)
			{
				return x.minOrMaxPerStar.CompareTo(y.minOrMaxPerStar);
			}
		}

		private class CompareStarMinMaxByPerStarDesc : IComparer<StarMinMax>
		{
			public int Compare(StarMinMax x, StarMinMax y)
			{
				return -x.minOrMaxPerStar.CompareTo(y.minOrMaxPerStar);
			}
		}

		private class CompareChildInfoByCellGroup : IComparer<ChildInfo>
		{
			public int Compare(ChildInfo x, ChildInfo y)
			{
				//return x.cellGroup - y.cellGroup;//??? maybe we just want this, also cell groups should not have span stuff? - also the span stuff is ordered below, so maybe we don't need cell groups with span

				int ret = x.cellGroup - y.cellGroup;
				if (ret == 0 && (x.cellGroup == CellGroup_PixelOrAutoColRowWithSpan || x.cellGroup == CellGroup_AutoColStarRowWithSpan || x.cellGroup == CellGroup_PixelOrStarColStarRowWithSpan))
				{
					ret = x.col - y.col;
					if (ret == 0)
					{
						ret = x.colSpan - y.colSpan;

						// sort by row/rowspan because we need cells with no row spans (rowSpan = 1) to appear before those that do have rowspans - I don't think this is true???
						//if (ret == 0)
						//{
						//	ret = x.row - y.row;
						//	if (ret == 0)
						//	{
						//		ret = x.rowSpan - y.rowSpan;
						//	}
						//}
					}
				}
				return ret;
			}
		}

		//??? add a few more groups - do we actually need the WithSpan groups if s
		private const int CellGroup_PixelOrAutoColRow = 0;
		private const int CellGroup_PixelOrAutoColRowWithSpan = 1; // maybe make 2 one with a col span and the ones with just rowspan and no colspan aren't in it???
		private const int CellGroup_AutoColStarRow = 2;
		private const int CellGroup_AutoColStarRowWithSpan = 3;
		private const int CellGroup_StarColPixelOrAutoRow = 4;
		private const int CellGroup_StarColPixelOrAutoRowWithSpan = 5;
		private const int CellGroup_PixelOrStarColStarRow = 6;
		private const int CellGroup_PixelOrStarColStarRowWithSpan = 7;
		private const int CellGroup_AfterLast = 8; // this isn't a real cell group

#if CollectPerformanceStats
		public long preMeasureTicks;
		public long measureTicks;
		public long postMeasureTicks;
		public long shortMeasureTicks;
		public long childMeasureTicks;
		public int shortMeasureCount;

		public long childSortTicks;

		public long preArrangeTicks;
		public long arrangeTicks;
		public long shortArrangeTicks;
		public long childArrangeTicks;
		public int shortArrangeCount;
#endif

		private CompareStarMinMaxByPerStar compareStarMinMaxByPerStar = new CompareStarMinMaxByPerStar(); //??? maybe create only if needed
		private CompareStarMinMaxByPerStarDesc compareStarMinMaxByPerStarDesc = new CompareStarMinMaxByPerStarDesc(); //??? maybe create only if needed
		private CompareChildInfoByCellGroup compareChildInfoByCellGroup;

		private bool haveColsChanged;
		private bool haveRowsChanged;
		private bool haveChildrenChanged;

		private bool haveStarColAndAutoRowChildren; // this doesn't need to change if infinite width forces star

		private bool wasInfiniteWidth;
		private bool wasInfiniteHeight;

		private int starColCount;
		private int starRowCount;

		private double totalStarsInCols;
		private double totalStarsInRows;

		private double totalPixelColWidth; // this is the total width (constrained) for all Pixel (absolute) sized cols
		private double totalPixelRowHeight; // this is the total height (constrained) for all Pixel (absolute) sized rows

		private double totalAutoColWidth;
		private double totalAutoRowHeight;

		private double totalMinStarColWidth;
		private double totalMaxStarColWidth;

		private double totalMinStarRowHeight;
		private double totalMaxStarRowHeight;

		private GridColRowInfo[] colInfoArray;
		private int colInfoArrayCount;

		private GridColRowInfo[] rowInfoArray;
		private int rowInfoArrayCount;

		private StarMinMax[] starColMinArray;
		private int starColMinArrayCount;

		private StarMinMax[] starColMaxArray; // the count of elements for this is starColCount

		private StarMinMax[] starRowMinArray;
		private int starRowMinArrayCount;

		private StarMinMax[] starRowMaxArray; // the count of elemens for this is starRowCount

		internal ChildInfo[] childInfoArray;
		internal int childInfoArrayCount; // the # of elems in the array that are being used

		int firstIndexForOtherCellGroups; // the first index of childInfoArray of cell groups > 0

		double desiredWidth;
		double desiredHeight;

		//??? remove this function when replaced RowDefinition to LayoutRowDefinition, same for col
		private LayoutGridUnitType GridUnitTypeToLayout(GridUnitType gridUnitType)
		{
			LayoutGridUnitType layoutUnitType;
			switch (gridUnitType)
			{
				case GridUnitType.Auto:
					layoutUnitType = LayoutGridUnitType.Auto;
					break;
				case GridUnitType.Pixel:
					layoutUnitType = LayoutGridUnitType.Pixel;
					break;
				default:
					layoutUnitType = LayoutGridUnitType.Star;
					break;
			}
			return layoutUnitType;
		}

		private void CreateColInfo(bool isInfinite)
		{
			colInfoArrayCount = ColumnDefinitions.Count;
			if (colInfoArrayCount == 0)
			{
				colInfoArrayCount++;
			}

			if (colInfoArray == null || colInfoArray.Length < colInfoArrayCount)
			{
				colInfoArray = new GridColRowInfo[colInfoArrayCount];
			}
			else
			{
				Array.Clear(colInfoArray, 0, colInfoArrayCount); // clear so that we don't have to set members to default values because we know they will be 0, false, etc.
			}

			totalPixelColWidth = 0;

			int starColCountMinNotZero = 0;

			double largestStarMinPixelColWidth = 0;
			double smallestStarMaxPixelColWidth = double.PositiveInfinity;

			if (ColumnDefinitions.Count == 0)
			{
				// no items in ColumnDefinitions means there is a single col that is 1*, this is how Grid works
				ref GridColRowInfo cr = ref colInfoArray[0];

				cr.unitType = LayoutGridUnitType.Star;
				if (isInfinite)
				{
					cr.effectiveUnitType = LayoutGridUnitType.Auto;
				}
				else
				{
					cr.effectiveUnitType = LayoutGridUnitType.Star;
				}

				cr.stars = 1.0;
				cr.starLengthPercent = 1.0;
				cr.maxPixelLength = double.PositiveInfinity;

				totalStarsInCols = 1.0;
				starColCount = 1;
				totalMaxStarColWidth = double.PositiveInfinity;
			}
			else
			{
				totalStarsInCols = 0;
				starColCount = 0;

				for (int i = 0; i < colInfoArrayCount; i++)
				{
					//??? don't use ColumnDefinition, use LayoutGridColumnDefinition
					ColumnDefinition c = ColumnDefinitions[i];

					ref GridColRowInfo cr = ref colInfoArray[i];

					cr.unitType = GridUnitTypeToLayout(c.Width.GridUnitType);
					if (isInfinite && cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Auto;
					}
					else
					{
						cr.effectiveUnitType = cr.unitType;
					}

					if (c.MinWidth < c.MaxWidth)
					{
						cr.minPixelLength = c.MinWidth;
						cr.maxPixelLength = c.MaxWidth;
					}
					else
					{
						// set both min and max to min - this is how Grid works - it uses min for both
						cr.minPixelLength = c.MinWidth;
						cr.maxPixelLength = c.MinWidth;
					}

					switch (cr.unitType) // use unitType instead of effectiveUnitType here
					{
						case LayoutGridUnitType.Pixel:
							if (c.Width.Value > cr.maxPixelLength)
							{
								cr.constrainedPixelLength = cr.maxPixelLength;
							}
							else if (c.Width.Value < cr.minPixelLength)
							{
								cr.constrainedPixelLength = cr.minPixelLength;
							}
							else
							{
								cr.constrainedPixelLength = c.Width.Value;
							}

							totalPixelColWidth += cr.constrainedPixelLength;
							break;
						case LayoutGridUnitType.Star:
							if (c.Width.Value == 0)
							{
								// treat this like a pixel size with a pixel length = min

								cr.unitType = LayoutGridUnitType.Pixel;
								cr.effectiveUnitType = LayoutGridUnitType.Pixel;
								cr.constrainedPixelLength = cr.minPixelLength;

								totalPixelColWidth += cr.constrainedPixelLength;
							}
							else
							{
								if (cr.maxPixelLength < smallestStarMaxPixelColWidth)
								{
									smallestStarMaxPixelColWidth = cr.maxPixelLength;
								}

								if (cr.minPixelLength > largestStarMinPixelColWidth)
								{
									largestStarMinPixelColWidth = cr.minPixelLength;
								}

								starColCount++;
								cr.stars = c.Width.Value;
								cr.starLengthPercent = c.Width.Value;
								if (cr.minPixelLength != 0)
								{
									starColCountMinNotZero++;
									totalMinStarColWidth += cr.minPixelLength;
								}
								totalMaxStarColWidth += cr.maxPixelLength;
								totalStarsInCols += cr.starLengthPercent;
							}
							break;
					}
				}
			}

			starColMinArrayCount = starColCountMinNotZero;

			if (starColCount > 0)
			{
				if (starColMaxArray == null || starColMaxArray.Length < starColCount)
				{
					starColMaxArray = new StarMinMax[starColCount];
				}

				if (starColMinArrayCount > 0 && (starColMinArray == null || starColMinArray.Length < starColMinArrayCount))
				{
					starColMinArray = new StarMinMax[starColMinArrayCount];
				}

				StarMinMax s;
				int starIndexMax = 0;
				int starIndexMin = 0;
				for (int i = 0; i < colInfoArrayCount; i++)
				{
					ref GridColRowInfo cr = ref colInfoArray[i];

					if (cr.effectiveUnitType == LayoutGridUnitType.Star)
					{
						cr.starLengthPercent /= totalStarsInCols; // convert from star length to star length percent

						s.index = i;
						if (double.IsPositiveInfinity(cr.maxPixelLength))
						{
							s.minOrMaxPerStar = double.PositiveInfinity;
						}
						else
						{
							s.minOrMaxPerStar = cr.maxPixelLength / cr.stars;
						}

						starColMaxArray[starIndexMax++] = s;

						if (starColMinArrayCount > 0 && cr.minPixelLength > 0)
						{
							s.minOrMaxPerStar = cr.minPixelLength / cr.stars;

							starColMinArray[starIndexMin++] = s;
						}
					}
				}

				if (starColCount > 1 && !double.IsPositiveInfinity(smallestStarMaxPixelColWidth))
				{
					Array.Sort(starColMaxArray, 0, starColCount, compareStarMinMaxByPerStar);
				}

				if (starColMinArrayCount > 1 && largestStarMinPixelColWidth > 0)
				{
					Array.Sort(starColMinArray, 0, starColMinArrayCount, compareStarMinMaxByPerStarDesc);
				}
			}

			haveColsChanged = false; // set this to false because we got all of the col info in this function
		}

		private void CreateRowInfo(bool isInfinite)
		{
			rowInfoArrayCount = RowDefinitions.Count;
			if (rowInfoArrayCount == 0)
			{
				rowInfoArrayCount++;
			}

			if (rowInfoArray == null || rowInfoArray.Length < rowInfoArrayCount)
			{
				rowInfoArray = new GridColRowInfo[rowInfoArrayCount];
			}
			else
			{
				Array.Clear(rowInfoArray, 0, rowInfoArrayCount); // clear so that we don't have to set members to default values because we know they will be 0, false, etc.
			}

			totalPixelRowHeight = 0;

			int starRowCountMinNotZero = 0;

			double largestStarMinPixelRowHeight = 0;
			double smallestStarMaxPixelRowHeight = double.PositiveInfinity;

			if (RowDefinitions.Count == 0)
			{
				// no items in RowumnDefinitions means there is a single row that is 1*, this is how Grid works
				ref GridColRowInfo cr = ref rowInfoArray[0];

				cr.unitType = LayoutGridUnitType.Star;
				if (isInfinite)
				{
					cr.effectiveUnitType = LayoutGridUnitType.Auto;
				}
				else
				{
					cr.effectiveUnitType = LayoutGridUnitType.Star;
				}

				cr.stars = 1.0;
				cr.starLengthPercent = 1.0;
				cr.maxPixelLength = double.PositiveInfinity;

				totalStarsInRows = 1.0;
				starRowCount = 1;
				totalMaxStarRowHeight = double.PositiveInfinity;
			}
			else
			{
				totalStarsInRows = 0;
				starRowCount = 0;

				for (int i = 0; i < rowInfoArrayCount; i++)
				{
					//??? don't use RowumnDefinition, use LayoutGridRowumnDefinition
					RowDefinition c = RowDefinitions[i];

					ref GridColRowInfo cr = ref rowInfoArray[i];

					cr.unitType = GridUnitTypeToLayout(c.Height.GridUnitType);
					if (isInfinite && cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Auto;
					}
					else
					{
						cr.effectiveUnitType = cr.unitType;
					}

					if (c.MinHeight < c.MaxHeight)
					{
						cr.minPixelLength = c.MinHeight;
						cr.maxPixelLength = c.MaxHeight;
					}
					else
					{
						// set both min and max to min - this is how Grid works - it uses min for both
						cr.minPixelLength = c.MinHeight;
						cr.maxPixelLength = c.MinHeight;
					}

					switch (cr.unitType) // use unitType instead of effectiveUnitType here
					{
						case LayoutGridUnitType.Pixel:
							if (c.Height.Value > cr.maxPixelLength)
							{
								cr.constrainedPixelLength = cr.maxPixelLength;
							}
							else if (c.Height.Value < cr.minPixelLength)
							{
								cr.constrainedPixelLength = cr.minPixelLength;
							}
							else
							{
								cr.constrainedPixelLength = c.Height.Value;
							}

							totalPixelRowHeight += cr.constrainedPixelLength;
							break;
						case LayoutGridUnitType.Star:
							if (c.Height.Value == 0)
							{
								// treat this like a pixel size with a pixel length = min

								cr.unitType = LayoutGridUnitType.Pixel;
								cr.effectiveUnitType = LayoutGridUnitType.Pixel;
								cr.constrainedPixelLength = cr.minPixelLength;

								totalPixelRowHeight += cr.constrainedPixelLength;
							}
							else
							{
								if (cr.maxPixelLength < smallestStarMaxPixelRowHeight)
								{
									smallestStarMaxPixelRowHeight = cr.maxPixelLength;
								}

								if (cr.minPixelLength > largestStarMinPixelRowHeight)
								{
									largestStarMinPixelRowHeight = cr.minPixelLength;
								}

								starRowCount++;
								cr.stars = c.Height.Value;
								cr.starLengthPercent = c.Height.Value;
								if (cr.minPixelLength != 0)
								{
									starRowCountMinNotZero++;
									totalMinStarRowHeight += cr.minPixelLength;
								}
								totalMaxStarRowHeight += cr.maxPixelLength;
								totalStarsInRows += cr.starLengthPercent;
							}
							break;
					}
				}
			}

			starRowMinArrayCount = starRowCountMinNotZero;

			if (starRowCount > 0)
			{
				if (starRowMaxArray == null || starRowMaxArray.Length < starRowCount)
				{
					starRowMaxArray = new StarMinMax[starRowCount];
				}

				if (starRowMinArrayCount > 0 && (starRowMinArray == null || starRowMinArray.Length < starRowMinArrayCount))
				{
					starRowMinArray = new StarMinMax[starRowMinArrayCount];
				}

				StarMinMax s;
				int starIndexMax = 0;
				int starIndexMin = 0;
				for (int i = 0; i < rowInfoArrayCount; i++)
				{
					ref GridColRowInfo cr = ref rowInfoArray[i];

					if (cr.effectiveUnitType == LayoutGridUnitType.Star)
					{
						cr.starLengthPercent /= totalStarsInRows; // convert from star length to star length percent

						s.index = i;
						if (double.IsPositiveInfinity(cr.maxPixelLength))
						{
							s.minOrMaxPerStar = double.PositiveInfinity;
						}
						else
						{
							s.minOrMaxPerStar = cr.maxPixelLength / cr.stars;
						}

						starRowMaxArray[starIndexMax++] = s;

						if (starRowMinArrayCount > 0 && cr.minPixelLength > 0)
						{
							s.minOrMaxPerStar = cr.minPixelLength / cr.stars;

							starRowMinArray[starIndexMin++] = s;
						}
					}
				}

				if (starRowCount > 1 && !double.IsPositiveInfinity(smallestStarMaxPixelRowHeight))
				{
					Array.Sort(starRowMaxArray, 0, starRowCount, compareStarMinMaxByPerStar);
				}

				if (starRowMinArrayCount > 1 && largestStarMinPixelRowHeight > 0)
				{
					Array.Sort(starRowMinArray, 0, starRowMinArrayCount, compareStarMinMaxByPerStarDesc);
				}
			}

			haveRowsChanged = false; // set this to false because we got all of the row info in this function
		}

		private static void SetupColRowInfo(GridColRowInfo[] infoArray, int infoArrayCount, bool isInfinite)
		{
			if (isInfinite)
			{
				// change any star size rows to effectiveUnitType = auto
				for (int i = 0; i < infoArrayCount; i++)
				{
					ref GridColRowInfo cr = ref infoArray[i];
					cr.spanExtraLengthOrPosition = 0;

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Auto;
						cr.constrainedPixelLength = 0;
					}
				}
			}
			else
			{
				for (int i = 0; i < infoArrayCount; i++)
				{
					ref GridColRowInfo cr = ref infoArray[i];
					cr.spanExtraLengthOrPosition = 0;

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Star;
						cr.constrainedPixelLength = 0; // need to set to 0 here because DistributeStarColWidth and DistributeStarRowHeight rely on this
					}
				}
			}
		}

		private void CreateChildInfo(int childrenCnt, bool isInfiniteWidth, bool isInfiniteHeight)
		{
			haveStarColAndAutoRowChildren = false;

			int maxCol = colInfoArrayCount - 1;
			int maxRow = rowInfoArrayCount - 1;

			int nextIndexForCellGroup0 = 0;
			firstIndexForOtherCellGroups = childrenCnt - 1;
			int lastIndexForOtherCellGroups = childrenCnt - 1;

			for (int i = 0; i < childrenCnt; i++)
			{
				UIElement child = InternalChildren[i];

				if (child == null)
				{
					if (firstIndexForOtherCellGroups < childrenCnt - 1)
					{
						// need to move the last item into a place that 
						childInfoArray[firstIndexForOtherCellGroups] = childInfoArray[lastIndexForOtherCellGroups];
						lastIndexForOtherCellGroups--;
					}
					firstIndexForOtherCellGroups--;

					continue;
				}

				int col = GetGridColumn(child); // ??? change these to GetColumn, etc - we don't want any dependencies on Grid at all
				int row = GetGridRow(child);

				// xaml allows a Grid.Column or Grid.Row that is beyond the max row/col - in this case it uses the max row/col
				// but it won't allow negative values - throws an exception when running 
				if (col > maxCol)
				{
					col = maxCol;
				}

				if (row > maxRow)
				{
					row = maxRow;
				}

				int colSpan = GetGridColumnSpan(child); // ??? change these to GetColumnSpan, etc - we don't want any dependencies on Grid at all
				int rowSpan = GetGridRowSpan(child);

				if (colSpan > 1 && col + colSpan > colInfoArrayCount)
				{
					colSpan = colInfoArrayCount - col;
				}

				if (rowSpan > 1 && row + rowSpan > rowInfoArrayCount)
				{
					rowSpan = rowInfoArrayCount - row;
				}

				LayoutGridUnitType colType = colInfoArray[col].unitType;
				LayoutGridUnitType rowType = rowInfoArray[row].unitType;

				// cellGroup must be set based on the unitTypes, not effectiveUnitTypes
				if (colSpan <= 1 && rowSpan <= 1)
				{
					// some of these if statements could be replaced by setting bit switches from the colType/rowType to determine the cellGroup??? - don't think it's worth it though
					if (colType == LayoutGridUnitType.Auto && rowType == LayoutGridUnitType.Star)
					{
						ref ChildInfo n = ref childInfoArray[firstIndexForOtherCellGroups--];
						n.child = child;
						n.col = col;
						n.row = row;
						n.colSpan = colSpan;
						n.rowSpan = rowSpan;
						n.cellGroup = CellGroup_AutoColStarRow;
					}
					else if (rowType == LayoutGridUnitType.Star)
					{
						ref ChildInfo n = ref childInfoArray[firstIndexForOtherCellGroups--];
						n.child = child;
						n.col = col;
						n.row = row;
						n.colSpan = colSpan;
						n.rowSpan = rowSpan;
						n.cellGroup = CellGroup_PixelOrStarColStarRow;
					}
					else if (colType == LayoutGridUnitType.Star)
					{
						ref ChildInfo n = ref childInfoArray[firstIndexForOtherCellGroups--];
						n.child = child;
						n.col = col;
						n.row = row;
						n.colSpan = colSpan;
						n.rowSpan = rowSpan;
						n.cellGroup = CellGroup_StarColPixelOrAutoRow;

						haveStarColAndAutoRowChildren |= (rowType == LayoutGridUnitType.Auto);
					}
					else
					{
						ref ChildInfo n = ref childInfoArray[nextIndexForCellGroup0++];
						n.child = child;
						n.col = col;
						n.row = row;
						n.colSpan = colSpan;
						n.rowSpan = rowSpan;
						n.cellGroup = CellGroup_PixelOrAutoColRow;
					}
				}
				else
				{
					//??? probably have to reset/get this stuff and re-sort if infinite width/height and star is treated as auto
					//ushort colFlags = 0;
					//ushort rowFlags;

					ushort colFlags = 0;
					if (colSpan > 1)
					{
						int maxColOrRowInSpanPlus1 = col + colSpan;

						int j = col;
						do
						{
							ref GridColRowInfo cr = ref colInfoArray[j];

							colFlags |= (ushort)cr.unitType;

							j++;
						} while (j < maxColOrRowInSpanPlus1);

						if ((colFlags & ChildFlag_SpanHasAuto) != 0 && ((colFlags & ChildFlag_SpanHasStar) == 0 || isInfiniteWidth))
						{
							colFlags |= ChildFlag_SpanHasAutoNoStar;
						}
					}

					ushort rowFlags = 0;
					if (rowSpan > 1)
					{
						int maxColOrRowInSpanPlus1 = row + rowSpan;

						int j = row;
						do
						{
							ref GridColRowInfo cr = ref rowInfoArray[j];

							rowFlags |= (ushort)cr.unitType; //??? do these casts cost something?

							j++;
						} while (j < maxColOrRowInSpanPlus1);

						if ((rowFlags & ChildFlag_SpanHasAuto) != 0 && ((rowFlags & ChildFlag_SpanHasStar) == 0 || isInfiniteHeight))
						{
							rowFlags |= ChildFlag_SpanHasAutoNoStar;
						}
					}

					// cellGroup must be set based on the unitTypes, not effectiveUnitTypes
					bool colOrSpanHasStars = (colFlags & ChildFlag_SpanHasStar) != 0 || (colType == LayoutGridUnitType.Star);
					bool rowOrSpanHasStars = (rowFlags & ChildFlag_SpanHasStar) != 0 || (rowType == LayoutGridUnitType.Star);

					haveStarColAndAutoRowChildren |= colOrSpanHasStars && ((rowFlags & ChildFlag_SpanHasAuto) != 0 || (rowType == LayoutGridUnitType.Auto));

					ref ChildInfo n = ref childInfoArray[firstIndexForOtherCellGroups--];
					n.child = child;
					n.col = col;
					n.row = row;
					n.colSpan = colSpan;
					n.rowSpan = rowSpan;
					n.colFlags = colFlags;
					n.rowFlags = rowFlags;

					if (!colOrSpanHasStars && !rowOrSpanHasStars)
					{
						n.cellGroup = CellGroup_PixelOrAutoColRowWithSpan;
					}
					else if (!colOrSpanHasStars && ((n.colFlags & ChildFlag_SpanHasAuto) != 0 || (colType == LayoutGridUnitType.Auto)) && rowOrSpanHasStars)
					{
						n.cellGroup = CellGroup_AutoColStarRowWithSpan;
					}
					else if (colOrSpanHasStars && !rowOrSpanHasStars)
					{
						n.cellGroup = CellGroup_StarColPixelOrAutoRowWithSpan;
					}
					else
					{
						n.cellGroup = CellGroup_PixelOrStarColStarRowWithSpan;
					}
				}
			}
			childInfoArrayCount = lastIndexForOtherCellGroups + 1;
			childInfoArray[childInfoArrayCount].cellGroup = CellGroup_AfterLast;
			childInfoArrayCount++;

			Debug.Assert(nextIndexForCellGroup0 == firstIndexForOtherCellGroups + 1);

			if (firstIndexForOtherCellGroups < lastIndexForOtherCellGroups)
			{
				firstIndexForOtherCellGroups++; // this is now the real first index, before it was one before the first index

				// only sort if there are different cell groups that require sorting
				if (compareChildInfoByCellGroup == null)
				{
					compareChildInfoByCellGroup = new CompareChildInfoByCellGroup();
				}

#if CollectPerformanceStats
				long startTicksChildSort = Stopwatch.GetTimestamp();
#endif

				Array.Sort(childInfoArray, firstIndexForOtherCellGroups, (lastIndexForOtherCellGroups - firstIndexForOtherCellGroups) + 1, compareChildInfoByCellGroup);

#if CollectPerformanceStats
				childSortTicks += Stopwatch.GetTimestamp() - startTicksChildSort;
#endif

				int childInfoArrayCountMinusOne = childInfoArrayCount - 1;
				int childInfoArrayCountMinusTwo = childInfoArrayCount - 2;
				for (int i = firstIndexForOtherCellGroups; i < childInfoArrayCountMinusOne; i++)
				{
					ref ChildInfo n = ref childInfoArray[i];

					if ((n.colFlags & ChildFlag_SpanHasAutoNoStar) != 0 && n.colSpan > 1 && (i == childInfoArrayCountMinusTwo || n.col != childInfoArray[i + 1].col || n.colSpan != childInfoArray[i + 1].colSpan))
					{
						n.colFlags |= ChildFlag_IsLastOfSameSpan;
					}
				}
			}
		}

		private void DistributeStarColWidth(double starLength)
		{
			int idx;
			int j;

			if (starLength <= totalMinStarColWidth)
			{
				// just assign min to all star cols
				for (int i = 0; i < starColCount; i++)
				{
					idx = starColMaxArray[i].index;

					ref GridColRowInfo cr = ref colInfoArray[idx];
					cr.constrainedPixelLength = cr.minPixelLength;
				}
			}
			else if (starLength >= totalMaxStarColWidth)
			{
				// just assign max to all star cols
				for (int i = 0; i < starColCount; i++)
				{
					idx = starColMaxArray[i].index;

					ref GridColRowInfo cr = ref colInfoArray[idx];
					cr.constrainedPixelLength = cr.maxPixelLength;
				}
			}
			else
			{
				double availablePixels = starLength;
				double availableStars = totalStarsInCols;

				double pixelsPerStar;
				pixelsPerStar = availablePixels / availableStars;

				int lastMinConstrainedIndex = -1;

				int unresolvedCount = starColCount;

				// loop through mins (if any)
				for (int i = 0; i < starColMinArrayCount; i++)
				{
					ref StarMinMax t = ref starColMinArray[i];

					ref GridColRowInfo cr = ref colInfoArray[t.index];

					if (pixelsPerStar < t.minOrMaxPerStar)
					{
						availablePixels -= cr.minPixelLength;
						availableStars -= cr.stars;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedPixelLength = cr.minPixelLength;

						lastMinConstrainedIndex = i;

						unresolvedCount--;
					}
					else
					{
						break; // because this array is sorted, once we know the min isn't constrained anymore then no more mins will be constrained, so just exit the loop
					}
				}

				// loop through maxs - this includes everything even if the col/row doens't have a max - it will have infinity as max
				for (int i = 0; i < starColCount; i++)
				{
					ref StarMinMax t = ref starColMaxArray[i];

					ref GridColRowInfo cr = ref colInfoArray[t.index];

					if (pixelsPerStar > t.minOrMaxPerStar)
					{
						availablePixels -= cr.maxPixelLength;
						availableStars -= cr.stars;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedPixelLength = cr.maxPixelLength;

						unresolvedCount--;

						j = lastMinConstrainedIndex;
						while (j >= 0)
						{
							ref StarMinMax tMin = ref starColMinArray[j];

							ref GridColRowInfo crMin = ref colInfoArray[tMin.index];

							if (pixelsPerStar > tMin.minOrMaxPerStar)
							{
								availablePixels += crMin.minPixelLength;
								availableStars += crMin.stars;

								pixelsPerStar = availablePixels / availableStars;

								crMin.constrainedPixelLength = 0; // this will get reprocessed in the outer max loop

								lastMinConstrainedIndex = --j;

								unresolvedCount++;
							}
							else
							{
								break;
							}
						}
					}
					else if (cr.constrainedPixelLength == 0)
					{
						if (unresolvedCount == 1)
						{
							cr.constrainedPixelLength = availablePixels; // the last col/row takes all remaining pixels

							break;
						}

						cr.constrainedPixelLength = cr.stars * pixelsPerStar;

						availablePixels -= cr.constrainedPixelLength;
						availableStars -= cr.stars;

						unresolvedCount--;
					}
				}
			}
		}

		private void DistributeStarRowHeight(double starLength)
		{
			int idx;
			int j;

			if (starLength <= totalMinStarRowHeight)
			{
				// just assign min to all star rows
				for (int i = 0; i < starRowCount; i++)
				{
					idx = starRowMaxArray[i].index;

					ref GridColRowInfo cr = ref rowInfoArray[idx];
					cr.constrainedPixelLength = cr.minPixelLength;
				}
			}
			else if (starLength >= totalMaxStarRowHeight)
			{
				// just assign max to all star rows
				for (int i = 0; i < starRowCount; i++)
				{
					idx = starRowMaxArray[i].index;

					ref GridColRowInfo cr = ref rowInfoArray[idx];
					cr.constrainedPixelLength = cr.maxPixelLength;
				}
			}
			else
			{
				double availablePixels = starLength;
				double availableStars = totalStarsInRows;

				double pixelsPerStar;
				pixelsPerStar = availablePixels / availableStars;

				int lastMinConstrainedIndex = -1;

				int unresolvedCount = starRowCount;

				// loop through mins (if any)
				for (int i = 0; i < starRowMinArrayCount; i++)
				{
					ref StarMinMax t = ref starRowMinArray[i];

					ref GridColRowInfo cr = ref rowInfoArray[t.index];

					if (pixelsPerStar < t.minOrMaxPerStar)
					{
						availablePixels -= cr.minPixelLength;
						availableStars -= cr.stars;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedPixelLength = cr.minPixelLength;

						lastMinConstrainedIndex = i;

						unresolvedCount--;
					}
					else
					{
						break; // because this array is sorted, once we know the min isn't constrained anymore then no more mins will be constrained, so just exit the loop
					}
				}

				// loop through maxs - this includes everything even if the row/row doens't have a max - it will have infinity as max
				for (int i = 0; i < starRowCount; i++)
				{
					ref StarMinMax t = ref starRowMaxArray[i];

					ref GridColRowInfo cr = ref rowInfoArray[t.index];

					if (pixelsPerStar > t.minOrMaxPerStar)
					{
						availablePixels -= cr.maxPixelLength;
						availableStars -= cr.stars;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedPixelLength = cr.maxPixelLength;

						unresolvedCount--;

						j = lastMinConstrainedIndex;
						while (j >= 0)
						{
							ref StarMinMax tMin = ref starRowMinArray[j];

							ref GridColRowInfo crMin = ref rowInfoArray[tMin.index];

							if (pixelsPerStar > tMin.minOrMaxPerStar)
							{
								availablePixels += crMin.minPixelLength;
								availableStars += crMin.stars;

								pixelsPerStar = availablePixels / availableStars;

								crMin.constrainedPixelLength = 0; // this will get reprocessed in the outer max loop

								lastMinConstrainedIndex = --j;

								unresolvedCount++;
							}
							else
							{
								break;
							}
						}
					}
					else if (cr.constrainedPixelLength == 0)
					{
						if (unresolvedCount == 1)
						{
							cr.constrainedPixelLength = availablePixels; // the last row/row takes all remaining pixels

							break;
						}

						cr.constrainedPixelLength = cr.stars * pixelsPerStar;

						availablePixels -= cr.constrainedPixelLength;
						availableStars -= cr.stars;

						unresolvedCount--;
					}
				}
			}
		}

		private void DistributeAutoSpan(GridColRowInfo[] infoArray, int startColOrRow, int maxColOrRowInSpanPlusOne, int spanUnresolvedAutoCount, double remainingSpanExtraLength,
			double existingSpanExtraLength, bool hasSomeMaxLength)
		{
			double extraLengthPerAuto = remainingSpanExtraLength / spanUnresolvedAutoCount;
			double availableExtraToDistribute = remainingSpanExtraLength - existingSpanExtraLength;

			bool noMaxConstrainedInPriorLoop;
			bool noMaxConstrainedInThisLoop = !hasSomeMaxLength;
			double used;

			do
			{
				noMaxConstrainedInPriorLoop = noMaxConstrainedInThisLoop;

				noMaxConstrainedInThisLoop = true;
				int j = startColOrRow;
				do
				{
					ref GridColRowInfo cr = ref infoArray[j];

					if (cr.effectiveUnitType == LayoutGridUnitType.Auto && !cr.isResolved)
					{
						if (cr.constrainedPixelLength + extraLengthPerAuto > cr.maxPixelLength)
						{
							noMaxConstrainedInThisLoop = false;

							spanUnresolvedAutoCount--;

							double length;
							if (cr.constrainedPixelLength >= cr.minPixelLength)
							{
								length = cr.constrainedPixelLength;
							}
							else
							{
								length = cr.minPixelLength;
							}

							used = cr.maxPixelLength - length;
							remainingSpanExtraLength -= used;

							cr.spanExtraLengthOrPosition = 0;
							cr.constrainedPixelLength = cr.maxPixelLength;

							extraLengthPerAuto = remainingSpanExtraLength / spanUnresolvedAutoCount;

							cr.isResolved = true;
						}
						else if (spanUnresolvedAutoCount == 1 || noMaxConstrainedInPriorLoop)
						{
							used = extraLengthPerAuto - cr.spanExtraLengthOrPosition;

							if (used > availableExtraToDistribute)
							{
								cr.spanExtraLengthOrPosition += availableExtraToDistribute;

								return;
							}
							else if (used > 0)
							{
								cr.spanExtraLengthOrPosition = extraLengthPerAuto;

								availableExtraToDistribute -= used;
							}

							if (--spanUnresolvedAutoCount == 0)
							{
								return;
							}

							remainingSpanExtraLength -= extraLengthPerAuto;

							if (used < 0)
							{
								extraLengthPerAuto = remainingSpanExtraLength / spanUnresolvedAutoCount;
							}
						}
					}

					j++;
				} while (j < maxColOrRowInSpanPlusOne);
			} while (spanUnresolvedAutoCount > 0);
		}

		private double GetSpanLength(GridColRowInfo[] infoArray, int startColOrRow, int span)
		{
			int maxColOrRowInSpanPlus1 = startColOrRow + span; // instead of passing in span, keep maxColOrRowInSpan calculated and use it???

			double length = 0;

			int i = startColOrRow;
			do
			{
				ref GridColRowInfo n = ref infoArray[i];

				//??? I think this can just be length += n.constrainedPixelLength + n.spanExtraLength; without any if/else because we should be min constrained at this point - check this - probably not
				if (n.constrainedPixelLength + n.spanExtraLengthOrPosition >= n.minPixelLength)
				{
					length += n.constrainedPixelLength + n.spanExtraLengthOrPosition;
				}
				else
				{
					length += n.minPixelLength; // auto might not have been min constrained at this point, but we want min constrained for span length??? I think MinSize is not minPixelLength
				}

				i++;
			} while (i < maxColOrRowInSpanPlus1);

			return length;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			Size availableChildSize;
			double availableChildWidth;
			double availableChildHeight;
			int i;

#if CollectPerformanceStats
			preMeasureTicks = 0;
			measureTicks = 0;
			postMeasureTicks = 0;
			shortMeasureTicks = 0;
			childMeasureTicks = 0;
			shortMeasureCount = 0;

			long startTicks = Stopwatch.GetTimestamp();
			long startTicksChildMeasure;
#endif

			int childrenCount = InternalChildren.Count;

			//??? put these back to local variables?
			//double desiredWidth = 0;
			//double desiredHeight = 0;
			desiredWidth = 0;
			desiredHeight = 0;

			totalAutoColWidth = 0;
			totalAutoRowHeight = 0;

			bool isInfiniteWidth = double.IsPositiveInfinity(availableSize.Width);
			bool isInfiniteHeight = double.IsPositiveInfinity(availableSize.Height);

			if (ColumnDefinitions.Count <= 1 && RowDefinitions.Count <= 1)
			{
				// there is only a single cell in the grid (single column and single row), so we don't need to do the expensive processing, just do the easy/inexpensive processing below and return
				double minWidth;
				double maxWidth;
				if (ColumnDefinitions.Count == 1)
				{
					ColumnDefinition c = ColumnDefinitions[0];
					LayoutGridUnitType colUnitType = GridUnitTypeToLayout(c.Width.GridUnitType);
					minWidth = c.MinWidth;
					maxWidth = c.MaxWidth;

					if (minWidth > maxWidth)
					{
						maxWidth = minWidth; // don't swap these, just set the max = min, which is the greater of the 2
					}

					if (colUnitType == LayoutGridUnitType.Auto)
					{
						availableChildWidth = double.PositiveInfinity;//??? I think this should be Max
					}
					else if (colUnitType == LayoutGridUnitType.Star)
					{
						if (isInfiniteWidth)
						{
							availableChildWidth = double.PositiveInfinity;//??? I think this should be Max
						}
						else
						{
							availableChildWidth = availableSize.Width;

							if (availableChildWidth > maxWidth)
							{
								availableChildWidth = maxWidth;
							}
							else if (availableChildWidth < minWidth)
							{
								availableChildWidth = minWidth;
							}
						}
					}
					else // must be pixel/absolute
					{
						availableChildWidth = c.Width.Value;

						if (availableChildWidth > maxWidth)
						{
							availableChildWidth = maxWidth;
						}
						else if (availableChildWidth < minWidth)
						{
							availableChildWidth = minWidth;
						}

						totalPixelColWidth = availableChildWidth;
					}
				}
				else
				{
					// the default is star
					availableChildWidth = availableSize.Width; // this could be positive infinity
					minWidth = 0;
					maxWidth = double.PositiveInfinity;
				}

				double minHeight;
				double maxHeight;
				if (RowDefinitions.Count == 1)
				{
					RowDefinition r = RowDefinitions[0];
					LayoutGridUnitType rowUnitType = GridUnitTypeToLayout(r.Height.GridUnitType);
					minHeight = r.MinHeight;
					maxHeight = r.MaxHeight;

					if (minHeight > maxHeight)
					{
						maxHeight = minHeight; // don't swap these, just set the max = min, which is the greater of the 2
					}

					if (rowUnitType == LayoutGridUnitType.Auto)
					{
						availableChildHeight = double.PositiveInfinity;//??? I think this should be Max
					}
					else if (rowUnitType == LayoutGridUnitType.Star)
					{
						if (isInfiniteHeight)
						{
							availableChildHeight = double.PositiveInfinity;//??? I think this should be Max
						}
						else
						{
							availableChildHeight = availableSize.Height; // this could be positive infinity

							if (availableChildHeight > maxHeight)
							{
								availableChildHeight = maxHeight;
							}
							else if (availableChildHeight < minHeight)
							{
								availableChildHeight = minHeight;
							}
						}
					}
					else // must be pixel/absolute
					{
						availableChildHeight = r.Height.Value;

						if (availableChildHeight > maxHeight)
						{
							availableChildHeight = maxHeight;
						}
						else if (availableChildHeight < minHeight)
						{
							availableChildHeight = minHeight;
						}

						totalPixelRowHeight = availableChildHeight;
					}
				}
				else
				{
					// the default is star
					availableChildHeight = availableSize.Height; // this could be positive infinity
					minHeight = 0;
					maxHeight = double.PositiveInfinity;
				}

				if (!double.IsPositiveInfinity(availableChildWidth))
				{
					desiredWidth = availableChildWidth;
				}

				if (!double.IsPositiveInfinity(availableChildHeight))
				{
					desiredHeight = availableChildHeight;
				}

				for (i = 0; i < childrenCount; i++)
				{
					UIElement child = InternalChildren[i];

					//??? if there is a single col and row with A/A, does this work - maybe need to use infinite and then again with the size returned from Measure - look at the screens to see if the button text shows
					if (child != null)
					{
						availableChildSize.Width = availableChildWidth;
						availableChildSize.Height = availableChildHeight;

#if CollectPerformanceStats
						startTicksChildMeasure = Stopwatch.GetTimestamp();
#endif

						child.Measure(availableChildSize);

#if CollectPerformanceStats
						childMeasureTicks += Stopwatch.GetTimestamp() - startTicksChildMeasure;
#endif

						if (double.IsPositiveInfinity(availableChildWidth))
						{
							double width = child.DesiredSize.Width;

							if (width > desiredWidth)
							{
								desiredWidth = width;
								totalAutoColWidth = width;
							}
						}

						if (double.IsPositiveInfinity(availableChildHeight))
						{
							double height = child.DesiredSize.Height;

							if (height > desiredHeight)
							{
								desiredHeight = height;
								totalAutoRowHeight = height;
							}
						}
					}
				}

				if (desiredWidth < minWidth)
				{
					desiredWidth = minWidth;
				}
				else if (desiredWidth > maxWidth)
				{
					desiredWidth = maxWidth;
				}

				if (desiredHeight < minHeight)
				{
					desiredHeight = minHeight;
				}
				else if (desiredHeight > maxHeight)
				{
					desiredHeight = maxHeight;
				}

#if CollectPerformanceStats
				shortMeasureTicks = Stopwatch.GetTimestamp() - startTicks;
				shortMeasureCount++;
#endif
				return new Size(desiredWidth, desiredHeight);
			}

			if (colInfoArray == null || haveColsChanged)
			{
				CreateColInfo(isInfiniteWidth);
			}
			else
			{
				SetupColRowInfo(colInfoArray, colInfoArrayCount, isInfiniteWidth);
			}

			if (rowInfoArray == null || haveRowsChanged)
			{
				CreateRowInfo(isInfiniteHeight);
			}
			else
			{
				SetupColRowInfo(rowInfoArray, rowInfoArrayCount, isInfiniteHeight);
			}

			//??? maybe only do this block of code below if we didn't already call CreateColInfo - should need to pass in isInfiniteWidth to CreateColInfo and deal with that?
			// probably should put these in a function and call it - have a single function for both col and row
			// the overhead should be minimal because it only gets called twice at most for each MeasureOverride call and it lowers the amount of code that is in the 

			//??? maybe only do this block of code below if we didn't already call CreateRowInfo - should need to pass in isInfiniteHeight to CreateRowInfo and deal with that?

			if (childInfoArray == null || childInfoArray.Length < childrenCount + 1)
			{
				childInfoArray = new ChildInfo[childrenCount + 1]; // need to add 1 because we are storing a Last bogus item in this array to make the loop through the items easier
				CreateChildInfo(childrenCount, isInfiniteWidth, isInfiniteHeight);
			}
			else if (childInfoArray.Length != childrenCount + 1 || haveChildrenChanged)
			{
				Array.Clear(childInfoArray, 0, childrenCount); // clear the array so that we don't have to set default values - don't need to clear the Last item
				CreateChildInfo(childrenCount, isInfiniteWidth, isInfiniteHeight);
			}
			else
			{
				bool isWidthDiff = isInfiniteWidth != wasInfiniteWidth;
				bool isHeightDiff = isInfiniteHeight != wasInfiniteHeight;
				if (isWidthDiff || isHeightDiff)
				{
					// if something changed in the infinite width / height values, then need to change things about the children array
					//??? do we need to change the cell group and resort?
					for (i = 0; i < childInfoArrayCount; i++)
					{
						ref ChildInfo n = ref childInfoArray[i];

						if (isWidthDiff)
						{
							if (isInfiniteWidth)
							{
								if ((n.colFlags & ChildFlag_SpanHasAuto) != 0 && (n.colFlags & ChildFlag_SpanHasStar) == 0)
								{
									n.colFlags |= ChildFlag_SpanHasAutoNoStar;
								}
								else
								{
									n.colFlags &= ChildFlag_SpanHasAutoNoStar_Reset;
								}
							}
							else if ((n.colFlags & ChildFlag_SpanHasAuto) != 0 && (n.colFlags & ChildFlag_SpanHasStar) == 0)
							{
								n.colFlags |= ChildFlag_SpanHasAutoNoStar;
							}
							else
							{
								n.colFlags &= ChildFlag_SpanHasAutoNoStar_Reset;
							}
						}

						if (isHeightDiff)
						{
							if (isInfiniteHeight)
							{
								if ((n.rowFlags & ChildFlag_SpanHasAuto) != 0 && (n.rowFlags & ChildFlag_SpanHasStar) == 0)
								{
									n.rowFlags |= ChildFlag_SpanHasAutoNoStar;
								}
								else
								{
									n.rowFlags &= ChildFlag_SpanHasAutoNoStar_Reset;
								}
							}
							else if ((n.rowFlags & ChildFlag_SpanHasAuto) != 0 && (n.rowFlags & ChildFlag_SpanHasStar) == 0)
							{
								n.rowFlags |= ChildFlag_SpanHasAutoNoStar;
							}
							else
							{
								n.rowFlags &= ChildFlag_SpanHasAutoNoStar_Reset;
							}
						}
					}
				}
			}

			wasInfiniteWidth = isInfiniteWidth;
			wasInfiniteHeight = isInfiniteHeight;

#if CollectPerformanceStats
			preMeasureTicks = Stopwatch.GetTimestamp() - startTicks;
			startTicks = Stopwatch.GetTimestamp();
#endif

			double length;
			double maxSpanLength = 0;

			bool isAutoColLengthResolved = false;
			bool isAutoRowLengthResolved = false;
			bool isStarColLengthResolved = false;
			bool isStarRowLengthResolved = false;

			if (starColCount == 0 || isInfiniteWidth)
			{
				isStarColLengthResolved = true;
			}

			int distributeStarRowHeightBeforeCellGroup;
			bool remeasureAutoColStarRowGroup = false;
			if (starRowCount > 0 && !isInfiniteHeight)
			{
				if (!haveStarColAndAutoRowChildren || isInfiniteWidth)
				{
					distributeStarRowHeightBeforeCellGroup = CellGroup_AutoColStarRow;
				}
				else
				{
					distributeStarRowHeightBeforeCellGroup = CellGroup_PixelOrStarColStarRow;
					remeasureAutoColStarRowGroup = true;
				}
			}
			else
			{
				// don't call DistributeStarRowHeight at all because there aren't any effective star rows
				distributeStarRowHeightBeforeCellGroup = CellGroup_PixelOrStarColStarRow; // distributeStarRowHeightBeforeCellGroup is also used as the point that auto rows are finished and we can min/max constrain auto rows
				isStarRowLengthResolved = true;
			}

			int firstChildIndexOfAutoColStarRowCellGroup = -1;
			int firstChildIndexForAutoSpanDistribution = -1;
			int lastChildIndexForAutoSpanDistribution = -1;

			int priorCellGroup = 0;
			int cellGroup;
			int col;
			int row;
			int colSpan;
			int rowSpan;

			for (i = 0; ; i++) 
			{
				ref ChildInfo n = ref childInfoArray[i];

				cellGroup = n.cellGroup;

				if (cellGroup > priorCellGroup)
				{
					// the reason the spans need to be processed afterwards is because for example, there may be a child with colspan = 2 and another child with rowspan = 2 in the same cell
					// need to process the non-span row and col and get the sizes for these first
					if (firstChildIndexForAutoSpanDistribution != -1)
					{
						for (int j = firstChildIndexForAutoSpanDistribution; j <= lastChildIndexForAutoSpanDistribution; j++)
						{
							ref ChildInfo n2 = ref childInfoArray[j];

							col = n2.col;
							row = n2.row;

							colSpan = n2.colSpan;
							rowSpan = n2.rowSpan;

							if ((n2.colFlags & ChildFlag_SpanHasAutoNoStar) != 0)
							{
								length = n2.child.DesiredSize.Width; // this is the desired length of the auto columns only

								if (length > maxSpanLength)
								{
									maxSpanLength = length;
								}

								if ((n2.colFlags & ChildFlag_IsLastOfSameSpan) != 0)
								{
									double totalAutoLengthBeforeDistribute = 0;
									double totalAutoLengthBeforeDistributeWithMin = 0;
									double totalPixelLength = 0;
									double existingSpanExtraLength = 0;
									int maxColOrRowInSpanPlus1 = col + colSpan;
									int spanAutoCount = 0;

									int k;

									k = col;
									bool hasSomeMaxLength = false;
									do
									{
										ref GridColRowInfo cr = ref colInfoArray[k++];

										if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
										{
											spanAutoCount++;
											totalAutoLengthBeforeDistribute += cr.constrainedPixelLength;

											if (cr.constrainedPixelLength >= cr.minPixelLength)
											{
												totalAutoLengthBeforeDistributeWithMin += cr.constrainedPixelLength;
											}
											else
											{
												totalAutoLengthBeforeDistributeWithMin += cr.minPixelLength;
											}

											existingSpanExtraLength += cr.spanExtraLengthOrPosition;
											cr.isResolved = false;

											hasSomeMaxLength |= !double.IsPositiveInfinity(cr.maxPixelLength);
										}
										else if (cr.effectiveUnitType == LayoutGridUnitType.Pixel)
										{
											totalPixelLength += cr.constrainedPixelLength;
										}

									} while (k < maxColOrRowInSpanPlus1);

									maxSpanLength -= totalPixelLength; // maxSpanLength is now the desired length just for all auto
									if (maxSpanLength > totalAutoLengthBeforeDistributeWithMin + existingSpanExtraLength)
									{
										DistributeAutoSpan(colInfoArray, col, maxColOrRowInSpanPlus1, spanAutoCount, maxSpanLength - totalAutoLengthBeforeDistributeWithMin, existingSpanExtraLength, hasSomeMaxLength);
									}

									maxSpanLength = 0;
								}
							}

							if ((n2.rowFlags & ChildFlag_SpanHasAutoNoStar) != 0)
							{
								length = n2.child.DesiredSize.Height; // this is the desired length of the auto rows only

								double totalAutoLengthBeforeDistribute = 0;
								double totalAutoLengthBeforeDistributeWithMin = 0;
								double totalPixelLength = 0;
								double existingSpanExtraLength = 0;
								int maxColOrRowInSpanPlus1 = row + rowSpan;
								int spanAutoCount = 0;

								int k;

								k = row;
								bool hasSomeMaxLength = false;
								do
								{
									ref GridColRowInfo cr = ref rowInfoArray[k++];

									if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
									{
										spanAutoCount++;
										totalAutoLengthBeforeDistribute += cr.constrainedPixelLength;

										if (cr.constrainedPixelLength >= cr.minPixelLength)
										{
											totalAutoLengthBeforeDistributeWithMin += cr.constrainedPixelLength;
										}
										else
										{
											totalAutoLengthBeforeDistributeWithMin += cr.minPixelLength;
										}

										existingSpanExtraLength += cr.spanExtraLengthOrPosition;
										cr.isResolved = false;

										hasSomeMaxLength |= !double.IsPositiveInfinity(cr.maxPixelLength);
										//??? is double.IsPositiveInfinity inlined - otherwise inline it - look at what it is
									}
									else if (cr.effectiveUnitType == LayoutGridUnitType.Pixel)
									{
										totalPixelLength += cr.constrainedPixelLength;
									}

								} while (k < maxColOrRowInSpanPlus1);

								length -= totalPixelLength; // length is now the desired length just for all auto
								if (length > totalAutoLengthBeforeDistributeWithMin + existingSpanExtraLength)
								{
									DistributeAutoSpan(rowInfoArray, row, maxColOrRowInSpanPlus1, spanAutoCount, length - totalAutoLengthBeforeDistributeWithMin, existingSpanExtraLength, hasSomeMaxLength);
								}
							}
						}
						firstChildIndexForAutoSpanDistribution = -1;
					}

					if (!isAutoColLengthResolved && cellGroup >= CellGroup_StarColPixelOrAutoRow)
					{
						isAutoColLengthResolved = true;

						// min constrain the auto cols so we have an accurate totalAutoColWidth
						for (int j = 0; j < colInfoArrayCount; j++)
						{
							ref GridColRowInfo cr = ref colInfoArray[j];

							if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
							{
								if (cr.constrainedPixelLength < cr.minPixelLength)
								{
									cr.constrainedPixelLength = cr.minPixelLength + cr.spanExtraLengthOrPosition;
								}
								else
								{
									cr.constrainedPixelLength += cr.spanExtraLengthOrPosition;
								}
								totalAutoColWidth += cr.constrainedPixelLength;
							}
						}
					}

					if (!isStarColLengthResolved && cellGroup >= CellGroup_StarColPixelOrAutoRow)
					{
						DistributeStarColWidth(availableSize.Width - (totalPixelColWidth + totalAutoColWidth));
						isStarColLengthResolved = true;
					}

					if (!isAutoRowLengthResolved && cellGroup >= distributeStarRowHeightBeforeCellGroup)
					{
						isAutoRowLengthResolved = true;

						// min constrain the auto rows so we have an accurate totalAutoRowHeight
						for (int j = 0; j < rowInfoArrayCount; j++)
						{
							ref GridColRowInfo cr = ref rowInfoArray[j];

							if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
							{
								if (cr.constrainedPixelLength < cr.minPixelLength)
								{
									cr.constrainedPixelLength = cr.minPixelLength + cr.spanExtraLengthOrPosition;
								}
								else
								{
									cr.constrainedPixelLength += cr.spanExtraLengthOrPosition;
								}
								totalAutoRowHeight += cr.constrainedPixelLength;
							}
						}
					}

					if (!isStarRowLengthResolved && cellGroup >= distributeStarRowHeightBeforeCellGroup)
					{
						DistributeStarRowHeight(availableSize.Height - (totalPixelRowHeight + totalAutoRowHeight));
						isStarRowLengthResolved = true;
					}

					if (cellGroup == CellGroup_AutoColStarRow || cellGroup == CellGroup_AutoColStarRowWithSpan)
					{
						if (remeasureAutoColStarRowGroup && firstChildIndexOfAutoColStarRowCellGroup == -1)
						{
							firstChildIndexOfAutoColStarRowCellGroup = i;
						}
					}

					if (cellGroup > CellGroup_StarColPixelOrAutoRowWithSpan && firstChildIndexOfAutoColStarRowCellGroup != -1)
					{
						for (int j = firstChildIndexOfAutoColStarRowCellGroup; j < childInfoArrayCount; j++) // loop one past so that the cell group change logic can be in one place
						{
							ref ChildInfo n2 = ref childInfoArray[j];

							if (n2.cellGroup > CellGroup_AutoColStarRowWithSpan)
							{
								break;
							}

							col = n2.col;
							row = n2.row;

							ref GridColRowInfo c2 = ref colInfoArray[col];
							ref GridColRowInfo r2 = ref rowInfoArray[row];

							colSpan = n2.colSpan;
							rowSpan = n2.rowSpan;

							availableChildWidth = double.PositiveInfinity;

							if (rowSpan > 1)
							{
								availableChildHeight = GetSpanLength(rowInfoArray, row, rowSpan);
							}
							else
							{
								availableChildHeight = r2.constrainedPixelLength + r2.spanExtraLengthOrPosition;
							}

							availableChildSize.Width = availableChildWidth;
							availableChildSize.Height = availableChildHeight;

#if CollectPerformanceStats
							startTicksChildMeasure = Stopwatch.GetTimestamp();
#endif

							n2.child.Measure(availableChildSize); // call Measure for every child with the correct width and height that the child gets, even if we don't need the size - like with pixel or star sizing - here is is called a 2nd time with the correct width and height

#if CollectPerformanceStats
							childMeasureTicks += Stopwatch.GetTimestamp() - startTicksChildMeasure;
#endif
						}

						firstChildIndexOfAutoColStarRowCellGroup = -1;
					}

					if (cellGroup == CellGroup_AfterLast)
					{
						break; // this is when we exit the main for loop
					}

					priorCellGroup = cellGroup;
				}

				col = n.col;
				row = n.row;

				ref GridColRowInfo c = ref colInfoArray[col];
				ref GridColRowInfo r = ref rowInfoArray[row];

				colSpan = n.colSpan;
				rowSpan = n.rowSpan;

#if DEBUG //???
				string name = (string)n.child.GetValue(FrameworkElement.NameProperty);
				if (name == "btn7")
				{
					int asdf = 1;
				}
#endif

				if (colSpan > 1)
				{
					if ((n.colFlags & ChildFlag_SpanHasAutoNoStar) != 0)
					{
						availableChildWidth = double.PositiveInfinity;//??? I think this should be max?
					}
					else
					{
						availableChildWidth = GetSpanLength(colInfoArray, col, colSpan); //??? probably inline this later
					}
				}
				else
				{
					if (c.effectiveUnitType == LayoutGridUnitType.Auto)
					{
						availableChildWidth = double.PositiveInfinity;//??? I think this should be max?
					}
					else
					{
						if (c.constrainedPixelLength >= c.minPixelLength)
						{
							availableChildWidth = c.constrainedPixelLength;
						}
						else
						{
							availableChildWidth = c.minPixelLength; //??? is this correct to use min
						}
					}
				}

				if (rowSpan > 1)
				{
					if ((n.rowFlags & ChildFlag_SpanHasAutoNoStar) != 0)
					{
						availableChildHeight = double.PositiveInfinity;//??? I think this should be max?
					}
					else
					{
						availableChildHeight = GetSpanLength(rowInfoArray, row, rowSpan);
					}
				}
				else
				{
					if (r.effectiveUnitType == LayoutGridUnitType.Auto)
					{
						availableChildHeight = double.PositiveInfinity;//??? I think this should be max?
					}
					else
					{
						if (r.constrainedPixelLength >= r.minPixelLength)
						{
							availableChildHeight = r.constrainedPixelLength;
						}
						else
						{
							availableChildHeight = r.minPixelLength; //??? is this correct to use min - it's possible to get here for star row and auto col
						}
					}
				}

				availableChildSize.Width = availableChildWidth;
				availableChildSize.Height = availableChildHeight;

#if CollectPerformanceStats
				startTicksChildMeasure = Stopwatch.GetTimestamp();
#endif


				n.child.Measure(availableChildSize); // call Measure for every child, even if we don't need the size - like with pixel or star sizing

#if CollectPerformanceStats
				childMeasureTicks += Stopwatch.GetTimestamp() - startTicksChildMeasure;
#endif

				if (colSpan == 1 && c.effectiveUnitType == LayoutGridUnitType.Auto)
				{
					length = n.child.DesiredSize.Width - c.spanExtraLengthOrPosition;

					if (length > c.constrainedPixelLength)
					{
						if (length <= c.maxPixelLength)
						{
							c.constrainedPixelLength = length;
						}
						else
						{
							c.constrainedPixelLength = c.maxPixelLength;
						}
					}
				}
				else if ((n.colFlags & ChildFlag_SpanHasAutoNoStar) != 0)
				{
					if (firstChildIndexForAutoSpanDistribution == -1)
					{
						firstChildIndexForAutoSpanDistribution = i;
					}
					lastChildIndexForAutoSpanDistribution = i;
				}

				if (rowSpan == 1 && r.effectiveUnitType == LayoutGridUnitType.Auto)
				{
					length = n.child.DesiredSize.Height - r.spanExtraLengthOrPosition;

					if (length > r.constrainedPixelLength)
					{
						if (length <= r.maxPixelLength)
						{
							r.constrainedPixelLength = length;
						}
						else
						{
							r.constrainedPixelLength = r.maxPixelLength;
						}
					}
				}
				else if ((n.rowFlags & ChildFlag_SpanHasAutoNoStar) != 0)
				{
					if (firstChildIndexForAutoSpanDistribution == -1)
					{
						firstChildIndexForAutoSpanDistribution = i;
					}
					lastChildIndexForAutoSpanDistribution = i;
				}
			}

#if CollectPerformanceStats
			measureTicks = Stopwatch.GetTimestamp() - startTicks;
			startTicks = Stopwatch.GetTimestamp();
#endif

			if (isInfiniteWidth)
			{
				// infinite width means return the min width that the content can fit in
				desiredWidth = totalPixelColWidth + totalAutoColWidth;
			}
			else
			{
				// I don't know if it matters what the desired width is when availableSize.Width is not infinite, so just pass back the size that is available???

				desiredWidth = totalPixelColWidth + totalAutoColWidth + totalMinStarColWidth;//??? not sure this is correct
			}

			if (isInfiniteHeight)
			{
				// infinite height means return the min height that the content can fit in
				desiredHeight = totalPixelRowHeight + totalAutoRowHeight;
			}
			else
			{
				desiredHeight = totalPixelRowHeight + totalAutoRowHeight + totalMinStarRowHeight;//??? not sure this is correct
			}

			//??? retSize should never be more (width or height than what gets passed in)?
			Size retSize = new Size(desiredWidth, desiredHeight);

#if CollectPerformanceStats
			postMeasureTicks = Stopwatch.GetTimestamp() - startTicks;
#endif

			return retSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
#if CollectPerformanceStats
			preArrangeTicks = 0;
			arrangeTicks = 0;
			shortArrangeTicks = 0;
			childArrangeTicks = 0;
			shortArrangeCount = 0;

			long startTicks = Stopwatch.GetTimestamp();
			long startTicksChildArrange;
#endif

			if (ColumnDefinitions.Count <= 1 && RowDefinitions.Count <= 1)
			{
				int childrenCount = InternalChildren.Count;
				Rect arrangeRect = new Rect(0, 0, desiredWidth, desiredHeight); //??? should be able to just use this.DesiredSize.Width, ...
																				//Rect arrangeRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
				for (int i = 0; i < childrenCount; i++)
				{
					UIElement child = InternalChildren[i];

					if (child != null)
					{
#if CollectPerformanceStats
						startTicksChildArrange = Stopwatch.GetTimestamp();
#endif

						child.Arrange(arrangeRect);//??? don't use finalSize, use desiredSize? - Grid uses finalSize, so why don't we?

#if CollectPerformanceStats
						childArrangeTicks += Stopwatch.GetTimestamp() - startTicksChildArrange;
#endif
					}
				}

#if CollectPerformanceStats
				shortArrangeTicks = Stopwatch.GetTimestamp() - startTicks;
				shortArrangeCount++;
#endif
				return finalSize;
			}

			double position;
			position = 0;
			for (int i = 0; i < colInfoArrayCount; i++)
			{
				ref GridColRowInfo cr = ref colInfoArray[i];

				// set the positions

				cr.spanExtraLengthOrPosition = position;

				position += cr.constrainedPixelLength;
			}

			position = 0;
			for (int i = 0; i < rowInfoArrayCount; i++)
			{
				ref GridColRowInfo cr = ref rowInfoArray[i];

				// set the positions

				cr.spanExtraLengthOrPosition = position;

				position += cr.constrainedPixelLength;
			}

#if CollectPerformanceStats
			preArrangeTicks = Stopwatch.GetTimestamp() - startTicks;
			startTicks = Stopwatch.GetTimestamp();
#endif

			double width;
			double height;

			// the last item in childInfoArray is just the bogus Last item - don't try to arrange this
			for (int i = 0; i < childInfoArrayCount - 1; i++)
			{
				//??? I don't think we can set the ColumnDefinition.ActualWidth/ActualHeight because these are probably internal only - the properties are get only and get probably isn't defined to be anything that can be set publicly
				// we really need to do this - maybe define our own Coldef and  rowdef types LayoutColDef and LayoutRowDef?  or just LayoutColumn LayoutRow

				ref ChildInfo n = ref childInfoArray[i];

				ref GridColRowInfo c = ref colInfoArray[n.col];
				ref GridColRowInfo r = ref rowInfoArray[n.row];

				width = c.constrainedPixelLength;
				if (n.colSpan > 1)
				{
					int maxColPlus1 = n.col + n.colSpan;
					int j = n.col + 1;
					do
					{
						width += colInfoArray[j].constrainedPixelLength;
					} while (++j < maxColPlus1);
				}

				height = r.constrainedPixelLength;
				if (n.rowSpan > 1)
				{
					int maxRowPlus1 = n.row + n.rowSpan;
					int j = n.row + 1;
					do
					{
						height += rowInfoArray[j].constrainedPixelLength;
					} while (++j < maxRowPlus1);
				}

#if CollectPerformanceStats
				startTicksChildArrange = Stopwatch.GetTimestamp();
#endif

				n.child.Arrange(new Rect(c.spanExtraLengthOrPosition, r.spanExtraLengthOrPosition, width, height));

#if CollectPerformanceStats
				childArrangeTicks += Stopwatch.GetTimestamp() - startTicksChildArrange;
#endif
			}

#if CollectPerformanceStats
			arrangeTicks = Stopwatch.GetTimestamp() - startTicks;
#endif

			return finalSize;
		}

		//??? remove this function, get this from col def ActualWidth
		public double GetColWidth(int col)
		{
			double length = 0;

			if (colInfoArray != null && colInfoArrayCount > 0)
			{
				if (col > colInfoArrayCount - 1)
				{
					col = colInfoArrayCount - 1;
				}

				length = colInfoArray[col].constrainedPixelLength;
			}
			else
			{
				length = desiredWidth;
			}

			return length;
		}

		//??? remove this function, get this from row def ActualWidth
		public double GetRowHeight(int row)
		{
			double length = 0;

			if (rowInfoArray != null && rowInfoArrayCount > 0)
			{
				if (row > rowInfoArrayCount - 1)
				{
					row = rowInfoArrayCount - 1;
				}

				length = rowInfoArray[row].constrainedPixelLength;
			}
			else
			{
				length = desiredHeight;
			}

			return length;
		}

		#if DEBUG
		public int GetChildCellGroup(UIElement child)
		{
			int cellGroup = -1;

			for (int i = 0; i < childInfoArrayCount - 1; i++)
			{
				if (childInfoArray[i].child == child)
				{
					cellGroup = childInfoArray[i].cellGroup;
					break;
				}
			}
			return cellGroup;
		}
		#endif
	}

	public class LayoutGridRowDefinition : ColumnDefinition
	{
		//??? what is a good name for Col/Row that can be used for both?
	}

	public class RowDef : RowDefinition
	{

	}

#if false
    class ColumnDef : System.Windows.DependencyObject //??? maybe derive from this so you can have dependency fields
	{

	}

	class RowDef : System.Windows.DependencyObject //??? maybe derive from this so you can have dependency fields
	{

	}
#endif
	//
	// Summary:
	//     Provides access to an ordered, strongly typed collection of ColumnDef
	//     objects.
	//???[System.Reflection.DefaultMember("Item")]
	public sealed class ColumnDefCollection : IList<ColumnDefinition>, ICollection<ColumnDefinition>, IEnumerable<ColumnDefinition>, IEnumerable, IList, ICollection
	{
		private List<ColumnDefinition> columnDefList = new List<ColumnDefinition>();

		//
		// Summary:
		//     Gets a value that indicates the current item within a ColumnDefCollection.
		//
		// Parameters:
		//   index:
		//     The current item in the collection.
		//
		// Returns:
		//     The element at the specified index.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is not a valid index position in the collection.
		public ColumnDefinition this[int index]
		{
			get
			{
				return columnDefList[index];
			}

			set
			{
				columnDefList[index] = value;
			}
		}

		object IList.this[int index] { get => ((IList)columnDefList)[index]; set => ((IList)columnDefList)[index] = value; }

		//
		// Summary:
		//     Gets the total number of items within this instance of ColumnDefCollection
		//
		// Returns:
		//     The total number of items in the collection. This property has no default value.
		public int Count { get { return columnDefList.Count; } }
		//
		// Summary:
		//     Gets a value that indicates whether a ColumnDefCollection
		//     is read-only.
		//
		// Returns:
		//     true if the collection is read-only; otherwise false. This property has no default
		//     value.
		public bool IsReadOnly { get { return false; } }
		//
		// Summary:
		//     Gets a value that indicates whether access to this ColumnDefCollection
		//     is synchronized (thread-safe).
		//
		// Returns:
		//     true if access to this collection is synchronized; otherwise, false.
		public bool IsSynchronized { get { return false; } }
		//
		// Summary:
		//     Gets an object that can be used to synchronize access to the ColumnDefCollection
		//
		// Returns:
		//     An object that can be used to synchronize access to the ColumnDefCollection
		public object SyncRoot { get; }

		public bool IsFixedSize => ((IList)columnDefList).IsFixedSize;

		//
		// Summary:
		//     Adds a ColumnDef element to a ColumnDefCollection
		//
		// Parameters:
		//   value:
		//     Identifies the ColumnDef to add to the collection.
		public void Add(ColumnDefinition value)
		{
			columnDefList.Add(value);
		}

		public int Add(object value)
		{
			return ((IList)columnDefList).Add(value);
		}

		//
		// Summary:
		//     Clears the content of the ColumnDefCollection
		public void Clear()
		{
			columnDefList.Clear();
		}
		//
		// Summary:
		//     Determines whether a given ColumnDef exists within
		//     a ColumnDefCollection.
		//
		// Parameters:
		//   value:
		//     Identifies the ColumnDef that is being tested.
		//
		// Returns:
		//     true if the ColumnDef exists within the collection;
		//     otherwise false.
		public bool Contains(ColumnDefinition value)
		{
			return columnDefList.Contains(value);
		}

		public bool Contains(object value)
		{
			return ((IList)columnDefList).Contains(value);
		}

		//
		// Summary:
		//     Copies an array of ColumnDef objects to a given index
		//     position within a ColumnDefCollection.
		//
		// Parameters:
		//   array:
		//     An array of ColumnDef objects.
		//
		//   index:
		//     Identifies the index position within array to which the ColumnDef
		//     objects are copied.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     array is null.
		//
		//   T:System.ArgumentException:
		//     array is multidimensional.-or- The number of elements in the source System.Collections.ICollection
		//     is greater than the available space from index to the end of the destination
		//     array.
		//
		//   T:System.ArgumentOutOfRangeException:
		//     index is less than zero.
		public void CopyTo(ColumnDefinition[] array, int index)
		{
			columnDefList.CopyTo(array, index);
		}

		public void CopyTo(Array array, int index)
		{
			((IList)columnDefList).CopyTo(array, index);
		}

		public IEnumerator<ColumnDefinition> GetEnumerator()
		{
			return ((IList<ColumnDefinition>)columnDefList).GetEnumerator();
		}

		//
		// Summary:
		//     Returns the index position of a given ColumnDef within
		//     a ColumnDefCollection.
		//
		// Parameters:
		//   value:
		//     The ColumnDef whose index position is desired.
		//
		// Returns:
		//     The index of value if found in the collection; otherwise, -1.
		public int IndexOf(ColumnDefinition value)
		{
			return columnDefList.IndexOf(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)columnDefList).IndexOf(value);
		}

		//
		// Summary:
		//     Inserts a ColumnDef at the specified index position
		//     within a ColumnDef.
		//
		// Parameters:
		//   index:
		//     The position within the collection where the item is inserted.
		//
		//   value:
		//     The ColumnDef to insert.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		public void Insert(int index, ColumnDefinition value)
		{
			columnDefList.Insert(index, value);
		}

		public void Insert(int index, object value)
		{
			((IList)columnDefList).Insert(index, value);
		}

		//
		// Summary:
		//     Removes a ColumnDef from a ColumnDefCollection.
		//
		// Parameters:
		//   value:
		//     The ColumnDef to remove from the collection.
		//
		// Returns:
		//     true if the ColumnDef was found in the collection
		//     and removed; otherwise, false.
		public bool Remove(ColumnDefinition value)
		{
			return columnDefList.Remove(value);
		}

		public void Remove(object value)
		{
			((IList)columnDefList).Remove(value);
		}

		//
		// Summary:
		//     Removes a ColumnDef from a ColumnDefCollection
		//     at the specified index position.
		//
		// Parameters:
		//   index:
		//     The position within the collection at which the ColumnDef
		//     is removed.
		public void RemoveAt(int index)
		{
			columnDefList.RemoveAt(index);
		}
		//
		// Summary:
		//     Removes a range of ColumnDef objects from a ColumnDefCollection.
		//
		// Parameters:
		//   index:
		//     The position within the collection at which the first ColumnDef
		//     is removed.
		//
		//   count:
		//     The total number of ColumnDef objects to remove from
		//     the collection.
		public void RemoveRange(int index, int count)
		{
			columnDefList.RemoveRange(index, count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<ColumnDefinition>)columnDefList).GetEnumerator();
		}
	}

	//
	// Summary:
	//     Provides access to an ordered, strongly typed collection of System.Windows.Controls.RowDefinition
	//     objects.
	//???[System.Reflection.DefaultMember("Item")]
	public sealed class RowDefCollection : IList<RowDefinition>, ICollection<RowDefinition>, IEnumerable<RowDefinition>, IEnumerable, IList, ICollection
	{
		private List<RowDefinition> rowDefList = new List<RowDefinition>();

		//
		// Summary:
		//     Gets a value that indicates the current item within a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   index:
		//     The current item in the collection.
		//
		// Returns:
		//     The element at the specified index.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is not a valid index position in the collection.
		public RowDefinition this[int index]
		{
			get
			{
				return rowDefList[index];
			}

			set
			{
				rowDefList[index] = value;
			}
		}

		object IList.this[int index] { get => ((IList)rowDefList)[index]; set => ((IList)rowDefList)[index] = value; }

		//
		// Summary:
		//     Gets the total number of items within this instance of System.Windows.Controls.RowDefinitionCollection.
		//
		// Returns:
		//     The total number of items in the collection. This property has no default value.
		public int Count { get { return rowDefList.Count; } }
		//
		// Summary:
		//     Gets a value that indicates whether a System.Windows.Controls.RowDefinitionCollection
		//     is read-only.
		//
		// Returns:
		//     true if the collection is read-only; otherwise false. This property has no default
		//     value.
		public bool IsReadOnly { get { return false; } }
		//
		// Summary:
		//     Gets a value that indicates whether access to this System.Windows.Controls.RowDefinitionCollection
		//     is synchronized (thread-safe).
		//
		// Returns:
		//     true if access to this collection is synchronized; otherwise, false.
		public bool IsSynchronized { get { return false; } }
		//
		// Summary:
		//     Gets an object that can be used to synchronize access to the System.Windows.Controls.RowDefinitionCollection.
		//
		// Returns:
		//     An object that can be used to synchronize access to the System.Windows.Controls.RowDefinitionCollection.
		public object SyncRoot { get; }

		public bool IsFixedSize => ((IList)rowDefList).IsFixedSize;

		//
		// Summary:
		//     Adds a System.Windows.Controls.RowDefinition element to a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   value:
		//     Identifies the System.Windows.Controls.RowDefinition to add to the collection.
		public void Add(RowDefinition value)
		{
			rowDefList.Add(value);
		}

		public int Add(object value)
		{
			return ((IList)rowDefList).Add(value);
		}

		//
		// Summary:
		//     Clears the content of the System.Windows.Controls.RowDefinitionCollection.
		public void Clear()
		{
			rowDefList.Clear();
		}
		//
		// Summary:
		//     Determines whether a given System.Windows.Controls.RowDefinition exists within
		//     a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   value:
		//     Identifies the System.Windows.Controls.RowDefinition that is being tested.
		//
		// Returns:
		//     true if the System.Windows.Controls.RowDefinition exists within the collection;
		//     otherwise false.
		public bool Contains(RowDefinition value)
		{
			return rowDefList.Contains(value);
		}

		public bool Contains(object value)
		{
			return ((IList)rowDefList).Contains(value);
		}

		//
		// Summary:
		//     Copies an array of System.Windows.Controls.RowDefinition objects to a given index
		//     position within a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   array:
		//     An array of System.Windows.Controls.RowDefinition objects.
		//
		//   index:
		//     Identifies the index position within array to which the System.Windows.Controls.RowDefinition
		//     objects are copied.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     array is null.
		//
		//   T:System.ArgumentException:
		//     array is multidimensional.-or- The number of elements in the source System.Collections.ICollection
		//     is greater than the available space from index to the end of the destination
		//     array.
		//
		//   T:System.ArgumentOutOfRangeException:
		//     index is less than zero.
		public void CopyTo(RowDefinition[] array, int index)
		{
			rowDefList.CopyTo(array, index);
		}

		public void CopyTo(Array array, int index)
		{
			((IList)rowDefList).CopyTo(array, index);
		}

		public IEnumerator<RowDefinition> GetEnumerator()
		{
			return ((IList<RowDefinition>)rowDefList).GetEnumerator();
		}

		//
		// Summary:
		//     Returns the index position of a given System.Windows.Controls.RowDefinition within
		//     a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   value:
		//     The System.Windows.Controls.RowDefinition whose index position is desired.
		//
		// Returns:
		//     The index of value if found in the collection; otherwise, -1.
		public int IndexOf(RowDefinition value)
		{
			return rowDefList.IndexOf(value);
		}

		public int IndexOf(object value)
		{
			return ((IList)rowDefList).IndexOf(value);
		}

		//
		// Summary:
		//     Inserts a System.Windows.Controls.RowDefinition at the specified index position
		//     within a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   index:
		//     The position within the collection where the item is inserted.
		//
		//   value:
		//     The System.Windows.Controls.RowDefinition to insert.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     index is not a valid index in the System.Collections.IList.
		public void Insert(int index, RowDefinition value)
		{
			rowDefList.Insert(index, value);
		}

		public void Insert(int index, object value)
		{
			((IList)rowDefList).Insert(index, value);
		}

		//
		// Summary:
		//     Removes a System.Windows.Controls.RowDefinition from a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   value:
		//     The System.Windows.Controls.RowDefinition to remove from the collection.
		//
		// Returns:
		//     true if the System.Windows.Controls.RowDefinition was found in the collection
		//     and removed; otherwise, false.
		public bool Remove(RowDefinition value)
		{
			return rowDefList.Remove(value);
		}

		public void Remove(object value)
		{
			((IList)rowDefList).Remove(value);
		}

		//
		// Summary:
		//     Removes a System.Windows.Controls.RowDefinition from a System.Windows.Controls.RowDefinitionCollection
		//     at the specified index position.
		//
		// Parameters:
		//   index:
		//     The position within the collection at which the System.Windows.Controls.RowDefinition
		//     is removed.
		public void RemoveAt(int index)
		{
			rowDefList.RemoveAt(index);
		}
		//
		// Summary:
		//     Removes a range of System.Windows.Controls.RowDefinition objects from a System.Windows.Controls.RowDefinitionCollection.
		//
		// Parameters:
		//   index:
		//     The position within the collection at which the first System.Windows.Controls.RowDefinition
		//     is removed.
		//
		//   count:
		//     The total number of System.Windows.Controls.RowDefinition objects to remove from
		//     the collection.
		public void RemoveRange(int index, int count)
		{
			rowDefList.RemoveRange(index, count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IList<RowDefinition>)rowDefList).GetEnumerator();
		}
	}
}
