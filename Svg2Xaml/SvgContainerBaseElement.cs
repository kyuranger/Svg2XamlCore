﻿////////////////////////////////////////////////////////////////////////////////
//
//  SvgContainerBaseElement.cs - This file is part of Svg2Xaml.
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
using System.Linq;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System.Diagnostics;

  //****************************************************************************
  /// <summary>
  ///   Base element for all container elements.
  /// </summary>
  class SvgContainerBaseElement
    : SvgBaseElement
  {
    //==========================================================================
    public readonly List<SvgBaseElement> Children = new List<SvgBaseElement>();

    //==========================================================================
    public SvgContainerBaseElement(SvgDocument document, SvgBaseElement parent, XElement containerElement)
      : base(document, parent, containerElement)
    {
      foreach(XElement element in from element in containerElement.Elements()
                                  where element.Name.NamespaceName == "http://www.w3.org/2000/svg"
                                  select element)
      switch(element.Name.LocalName)
      {
        case "svg":
          Children.Add(new SvgSVGElement(document, this, element));
          break;

        case "g":
          Children.Add(new SvgGElement(document, this, element));
          break;

        case "defs":
          Children.Add(new SvgDefsElement(document, this, element));
          break;

        case "symbol":
          Children.Add(new SvgSymbolElement(document, this, element));
          break;

        case "clipPath":
          Children.Add(new SvgClipPathElement(document, this, element));
          break;

        case "mask":
          Children.Add(new SvgMaskElement(document, this, element));
          break;

        case "pattern":
          Children.Add(new SvgPatternElement(document, this, element));
          break;

        case "marker":
          Children.Add(new SvgMarkerElement(document, this, element));
          break;

        case "a":
          Children.Add(new SvgAElement(document, this, element));
          break;

        case "switch":
          Children.Add(new SvgSwitchElement(document, this, element));
          break;

        case "path":
          Children.Add(new SvgPathElement(document, this, element));
          break;

        case "text":
          Children.Add(new SvgTextElement(document, this, element));
          break;

        case "rect":
          Children.Add(new SvgRectElement(document, this, element));
          break;

        case "circle":
          Children.Add(new SvgCircleElement(document, this, element));
          break;

        case "ellipse":
          Children.Add(new SvgEllipseElement(document, this, element));
          break;

        case "line":
          Children.Add(new SvgLineElement(document, this, element));
          break;

        case "polyline":
          Children.Add(new SvgPolylineElement(document, this, element));
          break;

        case "polygon":
          Children.Add(new SvgPolygonElement(document, this, element));
          break;

        case "image":
          Children.Add(new SvgImageElement(document, this, element));
          break;

        case "use":
          Children.Add(new SvgUseElement(document, this, element));
          break;

        case "linearGradient":
          Children.Add(new SvgLinearGradientElement(document, this, element));
          break;

        case "radialGradient":
          Children.Add(new SvgRadialGradientElement(document, this, element));
          break;

        case "filter":
          Children.Add(new SvgFilterElement(document, this, element));
          break;

        case "metadata":
          Children.Add(new SvgMetadataElement(document, this, element));
          break;

        case "flowRoot":
          Children.Add(new SvgFlowRootElement(document, this, element));
          break;

        case "flowRegion":
          Children.Add(new SvgFlowRegionElement(document, this, element));
          break;

        case "flowPara":
          Children.Add(new SvgFlowParaElement(document, this, element));
          break;

        case "flowSpan":
          Children.Add(new SvgFlowSpanElement(document, this, element));
          break;

        case "tspan":
          Children.Add(new SvgTSpanElement(document, this, element));
          break;

        case "foreignObject":
          Children.Add(new SvgForeignObjectElement(document, this, element));
          break;

        case "style":
          Children.Add(new SvgStyleElement(document, this, element));
          break;

        default:
          //throw new NotImplementedException(String.Format("Unhandled svg element: {0}", element));
          Debug.WriteLine("Unhandled svg element: {0}", element);
          break;
      }
    }


  } // class SvgContainerBaseElement

}
