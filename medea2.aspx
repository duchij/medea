<%@ Page Language="C#" AutoEventWireup="true" CodeFile="medea2.aspx.cs" Inherits="medea2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" ID="msg_lbl"></asp:Label>
        <%--Zadaj pocet riadkov:<asp:TextBox ID="rows_txt" runat="server"></asp:TextBox><asp:Button ID="runSql_btn" runat="server" Text="Start" OnClick="runSqlFnc" />--%>
        <asp:PlaceHolder ID="data_plh" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
