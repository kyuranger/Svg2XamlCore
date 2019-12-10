////////////////////////////////////////////////////////////////////////////////
//
//  SvgBaseElement.cs - This file is part of Svg2Xaml.
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
using System.Xml;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  //****************************************************************************
  /// <summary>
  ///   Base class for all other SVG elements.
  /// </summary>
  class SvgBaseElement
  {

    //==========================================================================
    public readonly SvgDocument Document;

    //==========================================================================
    public readonly string Reference = null;

    //==========================================================================
    public SvgSVGElement Root
    {
      get
      {
        return Document.Root;
      }
    }

    //==========================================================================
    public readonly SvgBaseElement Parent;

    //==========================================================================
    public readonly string Id = null;

    //==========================================================================
    public readonly XElement Element;

    //==========================================================================
    protected SvgBaseElement(SvgDocument document, SvgBaseElement parent, XElement element)
    {
      Document = document;
      Parent   = parent;

      // Create attributes from styles...

      // ... use element name as class ...
      this.SetStyleAttributesForClasses(document, element, element.Name.LocalName);

      // ... use id as class ...
      var idAttribute = element.Attribute("id");
      if (idAttribute != null)
      {
        this.SetStyleAttributesForClasses(document, element, "#" + idAttribute.Value.Trim());
      }

      // ... use class attribute ... 
      var classAttribute = element.Attribute("class");
      if (classAttribute != null)
      {
        var classNames = classAttribute.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        classNames = Array.ConvertAll(classNames, className => "." + className);
        this.SetStyleAttributesForClasses(document, element, classNames);
      }

      // ... use style attribute ... 
      XAttribute styleAttribute = element.Attribute("style");
      if(styleAttribute != null)
      {
        foreach(string property in styleAttribute.Value.Split(';'))
        {
          string[] tokens = property.Split(':');
          if(tokens.Length == 2)
            try
            {
              element.SetAttributeValue(tokens[0], tokens[1]);
            }
            catch(XmlException ex)
            {
              Debug.WriteLine(ex);
            }
        }
        styleAttribute.Remove();
      }

      if(idAttribute != null)
        Document.Elements[Id = idAttribute.Value] = this;

      XAttribute hrefAttribute = element.Attribute(XName.Get("href", "http://www.w3.org/1999/xlink"));
      if(hrefAttribute != null)
      {
        string reference = hrefAttribute.Value;
        if(reference.StartsWith("#"))
          Reference = reference.Substring(1);
      }

      Element = element;
    }

    private void SetStyleAttributesForClasses(SvgDocument document, XElement element, params string[] classes)
    {
      foreach (var className in classes)
      {
        IDictionary<string, string> dict;
        if (document.StyleDictionary.TryGetValue(className, out dict))
        {
          if (dict != null)
          {
            foreach (var keyValue in dict)
            {
              try
              {
                element.SetAttributeValue(keyValue.Key, keyValue.Value);
              }
              catch (XmlException ex)
              {
                Debug.WriteLine(ex);
              }
            }
          }
        }
      }
    }

  } // class SvgBaseElement

}
