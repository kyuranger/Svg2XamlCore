namespace Svg2Xaml
{
  using System.Windows;
  using System.Windows.Media;
  using System.Windows.Shapes;

  public class SvgShape : Shape
  {
    private DrawingImage previousSource = null;

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(DrawingImage),
      typeof(SvgShape),
      new FrameworkPropertyMetadata(
        null,
        FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        null);

    public DrawingImage Source
    {
      get
      {
        return (DrawingImage)this.GetValue(SourceProperty);
      }

      set
      {
        this.SetValue(SourceProperty, value);
      }
    }

    protected override Geometry DefiningGeometry
    {
      get
      {
        var source = this.Source;
        if (this.previousSource != source)
        {
          // ToDo: Bindungen setzen bzw. umbiegen (anhand von #id / .class).
          this.previousSource = source;
        }

        if (source != null)
        {
          var drawing = source.Drawing as GeometryDrawing;
          if (drawing != null)
          {
            return drawing.Geometry;
          }
        }

        return Geometry.Empty;
      }
    }
  }
}
