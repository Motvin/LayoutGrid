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
		private int FindGridParentSiblingIndex()
		{
			int idx = 0;
			LayoutGrid gp = this;
			FrameworkElement parent = gp.Parent as FrameworkElement;
			do
			{
				if (parent == null)
				{
					break;
				}

				gp = parent as LayoutGrid;
				if (gp != null)
				{
					if (gp.gridLinesParentSiblingIndex >= 0)
					{
						idx = gp.gridLinesParentSiblingIndex + 1;

						// look for any existing sibling LayoutGrids (these are the parent's children) - go backwards because we want the last LayoutGrid sibling

						int childrenCount = gp.InternalChildren.Count;
						for (int i = childrenCount - 1; i >= 0; i--)
						{
							UIElement child = gp.InternalChildren[i];
							LayoutGrid g = child as LayoutGrid;
							if (g != null)
							{
								if (g.gridLinesParentSiblingIndex >= 0)
								{
									idx = g.gridLinesParentSiblingIndex + 1;
									break;
								}
							}
						}
					}
				}
				parent = parent.Parent as FrameworkElement;

			} while (true);

			return idx;
		}

		private static bool IsDebugMode()
		{
			return System.Diagnostics.Debugger.IsAttached;
		}

		public enum GridLinesColorMixType
		{
			None,
			Single,
			Distinct,
			HeatMap // blue outer, teal inner, then light green, green, yellow, orange, red
		}

		private int gridLinesParentSiblingIndex = -1; // -1 means this isn't determined yet  - don't determine until necessary (before drawing the grid lines) ??? move this to other member vars. declarations - 

		//??? these are distinct colors that can be used for different grid lines - could also look at visual studio colors
		// vis. studio dash pattern is 3/3, a selection box (int Paint) dash pattern is 4 blue, 4 white
		private static Color[] distinctColorArray = new Color[]
		{
			Color.FromArgb(255, 230, 25, 75), // redColor
			Color.FromArgb(255, 60, 180, 75), // greenColor
			Color.FromArgb(255, 0, 130, 200), // blueColor

			Color.FromArgb(255, 245, 130, 48), // orangeColor
			Color.FromArgb(255, 70, 240, 240), //cyanColor
			Color.FromArgb(255, 240, 50, 230), // magentaColor

			Color.FromArgb(255, 255, 255, 25), // yellowColor
			Color.FromArgb(255, 128, 0, 0), // maroonColor
			Color.FromArgb(255, 0, 128, 128), // tealColor

			Color.FromArgb(255, 0, 0, 128), // navyColor
			Color.FromArgb(255, 170, 110, 40), // brownColor
			Color.FromArgb(255, 250, 190, 190), // pinkColor

			Color.FromArgb(255, 255, 250, 200), // beigeColor
			Color.FromArgb(255, 170, 255, 195), // mintColor
			Color.FromArgb(255, 230, 190, 255), // lavendarColor
		};

		//??? i'm not sure that these properties affect both arrange and measure
		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty ColumnProperty = 
			DependencyProperty.RegisterAttached("Column", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty RowProperty = 
			DependencyProperty.RegisterAttached("Row", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty ColumnSpanProperty = 
			DependencyProperty.RegisterAttached("ColumnSpan", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		//[CommonDependencyPropertyAttribute]
		public static readonly DependencyProperty RowSpanProperty = 
			DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(LayoutGrid), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public static readonly DependencyProperty IsSharedSizeScopeProperty = 
			DependencyProperty.RegisterAttached("IsSharedSizeScope", typeof(bool), typeof(LayoutGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public static readonly DependencyProperty ShowGridLinesProperty =
			DependencyProperty.RegisterAttached("ShowGridLines", typeof(bool), typeof(LayoutGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnShowGridLinesPropertyChanged)));

		public static readonly DependencyProperty GridLinesColorMixProperty =
			DependencyProperty.RegisterAttached("GridLinesColorMix", typeof(GridLinesColorMixType), typeof(LayoutGrid), new FrameworkPropertyMetadata(GridLinesColorMixType.None, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnGridLinesColorMixPropertyChanged)));

		private static void OnShowGridLinesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LayoutGrid g = d as LayoutGrid;

			if (g != null)
			{
				bool val = false;
				if (e.NewValue is bool)
				{
					val = (bool)e.NewValue;
				}

				if (val)
				{
					g.gridLinesVisual = new LayoutGridLinesVisual();
				}
				g.InvalidateVisual();
			}
			//???
		}

		private static void OnGridLinesColorMixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			LayoutGrid g = d as LayoutGrid;

			if (g != null)
			{
				GridLinesColorMixType val = GridLinesColorMixType.None;
				if (e.NewValue is GridLinesColorMixType)
				{
					val = (GridLinesColorMixType)e.NewValue;
				}

				if (val != GridLinesColorMixType.None)
				{
					g.gridLinesVisual = new LayoutGridLinesVisual();
				}
				g.InvalidateVisual();
			}
			//???
		}

		//private bool showGridLines;//??? is it ok to use this instead of GetValue(ShowGridLinesProperty, ...)
		public bool ShowGridLines
		{
			get
			{
				return (bool)GetValue(ShowGridLinesProperty);
			}

			set
			{
				SetValue(ShowGridLinesProperty, value);
			}
		}

		public GridLinesColorMixType GridLinesColorMix
		{
			get
			{
				return (GridLinesColorMixType)GetValue(GridLinesColorMixProperty);
			}

			set
			{
				SetValue(GridLinesColorMixProperty, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColumnDefCollection ColumnDefinitions { get; } = new ColumnDefCollection();

		// probably need to make these dependency properties that affect arrange and measure???
		public double ColumnSpacing { get; set; }
		public double RowSpacing { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RowDefCollection RowDefinitions { get; } = new RowDefCollection();

		protected override int VisualChildrenCount { get { return base.VisualChildrenCount + (ShowGridLines ? 1 : 0); } } //??? maybe need to add in grid lines renderer if this is used, otherwise we don't have to override this at all

		// allow user to pass in the pen for grid lines drawing and to specify if outside border should be shown???
		private class LayoutGridLinesVisual : DrawingVisual
		{
			Brush gridLinesBrush;
			Pen gridLinesPen;

			public LayoutGridLinesVisual()
			{
				//??? create and freeze drawing objects needed to draw grid lines
				//mainCanvas.SnapsToDevicePixels = true;

				//gridLines = new Image();
				//gridLines.SetValue(Canvas.ZIndexProperty, 100);
				//gridLines.SnapsToDevicePixels = true;

				////Draw the grid        
				//DrawingVisual gridLinesVisual = new DrawingVisual();
				//DrawingContext dct = gridLinesVisual.RenderOpen();
				//Pen blackPen = new Pen(Brushes.Black, 1.0);
				//blackPen.Freeze();

				////Draw the horizontal lines        
				//Point x = new Point(0, 0.5);
				//Point y = new Point(_width, 0.5);
				//for (int i = 0; i <= _rows; i++)
				//{
				//	dct.DrawLine(blackPen, x, y);
				//	x.Offset(0, _yOffset);
				//	y.Offset(0, _yOffset);
				//}
				////Draw the vertical lines        
				//x = new Point(0.5, 0);
				//y = new Point(0.5, _height);
				//for (int i = 0; i <= _columns; i++)
				//{
				//	dct.DrawLine(blackPen, x, y);
				//	x.Offset(_xOffset, 0);
				//	y.Offset(_xOffset, 0);
				//}

				//dct.Close();

				//RenderTargetBitmap bmp = new RenderTargetBitmap((int)mainCanvas.ActualWidth, (int)mainCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
				//bmp.Render(gridLinesVisual);
				//bmp.Freeze();
				//gridLines.Source = bmp;

				//mainCanvas.Children.Add(gridLines);
			}

			//??? why is this called at the end of ArrangeOverride
			public void DrawGridLines(LayoutGrid g)
			{
				// this doesn't draw the outside grid border - maybe have an option to draw this???
				if (g.ColumnDefinitions.Count > 1 || g.RowDefinitions.Count > 1)
				{
					if (gridLinesPen == null)
					{
						Color brushColor;

						bool isBrushColorSet = false;
						GridLinesColorMixType mix = g.GridLinesColorMix;
						if (mix == GridLinesColorMixType.None) // try to get the mix from a parent if none because any other value is passed on to the children
						{
							// if a parent has a non-None mix value, then it is inherited from the parent to the child
							LayoutGrid gp = g;
							do
							{
								if (gp.Parent == null)
								{
									break;
								}

								gp = gp.Parent as LayoutGrid;
								if (gp != null)
								{
									mix = gp.GridLinesColorMix;
									if (mix != GridLinesColorMixType.None)
									{
										break;
									}
								}

							} while (true);
						}

						if (mix == GridLinesColorMixType.Distinct)
						{
							if (g.gridLinesParentSiblingIndex < 0)
							{
								g.gridLinesParentSiblingIndex = g.FindGridParentSiblingIndex();
							}

							brushColor = distinctColorArray[g.gridLinesParentSiblingIndex % distinctColorArray.Length];
							isBrushColorSet = true;
						}

						if (!isBrushColorSet)
						{
							brushColor = distinctColorArray[0];
						}

						gridLinesBrush = new SolidColorBrush(brushColor);
						gridLinesBrush.Freeze();
						gridLinesPen = new Pen(gridLinesBrush, 1.0);
						gridLinesPen.DashStyle = new DashStyle(new double[] { 2.0, 3.0 }, 0);
						gridLinesPen.StartLineCap = PenLineCap.Flat;
						gridLinesPen.EndLineCap = PenLineCap.Flat;
						gridLinesPen.Freeze();
					}

					g.SnapsToDevicePixels = true; //??? if not true originally, then set this back?
					//RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
					//SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
					using (DrawingContext dc = RenderOpen())
					{
						double lastColPos = 0;
						double lastRowPos = 0;

						if (g.rowInfoArrayCount > 0)
						{
							lastRowPos = Math.Round(g.rowInfoArray[g.rowInfoArrayCount - 1].spanExtraLength_Or_Position + g.rowInfoArray[g.rowInfoArrayCount - 1].constrainedLength - 1.0) + 0.5;
						}

						for (int i = 1; ; i++)
						{
							ref GridColRowInfo cr = ref g.colInfoArray[i];

							//Brush brush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // red
							//Pen pen = new Pen(brush, 1.0);
							//pen.DashStyle = new DashStyle(new double[] { 1, 3 }, 0);
							//pen.StartLineCap = PenLineCap.Flat;
							//pen.EndLineCap = PenLineCap.Flat;
							double x = Math.Round(cr.spanExtraLength_Or_Position) + 0.5;
							dc.DrawLine(gridLinesPen, new Point(x, 0.5), new Point(x, lastRowPos));

							if (i == g.colInfoArrayCount - 1)
							{
								lastColPos = Math.Round(cr.spanExtraLength_Or_Position + cr.constrainedLength - 1.0) + 0.5;
								break;
							}
						}

						for (int i = 1; i < g.rowInfoArrayCount; i++)
						{
							ref GridColRowInfo cr = ref g.rowInfoArray[i];

							//Brush brush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // red
							//Pen pen = new Pen(brush, 1.0);
							//pen.DashStyle = new DashStyle(new double[] { 1, 3 }, 0);
							//pen.StartLineCap = PenLineCap.Flat;
							//pen.EndLineCap = PenLineCap.Flat;
							double y = Math.Round(cr.spanExtraLength_Or_Position) + 0.5;
							dc.DrawLine(gridLinesPen, new Point(0.5, y), new Point(lastColPos, y));
						}
					}
				}
			}
		}
		LayoutGridLinesVisual gridLinesVisual;

		//??? maybe grid needs to implement these
		// the base version supports zindex, but maybe need to override this to display the gridlines
		protected override Visual GetVisualChild(int index)
		{
			if (ShowGridLines)
			{
				if (index == VisualChildrenCount - 1)
				{
					return gridLinesVisual; //??? return the Visual
				}
				else
				{
					return base.GetVisualChild(index);
				}
			}
			else
			{
				return base.GetVisualChild(index);
			}
			//return Children[index];
			//???return Children[(Children.Count - 1) - index];
		}

		//??? maybe grid needs to implement these
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{

		}

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

		private const ushort ChildFlag_SpanHasAuto = 1; // this flag must reflect the unitTypes (not effectiveUnitTypes)
		private const ushort ChildFlag_SpanHasStar = 1 << 1; // this flag must reflect the unitTypes (not effectiveUnitTypes)
		private const ushort ChildFlag_SpanHasPixel = 1 << 2; // don't actually use this for anything
		private const ushort ChildFlag_SpanHasAutoNoStar = 1 << 3; // this flag must reflect the effectiveUnitTypes
		private const ushort ChildFlag_IsLastOfSameSpan = 1 << 4;

		private const ushort ChildFlag_SpanHasAutoNoStar_Reset = (ushort)0xffff ^ ChildFlag_SpanHasAutoNoStar;
		private const ushort ChildFlag_SpanHasAutoOrStar = ChildFlag_SpanHasAuto | ChildFlag_SpanHasStar;

		[Flags]
		public enum LayoutGridUnitType : ushort
		{
			Auto = ChildFlag_SpanHasAuto,
			Star = ChildFlag_SpanHasStar,
			Pixel = ChildFlag_SpanHasPixel
		}

		private struct GridColRowInfo
		{
			public double constrainedLength; // this can be set again in MeasureOverride and ArrangeOverride
			public double spanExtraLength_Or_Position; // this has a dual purpose as auto spanExtraLength or star desired length (within MeasureOverride) and position (within ArrangeOverride)

			public double minLength;
			public double maxLength;

			public double stars_Or_AutoDesiredLength; // this has a dual purpose as stars (for a star effective col/row) or the desired length of auto when the auto starts a span that goes into a star

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
			public double minOrMaxPerStar; // this is minLength or maxLength divided by # of stars for the col/row
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

		//??? add a few more groups
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

#if DEBUG
		public List<Size> measureSizeParams = new List<Size>();
		public List<Size> measureSizeReturns = new List<Size>();

		public List<Size> arrangeSizeParams = new List<Size>();
		public List<Size> arrangeSizeReturns = new List<Size>();
#endif

		private CompareStarMinMaxByPerStar compareStarMinMaxByPerStar = new CompareStarMinMaxByPerStar(); //??? maybe create only if needed
		private CompareStarMinMaxByPerStarDesc compareStarMinMaxByPerStarDesc = new CompareStarMinMaxByPerStarDesc(); //??? maybe create only if needed
		private CompareChildInfoByCellGroup compareChildInfoByCellGroup;

		private bool haveColsChanged;
		private bool haveRowsChanged;
		private bool haveChildrenChanged;

		private bool haveStarColAndAutoRowChildren; // this doesn't change if infinite width forces star

		private bool wasInfiniteWidth;
		private bool wasInfiniteHeight;

		private int starColCount;
		private int starRowCount;

		private double totalStarsInCols;
		private double totalStarsInRows;

		private double totalStarColWidthDesired;
		private double totalStarRowHeightDesired;

		private double totalPixelColWidth; // this is the total width (constrained) for all Pixel (absolute) sized cols
		private double totalPixelRowHeight; // this is the total height (constrained) for all Pixel (absolute) sized rows

		private double totalAutoColWidth;
		private double totalAutoRowHeight;

		private double totalAutoColWidthDesired;
		private double totalAutoRowHeightDesired;

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

		StarMinMax[] starColMinArrayForInfinite;
		int starColMinArrayForInfiniteCount;

		private StarMinMax[] starColMaxArray; // the count of elements for this is starColCount

		private StarMinMax[] starRowMinArray;
		private int starRowMinArrayCount;

		StarMinMax[] starRowMinArrayForInfinite;
		int starRowMinArrayForInfiniteCount;

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

				cr.stars_Or_AutoDesiredLength = 1.0;
				cr.maxLength = double.PositiveInfinity;

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
						cr.minLength = c.MinWidth;
						cr.maxLength = c.MaxWidth;
					}
					else
					{
						// set both min and max to min - this is how Grid works - it uses min for both
						cr.minLength = c.MinWidth;
						cr.maxLength = c.MinWidth;
					}

					switch (cr.unitType) // use unitType instead of effectiveUnitType here
					{
						case LayoutGridUnitType.Pixel:
							if (c.Width.Value > cr.maxLength)
							{
								cr.constrainedLength = cr.maxLength;
							}
							else if (c.Width.Value < cr.minLength)
							{
								cr.constrainedLength = cr.minLength;
							}
							else
							{
								cr.constrainedLength = c.Width.Value;
							}

							totalPixelColWidth += cr.constrainedLength;
							break;
						case LayoutGridUnitType.Star:
							if (c.Width.Value == 0)
							{
								// treat this like a pixel size with a pixel length = min

								cr.unitType = LayoutGridUnitType.Pixel;
								cr.effectiveUnitType = LayoutGridUnitType.Pixel;
								cr.constrainedLength = cr.minLength;

								totalPixelColWidth += cr.constrainedLength;
							}
							else
							{
								if (cr.maxLength < smallestStarMaxPixelColWidth)
								{
									smallestStarMaxPixelColWidth = cr.maxLength;
								}

								if (cr.minLength > largestStarMinPixelColWidth)
								{
									largestStarMinPixelColWidth = cr.minLength;
								}

								starColCount++;
								cr.stars_Or_AutoDesiredLength = c.Width.Value;
								if (cr.minLength != 0)
								{
									starColCountMinNotZero++;
									totalMinStarColWidth += cr.minLength;
								}
								totalMaxStarColWidth += cr.maxLength;
								totalStarsInCols += cr.stars_Or_AutoDesiredLength;
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

					if (cr.unitType == LayoutGridUnitType.Star) // use unitType here instead of effectiveUnitType because this initializes values of the star arrays
					{
						s.index = i;
						if (double.IsPositiveInfinity(cr.maxLength))
						{
							s.minOrMaxPerStar = double.PositiveInfinity;
						}
						else
						{
							s.minOrMaxPerStar = cr.maxLength / cr.stars_Or_AutoDesiredLength;
						}

						starColMaxArray[starIndexMax++] = s;

						if (starColMinArrayCount > 0 && cr.minLength > 0)
						{
							s.minOrMaxPerStar = cr.minLength / cr.stars_Or_AutoDesiredLength;

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

		// this is the exact duplicate of CreateColInfo except that is renames all Col to Row and Width to Height
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

				cr.stars_Or_AutoDesiredLength = 1.0;
				cr.maxLength = double.PositiveInfinity;

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
						cr.minLength = c.MinHeight;
						cr.maxLength = c.MaxHeight;
					}
					else
					{
						// set both min and max to min - this is how Grid works - it uses min for both
						cr.minLength = c.MinHeight;
						cr.maxLength = c.MinHeight;
					}

					switch (cr.unitType) // use unitType instead of effectiveUnitType here
					{
						case LayoutGridUnitType.Pixel:
							if (c.Height.Value > cr.maxLength)
							{
								cr.constrainedLength = cr.maxLength;
							}
							else if (c.Height.Value < cr.minLength)
							{
								cr.constrainedLength = cr.minLength;
							}
							else
							{
								cr.constrainedLength = c.Height.Value;
							}

							totalPixelRowHeight += cr.constrainedLength;
							break;
						case LayoutGridUnitType.Star:
							if (c.Height.Value == 0)
							{
								// treat this like a pixel size with a pixel length = min

								cr.unitType = LayoutGridUnitType.Pixel;
								cr.effectiveUnitType = LayoutGridUnitType.Pixel;
								cr.constrainedLength = cr.minLength;

								totalPixelRowHeight += cr.constrainedLength;
							}
							else
							{
								if (cr.maxLength < smallestStarMaxPixelRowHeight)
								{
									smallestStarMaxPixelRowHeight = cr.maxLength;
								}

								if (cr.minLength > largestStarMinPixelRowHeight)
								{
									largestStarMinPixelRowHeight = cr.minLength;
								}

								starRowCount++;
								cr.stars_Or_AutoDesiredLength = c.Height.Value;
								if (cr.minLength != 0)
								{
									starRowCountMinNotZero++;
									totalMinStarRowHeight += cr.minLength;
								}
								totalMaxStarRowHeight += cr.maxLength;
								totalStarsInRows += cr.stars_Or_AutoDesiredLength;
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

					if (cr.unitType == LayoutGridUnitType.Star) // use unitType here instead of effectiveUnitType because this initializes values of the star arrays
					{
						s.index = i;
						if (double.IsPositiveInfinity(cr.maxLength))
						{
							s.minOrMaxPerStar = double.PositiveInfinity;
						}
						else
						{
							s.minOrMaxPerStar = cr.maxLength / cr.stars_Or_AutoDesiredLength;
						}

						starRowMaxArray[starIndexMax++] = s;

						if (starRowMinArrayCount > 0 && cr.minLength > 0)
						{
							s.minOrMaxPerStar = cr.minLength / cr.stars_Or_AutoDesiredLength;

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
					cr.spanExtraLength_Or_Position = 0;

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Auto;
						cr.constrainedLength = 0;
					}
				}
			}
			else
			{
				for (int i = 0; i < infoArrayCount; i++)
				{
					ref GridColRowInfo cr = ref infoArray[i];

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						cr.effectiveUnitType = LayoutGridUnitType.Star;
						cr.constrainedLength = 0; // need to set to 0 here because DistributeStarColWidth and DistributeStarRowHeight rely on this

						cr.spanExtraLength_Or_Position = cr.minLength; // this is used for effective star to store the desired length - not sure if we want to min constrain this??? - if this works, need to do the same thing in CreateColInfo/CreateRowInfo
					}
					else
					{
						cr.spanExtraLength_Or_Position = 0;
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

				// cellGroup must be set based on the unitTypes, not effectiveUnitTypes
				//LayoutGridUnitType colType = colInfoArray[col].unitType;
				//LayoutGridUnitType rowType = rowInfoArray[row].unitType;

				LayoutGridUnitType colType = colInfoArray[col].effectiveUnitType;
				LayoutGridUnitType rowType = rowInfoArray[row].effectiveUnitType;

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
						n.colFlags |= (ushort)colInfoArray[col].unitType;
						n.rowFlags |= (ushort)rowInfoArray[row].unitType;
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
						n.colFlags |= (ushort)colInfoArray[col].unitType;
						n.rowFlags |= (ushort)rowInfoArray[row].unitType;
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
						n.colFlags |= (ushort)colInfoArray[col].unitType;
						n.rowFlags |= (ushort)rowInfoArray[row].unitType;

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
						n.colFlags |= (ushort)colInfoArray[col].unitType;
						n.rowFlags |= (ushort)rowInfoArray[row].unitType;
					}
				}
				else
				{
					//??? probably have to reset/get this stuff and re-sort if infinite width/height and star is treated as auto

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

						if (isInfiniteWidth)
						{
							if ((colFlags & ChildFlag_SpanHasAutoOrStar) != 0)
							{
								colFlags |= ChildFlag_SpanHasAutoNoStar;
							}
						}
						else
						{
							if ((colFlags & ChildFlag_SpanHasAuto) != 0 && ((colFlags & ChildFlag_SpanHasStar) == 0))
							{
								colFlags |= ChildFlag_SpanHasAutoNoStar;
							}
						}
					}
					else
					{
						colFlags |= (ushort)colInfoArray[col].unitType;
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

						if (isInfiniteHeight)
						{
							if ((rowFlags & ChildFlag_SpanHasAutoOrStar) != 0)
							{
								rowFlags |= ChildFlag_SpanHasAutoNoStar;
							}
						}
						else
						{
							if ((rowFlags & ChildFlag_SpanHasAuto) != 0 && ((rowFlags & ChildFlag_SpanHasStar) == 0))
							{
								rowFlags |= ChildFlag_SpanHasAutoNoStar;
							}
						}
					}
					else
					{
						rowFlags |= (ushort)rowInfoArray[row].unitType;
					}

					//bool colOrSpanHasStars = ((colFlags & ChildFlag_SpanHasStar) != 0 || (colType == LayoutGridUnitType.Star)) && !isInfiniteWidth;
					//bool rowOrSpanHasStars = ((rowFlags & ChildFlag_SpanHasStar) != 0 || (rowType == LayoutGridUnitType.Star)) && !isInfiniteHeight;

					bool colOrSpanHasStars = (colFlags & ChildFlag_SpanHasStar) != 0 && !isInfiniteWidth;
					bool rowOrSpanHasStars = (rowFlags & ChildFlag_SpanHasStar) != 0 && !isInfiniteHeight;
					bool colOrSpanHasAuto = (colFlags & ChildFlag_SpanHasAuto) != 0 || (isInfiniteWidth && (colFlags & ChildFlag_SpanHasStar) != 0);
					bool rowOrSpanHasAuto = (rowFlags & ChildFlag_SpanHasAuto) != 0 || (isInfiniteHeight && (rowFlags & ChildFlag_SpanHasStar) != 0);

					haveStarColAndAutoRowChildren |= colOrSpanHasStars && rowOrSpanHasAuto && !rowOrSpanHasStars; // if there is a row span from a star into an auto, this doesn't count because those spans aren't distributed
					//haveStarColAndAutoRowChildren |= colOrSpanHasStars && (rowType == LayoutGridUnitType.Auto); // if there is a row span from a star into an auto, this doesn't count because those spans aren't distributed
					//haveStarColAndAutoRowChildren |= colOrSpanHasStars && rowOrSpanHasAuto;
					//haveStarColAndAutoRowChildren |= colOrSpanHasStars && (rowFlags & ChildFlag_SpanHasAuto) != 0;
					//haveStarColAndAutoRowChildren |= colOrSpanHasStars && ((rowFlags & ChildFlag_SpanHasAutoNoStar) != 0);

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
					else if (!colOrSpanHasStars && colOrSpanHasAuto && rowOrSpanHasStars)
					//else if (!colOrSpanHasStars && (n.colFlags & ChildFlag_SpanHasAuto) != 0 && rowOrSpanHasStars)
					//else if (!colOrSpanHasStars && ((n.colFlags & ChildFlag_SpanHasAuto) != 0 || (colType == LayoutGridUnitType.Auto)) && rowOrSpanHasStars)
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

		private void DistributeStarColWidth(double starLength, StarMinMax [] starMinArray, int starMinArrayCount)
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
					cr.constrainedLength = cr.minLength;
				}
			}
			else if (starLength >= totalMaxStarColWidth)
			{
				// just assign max to all star cols
				for (int i = 0; i < starColCount; i++)
				{
					idx = starColMaxArray[i].index;

					ref GridColRowInfo cr = ref colInfoArray[idx];
					cr.constrainedLength = cr.maxLength;
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
				for (int i = 0; i < starMinArrayCount; i++)
				{
					ref StarMinMax t = ref starMinArray[i];

					ref GridColRowInfo cr = ref colInfoArray[t.index];

					if (pixelsPerStar < t.minOrMaxPerStar)
					{
						availablePixels -= cr.minLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedLength = cr.minLength;

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
						availablePixels -= cr.maxLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedLength = cr.maxLength;

						unresolvedCount--;

						j = lastMinConstrainedIndex;
						while (j >= 0)
						{
							ref StarMinMax tMin = ref starMinArray[j];

							ref GridColRowInfo crMin = ref colInfoArray[tMin.index];

							if (pixelsPerStar > tMin.minOrMaxPerStar)
							{
								availablePixels += crMin.minLength;
								availableStars += crMin.stars_Or_AutoDesiredLength;

								pixelsPerStar = availablePixels / availableStars;

								crMin.constrainedLength = 0; // this will get reprocessed in the outer max loop

								lastMinConstrainedIndex = --j;

								unresolvedCount++;
							}
							else
							{
								break;
							}
						}
					}
					else if (cr.constrainedLength == 0)
					{
						if (unresolvedCount == 1)
						{
							cr.constrainedLength = availablePixels; // the last col/row takes all remaining pixels

							break;
						}

						cr.constrainedLength = cr.stars_Or_AutoDesiredLength * pixelsPerStar;

						availablePixels -= cr.constrainedLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

						unresolvedCount--;
					}
				}
			}
		}

		// this is the exact same code as DistributeStarColWidth except it uses Row instead of Col and Height instead of Width - maybe make this a single function???
		private void DistributeStarRowHeight(double starLength, StarMinMax[] starMinArray, int starMinArrayCount)
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
					cr.constrainedLength = cr.minLength;
				}
			}
			else if (starLength >= totalMaxStarRowHeight)
			{
				// just assign max to all star rows
				for (int i = 0; i < starRowCount; i++)
				{
					idx = starRowMaxArray[i].index;

					ref GridColRowInfo cr = ref rowInfoArray[idx];
					cr.constrainedLength = cr.maxLength;
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
				for (int i = 0; i < starMinArrayCount; i++)
				{
					ref StarMinMax t = ref starMinArray[i];

					ref GridColRowInfo cr = ref rowInfoArray[t.index];

					if (pixelsPerStar < t.minOrMaxPerStar)
					{
						availablePixels -= cr.minLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedLength = cr.minLength;

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
						availablePixels -= cr.maxLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

						pixelsPerStar = availablePixels / availableStars;

						cr.constrainedLength = cr.maxLength;

						unresolvedCount--;

						j = lastMinConstrainedIndex;
						while (j >= 0)
						{
							ref StarMinMax tMin = ref starMinArray[j];

							ref GridColRowInfo crMin = ref rowInfoArray[tMin.index];

							if (pixelsPerStar > tMin.minOrMaxPerStar)
							{
								availablePixels += crMin.minLength;
								availableStars += crMin.stars_Or_AutoDesiredLength;

								pixelsPerStar = availablePixels / availableStars;

								crMin.constrainedLength = 0; // this will get reprocessed in the outer max loop

								lastMinConstrainedIndex = --j;

								unresolvedCount++;
							}
							else
							{
								break;
							}
						}
					}
					else if (cr.constrainedLength == 0)
					{
						if (unresolvedCount == 1)
						{
							cr.constrainedLength = availablePixels; // the last row/row takes all remaining pixels

							break;
						}

						cr.constrainedLength = cr.stars_Or_AutoDesiredLength * pixelsPerStar;

						availablePixels -= cr.constrainedLength;
						availableStars -= cr.stars_Or_AutoDesiredLength;

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
						double length;
						if (cr.constrainedLength >= cr.minLength)
						{
							length = cr.constrainedLength;
						}
						else
						{
							length = cr.minLength;
						}

						if (length + extraLengthPerAuto > cr.maxLength)
						{
							noMaxConstrainedInThisLoop = false;

							spanUnresolvedAutoCount--;

							used = cr.maxLength - length;
							remainingSpanExtraLength -= used;

							cr.spanExtraLength_Or_Position = 0;
							cr.constrainedLength = cr.maxLength;

							extraLengthPerAuto = remainingSpanExtraLength / spanUnresolvedAutoCount;

							cr.isResolved = true;
						}
						else if (spanUnresolvedAutoCount == 1 || noMaxConstrainedInPriorLoop)
						{
							used = extraLengthPerAuto - cr.spanExtraLength_Or_Position;

							if (used > availableExtraToDistribute)
							{
								cr.spanExtraLength_Or_Position += availableExtraToDistribute;

								return;
							}
							else if (used > 0)
							{
								cr.spanExtraLength_Or_Position = extraLengthPerAuto;

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
				ref GridColRowInfo cr = ref infoArray[i];

				//??? I think this can just be length += n.constrainedLength + n.spanExtraLength; without any if/else because we should be min constrained at this point - check this - probably not
				//??? I'm not sure spanExtraLengthOrPosition can be used for effective star below - maybe need an if statement
				if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
				{
					if (cr.constrainedLength + cr.spanExtraLength_Or_Position >= cr.minLength)
					{
						length += cr.constrainedLength + cr.spanExtraLength_Or_Position;
					}
					else
					{
						length += cr.minLength; // auto might not have been min constrained at this point, but we want min constrained for span length??? I think MinSize is not minLength
					}
				}
				else
				{
					if (cr.constrainedLength >= cr.minLength)
					{
						length += cr.constrainedLength;
					}
					else
					{
						length += cr.minLength; // star might not have been min constrained at this point?, but we want min constrained for span length??? I think MinSize is not minLength
					}
				}

				i++;
			} while (i < maxColOrRowInSpanPlus1);

			return length;
		}

		private double GetSpanLengthWithDesiredStar(GridColRowInfo[] infoArray, int startColOrRow, int span)
		{
			int maxColOrRowInSpanPlus1 = startColOrRow + span; // instead of passing in span, keep maxColOrRowInSpan calculated and use it???

			double length = 0;

			int i = startColOrRow;
			do
			{
				ref GridColRowInfo cr = ref infoArray[i];

				//??? I think this can just be length += n.constrainedLength + n.spanExtraLength; without any if/else because we should be min constrained at this point - check this - probably not
				//??? I'm not sure spanExtraLengthOrPosition can be used for effective star below - maybe need an if statement
				if (cr.effectiveUnitType == LayoutGridUnitType.Auto)
				{
					if (cr.constrainedLength + cr.spanExtraLength_Or_Position >= cr.minLength)
					{
						length += cr.constrainedLength + cr.spanExtraLength_Or_Position;
					}
					else
					{
						length += cr.minLength; // auto might not have been min constrained at this point, but we want min constrained for span length??? I think MinSize is not minLength
					}
				}
				else if (cr.effectiveUnitType == LayoutGridUnitType.Star)
				{
					length += cr.spanExtraLength_Or_Position; // for star this is the desired length
				}
				else
				{
					if (cr.constrainedLength >= cr.minLength)
					{
						length += cr.constrainedLength;
					}
					else
					{
						length += cr.minLength; // star might not have been min constrained at this point?, but we want min constrained for span length??? I think MinSize is not minLength
					}
				}

				i++;
			} while (i < maxColOrRowInSpanPlus1);

			return length;
		}

		//??? remove these
		public double actualColWidth;
		public double actualRowHeight;

		protected override Size MeasureOverride(Size availableSize)
		{
#if DEBUG
			measureSizeParams.Add(availableSize);
#endif

			Size availableChildSize;
			double availableChildWidth;
			double availableChildHeight;
			int i;
			int colSpan;
			int rowSpan;

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
			totalAutoColWidthDesired = 0;
			totalAutoRowHeightDesired = 0;
			totalStarColWidthDesired = 0;
			totalStarRowHeightDesired = 0;

			bool isInfiniteWidth = double.IsPositiveInfinity(availableSize.Width);
			bool isInfiniteHeight = double.IsPositiveInfinity(availableSize.Height);

			if (ColumnDefinitions.Count <= 1 && RowDefinitions.Count <= 1)
			{
				// there is only a single cell in the grid (single column and single row), so we don't need to do the expensive processing, just do the easy/inexpensive processing below and return
				double minWidth;
				double maxWidth;
				//bool colIsAuto = false;
				//bool rowIsAuto = false;
				bool isColPixel = false;
				bool isRowPixel = false;
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
						//colIsAuto = true;
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
						isColPixel = true;
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
						//rowIsAuto = true;
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
						isRowPixel = true;
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

				//if (!double.IsPositiveInfinity(availableChildWidth)) // this is auto or star that is effectively auto
				if (isColPixel)
				{
					desiredWidth = availableChildWidth;
				}

				//if (!double.IsPositiveInfinity(availableChildHeight)) // this is auto or star that is effectively auto
				if (isRowPixel) // this is auto or star that is effectively auto
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

						//if (double.IsPositiveInfinity(availableChildWidth))
						if (!isColPixel)
						{
							double width = child.DesiredSize.Width;

							if (width > desiredWidth)
							{
								desiredWidth = width;
								//if (colIsAuto)
								//{
								//	totalAutoColWidth = width;
								//}
							}
						}

						//if (double.IsPositiveInfinity(availableChildHeight))
						if (!isRowPixel)
						{
							double height = child.DesiredSize.Height;

							if (height > desiredHeight)
							{
								desiredHeight = height;
								//if (rowIsAuto)
								//{
								//	totalAutoRowHeight = height;
								//}
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

				// the code below just sets actualColWidth/actualRowHeight, but we really only need this for testing for now, so remove when we set this in the ColumnDefition, RowDefition??? - probably can just used desired instead of actual anyway? No, sometimes they are different (like for star)
				if (double.IsPositiveInfinity(availableChildWidth)) // this is auto or star that is effectively auto
				{
					actualColWidth = desiredWidth;
				}
				else
				{
					actualColWidth = availableChildWidth;
				}

				if (double.IsPositiveInfinity(availableChildHeight)) // this is auto or star that is effectively auto
				{
					actualRowHeight = desiredHeight;
				}
				else
				{
					actualRowHeight = availableChildHeight;
				}

#if CollectPerformanceStats
				shortMeasureTicks = Stopwatch.GetTimestamp() - startTicks;
				shortMeasureCount++;
#endif

#if DEBUG
				measureSizeReturns.Add(new Size(desiredWidth, desiredHeight));
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
					//??? do we need to change the cell group and resort? yes
					haveStarColAndAutoRowChildren = false;

					for (i = 0; i < childInfoArrayCount - 1; i++)
					{
						ref ChildInfo n = ref childInfoArray[i];

						LayoutGridUnitType colType = colInfoArray[n.col].effectiveUnitType;
						LayoutGridUnitType rowType = rowInfoArray[n.row].effectiveUnitType;

						colSpan = n.colSpan;
						rowSpan = n.rowSpan;

						if (colSpan <= 1 && rowSpan <= 1)
						{
							// some of these if statements could be replaced by setting bit switches from the colType/rowType to determine the cellGroup??? - don't think it's worth it though
							if (colType == LayoutGridUnitType.Auto && rowType == LayoutGridUnitType.Star)
							{
								n.cellGroup = CellGroup_AutoColStarRow;
							}
							else if (rowType == LayoutGridUnitType.Star)
							{
								n.cellGroup = CellGroup_PixelOrStarColStarRow;
							}
							else if (colType == LayoutGridUnitType.Star)
							{
								n.cellGroup = CellGroup_StarColPixelOrAutoRow;

								haveStarColAndAutoRowChildren |= (rowType == LayoutGridUnitType.Auto);
							}
							else
							{
								n.cellGroup = CellGroup_PixelOrAutoColRow;
							}
						}
						else
						{
							if (isWidthDiff)
							{
								if (n.colSpan > 1)
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
							}

							if (isHeightDiff)
							{
								if (n.rowSpan > 1)
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

							bool colOrSpanHasStars = (n.colFlags & ChildFlag_SpanHasStar) != 0 && !isInfiniteWidth;
							bool rowOrSpanHasStars = (n.rowFlags & ChildFlag_SpanHasStar) != 0 && !isInfiniteHeight;
							bool colOrSpanHasAuto = (n.colFlags & ChildFlag_SpanHasAuto) != 0 || (isInfiniteWidth && (n.colFlags & ChildFlag_SpanHasStar) != 0);
							bool rowOrSpanHasAuto = (n.rowFlags & ChildFlag_SpanHasAuto) != 0 || (isInfiniteHeight && (n.rowFlags & ChildFlag_SpanHasStar) != 0);

							haveStarColAndAutoRowChildren |= colOrSpanHasStars && rowOrSpanHasAuto && !rowOrSpanHasStars; // if there is a row span from a star into an auto, this doesn't count because those spans aren't distributed
							//haveStarColAndAutoRowChildren |= colOrSpanHasStars && (rowType == LayoutGridUnitType.Auto); // if there is a row span from a star into an auto, this doesn't count because those spans aren't distributed
							//haveStarColAndAutoRowChildren |= colOrSpanHasStars && rowOrSpanHasAuto;

							if (!colOrSpanHasStars && !rowOrSpanHasStars)
							{
								n.cellGroup = CellGroup_PixelOrAutoColRowWithSpan;
							}
							else if (!colOrSpanHasStars && colOrSpanHasAuto && rowOrSpanHasStars)
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

					if (firstIndexForOtherCellGroups >= 0)
					{
						int len = childInfoArrayCount - firstIndexForOtherCellGroups;
						if (len > 1)
						{
							if (compareChildInfoByCellGroup == null)
							{
								compareChildInfoByCellGroup = new CompareChildInfoByCellGroup();
							}
							Array.Sort(childInfoArray, firstIndexForOtherCellGroups, len, compareChildInfoByCellGroup);
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
											totalAutoLengthBeforeDistribute += cr.constrainedLength;

											if (cr.constrainedLength >= cr.minLength)
											{
												totalAutoLengthBeforeDistributeWithMin += cr.constrainedLength;
											}
											else
											{
												totalAutoLengthBeforeDistributeWithMin += cr.minLength;
											}

											existingSpanExtraLength += cr.spanExtraLength_Or_Position;
											cr.isResolved = false;

											hasSomeMaxLength |= !double.IsPositiveInfinity(cr.maxLength);
										}
										else if (cr.effectiveUnitType == LayoutGridUnitType.Pixel)
										{
											totalPixelLength += cr.constrainedLength;
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
										totalAutoLengthBeforeDistribute += cr.constrainedLength;

										if (cr.constrainedLength >= cr.minLength)
										{
											totalAutoLengthBeforeDistributeWithMin += cr.constrainedLength;
										}
										else
										{
											totalAutoLengthBeforeDistributeWithMin += cr.minLength;
										}

										existingSpanExtraLength += cr.spanExtraLength_Or_Position;
										cr.isResolved = false;

										hasSomeMaxLength |= !double.IsPositiveInfinity(cr.maxLength);
										//??? is double.IsPositiveInfinity inlined - otherwise inline it - look at what it is
									}
									else if (cr.effectiveUnitType == LayoutGridUnitType.Pixel)
									{
										totalPixelLength += cr.constrainedLength;
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
								if (cr.constrainedLength < cr.minLength)
								{
									cr.constrainedLength = cr.minLength + cr.spanExtraLength_Or_Position;
								}
								else
								{
									cr.constrainedLength += cr.spanExtraLength_Or_Position; //??? for some reason I thought we were constrained by max even if auto span, but this doesn't constrain by max here
								}
								cr.spanExtraLength_Or_Position = 0;

								if (cr.unitType == LayoutGridUnitType.Auto)
								{
									totalAutoColWidth += cr.constrainedLength; // don't add to this if it is a star treated as auto
								}
								//else
								//{
								//	totalStarColWidthDesired += cr.constrainedLength;
								//}
							}
						}
					}

					if (!isStarColLengthResolved && cellGroup >= CellGroup_StarColPixelOrAutoRow)
					{
						//totalStarColWidth = availableSize.Width - (totalPixelColWidth + totalAutoColWidth);//??? if this temp var totalStarColWidth isn't used remove it
						DistributeStarColWidth(availableSize.Width - (totalPixelColWidth + totalAutoColWidth), starColMinArray, starColMinArrayCount);
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
								if (cr.constrainedLength < cr.minLength)
								{
									cr.constrainedLength = cr.minLength + cr.spanExtraLength_Or_Position;
								}
								else
								{
									cr.constrainedLength += cr.spanExtraLength_Or_Position;
								}
								cr.spanExtraLength_Or_Position = 0;

								if (cr.unitType == LayoutGridUnitType.Auto)
								{
									totalAutoRowHeight += cr.constrainedLength; // don't add to this if it is a star treated as auto
								}
								//else
								//{
								//	totalStarRowHeightDesired += cr.constrainedLength;
								//}
							}
						}
					}

					if (!isStarRowLengthResolved && cellGroup >= distributeStarRowHeightBeforeCellGroup)
					{
						//totalStarRowHeight = availableSize.Height - (totalPixelRowHeight + totalAutoRowHeight);
						DistributeStarRowHeight(availableSize.Height - (totalPixelRowHeight + totalAutoRowHeight), starRowMinArray, starRowMinArrayCount);
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
						for (int j = firstChildIndexOfAutoColStarRowCellGroup; ; j++)
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

							rowSpan = n2.rowSpan;

							if (rowSpan > 1)
							{
								availableChildHeight = GetSpanLength(rowInfoArray, row, rowSpan);
							}
							else
							{
								availableChildHeight = r2.constrainedLength;
								//availableChildHeight = r2.constrainedLength + r2.spanExtraLengthOrPosition;
							}

							availableChildSize.Width = double.PositiveInfinity;
							availableChildSize.Height = availableChildHeight;

#if CollectPerformanceStats
							startTicksChildMeasure = Stopwatch.GetTimestamp();
#endif

							n2.child.Measure(availableChildSize); // call Measure for every child with the correct width and height that the child gets, even if we don't need the size - here Measure is called a 2nd time with the correct width and height

#if CollectPerformanceStats
							childMeasureTicks += Stopwatch.GetTimestamp() - startTicksChildMeasure;
#endif

							if (rowSpan == 1 && r2.effectiveUnitType == LayoutGridUnitType.Star)
							{
								length = n2.child.DesiredSize.Height;

								// for effective star, use spanExtraLengthOrPosition to store the desired size value - should this start out as the min value???
								if (length > r2.spanExtraLength_Or_Position)
								{
									if (length <= r2.maxLength)
									{
										r2.spanExtraLength_Or_Position = length;
									}
									else
									{
										r2.spanExtraLength_Or_Position = r2.maxLength;
									}
								}
							}
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

				Button btn = n.child as Button;
				if (btn != null && btn.Name == "btn79")
				{
					int afs = 1;//???
				}

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
						if (c.constrainedLength >= c.minLength)
						{
							availableChildWidth = c.constrainedLength;
						}
						else
						{
							availableChildWidth = c.minLength; //??? is this correct to use min
						}
					}
				}

				if (rowSpan > 1)
				{
					if ((n.rowFlags & ChildFlag_SpanHasAutoNoStar) != 0 || (remeasureAutoColStarRowGroup && (cellGroup == CellGroup_AutoColStarRow || cellGroup == CellGroup_AutoColStarRowWithSpan)))
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
					if (r.effectiveUnitType == LayoutGridUnitType.Auto || (remeasureAutoColStarRowGroup && (cellGroup == CellGroup_AutoColStarRow || cellGroup == CellGroup_AutoColStarRowWithSpan)))
					{
						availableChildHeight = double.PositiveInfinity;//??? I think this should be max?
					}
					else
					{
						if (r.constrainedLength >= r.minLength)
						{
							availableChildHeight = r.constrainedLength;
						}
						else
						{
							availableChildHeight = r.minLength; //??? is this correct to use min - it's possible to get here for star row and auto col
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
					length = n.child.DesiredSize.Width - c.spanExtraLength_Or_Position;

					if (length > c.constrainedLength)
					{
						if (length <= c.maxLength)
						{
							c.constrainedLength = length;
						}
						else
						{
							c.constrainedLength = c.maxLength;
						}
					}
				}
				else if (c.effectiveUnitType == LayoutGridUnitType.Star)
				{
					length = n.child.DesiredSize.Width;
					if (colSpan > 1)
					{
						double spanLength = GetSpanLengthWithDesiredStar(colInfoArray, col + 1, colSpan - 1); // this is the span length excluding this col
						length -= spanLength;
					}

					// for effective star, use spanExtraLengthOrPosition to store the desired size value - should this start out as the min value???
					if (length > c.spanExtraLength_Or_Position)
					{
						if (length <= c.maxLength)
						{
							c.spanExtraLength_Or_Position = length;
						}
						else
						{
							c.spanExtraLength_Or_Position = c.maxLength;
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
				else if (colSpan > 1 && c.effectiveUnitType == LayoutGridUnitType.Auto) // this is an auto and star span
				{
					length = n.child.DesiredSize.Width;

					if (colSpan > 1)
					{
						double spanLength = GetSpanLengthWithDesiredStar(colInfoArray, col + 1, colSpan - 1); // this is the span length excluding this col
						length -= spanLength;
					}

					if (c.unitType == LayoutGridUnitType.Auto)
					{
						if (length > c.stars_Or_AutoDesiredLength)
						{
							double desired;
							if (length <= c.maxLength)
							{
								desired = length;
							}
							else
							{
								desired = c.maxLength;
							}

							if (desired > c.constrainedLength && desired > c.stars_Or_AutoDesiredLength)
							{
								if (c.stars_Or_AutoDesiredLength > c.constrainedLength)
								{
									totalAutoColWidthDesired += (desired - c.constrainedLength) - (c.stars_Or_AutoDesiredLength - c.constrainedLength);
								}
								else
								{
									totalAutoColWidthDesired += (desired - c.constrainedLength);
								}
							}
							c.stars_Or_AutoDesiredLength = desired;
						}
					}
					else // must be star that is effectively auto
					{
						// not sure this works below because I think constrained gets added to desired at the wrong time??? - also, shouldn't this be span extra
						if (length > c.constrainedLength)
						{
							if (length <= c.maxLength)
							{
								c.constrainedLength = length;
							}
							else
							{
								c.constrainedLength = c.maxLength;
							}
						}
					}
				}

				if (rowSpan == 1 && r.effectiveUnitType == LayoutGridUnitType.Auto)
				{
					length = n.child.DesiredSize.Height - r.spanExtraLength_Or_Position;

					if (length > r.constrainedLength)
					{
						if (length <= r.maxLength)
						{
							r.constrainedLength = length;
						}
						else
						{
							r.constrainedLength = r.maxLength;
						}
					}
				}
				else if (r.effectiveUnitType == LayoutGridUnitType.Star)
				{
					length = n.child.DesiredSize.Height;
					if (rowSpan > 1)
					{
						double spanLength = GetSpanLengthWithDesiredStar(rowInfoArray, row + 1, rowSpan - 1); // this is the span length excluding this row
						length -= spanLength;
					}

					// for effective star, use spanExtraLengthOrPosition to store the desired size value - should this start out as the min value???
					if (length > r.spanExtraLength_Or_Position)
					{
						if (length <= r.maxLength)
						{
							r.spanExtraLength_Or_Position = length;
						}
						else
						{
							r.spanExtraLength_Or_Position = r.maxLength;
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
				else if (rowSpan > 1 && r.effectiveUnitType == LayoutGridUnitType.Auto) // this is an auto and star span
				{
					length = n.child.DesiredSize.Height;

					if (rowSpan > 1)
					{
						double spanLength = GetSpanLengthWithDesiredStar(rowInfoArray, row + 1, rowSpan - 1); // this is the span length excluding this row
						length -= spanLength;
					}

					if (r.unitType == LayoutGridUnitType.Auto)
					{
						if (length > r.stars_Or_AutoDesiredLength)
						{
							double desired;
							if (length <= r.maxLength)
							{
								desired = length;
							}
							else
							{
								desired = r.maxLength;
							}

							if (desired > r.constrainedLength && desired > r.stars_Or_AutoDesiredLength)
							{
								if (r.stars_Or_AutoDesiredLength > r.constrainedLength)
								{
									totalAutoRowHeightDesired += (desired - r.constrainedLength) - (r.stars_Or_AutoDesiredLength - r.constrainedLength);
								}
								else
								{
									totalAutoRowHeightDesired += (desired - r.constrainedLength);
								}
							}
							r.stars_Or_AutoDesiredLength = desired;
						}
					}
					else // must be star that is effectively auto
					{
						// not sure this works below because I think constrained gets added to desired at the wrong time??? - also, shouldn't this be span extra
						if (length > r.constrainedLength)
						{
							if (length <= r.maxLength)
							{
								r.constrainedLength = length;
							}
							else
							{
								r.constrainedLength = r.maxLength;
							}
						}
					}
				}
			}

#if CollectPerformanceStats
			measureTicks = Stopwatch.GetTimestamp() - startTicks;
			startTicks = Stopwatch.GetTimestamp();
#endif

			if (starColCount > 0)
			{
				for (int j = 0; j < colInfoArrayCount; j++)
				{
					ref GridColRowInfo cr = ref colInfoArray[j];

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						if (cr.effectiveUnitType == LayoutGridUnitType.Star)
						{
							double starLength;
							if (cr.spanExtraLength_Or_Position < cr.minLength)
							{
								starLength = cr.minLength;
							}
							else if (cr.spanExtraLength_Or_Position > cr.maxLength)
							{
								starLength = cr.maxLength;
							}
							else
							{
								starLength = cr.spanExtraLength_Or_Position;
							}

							if (starLength > cr.constrainedLength) // it seems like in some cases Grid does this check and in some cases it doesn't
							{
								starLength = cr.constrainedLength;
							}
							totalStarColWidthDesired += starLength;
						}
						else // must be star treated as auto
						{
							totalStarColWidthDesired += cr.constrainedLength;
						}
					}
				}
			}

			desiredWidth = totalPixelColWidth + totalAutoColWidth + totalAutoColWidthDesired + totalStarColWidthDesired;

			if (starRowCount > 0)
			{
				for (int j = 0; j < rowInfoArrayCount; j++)
				{
					ref GridColRowInfo cr = ref rowInfoArray[j];

					if (cr.unitType == LayoutGridUnitType.Star)
					{
						if (cr.effectiveUnitType == LayoutGridUnitType.Star)
						{
							double starLength;
							if (cr.spanExtraLength_Or_Position < cr.minLength)
							{
								starLength = cr.minLength;
							}
							else if (cr.spanExtraLength_Or_Position > cr.maxLength)
							{
								starLength = cr.maxLength;
							}
							else
							{
								starLength = cr.spanExtraLength_Or_Position;
							}

							if (starLength > cr.constrainedLength) // it seems like in some cases Grid does this check and in some cases it doesn't
							{
								starLength = cr.constrainedLength;
							}
							totalStarRowHeightDesired += starLength;
						}
						else // must be star treated as auto
						{
							totalStarRowHeightDesired += cr.constrainedLength;
						}
					}
				}
			}

			desiredHeight = totalPixelRowHeight + totalAutoRowHeight + totalStarRowHeightDesired;

			Size retSize = new Size(desiredWidth, desiredHeight);

#if CollectPerformanceStats
			postMeasureTicks = Stopwatch.GetTimestamp() - startTicks;
#endif

#if DEBUG
			measureSizeReturns.Add(retSize);
#endif
			return retSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
#if DEBUG
			arrangeSizeParams.Add(finalSize);
#endif

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
				Rect arrangeRect = new Rect(0, 0, actualColWidth, actualRowHeight); //??? should be able to just use this.DesiredSize.Width, ...
				//Rect arrangeRect = new Rect(0, 0, finalSize.Width, finalSize.Height); //??? should be able to just use this.DesiredSize.Width, ...
																				//Rect arrangeRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
				for (int i = 0; i < childrenCount; i++)
				{
					UIElement child = InternalChildren[i];

					if (child != null)
					{
#if CollectPerformanceStats
						startTicksChildArrange = Stopwatch.GetTimestamp();
#endif

						child.Arrange(arrangeRect);//??? don't use finalSize, use desiredSize? - Grid uses finalSize (or does it?), so why don't we?

#if CollectPerformanceStats
						childArrangeTicks += Stopwatch.GetTimestamp() - startTicksChildArrange;
#endif
					}
				}

#if CollectPerformanceStats
				shortArrangeTicks = Stopwatch.GetTimestamp() - startTicks;
				shortArrangeCount++;
#endif

#if DEBUG
				arrangeSizeReturns.Add(finalSize);
#endif
				
				return finalSize;
			}

			//??? I'm not sure these DistributeStarColWidth/DistributeStarRowHeight calls would ever get called in this function
			// they do get called if star mins take the grid past the avail length in MeasureOverride
			// but I don't think they need to be called because the min will get used anyway in MeasureOverride
			if (starColCount > 0 && finalSize.Width != DesiredSize.Width && wasInfiniteWidth)
			{
				StarMinMax[] starMinArray = starColMinArray;
				int starMinArrayCount = starColMinArrayCount;

				double starColWidth = finalSize.Width - (totalPixelColWidth + totalAutoColWidth);

				// for star, set all constrainedLength to 0 because setting to > 0 means it is constrained by min
				int constrainedCount = 0;
				for (int i = 0; i < starColCount; i++)
				{
					ref StarMinMax t = ref starColMaxArray[i];

					ref GridColRowInfo cr = ref colInfoArray[t.index];

					cr.spanExtraLength_Or_Position = 0; // do we need to set to 0???
					
					if (cr.constrainedLength != 0 && starColCount > 1)
					{
						if (constrainedCount == 0)
						{
							// treat this as a min
							if (starColMinArrayForInfinite == null || starColCount > starColMinArrayForInfinite.Length)
							{
								starColMinArrayForInfinite = new StarMinMax[starColCount];
							}
							starColMinArrayForInfiniteCount = starColCount;

							starMinArray = starColMinArrayForInfinite;
							starMinArrayCount = starColMinArrayForInfiniteCount;

							for (int j = 0; j < starColCount; j++)
							{
								// copy all of the mins
								ref StarMinMax t2 = ref starColMaxArray[j];

								ref GridColRowInfo cr2 = ref colInfoArray[t2.index];

								starMinArray[j].index = t2.index;
								starMinArray[j].minOrMaxPerStar = cr2.minLength / cr2.stars_Or_AutoDesiredLength;
							}
						}
						constrainedCount++;

						double minOrMaxPerStar = cr.constrainedLength / cr.stars_Or_AutoDesiredLength;
						if (minOrMaxPerStar > starMinArray[i].minOrMaxPerStar)
						{
							starMinArray[i].minOrMaxPerStar = minOrMaxPerStar;
							cr.spanExtraLength_Or_Position = cr.minLength; // use this as a temp place to save the actual min
							cr.minLength = cr.constrainedLength;
						}
					}
					cr.constrainedLength = 0;
				}

				if (constrainedCount > 0)
				{
					Array.Sort(starMinArray, 0, starMinArrayCount, compareStarMinMaxByPerStarDesc);
				}

				DistributeStarColWidth(starColWidth, starMinArray, starMinArrayCount);

				if (constrainedCount > 0)
				{
					for (int i = 0; i < starColCount; i++)
					{
						ref StarMinMax t = ref starColMaxArray[i];

						ref GridColRowInfo cr = ref colInfoArray[t.index];

						if (cr.spanExtraLength_Or_Position > 0)
						{
							cr.minLength = cr.spanExtraLength_Or_Position; // restore min to what it was
						}
					}
				}
			}

			if (starRowCount > 0 && finalSize.Height != DesiredSize.Height && wasInfiniteHeight)
			{
				StarMinMax[] starMinArray = starRowMinArray;
				int starMinArrayCount = starRowMinArrayCount;

				double starRowHeight = finalSize.Height - (totalPixelRowHeight + totalAutoRowHeight);

				// for star, set all constrainedLength to 0 because setting to > 0 means it is constrained by min
				int constrainedCount = 0;
				for (int i = 0; i < starRowCount; i++)
				{
					ref StarMinMax t = ref starRowMaxArray[i];

					ref GridColRowInfo cr = ref rowInfoArray[t.index];

					cr.spanExtraLength_Or_Position = 0; // do we need to set to 0???

					if (cr.constrainedLength != 0 && starRowCount > 1)
					{
						if (constrainedCount == 0)
						{
							// treat this as a min
							if (starColMinArrayForInfinite == null || starRowCount > starRowMinArrayForInfinite.Length)
							{
								starRowMinArrayForInfinite = new StarMinMax[starRowCount];
							}
							starRowMinArrayForInfiniteCount = starRowCount;

							starMinArray = starRowMinArrayForInfinite;
							starMinArrayCount = starRowMinArrayForInfiniteCount;

							for (int j = 0; j < starRowCount; j++)
							{
								// copy all of the mins
								ref StarMinMax t2 = ref starRowMaxArray[j];

								ref GridColRowInfo cr2 = ref rowInfoArray[t2.index];

								starMinArray[j].index = t2.index;
								starMinArray[j].minOrMaxPerStar = cr2.minLength / cr2.stars_Or_AutoDesiredLength;
							}
						}
						constrainedCount++;

						double minOrMaxPerStar = cr.constrainedLength / cr.stars_Or_AutoDesiredLength;
						if (minOrMaxPerStar > starMinArray[i].minOrMaxPerStar)
						{
							starMinArray[i].minOrMaxPerStar = minOrMaxPerStar;
							cr.spanExtraLength_Or_Position = cr.minLength; // use this as a temp place to save the actual min
							cr.minLength = cr.constrainedLength;
						}
					}
					cr.constrainedLength = 0;
				}

				if (constrainedCount > 0)
				{
					Array.Sort(starMinArray, 0, starMinArrayCount, compareStarMinMaxByPerStarDesc);
				}

				DistributeStarRowHeight(starRowHeight, starMinArray, starMinArrayCount);

				if (constrainedCount > 0)
				{
					for (int i = 0; i < starRowCount; i++)
					{
						ref StarMinMax t = ref starRowMaxArray[i];

						ref GridColRowInfo cr = ref rowInfoArray[t.index];

						if (cr.spanExtraLength_Or_Position > 0)
						{
							cr.minLength = cr.spanExtraLength_Or_Position; // restore min to what it was
						}
					}
				}
			}

			double position;
			position = 0;
			for (int i = 0; i < colInfoArrayCount; i++)
			{
				ref GridColRowInfo cr = ref colInfoArray[i];

				// set the positions

				cr.spanExtraLength_Or_Position = position;

				position += cr.constrainedLength;
			}

			position = 0;
			for (int i = 0; i < rowInfoArrayCount; i++)
			{
				ref GridColRowInfo cr = ref rowInfoArray[i];

				// set the positions

				cr.spanExtraLength_Or_Position = position;

				position += cr.constrainedLength;
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

				width = c.constrainedLength;
				if (n.colSpan > 1)
				{
					int maxColPlus1 = n.col + n.colSpan;
					int j = n.col + 1;
					do
					{
						width += colInfoArray[j].constrainedLength;
					} while (++j < maxColPlus1);
				}

				height = r.constrainedLength;
				if (n.rowSpan > 1)
				{
					int maxRowPlus1 = n.row + n.rowSpan;
					int j = n.row + 1;
					do
					{
						height += rowInfoArray[j].constrainedLength;
					} while (++j < maxRowPlus1);
				}

#if CollectPerformanceStats
				startTicksChildArrange = Stopwatch.GetTimestamp();
#endif

				n.child.Arrange(new Rect(c.spanExtraLength_Or_Position, r.spanExtraLength_Or_Position, width, height));

#if CollectPerformanceStats
				childArrangeTicks += Stopwatch.GetTimestamp() - startTicksChildArrange;
#endif
			}

#if CollectPerformanceStats
			arrangeTicks = Stopwatch.GetTimestamp() - startTicks;
#endif

			if (gridLinesVisual != null)
			{
				gridLinesVisual.DrawGridLines(this);
			}

#if DEBUG
			arrangeSizeReturns.Add(finalSize);
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

				length = colInfoArray[col].constrainedLength;
			}
			else
			{
				//??? not sure this is correct, maybe for now need to set this in a member variable until it is set in the ColumnDefinitions itself
				length = actualColWidth;
				//length = ActualWidth;
				//if (ColumnDefinitions.Count > 0)
				//{
				//	if (length < ColumnDefinitions[0].MinWidth)
				//	{
				//		length = ColumnDefinitions[0].MinWidth;
				//	}
				//	else if (length > ColumnDefinitions[0].MaxWidth)
				//	{
				//		length = ColumnDefinitions[0].MaxWidth;
				//	}
				//}
			}

			return length;
		}

		//??? remove this function, get this from row def ActualHeight
		public double GetRowHeight(int row)
		{
			double length = 0;

			if (rowInfoArray != null && rowInfoArrayCount > 0)
			{
				if (row > rowInfoArrayCount - 1)
				{
					row = rowInfoArrayCount - 1;
				}

				length = rowInfoArray[row].constrainedLength;
			}
			else
			{
				//??? not sure this is correct, maybe for now need to set this in a member variable until it is set in the RowDefinitions itself
				length = actualRowHeight;
				//length = ActualHeight;
				//if (RowDefinitions.Count > 0)
				//{
				//	if (length < RowDefinitions[0].MinHeight)
				//	{
				//		length = RowDefinitions[0].MinHeight;
				//	}
				//	else if (length > RowDefinitions[0].MaxHeight)
				//	{
				//		length = RowDefinitions[0].MaxHeight;
				//	}
				//}
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
