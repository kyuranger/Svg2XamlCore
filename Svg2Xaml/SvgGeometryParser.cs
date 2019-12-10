namespace Svg2Xaml
{
  using System;
  using System.Globalization;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Media;

  /// <summary> 
  /// Parser for XAML abbreviated geometry.
  /// SVG path spec is closely followed http://www.w3.org/TR/SVG11/paths.html 
  /// 3/23/2006, new parser for performance (fyuan)
  /// </summary>
  public class SvgGeometryParser
  {
    private readonly bool allowSign = true;

    private readonly bool allowComma = true;

    private readonly bool isFilled;

    private readonly bool isClosed = true;

    private readonly bool isStroked = true;

    private readonly bool isSmoothJoin = true;

    private StreamGeometryContext context;

    private int curIndex; // Location to read next character from 

    private bool figureStarted; // StartFigure is effective

    private IFormatProvider formatProvider;

    private Point lastPoint; // Last point

    private Point lastStart; // Last figure starting point

    private int pathLength;

    private string pathString; // Input string to be parsed

    private Point secondLastPoint; // The point before last point

    private char token; // Non whitespace character returned by ReadToken

    private SvgGeometryParser(bool isFilled = true)
    {
      this.isFilled = isFilled;
    }

    /// <summary>
    /// Parse a PathGeometry string.
    /// The PathGeometry syntax is the same as the PathFigureCollection syntax except that it
    /// may start with a "wsp*Fwsp*(0|1)" which indicate the winding mode (F0 is EvenOdd while 
    /// F1 is NonZero).
    /// </summary> 
    public static Geometry ParseGeometry(string pathString, bool isFilled = true)
    {
      var geometry = ParseGeometry(pathString, CultureInfo.InvariantCulture, isFilled);
      return geometry;
    }

    /// <summary>
    /// Parse a PathGeometry string.
    /// The PathGeometry syntax is the same as the PathFigureCollection syntax except that it
    /// may start with a "wsp*Fwsp*(0|1)" which indicate the winding mode (F0 is EvenOdd while 
    /// F1 is NonZero).
    /// </summary> 
    public static Geometry ParseGeometry(string pathString, IFormatProvider formatProvider, bool isFilled = true)
    {
      FillRule fillRule = FillRule.EvenOdd;
      StreamGeometry geometry = new StreamGeometry();
      StreamGeometryContext context = geometry.Open();

      ParseStringToStreamGeometryContext(context, pathString, formatProvider, ref fillRule, isFilled);

      geometry.FillRule = fillRule;
      geometry.Freeze();

      return geometry;
    }

    /// <summary>
    /// Given a mini-language representation of a Geometry - write it to the
    /// supplied streamgeometrycontext 
    /// </summary> 
    private static void ParseStringToStreamGeometryContext(
      StreamGeometryContext context,
      string pathString,
      IFormatProvider formatProvider,
      ref FillRule fillRule,
      bool isFilled = true)
    {
      using (context)
      {
        // Check to ensure that there's something to parse 
        if (pathString != null)
        {
          int curIndex = 0;

          // skip any leading space
          while ((curIndex < pathString.Length) && char.IsWhiteSpace(pathString, curIndex))
          {
            curIndex++;
          }

          // Is there anything to look at? 
          if (curIndex < pathString.Length)
          {
            // If so, we only care if the first non-WhiteSpace char encountered is 'F'
            if (pathString[curIndex] == 'F')
            {
              curIndex++;

              // Since we found 'F' the next non-WhiteSpace char must be 0 or 1 - look for it.
              while ((curIndex < pathString.Length) && char.IsWhiteSpace(pathString, curIndex))
              {
                curIndex++;
              }

              // If we ran out of text, this is an error, because 'F' cannot be specified without 0 or 1
              // Also, if the next token isn't 0 or 1, this too is illegal 
              if ((curIndex == pathString.Length) || ((pathString[curIndex] != '0') && (pathString[curIndex] != '1')))
              {
                throw new FormatException("Illegal Token");
              }

              fillRule = pathString[curIndex] == '0' ? FillRule.EvenOdd : FillRule.Nonzero;

              // Increment curIndex to point to the next char
              curIndex++;
            }
          }

          SvgGeometryParser parser = new SvgGeometryParser(isFilled);

          parser.ParseToGeometryContext(context, pathString, curIndex);
        }
      }
    }

    /// <summary> 
    /// Throw unexpected token exception
    /// </summary>
    private void ThrowBadToken()
    {
      throw new FormatException(string.Format("Unexpected Token: {0} {1}", this.pathString, this.curIndex - 1));
    }

    private bool More()
    {
      return this.curIndex < this.pathLength;
    }

    // Skip white space, one comma if allowed 
    private bool SkipWhiteSpace(bool allowComma)
    {
      bool commaMet = false;

      while (this.More())
      {
        char ch = this.pathString[this.curIndex];

        switch (ch)
        {
          case ' ':
          case '\n':
          case '\r':
          case '\t': // SVG whitespace 
            break;

          case ',':
            if (allowComma)
            {
              commaMet = true;
              allowComma = false; // one comma only 
            }
            else
            {
              this.ThrowBadToken();
            }
            break;

          default:
            // Avoid calling IsWhiteSpace for ch in (' ' .. 'z'] 
            if (((ch > ' ') && (ch <= 'z')) || !char.IsWhiteSpace(ch))
            {
              return commaMet;
            }
            break;
        }

        this.curIndex++;
      }

      return commaMet;
    }

    /// <summary>
    /// Read the next non whitespace character 
    /// </summary>
    /// <returns>True if not end of string</returns> 
    private bool ReadToken()
    {
      this.SkipWhiteSpace(!this.allowComma);

      // Check for end of string
      if (this.More())
      {
        this.token = this.pathString[this.curIndex++];

        return true;
      }
      else
      {
        return false;
      }
    }

    private bool IsNumber(bool allowComma)
    {
      bool commaMet = this.SkipWhiteSpace(allowComma);

      if (this.More())
      {
        this.token = this.pathString[this.curIndex];

        // Valid start of a number
        if ((this.token == '.') || (this.token == '-') || (this.token == '+')
            || ((this.token >= '0') && (this.token <= '9')) || (this.token == 'I') // Infinity 
            || (this.token == 'N')) // NaN
        {
          return true;
        }
      }

      if (commaMet) // Only allowed between numbers
      {
        this.ThrowBadToken();
      }

      return false;
    }

    private void SkipDigits(bool signAllowed)
    {
      // Allow for a sign 
      if (signAllowed && this.More()
          && ((this.pathString[this.curIndex] == '-') || this.pathString[this.curIndex] == '+'))
      {
        this.curIndex++;
      }

      while (this.More() && (this.pathString[this.curIndex] >= '0') && (this.pathString[this.curIndex] <= '9'))
      {
        this.curIndex++;
      }
    }

    /// <summary> 
    /// Read a floating point number
    /// </summary> 
    /// <returns></returns>
    private double ReadNumber(bool allowComma)
    {
      if (!this.IsNumber(allowComma))
      {
        this.ThrowBadToken();
      }

      bool simple = true;
      int start = this.curIndex;

      //
      // Allow for a sign 
      //
      // There are numbers that cannot be preceded with a sign, for instance, -NaN, but it's 
      // fine to ignore that at this point, since the CLR parser will catch this later. 
      //
      if (this.More() && ((this.pathString[this.curIndex] == '-') || this.pathString[this.curIndex] == '+'))
      {
        this.curIndex++;
      }

      // Check for Infinity (or -Infinity).
      if (this.More() && (this.pathString[this.curIndex] == 'I'))
      {
        //
        // Don't bother reading the characters, as the CLR parser will 
        // do this for us later.
        //
        this.curIndex = Math.Min(this.curIndex + 8, this.pathLength); // "Infinity" has 8 characters
        simple = false;
      }
      // Check for NaN 
      else if (this.More() && (this.pathString[this.curIndex] == 'N'))
      {
        // 
        // Don't bother reading the characters, as the CLR parser will
        // do this for us later.
        //
        this.curIndex = Math.Min(this.curIndex + 3, this.pathLength); // "NaN" has 3 characters 
        simple = false;
      }
      else
      {
        this.SkipDigits(!this.allowSign);

        // Optional period, followed by more digits
        if (this.More() && (this.pathString[this.curIndex] == '.'))
        {
          simple = false;
          this.curIndex++;
          this.SkipDigits(!this.allowSign);
        }

        // Exponent
        if (this.More() && ((this.pathString[this.curIndex] == 'E') || (this.pathString[this.curIndex] == 'e')))
        {
          simple = false;
          this.curIndex++;
          this.SkipDigits(this.allowSign);
        }
      }

      if (simple && (this.curIndex <= (start + 8))) // 32-bit integer
      {
        int sign = 1;

        if (this.pathString[start] == '+')
        {
          start++;
        }
        else if (this.pathString[start] == '-')
        {
          start++;
          sign = -1;
        }

        int value = 0;

        while (start < this.curIndex)
        {
          value = value * 10 + (this.pathString[start] - '0');
          start++;
        }

        return value * sign;
      }
      else
      {
        string subString = this.pathString.Substring(start, this.curIndex - start);

        try
        {
          return Convert.ToDouble(subString, this.formatProvider);
        }
        catch (FormatException except)
        {
          throw new FormatException(string.Format("Unexpected Token: {0} {1}", this.pathString, start), except);
        }
      }
    }

    /// <summary> 
    /// Read a bool: 1 or 0
    /// </summary> 
    /// <returns></returns> 
    private bool ReadBool()
    {
      this.SkipWhiteSpace(this.allowComma);

      if (this.More())
      {
        this.token = this.pathString[this.curIndex++];

        if (this.token == '0')
        {
          return false;
        }
        else if (this.token == '1')
        {
          return true;
        }
      }

      this.ThrowBadToken();

      return false;
    }

    /// <summary> 
    /// Read a relative point
    /// </summary> 
    /// <returns></returns> 
    private Point ReadPoint(char cmd, bool allowcomma)
    {
      double x = this.ReadNumber(allowcomma);
      double y = this.ReadNumber(this.allowComma);

      if (cmd >= 'a') // 'A' < 'a'. lower case for relative 
      {
        x += this.lastPoint.X;
        y += this.lastPoint.Y;
      }

      return new Point(x, y);
    }

    /// <summary> 
    /// Reflect _secondLastPoint over _lastPoint to get a new point for smooth curve
    /// </summary> 
    /// <returns></returns> 
    private Point Reflect()
    {
      return new Point(2 * this.lastPoint.X - this.secondLastPoint.X, 2 * this.lastPoint.Y - this.secondLastPoint.Y);
    }

    private void EnsureFigure()
    {
      if (!this.figureStarted)
      {
        this.context.BeginFigure(this.lastStart, this.isFilled, !this.isClosed);
        this.figureStarted = true;
      }
    }

    /// <summary>
    /// Parse a PathFigureCollection string 
    /// </summary> 
    internal void ParseToGeometryContext(StreamGeometryContext context, string pathString, int startIndex)
    {
      // [BreakingChange] Dev10 Bug #453199 
      // We really should throw an ArgumentNullException here for context and pathString.

      // From original code 
      // This is only used in call to Double.Parse
      this.formatProvider = CultureInfo.InvariantCulture;

      this.context = context;
      this.pathString = pathString;
      this.pathLength = pathString.Length;
      this.curIndex = startIndex;

      this.secondLastPoint = new Point(0, 0);
      this.lastPoint = new Point(0, 0);
      this.lastStart = new Point(0, 0);

      this.figureStarted = false;

      bool first = true;

      char lastCmd = ' ';

      while (this.ReadToken()) // Empty path is allowed in XAML
      {
        char cmd = this.token;

        if (first)
        {
          if ((cmd != 'M') && (cmd != 'm')) // Path starts with M|m
          {
            this.ThrowBadToken();
          }

          first = false;
        }

        switch (cmd)
        {
          case 'm':
          case 'M':
            // XAML allows multiple points after M/m 
            this.lastPoint = this.ReadPoint(cmd, !this.allowComma);

            context.BeginFigure(this.lastPoint, this.isFilled, !this.isClosed);
            this.figureStarted = true;
            this.lastStart = this.lastPoint;
            lastCmd = 'M';

            while (this.IsNumber(this.allowComma))
            {
              this.lastPoint = this.ReadPoint(cmd, !this.allowComma);

              context.LineTo(this.lastPoint, this.isStroked, !this.isSmoothJoin);
              lastCmd = 'L';
            }
            break;

          case 'l':
          case 'L':
          case 'h':
          case 'H':
          case 'v':
          case 'V':
            this.EnsureFigure();

            do
            {
              switch (cmd)
              {
                case 'l':
                  this.lastPoint = this.ReadPoint(cmd, !this.allowComma);
                  break;
                case 'L':
                  this.lastPoint = this.ReadPoint(cmd, !this.allowComma);
                  break;
                case 'h':
                  this.lastPoint.X += this.ReadNumber(!this.allowComma);
                  break;
                case 'H':
                  this.lastPoint.X = this.ReadNumber(!this.allowComma);
                  break;
                case 'v':
                  this.lastPoint.Y += this.ReadNumber(!this.allowComma);
                  break;
                case 'V':
                  this.lastPoint.Y = this.ReadNumber(!this.allowComma);
                  break;
              }

              context.LineTo(this.lastPoint, this.isStroked, !this.isSmoothJoin);
            }
            while (this.IsNumber(this.allowComma));

            lastCmd = 'L';
            break;

          case 'c':
          case 'C': // cubic Bezier
          case 's':
          case 'S': // smooth cublic Bezier 
            this.EnsureFigure();

            do
            {
              Point p;

              if ((cmd == 's') || (cmd == 'S'))
              {
                if (lastCmd == 'C')
                {
                  p = this.Reflect();
                }
                else
                {
                  p = this.lastPoint;
                }

                this.secondLastPoint = this.ReadPoint(cmd, !this.allowComma);
              }
              else
              {
                p = this.ReadPoint(cmd, !this.allowComma);

                this.secondLastPoint = this.ReadPoint(cmd, this.allowComma);
              }

              this.lastPoint = this.ReadPoint(cmd, this.allowComma);

              context.BezierTo(p, this.secondLastPoint, this.lastPoint, this.isStroked, !this.isSmoothJoin);

              lastCmd = 'C';
            }
            while (this.IsNumber(this.allowComma));

            break;

          case 'q':
          case 'Q': // quadratic Bezier
          case 't':
          case 'T': // smooth quadratic Bezier 
            this.EnsureFigure();

            do
            {
              if ((cmd == 't') || (cmd == 'T'))
              {
                if (lastCmd == 'Q')
                {
                  this.secondLastPoint = this.Reflect();
                }
                else
                {
                  this.secondLastPoint = this.lastPoint;
                }

                this.lastPoint = this.ReadPoint(cmd, !this.allowComma);
              }
              else
              {
                this.secondLastPoint = this.ReadPoint(cmd, !this.allowComma);
                this.lastPoint = this.ReadPoint(cmd, this.allowComma);
              }

              context.QuadraticBezierTo(this.secondLastPoint, this.lastPoint, this.isStroked, !this.isSmoothJoin);

              lastCmd = 'Q';
            }
            while (this.IsNumber(this.allowComma));

            break;

          case 'a':
          case 'A':
            this.EnsureFigure();

            do
            {
              // A 3,4 5, 0, 0, 6,7 
              double w = this.ReadNumber(!this.allowComma);
              double h = this.ReadNumber(this.allowComma);
              double rotation = this.ReadNumber(this.allowComma);
              bool large = this.ReadBool();
              bool sweep = this.ReadBool();

              this.lastPoint = this.ReadPoint(cmd, this.allowComma);

              context.ArcTo(
                this.lastPoint,
                new Size(w, h),
                rotation,
                large,
#if PBTCOMPILER
                            sweep, 
#else
                sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
#endif
                this.isStroked,
                !this.isSmoothJoin);
            }
            while (this.IsNumber(this.allowComma));

            lastCmd = 'A';
            break;

          case 'z':
          case 'Z':
            this.EnsureFigure();
            //context.SetClosedState(isClosed);
            var mi = context.GetType().GetMethod("SetClosedState", BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi != null)
            {
              var pia = mi.GetParameters();
              if (pia != null && pia.Length == 1 && pia[0].ParameterType == typeof(bool))
              {
                mi.Invoke(context, new object[] { this.isClosed });
              }
            }

            this.figureStarted = false;
            lastCmd = 'Z';

            this.lastPoint = this.lastStart; // Set reference point to be first point of current figure 
            break;

          default:
            this.ThrowBadToken();
            break;
        }
      }
    }
  }
}