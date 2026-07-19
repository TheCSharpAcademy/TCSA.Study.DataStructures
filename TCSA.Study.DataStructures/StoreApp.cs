using Spectre.Console;
using TCSA.Study.DataStructures.Models;

namespace TCSA.Study.DataStructures;

public sealed class StoreApp
{
    private static readonly string[] Departments =
        [
           "Electronics",
           "Groceries",
           "Clothing",
           "Home",
           "Sports"
        ];

    private static readonly List<Product> Products =
        [
            new Product
            {
                Name = "Wireless Mouse",
                Sku = "ELE-001",
                Price = 29.99m,
                Department = "Electronics",
                StockQuantity = 12
            },
            new Product
            {
                Name = "Coffee Beans",
                Sku = "GRO-004",
                Price = 14.50m,
                Department = "Groceries",
                StockQuantity = 20
            }
        ];

    private static readonly Dictionary<string, Product> ProductsBySku =
    Products.ToDictionary(
        product => product.Sku,
        StringComparer.OrdinalIgnoreCase);

    public void Run()
    {
        bool isRunning = true;

        while (isRunning)
        {
            AnsiConsole.Clear();
            DisplayTitle();

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(5)
                    .AddChoices(
                        "View Departments",
                        "Manage Products",
                        "Find Product by SKU",
                        "Manage Product Tags",
                        "Exit"));

            switch (choice)
            {
                case "View Departments":
                    ShowDepartments();
                    break;
                case "Manage Products":
                    ShowProductMenu();
                    break;
                case "Find Product by SKU":
                    FindProductBySku();
                    break;
                case "Manage Product Tags":
                    ManageProductTags();
                    break;
                case "Exit":
                    isRunning = false;
                    break;
            }
        }
    }

    private static void ManageProductTags()
    {
        AnsiConsole.Clear();

        if (Products.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]There are no products.[/]");
            Pause();
            return;
        }

        string sku = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a product:")
                .AddChoices(Products.Select(product => product.Sku)));

        Product product = ProductsBySku[sku];
        bool returnToMainMenu = false;

        while (!returnToMainMenu)
        {
            AnsiConsole.Clear();
            ShowProductTags(product);

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Manage tags for [green]{Markup.Escape(product.Name)}[/]")
                    .AddChoices("Add Tag", "Remove Tag", "Return"));

            switch (choice)
            {
                case "Add Tag":
                    AddProductTag(product);
                    break;
                case "Remove Tag":
                    RemoveProductTag(product);
                    break;
                case "Return":
                    returnToMainMenu = true;
                    break;
            }
        }
    }

    private static void ShowProductTags(Product product)
    {
        AnsiConsole.MarkupLine(
            $"[green]Product Tags - {Markup.Escape(product.Name)}[/]");

        if (product.Tags.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No tags assigned.[/]");
            return;
        }

        foreach (string tag in product.Tags.OrderBy(tag => tag))
        {
            AnsiConsole.MarkupLine($"- {Markup.Escape(tag)}");
        }
    }

    private static void AddProductTag(Product product)
    {
        string tag = AnsiConsole.Ask<string>("Tag to add:").Trim();

        if (string.IsNullOrWhiteSpace(tag))
        {
            AnsiConsole.MarkupLine("[red]A tag cannot be empty.[/]");
        }
        else if (product.Tags.Add(tag))
        {
            AnsiConsole.MarkupLine("[green]Tag added.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine(
                $"[yellow]The tag {Markup.Escape(tag)} is already assigned.[/]");
        }

        Pause();
    }

    private static void RemoveProductTag(Product product)
    {
        if (product.Tags.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]This product has no tags.[/]");
            Pause();
            return;
        }

        string tag = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a tag to remove:")
                .AddChoices(product.Tags.OrderBy(tag => tag)));

        product.Tags.Remove(tag);
        AnsiConsole.MarkupLine("[green]Tag removed.[/]");
        Pause();
    }
    private static void FindProductBySku()
    {
        AnsiConsole.Clear();

        string sku = AnsiConsole.Ask<string>("Enter product SKU:").Trim();

        if (!ProductsBySku.TryGetValue(sku, out Product? product))
        {
            AnsiConsole.MarkupLine(
                $"[yellow]No product was found with SKU {Markup.Escape(sku)}.[/]");
            Pause();
            return;
        }

        var table = new Table()
            .Title("[green]Product Details[/]")
            .AddColumn("Field")
            .AddColumn("Value")
            .AddRow("Name", Markup.Escape(product.Name))
            .AddRow("SKU", Markup.Escape(product.Sku))
            .AddRow("Department", Markup.Escape(product.Department))
            .AddRow("Price", product.Price.ToString("C"))
            .AddRow("Stock", product.StockQuantity.ToString());

        AnsiConsole.Write(table);
        Pause();
    }

    private static void ShowProductMenu()
    {
        bool returnToMainMenu = false;

        while (!returnToMainMenu)
        {
            AnsiConsole.Clear();

            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Product Management[/]")
                    .AddChoices(
                        "View All Products",
                        "Add Product",
                        "Update Product",
                        "Return to Main Menu"));

            switch (choice)
            {
                case "View All Products":
                    ShowProducts();
                    break;
                case "Add Product":
                    AddProduct();
                    break;
                case "Update Product":
                    UpdateProduct();
                    break;
                case "Return to Main Menu":
                    returnToMainMenu = true;
                    break;
            }
        }
    }

    private static void ShowProducts()
    {
        AnsiConsole.Clear();

        var table = new Table()
            .Title("[green]Products[/]")
            .AddColumn("SKU")
            .AddColumn("Name")
            .AddColumn("Department")
            .AddColumn("Price")
            .AddColumn("Stock");

        foreach (Product product in Products)
        {
            table.AddRow(
                Markup.Escape(product.Sku),
                Markup.Escape(product.Name),
                Markup.Escape(product.Department),
                product.Price.ToString("C"),
                product.StockQuantity.ToString());
        }

        AnsiConsole.Write(table);
        Pause();
    }

    private static void AddProduct()
    {
        AnsiConsole.Clear();

        string sku = AnsiConsole.Ask<string>("SKU:").Trim().ToUpperInvariant();

        if (ProductsBySku.ContainsKey(sku))
        {
            AnsiConsole.MarkupLine(
                $"[red]A product with SKU {Markup.Escape(sku)} already exists.[/]");
            Pause();
            return;
        }

        string name = AnsiConsole.Ask<string>("Name:").Trim();
        string department = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Department:")
                .AddChoices(Departments));
        decimal price = AnsiConsole.Ask<decimal>("Price:");
        int stockQuantity = AnsiConsole.Ask<int>("Stock quantity:");

        var product = new Product
        {
            Name = name,
            Sku = sku,
            Price = price,
            Department = department,
            StockQuantity = stockQuantity
        };

        Products.Add(product);
        ProductsBySku.Add(product.Sku, product);
        Pause();
    }

    private static void UpdateProduct()
    {
        AnsiConsole.Clear();

        if (Products.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]There are no products to update.[/]");
            Pause();
            return;
        }

        string selectedSku = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose a product:")
                .AddChoices(Products.Select(product => product.Sku)));

        Product product = Products.Find(product => product.Sku == selectedSku)!;

        product.Name = AnsiConsole.Ask("Name:", product.Name);
        product.Department = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Department:")
                .AddChoices(Departments));
        product.Price = AnsiConsole.Ask("Price:", product.Price);
        product.StockQuantity = AnsiConsole.Ask("Stock quantity:", product.StockQuantity);

        AnsiConsole.MarkupLine("[green]Product updated successfully.[/]");
        Pause();
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey(intercept: true);
    }

    private static void DisplayTitle()
    {
        AnsiConsole.Write(
            new FigletText("Store Management")
                .Centered()
                .Color(Color.Green));
    }

    private static void ShowDepartments()
    {
        AnsiConsole.Clear();

        var table = new Table()
            .Title("[green]Store Departments[/]")
            .AddColumn("Number")
            .AddColumn("Department");

        for (int index = 0; index < Departments.Length; index++)
        {
            table.AddRow(
                (index + 1).ToString(),
                Markup.Escape(Departments[index]));
        }

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine("[grey]Press any key to return to the menu...[/]");
        Console.ReadKey(intercept: true);
    }
}