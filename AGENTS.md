# XHS 项目代理规则

本文件定义 XHS 项目中的统一协作规则，所有代理、自动化脚本和代码修改都必须遵守。

## 通用规则

1. 项目中所有需要中文的地方一律使用简体中文。
2. 文本文件默认保存为 UTF-8 with BOM 编码。
3. C# 类名、属性名和字段名统一使用 C# 命名风格，公共成员使用 PascalCase。
4. 不允许使用硬编码，配置项、连接字符串和可变业务规则必须集中配置或统一管理。
5. 所有 `aspx` 文件中的中文内容一律使用简体中文，文件保存格式统一为 UTF-8 with BOM。
6. `aspx` 页面中的浏览按钮统一使用 `btn5` 样式，标准写法为 `<li class="btn5"><asp:LinkButton ID="lnk_view" runat="server" OnClick="lnk_view_Click" ToolTip="浏览">浏览</asp:LinkButton></li>`。
7. `aspx` 页面中的上传按钮统一使用 `btn13` 样式，对应图片资源为 `up.jpeg`，标准写法为 `<li class="btn13"><asp:LinkButton ID="lnkbutton_upload" runat="server" OnClick="lnkbutton_upload_Click" ToolTip="上传">上传</asp:LinkButton></li>`。
8. `aspx` 页面中的保存按钮统一使用 `btn3` 样式，并放入 `mod2` 区域，标准写法为 `<li class="btn3"><asp:LinkButton ID="lnkbutton_save" runat="server" ToolTip="保存/save" OnClick="lnkbutton_save_Click">保存/save</asp:LinkButton></li>`。

## 分层约束

1. BLL 层不允许直接执行 SQL 语句。
2. 所有 SQL 语句必须在 MSSQL 层执行。
3. `Model` 中带 `Info` 后缀的类一般都有对应的数据表，表名统一为 `tb_去掉Info后缀的类名`。
4. 当前项目模型层统一命名为 `XHS.Model`，模型类命名空间统一使用 `XHS.Model`，子目录模型使用 `XHS.Model.子命名空间`。
5. `IDAL` 中定义的方法，`MSSQL` 层必须提供对应实现。
6. `DALFactory` 负责创建 `MSSQL` 层实例。
7. `BLL` 层负责向页面提供可调用的方法，不直接承担底层 SQL 执行职责。
8. `MSSQL` 层的 `Insert`、`Update`、`Delete` 方法统一接收 `List<Info>` 类型参数，方法统一返回 `ParamterInfo`。
9. `Get` 类方法统一返回 `List<Info>` 类型结果。
10. `BLL` 层通过构造函数初始化 `dal` 成员，例如 `dal = XHS.DALFactory.XXX.Create();`，业务方法统一通过该 `dal` 成员调用。
11. `BLL` 层的保存类方法按以下方式处理：先调用 `dal` 获取 `ParamterInfo`，再使用 `IList source = new ArrayList(); source.Add(paramterInfo);` 调用 `Common.Save(source)`，方法返回 `string`。
12. 数据库连接字符串统一为 `server=.;Pooling=false;database=XHS;uid=sa;pwd=MES2016mj`。
13. 使用 `sqlcmd` 连接本地数据库时，统一使用命令前缀 `sqlcmd -S . -d XHS -U sa -P MES2016mj -N -C -Q`。

## 测试要求

1. 页面功能完成后，使用 Playwright 生成或补充对应测试。

