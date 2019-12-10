////////////////////////////////////////////////////////////////////////////////
//
//  SvgReader.cs - This file is part of Svg2Xaml.
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
using System.IO;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace Svg2Xaml
{
  using System;
  using System.ComponentModel;
  using System.Globalization;
  using System.Linq;
  using System.Text;

  //****************************************************************************
  /// <summary>
  ///   Provides methods to read (and render) SVG documents.
  /// </summary>
  public static class SvgReader
  {

    //==========================================================================
    /// <summary>
    ///   Loads an SVG document and renders it into a 
    ///   <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="reader">
    ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
    ///   document.
    /// </param>
    /// <param name="options">
    ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
    ///   rendering the SVG document.
    /// </param>
    /// <returns>
    ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
    /// </returns>
    public static DrawingImage Load(XmlReader reader, SvgReaderOptions options)
    {
      if (options == null)
        options = new SvgReaderOptions();

      XDocument document = XDocument.Load(reader);
      if (document.Root.Name.NamespaceName != "http://www.w3.org/2000/svg")
        throw new XmlException("Root element is not in namespace 'http://www.w3.org/2000/svg'.");
      if (document.Root.Name.LocalName != "svg")
        throw new XmlException("Root element is not an <svg> element.");

      return new SvgDocument(document.Root, options).Draw();
    }

    //==========================================================================
    /// <summary>
    ///   Loads an SVG document and renders it into a 
    ///   <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="reader">
    ///   A <see cref="XmlReader"/> to read the XML structure of the SVG 
    ///   document.
    /// </param>
    /// <returns>
    ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
    /// </returns>
    public static DrawingImage Load(XmlReader reader)
    {
      return Load(reader, null);
    }

    //==========================================================================
    /// <summary>
    ///   Loads an SVG document and renders it into a 
    ///   <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="stream">
    ///   A <see cref="Stream"/> to read the XML structure of the SVG 
    ///   document.
    /// </param>
    /// <param name="options">
    ///   <see cref="SvgReaderOptions"/> to use for parsing respectively 
    ///   rendering the SVG document.
    /// </param>
    /// <returns>
    ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
    /// </returns>
    public static DrawingImage Load(Stream stream, SvgReaderOptions options)
    {
      using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
        return Load(reader, options);
    }

    //==========================================================================
    /// <summary>
    ///   Loads an SVG document and renders it into a 
    ///   <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="stream">
    ///   A <see cref="Stream"/> to read the XML structure of the SVG 
    ///   document.
    /// </param>
    /// <returns>
    ///   A <see cref="DrawingImage"/> containing the rendered SVG document.
    /// </returns>
    public static DrawingImage Load(Stream stream)
    {
      return Load(stream, null);
    }

    /// <summary>
    /// Entfernt aus diesem String die angegebenen Zeichen.
    /// </summary>
    /// <param name="input">Dieser String.</param>
    /// <param name="chars">Die betreffenden Zeichen.</param>
    /// <returns>Dieser String ohne die betreffenden Zeichen.</returns>
    internal static string RemoveChars(this string input, params char[] chars)
    {
      var sb = new StringBuilder();
      foreach (char t in input)
      {
        if (!chars.Contains(t))
        {
          sb.Append(t);
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Versucht dieses Objekt mit Hilfe von <see cref="TypeConverter.ConvertFrom(object)"/> 
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <param name="obj">Dieses Objekt.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <param name="cultureInfo">
    /// Die als aktuelle Kultur zu verwendenden CultureInfo.
    /// </param>
    /// <param name="typeDescriptorContext">
    /// Eine ITypeDescriptorContext-Schnittstelle, die einen Formatierungskontext bereitstellt. 
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvert<T>(
      this object obj,
      T defaultValue = default(T),
      CultureInfo cultureInfo = null,
      ITypeDescriptorContext typeDescriptorContext = null)
    {
      T value;
      try
      {
        var tc = TypeDescriptor.GetConverter(typeof(T));
        value = (T)tc.ConvertFrom(typeDescriptorContext, cultureInfo ?? CultureInfo.InvariantCulture, obj);
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Versucht diesen String mit Hilfe von <see cref="TypeConverter.ConvertFromString(string)"/> 
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <param name="str">Dieser String.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <param name="cultureInfo">
    /// Die als aktuelle Kultur zu verwendenden CultureInfo.
    /// </param>
    /// <param name="typeDescriptorContext">
    /// Eine ITypeDescriptorContext-Schnittstelle, die einen Formatierungskontext bereitstellt. 
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvert<T>(
      this string str,
      T defaultValue = default(T),
      CultureInfo cultureInfo = null,
      ITypeDescriptorContext typeDescriptorContext = null)
    {
      T value;
      try
      {
        var tc = TypeDescriptor.GetConverter(typeof(T));
        value = (T)tc.ConvertFromString(typeDescriptorContext, cultureInfo ?? CultureInfo.InvariantCulture, str);
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Versucht dieses Objekt mit Hilfe von <see cref="TypeConverter.ConvertFrom(object)"/> 
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <typeparam name="TC">Der zu verwendende <see cref="TypeConverter"/>.</typeparam>
    /// <param name="obj">Dieses Objekt.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <param name="cultureInfo">
    /// Die als aktuelle Kultur zu verwendenden CultureInfo.
    /// </param>
    /// <param name="typeDescriptorContext">
    /// Eine ITypeDescriptorContext-Schnittstelle, die einen Formatierungskontext bereitstellt. 
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvert<T, TC>(
      this object obj,
      T defaultValue = default(T),
      CultureInfo cultureInfo = null,
      ITypeDescriptorContext typeDescriptorContext = null)
      where TC : TypeConverter
    {
      T value;
      try
      {
        var tc = (TC)Activator.CreateInstance(typeof(TC));
        value = (T)tc.ConvertFrom(typeDescriptorContext, cultureInfo ?? CultureInfo.InvariantCulture, obj);
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Versucht diesen String mit Hilfe von <see cref="TypeConverter.ConvertFromString(string)"/> 
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <typeparam name="TC">Der zu verwendende <see cref="TypeConverter"/>.</typeparam>
    /// <param name="str">Dieser String.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <param name="cultureInfo">
    /// Die als aktuelle Kultur zu verwendenden CultureInfo.
    /// </param>
    /// <param name="typeDescriptorContext">
    /// Eine ITypeDescriptorContext-Schnittstelle, die einen Formatierungskontext bereitstellt. 
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvert<T, TC>(
      this string str,
      T defaultValue = default(T),
      CultureInfo cultureInfo = null,
      ITypeDescriptorContext typeDescriptorContext = null)
      where TC : TypeConverter
    {
      T value;
      try
      {
        var tc = (TC)Activator.CreateInstance(typeof(TC));
        value = (T)tc.ConvertFromString(typeDescriptorContext, cultureInfo ?? CultureInfo.InvariantCulture, str);
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Versucht dieses Objekt mit Hilfe von <see cref="TypeConverter.ConvertTo(object, Type)"/> 
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <typeparam name="TC">Der zu verwendende <see cref="TypeConverter"/>.</typeparam>
    /// <param name="obj">Dieses Objekt.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvertTo<T, TC>(this object obj, T defaultValue = default(T))
      where TC : TypeConverter
    {
      T value;
      try
      {
        var tc = (TC)Activator.CreateInstance(typeof(TC));
        value = (T)tc.ConvertTo(obj, typeof(T));
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

    /// <summary>
    /// Versucht diesen String mit Hilfe von <see cref="TypeConverter.ConvertTo(object, Type)"/>
    /// in den Typen <typeparamref name="T"/> zu konvertieren.
    /// </summary>
    /// <typeparam name="T">Der Zieltyp.</typeparam>
    /// <typeparam name="TC">Der zu verwendende <see cref="TypeConverter"/>.</typeparam>
    /// <param name="str">Dieser String.</param>
    /// <param name="defaultValue">
    /// Der Default-Wert, falls eine Konvertierung nicht möglich ist.
    /// </param>
    /// <returns>
    /// Der konvertierte Wert, falls möglich, sonst <paramref name="defaultValue"/>.
    /// </returns>
    internal static T TryConvertTo<T, TC>(this string str, T defaultValue = default(T))
      where TC : TypeConverter
    {
      T value;
      try
      {
        var tc = (TC)Activator.CreateInstance(typeof(TC));
        value = (T)tc.ConvertTo(str, typeof(T));
      }
      catch (Exception ex)
      {
        value = defaultValue;
      }

      return value;
    }

  } // class SvgReader

}
