////////////////////////////////////////////////////////////////////////////////
//
//  SvgTextElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Globalization;
  using System.Windows;
  using System.Xml;

  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;text&gt; element.
  /// </summary>
  class SvgTextElement
    : SvgDrawableContainerBaseElement
  {
    public readonly double FontSize;

    public readonly Typeface Typeface;

    public readonly string Text;

    public readonly SvgCoordinate X;

    public readonly SvgCoordinate Y;

    public readonly string TextAnchor;

    //==========================================================================
    public SvgTextElement(SvgDocument document, SvgBaseElement parent, XElement svgElement)
      : base(document, parent, svgElement)
    {
      var fontFamilyAttr = svgElement.Attribute("font-family");
      var fontFamilyStr = fontFamilyAttr != null ? fontFamilyAttr.Value : "sans-serif";

      var fontSizeAttr = svgElement.Attribute("font-size");
      var fontSizeStr = fontSizeAttr != null ? fontSizeAttr.Value : "12px";

      var fontStretchAttr = svgElement.Attribute("font-stretch");
      var fontStretchStr = fontStretchAttr != null ? fontStretchAttr.Value : "normal";

      var fontStyleAttr = svgElement.Attribute("font-style");
      var fontStyleStr = fontStyleAttr != null ? fontStyleAttr.Value : "normal";

      var fontWeightAttr = svgElement.Attribute("font-weight");
      var fontWeightStr = fontWeightAttr != null ? fontWeightAttr.Value : "normal";

      var letterSpacingAttr = svgElement.Attribute("letter-spacing");
      var letterSpacingStr = letterSpacingAttr != null ? letterSpacingAttr.Value : "0px";

      var lineHeightAttr = svgElement.Attribute("line-height");
      var lineHeightStr = lineHeightAttr != null ? lineHeightAttr.Value : "100%";

      var wordSpacingAttr = svgElement.Attribute("word-spacing");
      var wordSpacingStr = wordSpacingAttr != null ? wordSpacingAttr.Value : "0px";
      
      var textAnchorAttr = svgElement.Attribute("text-anchor");
      TextAnchor = textAnchorAttr != null ? textAnchorAttr.Value : "start";

      var xAttr = svgElement.Attribute("x");
      this.X = xAttr != null ? SvgCoordinate.Parse(xAttr.Value) : null;

      var yAttr = svgElement.Attribute("y");
      this.Y = yAttr != null ? SvgCoordinate.Parse(yAttr.Value) : null;

      var ff = new FontFamily(fontFamilyStr);
      var fs = fontStyleStr.TryConvert(FontStyles.Normal);
      var fw = fontWeightStr.TryConvert(FontWeights.Normal);
      var fc = fontStretchStr.TryConvert(FontStretches.Medium);

      this.FontSize = fontSizeStr.TryConvert<double, LengthConverter>(12.0);
      this.Typeface = new Typeface(ff, fs, fw, fc);

      var firstSubNode = svgElement.FirstNode;
      if (firstSubNode != null)
      {
        if (firstSubNode.NodeType == XmlNodeType.Text)
        {
          this.Text = ((XText)firstSubNode).Value;
        }
        else
        {
          this.Text = null;
        }
      }
    }

    public override Drawing Draw()
    {
      var dg = base.Draw() as DrawingGroup ?? new DrawingGroup();
      if (this.Text != null)
      {
        using (var dc = dg.Open())
        {
          var ft = new FormattedText(
            this.Text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            this.Typeface,
            this.FontSize,
            this.Fill.ToBrush(this));
          var x = this.X != null ? this.X.Value : 0.0;
          var y = this.Y != null ? this.Y.Value : 0.0;
          Point pt;
          switch(TextAnchor)
          { 
            case "middle":
              pt = new Point(x - ft.WidthIncludingTrailingWhitespace / 2, y - ft.Baseline);
              break;
            case "end":
              pt = new Point(x - ft.WidthIncludingTrailingWhitespace, y - ft.Baseline);
              break;
            case "start":
            case "inherit":
            default:
              pt = new Point(x, y - ft.Baseline);
              break;
        }
          // Bei SVG scheint der Punkt die Basislinie des Textes zu meinen und
          // bei WPF die obere linke Ecke. Daher dieser Hack.
          dc.DrawText(ft, pt);
        }
      }

      return dg;
    }
  } // class SvgTextElement

}
