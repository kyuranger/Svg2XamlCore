////////////////////////////////////////////////////////////////////////////////
//
//  SvgStyleElement.cs - This file is part of Svg2Xaml.
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
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System.Text;

  //****************************************************************************
  /// <summary>
  ///   Represents a &lt;style&gt; element.
  /// </summary>
  class SvgStyleElement
    : SvgBaseElement
  {

    //==========================================================================
    public SvgStyleElement(SvgDocument document, SvgBaseElement parent, XElement styleElement)
      : base(document, parent, styleElement)
    {
      var cssLine = styleElement.Value.RemoveChars('\n', '\r', '\t');
      var classesStyleTokens =
        cssLine.Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries)
          .Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();

      if (classesStyleTokens.Length % 2 == 0)
      {
        for (var i = 1; i < classesStyleTokens.Length; i += 2)
        {
          var classesTokens = classesStyleTokens[i - 1].Split(new []{ ',' }, StringSplitOptions.RemoveEmptyEntries);
          var styleTokens = classesStyleTokens[i].Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries);
          if (classesTokens.Length > 0 && classesStyleTokens.Length > 0)
          {
            foreach (var styleToken in styleTokens)
            {
              var styleKeyValue = styleToken.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
              if (styleKeyValue.Length == 2)
              {
                foreach (var className in classesTokens)
                {
                  var dict = document.GetStylesForClass(className.Trim());
                  dict.Add(styleKeyValue[0].Trim(), styleKeyValue[1].Trim());
                }
              }
            }
          }
        }
      }
    }

  } // class SvgStyleElement

}
