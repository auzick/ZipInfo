<%@ Page Language="C#" AutoEventWireup="true" Inherits="ZipInfo.UI.Admin.ZipInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>ZipInfo Utility</h1>
            <hr />
            <h2>Lookup</h2>
            <p><asp:Label ID="Label1" runat="server" AssociatedControlID="ZipLookupTextbox" Text="Zip code:"></asp:Label>
                &nbsp;
                <asp:TextBox ID="ZipLookupTextbox" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="LookupButton" runat="server" OnClick="LookupButton_Click" Text="Lookup" />
            </p>
            <p>
                <asp:Literal ID="LookupResultLiteral" runat="server"></asp:Literal>
            </p>
            
            <hr />
            <h2>Clear the zip code cache</h2>
            <p>
                <asp:Literal ID="CacheStateLiteral" runat="server"></asp:Literal>
            </p>
            <p>
                <asp:Button ID="ClearCacheButton" runat="server" Text="Clear Cache" OnClick="ClearCacheButton_Click" />
            </p>

            <hr />
            <h2>Reload data from file</h2>
            <p>
                <asp:CheckBox ID="ForceCheckbox" runat="server" Text="Force" />
                <br />
                <asp:Button ID="ReloadButton" runat="server" Text="Reload" OnClick="ReloadButton_Click" />
            </p>
            <p>
                <asp:Label ID="ResultLabel" runat="server"></asp:Label>
            </p>

        </div>
    </form>
</body>
</html>
