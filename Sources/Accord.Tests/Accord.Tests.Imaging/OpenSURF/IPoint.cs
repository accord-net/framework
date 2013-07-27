using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSURFcs
{
  public class IPoint
  {
    /// <summary>
    /// Default ctor
    /// </summary>
    public IPoint()
    {
      orientation = 0;
    }

    /// <summary>
    /// Coordinates of the detected interest point
    /// </summary>
    public float x, y;

    /// <summary>
    /// Detected scale
    /// </summary>
    public float scale;

    /// <summary>
    /// Response of the detected feature (strength)
    /// </summary>
    public float response;

    /// <summary>
    /// Orientation measured anti-clockwise from +ve x-axis
    /// </summary>
    public float orientation;

    /// <summary>
    /// Sign of laplacian for fast matching purposes
    /// </summary>
    public int laplacian;

    /// <summary>
    /// Descriptor vector
    /// </summary>
    public int descriptorLength;
    public float [] descriptor = null;
    public void SetDescriptorLength(int Size)
    {
      descriptorLength = Size;
      descriptor = new float[Size];
    }
  }
}
