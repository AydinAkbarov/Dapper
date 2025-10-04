using Dapper;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    public static int NavigateMenu<T>(List<T> options, string title, bool showBack = false, string lastOptionLabel = "<-back")
    {
        int selectedIndex = 0;
        int maxIndex = showBack ? options.Count : options.Count - 1;

        while (true)
        {

            Console.Clear();
            Console.WriteLine($"{title}\n");

            for (int i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($" | {options[i]}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                    Console.WriteLine($" | {options[i]}");
            }

            if (showBack)
            {
                if (selectedIndex == options.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n | {lastOptionLabel}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                    Console.WriteLine($"\n | {lastOptionLabel}");
            }

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1) < 0 ? maxIndex : selectedIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) > maxIndex ? 0 : selectedIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                if (showBack && selectedIndex == options.Count)
                    return -1;
                return selectedIndex;
            }
        }
    }
    static void AddProduct(SqlConnection con)
    {
        Console.Clear();


        //productName
        Console.Write("\n ~ Yeni mehsulun adini qeyd edin : ");
        string name = Console.ReadLine();

        //supplierID
        var sql = ("Select SupplierID, CompanyName FROM Suppliers ORDER BY SupplierID");
        var suppliers = con.Query(sql);

        Console.WriteLine($"\n Id - Company Name\n");
        foreach (var s in suppliers)
        {
            Console.WriteLine($" {s.SupplierID} - {s.CompanyName}");
        }
        Console.Write("\n ~ Mehsulu aldiginiz sirketin ID-sini daxil edin : ");
        int supplierId;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out supplierId))
            {
                if (suppliers.FirstOrDefault(x => x.SupplierID == supplierId) == null)
                    Console.WriteLine(" Bele IDde supplier tapilmadi");
                else
                    break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //categoryId
        var command = ("Select CategoryID, CategoryName FROM Categories");
        var categories = con.Query(command);
        Console.WriteLine($"\n Id - Category Name\n");
        foreach (var s in categories)
        {
            Console.WriteLine($" {s.CategoryID} - {s.CategoryName}");
        }
        Console.Write("\n ~ Mehsulun aid oldugu kategoriyani daxil edin : ");
        int categoryID;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out categoryID))
            {
                if (categories.FirstOrDefault(x => x.CategoryID == categoryID) == null)
                    Console.WriteLine(" Bele IDde kategoriya tapilmadi");
                else
                    break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //quantityPerUnit
        Console.Write("Quantity Per Unit-i daxil edin (10 boxes x 20 bags): ");
        string quantityPerUnit = Console.ReadLine();

        //UnitPrice
        decimal unitPrice;
        while (true)
        {
            Console.Write("\n Unit Price-i daxil edin: ");

            if (decimal.TryParse(Console.ReadLine(), out unitPrice))
            {
                break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //UnitInStock
        short unitInStock;
        while (true)
        {

            Console.Write("\n mehsuldan hal hazirda anbarda ne qeder var? : ");

            if (short.TryParse(Console.ReadLine(), out unitInStock))
            {
                break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //unitOnOrder
        short unitOnOrder;
        while (true)
        {

            Console.Write("\n Units On Order-u daxil edin: ");

            if (short.TryParse(Console.ReadLine(), out unitOnOrder))
            {
                break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //reorderlevel
        short reorderLevel;

        while (true)
        {
            Console.Write("\n Reorder Level-i daxil edin: ");

            if (short.TryParse(Console.ReadLine(), out reorderLevel))
            {
                break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }

        //discontinued 

        bool Discontinued;
        bool discontinued;
        Console.Write(" ~ Mehsul istifadeden cixib? (y/n): ");
        string input = Console.ReadLine();

        if (input.ToLower() == "y")
            discontinued = true;
        else
            discontinued = false;

        var comand = "INSERT INTO Products VALUES ( @ProductName, @SupplierID, @CategoryID, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued)";

        con.Execute(comand, new
        {
            ProductName = name,
            SupplierID = supplierId,
            CategoryID = categoryID,
            QuantityPerUnit = quantityPerUnit,
            UnitPrice = unitPrice,
            UnitsInStock = unitInStock,
            UnitsOnOrder = unitOnOrder,
            ReorderLevel = reorderLevel,
            Discontinued = discontinued
        });
        Console.WriteLine(" ugurla insert olundu");
    }

    static void EditProduct(SqlConnection con)
    {
        Console.WriteLine("\n ~ Products\n");
        var products = con.Query("SELECT * FROM Products");

        foreach (var p in products)
        {
            Console.WriteLine($"{p.ProductID} - {p.ProductName} - {p.UnitPrice}");
        }
        int productId;
        while (true)
        {
            Console.WriteLine("update etmek istediyiniz mehsulun idsini daxil edin    ");

            if (int.TryParse(Console.ReadLine(), out productId))
            {
                if (products.FirstOrDefault(x => x.ProductID == productId) == null)
                    Console.WriteLine(" Bele IDde kategoriya tapilmadi");
                else
                    break;
            }
            else
            {
                Console.WriteLine(" Yalnis input");
            }
        }
        Console.WriteLine("neyi update etmek isteyirsiniz?");
        List<string> choice = new List<string> { "ProductName", "UnitPrice" };
        var choiceIndex = NavigateMenu(choice, "~ Neyi update etmek isteyirsiniz?");
        if (choiceIndex == 0)
        {
            Console.WriteLine(" mehsulun yeni adini daxil edin");
            var newProductName = Console.ReadLine();

            var slq = "UPDATE Products SET ProductName =@ProductName WHERE ProductID = @productid; ";
            int affectedRows = con.Execute(slq, new
            {
                ProductName = newProductName,
                productid = productId
            });
        }

        if (choiceIndex == 1)
        {
            int UnitPrice;
            while (true)
            {
                Console.WriteLine(" mehsulun yeni unit pricesini daxil edin");

                if (int.TryParse(Console.ReadLine(), out UnitPrice))
                {
                    break;
                }
                else
                {
                    Console.WriteLine(" Yalnis input");
                }
            }
            var slq = "UPDATE Products SET UnitPrice =@unitPrice WHERE ProductID = @productID; ";
            int affectedRows = con.Execute(slq, new
            {
                unitPrice = UnitPrice,
                productID = productId
            });
        }
    }
    static void Main(string[] args)
    {
        var cs = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = NORSTWIND; Integrated Security = True; Connect Timeout = 30; Encrypt = False;";

        using var con = new SqlConnection(cs);
        var sql = string.Empty;

        List<string> choice = new List<string> { "Select All Products", "Add Product", "Edit Product", "remove Product", "Clear List" };
        int choiceIndex = NavigateMenu(choice, "");

        if (choiceIndex == 0)
        {
            Console.WriteLine("\n ~ Products\n");
            sql = "SELECT * FROM Products";
            var products = con.Query(sql);

            foreach (var p in products)
            {
                Console.WriteLine($"{p.ProductID} - {p.ProductName} - {p.UnitPrice}");
            }
        }
        else if (choiceIndex == 1)
        {
            AddProduct(con);
        }
        else if (choiceIndex == 2)
        {
            EditProduct(con);
        }
        else if (choiceIndex == 3)
        {
            Console.WriteLine("\n ~ Products\n");
            sql = "SELECT * FROM Products";
            var products = con.Query(sql);

            foreach (var p in products)
            {
                Console.WriteLine($"{p.ProductID} - {p.ProductName} - {p.UnitPrice}");
            }
            int productId;
            while (true)
            {
                Console.WriteLine("Silmek istediyiniz mehsulun idsini daxil edin    ");

                if (int.TryParse(Console.ReadLine(), out productId))
                {
                    break;
                }
                else
                {
                    Console.WriteLine(" Yalnis input");
                }
            }
            con.Execute("DELETE FROM [Order Details] WHERE ProductID = @Id", new { Id = productId });

            con.Execute("DELETE FROM Products WHERE ProductID = @Id", new { Id = productId });
        }
        else if (choiceIndex == 4)
        {
            string sql2 = @"
ALTER TABLE [Order Details]
DROP CONSTRAINT FK_OrderDetails_Products;

ALTER TABLE [Order Details]
ADD CONSTRAINT FK_OrderDetails_Products
FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
ON DELETE CASCADE
;
    ";
            string sql3 = "Delete From Products;";
            //con.Execute(sql2);
            con.Execute(sql3);

        }
    }
}
