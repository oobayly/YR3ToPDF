<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YR3ToPDF.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Convert YR3 print files to PDF</title>
  <script src="https://code.jquery.com/jquery-3.1.1.min.js" integrity="sha256-hVVnYaiADRTO2PzUGmuLJr8BLUSjGIZsDYGmIJLv2b8=" crossorigin="anonymous"></script>
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" />
  <style type="text/css">
    html, body {
      margin: 0;
      padding: 0;
      height: 100%;
      font-family: sans-serif;
    }

    .content {
      display: flex;
      flex-direction: column;
      height: 100%;
    }

    .content > * {
      margin: 1ex 1ex 0ex 1ex;
      border-bottom: 1px solid #ccc;
      padding-bottom: 1ex;
    }
    .content > *:last-child {
      border-bottom: 0;
    }

    .content form {
    }

    .content form .options {
      margin: 1ex 0;
    }

    .content iframe {
      flex: 1 0 0;
      border: 0;
    }

    .button {
      display: inline-block;
      box-sizing: border-box;
      border: 1px solid #ccc;
      padding: .5ex 1ex;
      width: 20ex;
      border-radius: 4px;
      background-color: white;
      font-size: 16px;
      cursor: pointer;
      text-align: center;
    }
    .button i {
      margin-right: .5ex;
    }

    .file-upload input[type='file'] {
      display: none;
    }

    .file-list > * {
      display: inline-block;
      font-size: smaller;
    }

    .file-list > *:not(:first-child) {
      margin-left: .5ex;
    }
    .file-list > *:not(:last-child):after {
      content: ", ";
    }

    .options > *:not(:first-child) {
      display: inline-block;
      margin-left: .5ex;
    }
  </style>
</head>
<body>
  <a href="https://github.com/oobayly/YR3ToPDF"><img style="position: absolute; top: 0; right: 0; border: 0;" src="https://camo.githubusercontent.com/a6677b08c955af8400f44c6298f40e7d19cc5b2d/68747470733a2f2f73332e616d617a6f6e6177732e636f6d2f6769746875622f726962626f6e732f666f726b6d655f72696768745f677261795f3664366436642e706e67" alt="Fork me on GitHub" data-canonical-src="https://s3.amazonaws.com/github/ribbons/forkme_right_gray_6d6d6d.png"></a>
  <div class="content">
    <h2>Convert YR3 print files to PDF</h2>
    <form method="post" action="Default.aspx" target="pdf" enctype="multipart/form-data" runat="server">
      <div>
        <div class="file-upload">
          <label for="files" class="button"><i class="fa fa-file-text"></i>Select files</label>
          <input type="file" id="files" name="files" accept=".txt,.S08" multiple="multiple" />
          <span class="file-list"></span>
        </div>
        <div class="options">
          <asp:CheckBox ID="checkWatermark" runat="server" Text="Include watermark" ToolTip="Watermark the results with the club burgee (if available)" />
        </div>
        <button class="button submit"><i class="fa fa-cloud-upload"></i>Convert files</button>
      </div>
    </form>

    <iframe name="pdf"></iframe>
  </div>  

  <script type="text/javascript">
    $(document).ready(function () {
      var frm = $("form").first();
      var config = {};

      var saveConfig = function (e) {
        var target = $(e.target);
        var value = null;
        if (target.prop("type") === "checkbox") {
          value = e.target.checked;
        }
        
        if (value == null) {
          delete config[e.target.id];
        } else {
          config[e.target.id] = value;
        }

        window.localStorage.setItem("config", JSON.stringify(config));
      };

      var init = function () {
        config = $.extend(config, JSON.parse(window.localStorage.getItem("config"), "{}"));

        $.each(config, function (key, value) {
          var target = frm.find("#" + key);
          if (target.prop("type") === "checkbox") {
            target[0].checked = value;
          }
        });

        frm.find("#files").on("change", function (e) {
          var fileList = $(".file-list");
          fileList.children().remove();
          $.each(e.target.files, function (index, item) {
            $("<span>", {
              title: item.size + " bytes"
            })
            .text(item.name)
            .appendTo(fileList);
          });

          frm.find(".submit").prop("disabled", e.target.files.length == 0);
        });

        frm.find("input[type='checkbox']").on("change", function (e) {
          saveConfig(e);
        });

        $(".submit").on("click", function (e) {
          e.target.form.submit();
        })

        frm.find(".submit").prop("disabled", true);
      };

      init();
    });
  </script>
</body>
</html>
