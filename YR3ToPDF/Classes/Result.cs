using System;
using System.Collections.Generic;

namespace YR3ToPDF.Classes {
  /// <summary>
  /// Represents an individual result.
  /// </summary>
  public class Result {
    #region Properties
    /// <summary>
    /// Gets or sets the discarded points.
    /// </summary>
    public decimal? Discards { get; set; }

    /// <summary>
    /// Gets or sets the description of the fleet.
    /// </summary>
    public string Fleet { get; set; }

    /// <summary>
    /// Gets or sets the name of the helm.
    /// </summary>
    public string Helm { get; set; }

    /// <summary>
    /// Gets or sets the gross points.
    /// </summary>
    public decimal Gross { get; set; }

    /// <summary>
    /// Gets or sets the nett points.
    /// </summary>
    public decimal? Nett { get; set; }

    /// <summary>
    /// Gets or sets the place of the result.
    /// </summary>
    public string Place { get; set; }

    /// <summary>
    /// Gets or sets the list of results.
    /// </summary>
    public string[] Results { get; set; }

    /// <summary>
    /// Gets or sets the sail number of the boat.
    /// </summary>
    public string SailNumber { get; set; }
    #endregion

    #region Constructors
    #endregion

    #region Methods
    #endregion
  }
}