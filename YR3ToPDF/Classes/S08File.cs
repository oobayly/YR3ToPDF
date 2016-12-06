using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YR3ToPDF.Classes {
  /// <summary>
  /// </summary>
  public class S08File {
    #region Properties
    /// <summary>
    /// Gets or sets the name of the class.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets the name of the club.
    /// </summary>
    public string Club { get; set; }

    /// <summary>
    /// Gets or sets the date of when the results were created.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the description of the results.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the number of discarded races.
    /// </summary>
    public int Discards { get; set; }

    /// <summary>
    /// Gets or sets the list of legends.
    /// </summary>
    public List<string> Legend { get; set; }

    /// <summary>
    /// Gets or sets the name of the result set.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the number of races sailed.
    /// </summary>
    public int Races { get; set; }

    /// <summary>
    /// Gets or sets the list of results.
    /// </summary>
    public List<Result> Results { get; set; }
    #endregion

    #region Constructors
    public S08File() {
    }

    public S08File(FileInfo file) {
      using (var fs = file.OpenRead()) {
        Import(fs);
      }
    }

    public S08File(Stream stream) {
      Import(stream);
    }
    #endregion

    #region Methods
    private int[] GetColumnWidths(string line) {
      var widths = new List<int>();
      foreach (var col in line.Split(' ')) {
        widths.Add(col.Length);
      }
      widths.RemoveAt(widths.Count - 1);

      // The final column is actually several
      // Final race (same as 1 -> n - 1)
      // Place (5)
      // Gross (7)
      // Discards (8)
      // Nett (6)
      var final = widths.Last();
      widths.RemoveAt(widths.Count - 1);

      widths.Add(widths[widths.Count - 2]);
      widths.Add(5);
      widths.Add(7);
      widths.Add(8);
      widths.Add(6);

      return widths.ToArray();
    }

    private void Import(Stream stream) {
      using (var reader = new StreamReader(stream, System.Text.Encoding.ASCII, true, 8192, true)) {
        ParseHeader(reader.ReadLine());
        reader.ReadLine(); // Blank

        Name = reader.ReadLine().Trim();
        reader.ReadLine(); // Blank

        Description = reader.ReadLine().Trim();
        reader.ReadLine(); // Blank

        ParseClass(reader.ReadLine());
        reader.ReadLine(); // Blank

        ParseRaces(reader.ReadLine());
        ParseDiscards(reader.ReadLine());
        if (Discards > 0)
          reader.ReadLine(); // Discards clarification
        reader.ReadLine(); // Blank
        reader.ReadLine(); // Places header
        reader.ReadLine(); // Horizontal rule

        // Cache the race header line and horizontal rule so we can determine the column widths
        var raceTitles = reader.ReadLine();
        reader.ReadLine(); // 2nd line of race header
        var widths = GetColumnWidths(reader.ReadLine());

        Results = new List<Result>();
        while (true) {
          var line = (reader.ReadLine() ?? "").Trim();
          if (line.Length == 0) {
            break;
          } else {
            ParseResult(line, widths);
          }
        }

        Legend = new List<string>();
        while (true) {
          var line = (reader.ReadLine() ?? "").Trim();
          if (line.Length == 0) {
            break;
          } else {
            Legend.Add(line);
          }
        }
      }
    }

    private void ParseClass(string line) {
      var parts = line.Split(':');
      if (parts.Length == 2) {
        Class = parts[1].Trim();
      } else {
        Class = null;
      }
    }

    private void ParseDiscards(string line) {
      var parts = line.Split('=');
      if (parts.Length == 2) {
        Discards = int.Parse(parts[1].Trim());
      } else {
        Discards = 0;
      }
    }

    private void ParseHeader(string line) {
      // Line is split by the Shift Out (SO - 0x0e), and terminated by the Device Control 4 (DC4 - 0x14) character
      var parts = line.Split((char)0x0e);

      try {
        Date = DateTime.ParseExact(parts[0], "'Printed on 'dd/MM/yyyy' at 'HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
      } catch {
        Date = DateTime.ParseExact(parts[0], "'Printed on 'dd/MM' at 'HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
      }

      Club = parts[1].Split((char)0x14)[0].Trim();
    }

    private void ParseRaces(string line) {
      var parts = line.Split('=');
      Races = int.Parse(parts[1].Trim());
    }

    private void ParseResult(string line, int[] widths) {
      var result = new Result();
      int start = 0;
      int index = 0;

      result.Helm = line.Substring(start, widths[index]).Trim();
      start += widths[index] + 1;
      index++;

      result.SailNumber = line.Substring(start, widths[index]).Trim();
      start += widths[index] + 1;
      index++;

      result.Fleet = line.Substring(start, widths[index]).Trim();
      start += widths[index] + 1;
      index++;
      index++; // Double space before places
      var places = new List<string>();
      for (int i = index; i < widths.Length - 4; i++) {
        places.Add(line.Substring(start, widths[index]).Trim());
        start += widths[index] + 1;
        index++;
      }
      result.Results = places.ToArray();

      result.Place = int.Parse(line.Substring(start, widths[index]).Trim());
      start += widths[index];
      index++;

      result.Gross = decimal.Parse(line.Substring(start, widths[index]).Trim());
      start += widths[index];
      index++;

      if ((start + widths[index]) < line.Length) {
        decimal val = 0;
        if (decimal.TryParse(line.Substring(start, widths[index]).Trim(), out val)) {
          result.Discards = val;
        } else {
          result.Discards = null;
        }
        start += widths[index];
        index++;

        if (decimal.TryParse(line.Substring(start, line.Length - start).Trim(), out val)) {
          result.Nett = val;
        } else {
          result.Nett = null;
        }
        start += widths[index];
        index++;
      }

      Results.Add(result);
    }
    #endregion
  }
}