////////////////////////////////////////////////////////////////////////////////
//
//  SvgTSpanElement.cs - This file is part of Svg2Xaml.
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
  using System.Globalization;
  using System.Windows;
  using System.Xml;

  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;tspan&gt; element.
  /// </summary>
  class SvgTSpanElement
    : SvgDrawableContainerBaseElement
  {

    public readonly string Text;

    public readonly SvgCoordinate X;

    public readonly SvgCoordinate Y;

    //==========================================================================
    public SvgTSpanElement(SvgDocument document, SvgBaseElement parent, XElement svgElement)
      : base(document, parent, svgElement)
    {
      if (svgElement.FirstNode != null && svgElement.FirstNode.NodeType == XmlNodeType.Text)
      {
        this.Text = svgElement.FirstNode.ToString();
      }

      var xAttr = svgElement.Attribute("x");
      this.X = xAttr != null ? SvgCoordinate.Parse(xAttr.Value) : null;

      var yAttr = svgElement.Attribute("y");
      this.Y = yAttr != null ? SvgCoordinate.Parse(yAttr.Value) : null;
    }

    public override Drawing Draw()
    {
      var dg = base.Draw() as DrawingGroup ?? new DrawingGroup();
      var txt = this.Parent as SvgTextElement;
      if (txt != null)
      {
        using (var dc = dg.Open())
        {
          var ft = new FormattedText(
            this.Text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            txt.Typeface,
            txt.FontSize,
            txt.Fill.ToBrush(txt));
          var x = this.X != null ? this.X.Value : 0.0;
          var y = this.Y != null ? this.Y.Value : 0.0;
          var pt = new Point(x, y - ft.Baseline);
          // Bei SVG scheint der Punkt die Basislinie des Textes zu meinen und
          // bei WPF die obere linke Ecke. Daher dieser Hack.
          dc.DrawText(ft, pt);
        }
      }

      return dg;
    }
  } // class SvgTSpanElement

}
