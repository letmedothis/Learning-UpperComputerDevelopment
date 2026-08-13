namespace C_Differences.Demos._05_LINQ;

/// <summary>
/// LINQ 演示 - Language Integrated Query
/// </summary>
public class LinqDemo
{
    // 示例数据
    private readonly List<Employee> _employees;
    private readonly List<LinqProduct> _products;

    public LinqDemo()
    {
        _employees = new List<Employee>
        {
            new(1, "张三", "技术部", 15000, 28),
            new(2, "李四", "市场部", 12000, 32),
            new(3, "王五", "技术部", 18000, 35),
            new(4, "赵六", "人事部", 10000, 26),
            new(5, "钱七", "技术部", 20000, 40),
            new(6, "孙八", "市场部", 13000, 29),
            new(7, "周九", "技术部", 16000, 33),
            new(8, "吴十", "人事部", 11000, 27)
        };

        _products = new List<LinqProduct>
        {
            new(1, "笔记本电脑", "电子产品", 5999.99m, 50),
            new(2, "机械键盘", "电子产品", 399.99m, 200),
            new(3, "办公桌", "家具", 1299.99m, 30),
            new(4, "显示器", "电子产品", 2499.99m, 80),
            new(5, "办公椅", "家具", 899.99m, 60),
            new(6, "鼠标", "电子产品", 99.99m, 500),
            new(7, "书架", "家具", 599.99m, 40),
            new(8, "打印机", "电子产品", 1599.99m, 25)
        };
    }

    public void Demo()
    {
        Console.WriteLine("1. 基本查询语法:");
        BasicQuerySyntax();

        Console.WriteLine("\n2. 方法语法 (Lambda):");
        MethodSyntax();

        Console.WriteLine("\n3. 过滤与排序:");
        FilteringAndSorting();

        Console.WriteLine("\n4. 投影与转换:");
        ProjectionAndTransformation();

        Console.WriteLine("\n5. 聚合操作:");
        AggregationOperations();

        Console.WriteLine("\n6. 分组操作:");
        GroupingOperations();

        Console.WriteLine("\n7. 连接操作:");
        JoinOperations();

        Console.WriteLine("\n8. 集合操作:");
        SetOperations();

        Console.WriteLine("\n9. 分页操作:");
        PagingOperations();

        Console.WriteLine("\n10. 实际应用 - 数据分析:");
        PracticalExample();
    }

    private void BasicQuerySyntax()
    {
        // 查询语法 (类似 SQL)
        var techEmployees = from e in _employees
                           where e.Department == "技术部"
                           select e;

        Console.WriteLine("   技术部员工:");
        foreach (var emp in techEmployees)
        {
            Console.WriteLine($"     {emp.Name} - {emp.Salary:C}");
        }
    }

    private void MethodSyntax()
    {
        // 方法语法 (Lambda 表达式)
        var techEmployees = _employees
            .Where(e => e.Department == "技术部")
            .OrderByDescending(e => e.Salary);

        Console.WriteLine("   技术部员工 (按薪资降序):");
        foreach (var emp in techEmployees)
        {
            Console.WriteLine($"     {emp.Name} - {emp.Salary:C}");
        }
    }

    private void FilteringAndSorting()
    {
        // 复杂过滤
        var highSalaryTechEmployees = _employees
            .Where(e => e.Department == "技术部" && e.Salary > 15000)
            .OrderBy(e => e.Age);

        Console.WriteLine("   技术部高薪员工 (按年龄排序):");
        foreach (var emp in highSalaryTechEmployees)
        {
            Console.WriteLine($"     {emp.Name} - 年龄:{emp.Age} - 薪资:{emp.Salary:C}");
        }

        // 多条件排序
        var sortedProducts = _products
            .Where(p => p.Price > 500)
            .OrderBy(p => p.Category)
            .ThenByDescending(p => p.Price);

        Console.WriteLine("\n   高价产品 (按类别和价格排序):");
        foreach (var prod in sortedProducts)
        {
            Console.WriteLine($"     {prod.Name} - {prod.Category} - {prod.Price:C}");
        }
    }

    private void ProjectionAndTransformation()
    {
        // 投影 - 选择特定字段
        var employeeSummaries = _employees
            .Where(e => e.Department == "技术部")
            .Select(e => new
            {
                e.Name,
                e.Age,
                AnnualSalary = e.Salary * 12
            });

        Console.WriteLine("   技术部员工摘要:");
        foreach (var summary in employeeSummaries)
        {
            Console.WriteLine($"     {summary.Name} - 年龄:{summary.Age} - 年薪:{summary.AnnualSalary:C}");
        }

        // 转换为不同类型
        var productNames = _products
            .Where(p => p.Category == "电子产品")
            .Select(p => p.Name)
            .ToList();

        Console.WriteLine($"\n   电子产品名称: {string.Join(", ", productNames)}");
    }

    private void AggregationOperations()
    {
        // 基本聚合
        var techDeptStats = _employees
            .Where(e => e.Department == "技术部")
            .Aggregate(
                new { Count = 0, TotalSalary = 0m, MinSalary = decimal.MaxValue, MaxSalary = decimal.MinValue },
                (acc, e) => new
                {
                    Count = acc.Count + 1,
                    TotalSalary = acc.TotalSalary + e.Salary,
                    MinSalary = Math.Min(acc.MinSalary, e.Salary),
                    MaxSalary = Math.Max(acc.MaxSalary, e.Salary)
                }
            );

        Console.WriteLine("   技术部统计:");
        Console.WriteLine($"     人数: {techDeptStats.Count}");
        Console.WriteLine($"     平均薪资: {techDeptStats.TotalSalary / techDeptStats.Count:C}");
        Console.WriteLine($"     最低薪资: {techDeptStats.MinSalary:C}");
        Console.WriteLine($"     最高薪资: {techDeptStats.MaxSalary:C}");

        // 使用内置聚合方法
        var totalProducts = _products.Count();
        var totalValue = _products.Sum(p => p.Price * p.Stock);
        var avgPrice = _products.Average(p => p.Price);

        Console.WriteLine($"\n   产品统计:");
        Console.WriteLine($"     产品总数: {totalProducts}");
        Console.WriteLine($"     库存总价值: {totalValue:C}");
        Console.WriteLine($"     平均价格: {avgPrice:C}");
    }

    private void GroupingOperations()
    {
        // 按部门分组
        var employeesByDept = _employees
            .GroupBy(e => e.Department)
            .Select(g => new
            {
                Department = g.Key,
                Count = g.Count(),
                AvgSalary = g.Average(e => e.Salary)
            })
            .OrderByDescending(x => x.AvgSalary);

        Console.WriteLine("   部门统计:");
        foreach (var dept in employeesByDept)
        {
            Console.WriteLine($"     {dept.Department}: {dept.Count}人, 平均薪资:{dept.AvgSalary:C}");
        }

        // 按类别分组产品
        var productsByCategory = _products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Products = g.OrderByDescending(p => p.Price).ToList()
            });

        Console.WriteLine("\n   产品分类:");
        foreach (var category in productsByCategory)
        {
            Console.WriteLine($"     {category.Category}:");
            foreach (var prod in category.Products)
            {
                Console.WriteLine($"       {prod.Name} - {prod.Price:C}");
            }
        }
    }

    private void JoinOperations()
    {
        // 创建部门数据
        var departments = new List<Department>
        {
            new(1, "技术部", "北京"),
            new(2, "市场部", "上海"),
            new(3, "人事部", "广州")
        };

        // 连接查询
        var employeeDetails = _employees
            .Join(departments,
                e => e.Department,
                d => d.Name,
                (e, d) => new
                {
                    e.Name,
                    e.Department,
                    d.Location,
                    e.Salary
                })
            .OrderBy(x => x.Department);

        Console.WriteLine("   员工详细信息 (包含部门位置):");
        foreach (var detail in employeeDetails)
        {
            Console.WriteLine($"     {detail.Name} - {detail.Department} - {detail.Location} - {detail.Salary:C}");
        }
    }

    private void SetOperations()
    {
        // 创建两个产品列表
        var expensiveProducts = _products.Where(p => p.Price > 1000).Select(p => p.Name);
        var electronicProducts = _products.Where(p => p.Category == "电子产品").Select(p => p.Name);

        // 集合操作
        Console.WriteLine("   集合操作演示:");

        // 交集
        var intersection = expensiveProducts.Intersect(electronicProducts);
        Console.WriteLine($"     高价电子产品: {string.Join(", ", intersection)}");

        // 并集
        var union = expensiveProducts.Union(electronicProducts);
        Console.WriteLine($"     高价产品 ∪ 电子产品: {string.Join(", ", union)}");

        // 差集
        var except = expensiveProducts.Except(electronicProducts);
        Console.WriteLine($"     高价非电子产品: {string.Join(", ", except)}");
    }

    private void PagingOperations()
    {
        int pageSize = 3;
        int pageNumber = 2;

        // 分页查询
        var pagedProducts = _products
            .OrderBy(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        Console.WriteLine($"   第 {pageNumber} 页产品 (每页 {pageSize} 个):");
        foreach (var prod in pagedProducts)
        {
            Console.WriteLine($"     {prod.Id}. {prod.Name} - {prod.Price:C}");
        }

        // 总页数
        int totalPages = (int)Math.Ceiling((double)_products.Count / pageSize);
        Console.WriteLine($"   总页数: {totalPages}");
    }

    private void PracticalExample()
    {
        // 综合应用：产品销售分析
        Console.WriteLine("   产品销售分析报告:");

        // 1. 按类别统计
        var categoryStats = _products
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                TotalValue = g.Sum(p => p.Price * p.Stock),
                AvgPrice = g.Average(p => p.Price),
                TopProduct = g.OrderByDescending(p => p.Price).First()
            })
            .OrderByDescending(x => x.TotalValue);

        foreach (var stat in categoryStats)
        {
            Console.WriteLine($"\n     {stat.Category}:");
            Console.WriteLine($"       产品数量: {stat.Count}");
            Console.WriteLine($"       库存总价值: {stat.TotalValue:C}");
            Console.WriteLine($"       平均价格: {stat.AvgPrice:C}");
            Console.WriteLine($"       最贵产品: {stat.TopProduct.Name} ({stat.TopProduct.Price:C})");
        }

        // 2. 库存预警
        var lowStockProducts = _products
            .Where(p => p.Stock < 50)
            .OrderBy(p => p.Stock);

        Console.WriteLine("\n     库存预警 (库存 < 50):");
        foreach (var prod in lowStockProducts)
        {
            Console.WriteLine($"       ⚠️ {prod.Name}: {prod.Stock} 件");
        }

        // 3. 价格区间分析
        var priceRanges = new[]
        {
            new { Min = 0m, Max = 500m, Label = "低价 (0-500)" },
            new { Min = 500m, Max = 2000m, Label = "中价 (500-2000)" },
            new { Min = 2000m, Max = decimal.MaxValue, Label = "高价 (2000+)" }
        };

        Console.WriteLine("\n     价格区间分布:");
        foreach (var range in priceRanges)
        {
            var count = _products.Count(p => p.Price >= range.Min && p.Price < range.Max);
            Console.WriteLine($"       {range.Label}: {count} 个产品");
        }
    }
}

// 示例数据模型
// 注意：这里故意命名为 LinqProduct 以避免与 Models.Product 冲突
// 实际项目中应该通过合理的命名空间设计来避免此类问题
public record Employee(int Id, string Name, string Department, decimal Salary, int Age);
public record LinqProduct(int Id, string Name, string Category, decimal Price, int Stock);
public record Department(int Id, string Name, string Location);
