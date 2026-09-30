# XHS 项目代码规范补充

## MSSQL 查询方法规范

1. 公开 `Get`、`Search` 查询方法只负责组装 SQL 和 `SqlParameter[]` 参数，不直接执行数据读取。
2. 每个查询实体统一通过对应的私有 `_get...` 方法读取数据。例如：

   ```csharp
   string querystring = "select * from tb_process order by listorder";
   List<ProcessInfo> ps = _getProcessInfo(querystring, null);

   private List<ProcessInfo> _getProcessInfo(string querystring, SqlParameter[] pars)
   {
       DataSet ds;
       if (pars != null)
       {
           ds = Data.getDataSet(querystring, pars);
       }
       else
       {
           ds = Data.getDataSet(querystring);
       }

       DataTable dt = ds.Tables[0];
       IList ilist = new ArrayList();
       // 将 DataRow 转换为 Info 对象并返回 List<Info>。
   }
   ```

3. 私有查询方法统一接收 `string querystring, SqlParameter[] pars`，统一负责 `DataSet` 读取和 `DataRow` 到 `Info` 的转换。
4. 查询条件必须使用参数化 SQL，不得通过字符串拼接用户输入值。
5. `Get` 类方法统一返回 `List<Info>`，SQL 只允许在 MSSQL 层执行。

## Playwright 测试部署规范

1. 修改 `XHSWorkFlow` 下的页面、后台代码、配置或静态资源后，必须先将本次修改涉及的文件同步到 `deploy/XHSWorkFlow` 对应目录，再进行 Playwright 测试。
2. 同步时只复制本次修改涉及的文件，保留用户在 `deploy` 目录中的无关修改，不得使用会覆盖整个目录或删除文件的操作。
3. 确认 IIS 的 `XHS` 应用实际指向 `deploy/XHSWorkFlow`，并在同步后重新加载应用；Playwright 测试统一使用 `http://localhost/XHS/`，登录地址统一使用 `http://localhost/XHS/login.aspx`。
4. 如果修改涉及程序集或编译输出，必须先完成对应项目的构建，并将更新后的程序集及其依赖同步到 `deploy/XHSWorkFlow/bin` 后再测试。
5. 测试报告中必须说明已同步到 `deploy`，避免把对源目录的修改误认为已部署到测试站点。

## Git 提交与推送规范

1. `deploy` 目录仅用于本地部署和 Playwright 测试，不需要上传到 GitHub；提交或推送时只包含源目录中的修改，忽略 `deploy` 目录中的对应文件。

## 文件权限规范

1. 发现准备修改的目标文件为只读文件时，必须先询问用户，获得确认后才能继续。
