////////////////////////////////////////////////////////////////////////////////
//
//  SvgDocument.cs - This file is part of Svg2Xaml.
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
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System.Windows.Markup;

  //****************************************************************************
  sealed class SvgDocument
  {
    //==========================================================================
    public readonly Dictionary<string, SvgBaseElement> Elements = new Dictionary<string,SvgBaseElement>();

    //==========================================================================
    public readonly SvgSVGElement    Root;
    public readonly SvgReaderOptions Options;
    public readonly IDictionary<string, IDictionary<string, string>> StyleDictionary;

    //==========================================================================
    public SvgDocument(XElement root, SvgReaderOptions options)
    {
      StyleDictionary = new Dictionary<string, IDictionary<string, string>>();
      Root    = new SvgSVGElement(this, null, root);
      Options = options;
    }

    //==========================================================================
    public DrawingImage Draw()
    {
      var drawing = Root.Draw();
      var drawingImage = new DrawingImage(drawing);
      //var xamlString = XamlWriter.Save(drawingImage);
      return drawingImage;
    }

    public IDictionary<string, string> GetStylesForClass(string className)
    {
      IDictionary<string, string> result;
      if (!this.StyleDictionary.TryGetValue(className, out result))
      {
        result = new Dictionary<string, string>();
        this.StyleDictionary.Add(className, result);
      }

      return result;
    }

  } // class SvgDocument

}
