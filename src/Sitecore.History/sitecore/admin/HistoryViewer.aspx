<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryViewer.aspx.cs" Inherits="SitecoreHistory.sitecore.admin.HistoryViewer" %>

<!DOCTYPE html>
<html>
<head>
    <title></title>
    <link href="/App_Themes/AU/css/main.css" rel="stylesheet" />
    <script type="text/javascript" src="/App_Themes/Base/js/jquery.js"></script>
    <style>
        body {
        }

        ul {
            margin: 0;
            padding: 0;
            list-style: none;
        }

        .histories > ul {
        }

            .histories > ul > li {
                margin: 20px 0;
                /*background-color: #4CAF50; */
                padding: 5px;
            }

                .histories > ul > li > ul {
                    margin: 0;
                }

                    .histories > ul > li > ul > li {
                        background-color: #F1F8E9;
                        padding: 2px 10px;
                        border-radius: 2px;
                    }

        li > section {
            background: #CDDC39;
            padding: 2px 10px;
            border-radius: 5px;
        }

        label {
            padding-left: 5px;
            padding-right: 5px;
        }

        .orignal {
            background: #B2EBF2;
            padding: 5px;
        }

        .update {
            background: #FFE0B2;
            padding: 5px;
        }

        .pt-1 {
            padding-top: 10px;
        }

        .pagination ul li {
            float: left;
            border: 1px solid #a9a9a9;
            margin: 10px;
        }
        .pagination ul li a {
            padding: 5px 10px;
            display: inline-block;
        }
    </style>
</head>
<body class="container">
    <form id="form1" runat="server">
        <div class="histories bs-component">
            <div class="card">
                <div class="row">
                    <div class="col-sm-6">
                        <asp:TextBox runat="server" ID="txtKeywords" Placeholder="Item id or Item Path" CssClass="form-control" />
                    </div>
                    <div class="col-sm-6">
                        <asp:DropDownList runat="server" ID="dpDate" CssClass="form-control">
                            <asp:ListItem Text="Last 7 days" Value="7" />
                            <asp:ListItem Text="Last month" Value="30" />
                            <asp:ListItem Text="Last 3 months" Value="90" />
                            <asp:ListItem Text="Last year" Value="365" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row pt-1">
                    <div class="col-sm-12">
                        <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_OnClick" />
                    </div>
                </div>
            </div>
            <asp:Repeater runat="server" ID="rptHistories">
                <HeaderTemplate>
                    <ul>
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <section>
                            <p>
                                Item ID:  
                                <label>
                                    <%# Eval("Key") %>
                                </label>
                            </p>
                            <p>
                                Item Path: 
                                <label>
                                    <%# GetPath(Container.DataItem) %>
                                </label>
                            </p>
                        </section>
                        <asp:Repeater runat="server" DataSource='<%# Container.DataItem %>'>
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li>
                                    <p>
                                        Date:<label><%# ((DateTime)Eval("Date")).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") %></label>User:<label><%# Eval("Editor") %></label>Language:<label><%# Eval("Language") %></label>
                                    </p>
                                    <asp:Repeater runat="server" DataSource='<%# Eval("Fields") %>'>
                                        <HeaderTemplate>
                                            <ul>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <li>
                                                <a class="change-summary" href="javascript:void(0);"><%# GetChange(Container.DataItem) %></a>
                                                <div class="row" style="display: none">
                                                    <div class="orignal col-md-6">
                                                        <%# Eval("OldValue") %>
                                                    </div>
                                                    <div class="update col-md-6">
                                                        <%# Eval("NewValue") %>
                                                    </div>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </ul>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
            <div class="pagination">
                <asp:Repeater runat="server" ID="rptPagination">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <a href="?id=<%=Keyword %>&date=<%=Date %>&pageIndex=<%# Container.DataItem %>"><%# int.Parse(Container.DataItem.ToString())+1 %></a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>
    <script type="text/javascript">
        (function ($) {
            $('a.change-summary').on('click', function () {
                var container = $(this).next('div');
                if (!!container) {
                    container.toggle();
                }
            });
        })(jQuery);
    </script>
</body>
</html>
