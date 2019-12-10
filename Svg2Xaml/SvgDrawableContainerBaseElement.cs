////////////////////////////////////////////////////////////////////////////////
//
//  SvgDrawableContainerBaseElement.cs - This file is part of Svg2Xaml.
//
//    Copyright (C) 2009,2011 Boris Richter <himself@boris-richter.net>
//
//  --------------------------------------------------------------------------
//
//  Svg2Xaml is free software: you can redistribute it and/or modify it under 
//  the terms of the GNU Lesser General Public License as published by the 
//  Free Software Foundation, either version 3 of the License, or (at your 
//  option) any later version.
//
//  Svg2Xaml is distributed in the hope that it will be useful, but WITHOUT 
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//  FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public 
//  License for more details.
//  
//  You should have received a copy of the GNU Lesser General Public License 
//  along with Svg2Xaml. If not, see <http://www.gnu.org/licenses/>.
//
//  --------------------------------------------------------------------------
//
//  $LastChangedRevision$
//  $LastChangedDate$
//  $LastChangedBy$
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace Svg2Xaml
{
  
  //****************************************************************************
  class SvgDrawableContainerBaseElement
    : SvgContainerBaseElement
  {

    //==========================================================================
    public readonly SvgViewbox ViewBox = new SvgViewbox(new Rect(0, 0, 0, 0));
    public readonly SvgLength Width = null;
    public readonly SvgLength Height = null;
    public readonly SvgLength Opacity = new SvgLength(1.0);
    public readonly SvgLength FillOpacity = new SvgLength(1.0);
    public readonly SvgLength StrokeOpacity = new SvgLength(1.0);
    public readonly SvgTransform Transform = null;
    public readonly SvgPaint Fill = new SvgColorPaint(new SvgColor(0, 0, 0));
    public readonly SvgPaint Stroke = null; /* new SvgColorPaint(new SvgColor(0, 0, 0)); */
    public readonly SvgLength StrokeWidth = new SvgLength(1);
    public readonly SvgStrokeLinecap StrokeLinecap = SvgStrokeLinecap.Butt;
    public readonly SvgStrokeLinejoin StrokeLinejoin = SvgStrokeLinejoin.Miter;
    public readonly double StrokeMiterlimit = 4;     // Double.None = inherit
    public readonly SvgLength StrokeDashoffset = new SvgLength(0);
    public readonly SvgLength[] StrokeDasharray = null; // null = none, Length[0] = inherit
    public readonly SvgURL ClipPath = null;
    public readonly SvgURL Filter = null;
    public readonly SvgURL Mask = null;
    public readonly SvgDisplay Display = SvgDisplay.Inline;
    public readonly SvgFillRule FillRule = SvgFillRule.Nonzero;

    //==========================================================================
    public SvgDrawableContainerBaseElement(SvgDocument document, SvgBaseElement parent, XElement drawableContainerElement)
      : base(document, parent, drawableContainerElement)
    {
      XAttribute viewBox_attribute = drawableContainerElement.Attribute("viewBox");
      if (viewBox_attribute != null)
          this.ViewBox = SvgViewbox.Parse(viewBox_attribute.Value);

      XAttribute opacity_attribute = drawableContainerElement.Attribute("opacity");
      if(opacity_attribute != null)
        Opacity = SvgLength.Parse(opacity_attribute.Value);

      XAttribute transform_attribute = drawableContainerElement.Attribute("transform");
      if(transform_attribute != null)
        Transform = SvgTransform.Parse(transform_attribute.Value);

      XAttribute clip_attribute = drawableContainerElement.Attribute("clip-path");
      if(clip_attribute != null)
        ClipPath = SvgURL.Parse(clip_attribute.Value);

      XAttribute filter_attribute = drawableContainerElement.Attribute("filter");
      if(filter_attribute != null)
        Filter = SvgURL.Parse(filter_attribute.Value);

      XAttribute mask_attribute = drawableContainerElement.Attribute("mask");
      if(mask_attribute != null)
        Mask = SvgURL.Parse(mask_attribute.Value);

      XAttribute display_attribute = drawableContainerElement.Attribute("display");
      if(display_attribute != null)
        switch(display_attribute.Value)
        {
          case "inline":
            Display = SvgDisplay.Inline;
            break;

          case "block":
            Display = SvgDisplay.Block;
            break;

          case "list-item":
            Display = SvgDisplay.ListItem;
            break;

          case "run-in":
            Display = SvgDisplay.RunIn;
            break;

          case "compact":
            Display = SvgDisplay.Compact;
            break;

          case "marker":
            Display = SvgDisplay.Marker;
            break;

          case "table":
            Display = SvgDisplay.Table;
            break;

          case "inline-table":
            Display = SvgDisplay.InlineTable;
            break;

          case "table-row-group":
            Display = SvgDisplay.TableRowGroup;
            break;

          case "table-header-group":
            Display = SvgDisplay.TableHeaderGroup;
            break;

          case "table-footer-group":
            Display = SvgDisplay.TableFooterGroup;
            break;

          case "table-row":
            Display = SvgDisplay.TableRow;
            break;

          case "table-column-group":
            Display = SvgDisplay.TableColumnGroup;
            break;

          case "table-column":
            Display = SvgDisplay.TableColumn;
            break;

          case "table-cell":
            Display = SvgDisplay.TableCell;
            break;

          case "table-caption":
            Display = SvgDisplay.TableCaption;
            break;

          case "none":
            Display = SvgDisplay.None;
            break;

          default:
            throw new NotImplementedException();
        }

      XAttribute fill_opacity_attribute = drawableContainerElement.Attribute("fill-opacity");
      if (fill_opacity_attribute != null)
        FillOpacity = SvgLength.Parse(fill_opacity_attribute.Value);

      XAttribute stroke_opacity_attribute = drawableContainerElement.Attribute("stroke-opacity");
      if (stroke_opacity_attribute != null)
        StrokeOpacity = SvgLength.Parse(stroke_opacity_attribute.Value);

      XAttribute fill_attribute = drawableContainerElement.Attribute("fill");
      if (fill_attribute != null)
        Fill = SvgPaint.Parse(fill_attribute.Value);

      XAttribute stroke_attribute = drawableContainerElement.Attribute("stroke");
      if (stroke_attribute != null)
        Stroke = SvgPaint.Parse(stroke_attribute.Value);

      XAttribute stroke_width_attribute = drawableContainerElement.Attribute("stroke-width");
      if (stroke_width_attribute != null)
        StrokeWidth = SvgLength.Parse(stroke_width_attribute.Value);

      XAttribute stroke_linecap_attribute = drawableContainerElement.Attribute("stroke-linecap");
      if (stroke_linecap_attribute != null)
        switch (stroke_linecap_attribute.Value)
        {
          case "butt":
            StrokeLinecap = SvgStrokeLinecap.Butt;
            break;

          case "round":
            StrokeLinecap = SvgStrokeLinecap.Round;
            break;

          case "square":
            StrokeLinecap = SvgStrokeLinecap.Square;
            break;

          case "inherit":
            StrokeLinecap = SvgStrokeLinecap.Inherit;
            break;

          default:
            throw new NotImplementedException();
        }

      XAttribute stroke_linejoin_attribute = drawableContainerElement.Attribute("stroke-linejoin");
      if (stroke_linejoin_attribute != null)
        switch (stroke_linejoin_attribute.Value)
        {
          case "miter":
            StrokeLinejoin = SvgStrokeLinejoin.Miter;
            break;

          case "round":
            StrokeLinejoin = SvgStrokeLinejoin.Round;
            break;

          case "bevel":
            StrokeLinejoin = SvgStrokeLinejoin.Bevel;
            break;

          case "inherit":
            StrokeLinejoin = SvgStrokeLinejoin.Inherit;
            break;

          default:
            throw new NotSupportedException();
        }

      XAttribute stroke_miterlimit_attribute = drawableContainerElement.Attribute("stroke-miterlimit");
      if (stroke_miterlimit_attribute != null)
      {
        if (stroke_miterlimit_attribute.Value == "inherit")
          StrokeMiterlimit = Double.NaN;
        else
        {
          double miterlimit = Double.Parse(stroke_miterlimit_attribute.Value, CultureInfo.InvariantCulture.NumberFormat);
          //if(miterlimit < 1)
          //throw new NotSupportedException("A miterlimit less than 1 is not supported.");
          StrokeMiterlimit = miterlimit;
        }
      }

      XAttribute stroke_dasharray_attribute = drawableContainerElement.Attribute("stroke-dasharray");
      if (stroke_dasharray_attribute != null)
      {
        if (stroke_dasharray_attribute.Value == "none")
          StrokeDasharray = null;
        else if (stroke_dasharray_attribute.Value == "inherit")
          StrokeDasharray = new SvgLength[0];
        else
        {
          List<SvgLength> lengths = new List<SvgLength>();
          var lengthTokens = stroke_dasharray_attribute.Value.Replace(";", "")
            .Trim()
            .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
          foreach (string length in lengthTokens)
          {
            lengths.Add(SvgLength.Parse(length));
          }

          if (lengths.Count % 2 == 1)
          {
            StrokeDasharray = new SvgLength[lengths.Count * 2];
            for (int i = 0; i < lengths.Count - 1; ++i)
            {
              StrokeDasharray[i] = lengths[i];
              StrokeDasharray[i + lengths.Count] = lengths[i];
            }
          }
          else
            StrokeDasharray = lengths.ToArray();

        }
      }

      XAttribute stroke_dashoffset_attribute = drawableContainerElement.Attribute("stroke-dashoffset");
      if (stroke_dashoffset_attribute != null)
        StrokeDashoffset = SvgLength.Parse(stroke_dashoffset_attribute.Value);

      XAttribute fill_rule_attribute = drawableContainerElement.Attribute("fill-rule");
      if (fill_rule_attribute != null)
        switch (fill_rule_attribute.Value)
        {
          case "nonzero":
            FillRule = SvgFillRule.Nonzero;
            break;

          case "evenodd":
            FillRule = SvgFillRule.Evenodd;
            break;

          case "inherit":
            FillRule = SvgFillRule.Inherit;
            break;

          default:
            throw new NotImplementedException();
        }

            // color, color-interpolation, color-rendering
            
            XAttribute width_attribute = drawableContainerElement.Attribute("width");
            if (width_attribute != null)
                Width = SvgLength.Parse(width_attribute.Value);
            XAttribute height_attribute = drawableContainerElement.Attribute("height");
            if (height_attribute != null)
                Height = SvgLength.Parse(height_attribute.Value);

            XAttribute preserveAspectRatio_attribute = drawableContainerElement.Attribute("preserveAspectRatio");
            if (preserveAspectRatio_attribute != null)
            {
                switch (preserveAspectRatio_attribute.Value)
                {
                    case "none":
                        if (Width != null && Height != null)
                        {
                            var scaleTransform = new SvgScaleTransform(
                                Width.ToDouble() / ViewBox.Value.Width,
                                Height.ToDouble() / ViewBox.Value.Height);
                            Width.ToDouble();
                            if (Transform == null)
                                Transform = scaleTransform;
                            else
                                Transform = new SvgTransformGroup(new[] { Transform, scaleTransform });
                        }
                        break;
                }
            }
      // overflow

    }

    //==========================================================================
    public virtual Geometry GetGeometry()
    {
      GeometryGroup geometry_group = new GeometryGroup();

      foreach(SvgBaseElement element in Children)
      {
        if(element is SvgDrawableBaseElement)
          geometry_group.Children.Add((element as SvgDrawableBaseElement).GetGeometry());
        else if(element is SvgDrawableContainerBaseElement)
          geometry_group.Children.Add((element as SvgDrawableContainerBaseElement).GetGeometry());
      }

      if(Transform != null)
        geometry_group.Transform = Transform.ToTransform();

      if(ClipPath != null)
      {
        SvgClipPathElement clip_path_element = Document.Elements[ClipPath.Id] as SvgClipPathElement;
        if(clip_path_element != null)
          return Geometry.Combine(geometry_group, clip_path_element.GetClipGeometry(), GeometryCombineMode.Exclude, null);
      }

      return geometry_group;
    }

    //==========================================================================
    public virtual Drawing Draw()
    {
      DrawingGroup drawing_group = new DrawingGroup();

      drawing_group.Opacity   = Opacity.ToDouble();
      if(Transform != null)
        drawing_group.Transform = Transform.ToTransform();

      if (ViewBox != null)
          drawing_group.Children.Add(ViewBox.Process());

      foreach(SvgBaseElement child_element in Children)
      {
        SvgBaseElement element = child_element;
        if(element is SvgUseElement)
          element = (element as SvgUseElement).GetElement();

        Drawing drawing = null;

        if(element is SvgDrawableBaseElement)
        {
          if((element as SvgDrawableBaseElement).Display != SvgDisplay.None)
            drawing = (element as SvgDrawableBaseElement).Draw();
        }
        else if(element is SvgDrawableContainerBaseElement)
        {
          if((element as SvgDrawableContainerBaseElement).Display != SvgDisplay.None)
            drawing = (element as SvgDrawableContainerBaseElement).Draw();
        }

        if(drawing != null)
          drawing_group.Children.Add(drawing);
      }

      if(Filter != null)
      {
        SvgFilterElement filter_element = Document.Elements[Filter.Id] as SvgFilterElement;
        if(filter_element != null)
          drawing_group.BitmapEffect = filter_element.ToBitmapEffect();
      }

      if(ClipPath != null)
      {
        SvgClipPathElement clip_path_element = Document.Elements[ClipPath.Id] as SvgClipPathElement;
        if(clip_path_element != null)
          drawing_group.ClipGeometry= clip_path_element.GetClipGeometry();
      }

      if(Mask != null)
      {
        SvgMaskElement mask_element = Document.Elements[Mask.Id] as SvgMaskElement;
        if(mask_element != null)
        {
          DrawingBrush opacity_mask = mask_element.GetOpacityMask();
          /*
          if(Transform != null)
            opacity_mask.Transform = Transform.ToTransform();
          */
          drawing_group.OpacityMask = opacity_mask;
        }
      }
        
      return drawing_group;
    }

  } // class SvgDrawableContainerBaseElement

}
