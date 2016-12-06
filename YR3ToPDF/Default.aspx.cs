using ICSharpCode.SharpZipLib.Zip;
using Siberix.Report;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YR3ToPDF {
  public partial class Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
      if (Request.Files.Count == 1) {
        Response.Clear();

        if (Request.Files[0].ContentLength != 0) {
          var report = GeneratePDF(Request.Files[0].InputStream);
          Response.ContentType = "application/pdf";
          Response.AddHeader("Content-Disposition", string.Format("inline; filename=\"{0:yyyy-MM-dd} - {1}.pdf\"", report.Info.Created, report.Info.Title));
          report.Publish(Response.OutputStream, FileFormat.PDF);
        }
        Response.End();

      } else if (Request.Files.Count > 1) {
        Response.Clear();
        Response.ContentType = "application/x-zip-compressed";
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"Results - {0:yyyy-MM-dd HHmm}.zip\"", DateTime.Now));

        var zip = new ZipOutputStream(Response.OutputStream);
        zip.SetLevel(9);

        for (int i = 0; i < Request.Files.Count; i++) {
          var report = GeneratePDF(Request.Files[i].InputStream);
          var entry = new ZipEntry(string.Format("{0:yyyy-MM-dd} - {1}.pdf", report.Info.Created, report.Info.Title));
          using (var ms = new System.IO.MemoryStream()) {
            report.Publish(ms, FileFormat.PDF);
            ms.Position = 0;

            entry.Size = ms.Length;
            zip.PutNextEntry(entry);
            ms.CopyTo(zip);
            zip.CloseEntry();
          }
        }

        zip.Close();
        Response.End();
      }
    }

    private Report GeneratePDF(System.IO.Stream stream) {
      var result = new Classes.S08File(stream, true);
      Bitmap watermark = null;
      if (checkWatermark.Checked) {
        watermark = GetWatermark(result.Club);
      }

      string data;
      stream.Position = 0;
      using (var reader = new System.IO.StreamReader(stream)) {
        data = reader.ReadToEnd();
      }

      var lines = new List<string>();
      using (var reader = new System.IO.StringReader(data)) {
        string line;
        while ((line = reader.ReadLine()) != null) {
          // Replace any control characters
          line = line.Replace((char)0x0e, ' ');
          line = line.Replace((char)0x14, ' ');

          lines.Add(line);
        }
      }

      // Remove any blank lines at the end
      while (lines.Last().Trim() == "") {
        lines.RemoveAt(lines.Count - 1);
      }

      var font = new Font("Courier New", 12, FontStyle.Regular);

      SizeF size;
      using (var img = new Bitmap(1, 1)) {
        using (var g = Graphics.FromImage(img)) {
          size = g.MeasureString(data, font);
        }
      }

      var report = new Report();
      report.Info.Title = result.Name;
      report.Info.Author = result.Club;
      report.Info.Creator = "Siberix Report Writer";
      report.Info.Created = result.Date;
      report.Info.Copyright = string.Format("{0:yyyy} {1}", result.Date, result.Club);

      var section = report.AddSection();
      section.Size = PageSize.A4;
      section.Orientation = Siberix.Report.Orientation.Landscape;
      section.Paddings = new Paddings(10f * 72 / 25.4f); // 10mm padding
      //section.Orientation = isLandscape ? Siberix.Report.Orientation.Landscape : Siberix.Report.Orientation.Portrait;

      var content = new SizeF(
        section.Size.Height - (section.Paddings.Left + section.Paddings.Right),
        section.Size.Width - (section.Paddings.Top + section.Paddings.Bottom)
        );

      float scale = content.Width / (size.Width * 72 / 96); // Use height, as it's the width for landscape
      if (scale > 1)
        scale = 1;

      var style = new Siberix.Report.Text.Style();
      style.Font = new Siberix.Graphics.Fonts.CourierNew(font.Size * scale);
      style.Brush = Siberix.Graphics.Brushes.Black;

      Siberix.Report.Grid.IGrid grid = null;
      foreach (var line in lines) {
        if (grid == null) {
          grid = AddGrid(section, content.Width, watermark);
        }

        var row = AddLine(grid, style, line);

        var measured = grid.Measure();
        if (measured.Height > content.Height) {
          grid.Remove(row);
          section.AddPageBreak();

          grid = AddGrid(section, content.Width, watermark);
          AddLine(grid, style, line);
        }
      }

      return report;
    }

    private Bitmap GetWatermark(string club) {
      switch (club) {
        case "LOUGH DERG YACHT CLUB":
          return new Bitmap(Server.MapPath("~/burgees/ldyc-watermark.png"));

        default:
          return null;
      }
    }

    private Siberix.Report.Grid.IGrid AddGrid(Siberix.Report.Section.ISection section, float width, Bitmap watermark = null) {
      if (watermark != null) {
        var wm = section.AddWatermark();
        wm.AddImage(new Siberix.Wrappers.GDIPlus.Image(watermark), 0, 0);
      }

      var grid = section.AddGrid();
      grid.Width = new DirectWidth(width);
      var column = grid.AddColumn();
      column.Width = new RelativeWidth(100);

      return grid;
    }

    private Siberix.Report.Grid.IRow AddLine(Siberix.Report.Grid.IGrid grid, Siberix.Report.Text.Style style, string line) {
      var row = grid.AddRow();
      var cell = row.AddCell();

      var text = cell.AddText();
      text.Width = new RelativeWidth(100);
      text.Style = style;
      text.AddContent(line);

      return row;
    }
  }
}