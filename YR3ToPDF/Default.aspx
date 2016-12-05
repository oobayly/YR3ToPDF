<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YR3ToPDF.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>YR3 to PDF</title>
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
      margin: 10px;
    }

    .content form {
    }

    .content iframe {
      flex: 1 0 0;
      border: 0;
    }
  </style>
</head>
<body>
  <div class="content">
    <h1>Convert YR3 print files to PDF</h1>
    <form method="post" action="Default.aspx" target="pdf" enctype="multipart/form-data">
    <div>
      <input type="file" name="files" accept=".txt,.S08" multiple="multiple" />
      <input type="submit" value="Convert files" />
    </div>
    </form>

    <iframe name="pdf"></iframe>
  </div>  
</body>
</html>
