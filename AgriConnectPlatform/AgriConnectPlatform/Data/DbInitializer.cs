using AgriConnectPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriConnectPlatform.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new AgriConnectContext(
                serviceProvider.GetRequiredService<DbContextOptions<AgriConnectContext>>());
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await context.Database.EnsureCreatedAsync();

            // Create roles first
            var roles = new[] { "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create employee account first
            string employeeEmail = "employee@agriconnect.co.za";
            string employeePassword = "Password@123!";

            var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
            if (employeeUser == null)
            {
                employeeUser = new IdentityUser 
                { 
                    UserName = employeeEmail, 
                    Email = employeeEmail,
                    EmailConfirmed = true 
                };
                var result = await userManager.CreateAsync(employeeUser, employeePassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }

            // Check if we need to create demo data
            if (context.Products.Any() && context.Farmers.Any())
            {
                Console.WriteLine("Database already contains data. Skipping initialization.");
                return;
            }

            var demoUsers = new[]
            {
                "farmer1@agriconnect.co.za",
                "farmer2@agriconnect.co.za",
                "farmer3@agriconnect.co.za",
                "farmer4@agriconnect.co.za"
            };

            Console.WriteLine("\n=== Starting Database Initialization ===");
            Console.WriteLine("Starting farmer user creation process...");
            var createdUsers = new List<IdentityUser>();
            
            foreach (var email in demoUsers)
            {
                Console.WriteLine($"Processing user: {email}");
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser == null)
                {
                    Console.WriteLine($"Creating new user: {email}");
                    var newUser = new IdentityUser 
                    { 
                        UserName = email, 
                        Email = email, 
                        EmailConfirmed = true 
                    };
                    var result = await userManager.CreateAsync(newUser, "Password@123!");
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Successfully created user: {email}");
                        await userManager.AddToRoleAsync(newUser, "Farmer");
                        createdUsers.Add(newUser);
                        Console.WriteLine($"Added user to createdUsers list. Count: {createdUsers.Count}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"Found existing user: {email}");
                    createdUsers.Add(existingUser);
                    Console.WriteLine($"Added existing user to createdUsers list. Count: {createdUsers.Count}");
                }
            }

            Console.WriteLine($"Final createdUsers count: {createdUsers.Count}");
            if (createdUsers.Count != 4)
            {
                Console.WriteLine("ERROR: Not all demo farmers were created successfully!");
                throw new Exception("Not all demo farmers were created successfully. Please check the logs for errors.");
            }

            // Create farmer records
            Console.WriteLine("Creating farmer records...");
            var farmers = new List<Farmer>
            {
                new Farmer { FarmerId = createdUsers[0].Id, FarmName = "Green Pastures Farm", Location = "Stellenbosch, Western Cape"},
                new Farmer { FarmerId = createdUsers[1].Id, FarmName = "Golden Harvest Organics", Location = "Hartbeespoort, North West"},
                new Farmer { FarmerId = createdUsers[2].Id, FarmName = "Eco-Farms", Location = "Durban, KwaZulu-Natal"},
                new Farmer { FarmerId = createdUsers[3].Id, FarmName = "Sustainable Roots", Location = "Middelburg, Mpumalanga" }
            };

            try
            {
                context.Farmers.AddRange(farmers);
                var saveResult = await context.SaveChangesAsync();
                Console.WriteLine($"SaveChanges result: {saveResult} farmers added");

                // Verify farmers were actually added
                var verifyFarmers = await context.Farmers.ToListAsync();
                Console.WriteLine($"Verification: Found {verifyFarmers.Count} farmers in database");
                foreach (var farmer in verifyFarmers)
                {
                    Console.WriteLine($"Farmer in DB: ID={farmer.FarmerId}, Name={farmer.FarmName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving farmers: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }

            // Only create products if we successfully created farmers
            if (context.Farmers.Any())
            {
                Console.WriteLine("Starting product creation...");
                var products = new List<Product>
                {
                    // Green Pastures Farm
                    new Product
                    {
                        productName = "Organic Tomatoes",
                        Description = "Juicy, red organic tomatoes grown in solar-powered greenhouses. High in lycopene and flavor.",
                        Category = ProductCategory.Vegetables,
                        DateCreated = DateTime.Now.AddDays(-5),
                        CreatedByUserId = createdUsers[0].Id
                    },
                    new Product
                    {
                        productName = "Free-Range Eggs",
                        Description = "Large, brown eggs from pasture-raised hens. Fed with organic, non-GMO feed.",
                        Category = ProductCategory.Poultry,
                        DateCreated = DateTime.Now.AddDays(-3),
                        CreatedByUserId = createdUsers[0].Id
                    },
                    new Product
                    {
                        productName = "Solar Dried Figs",
                        Description = "Naturally sweet figs, dried using solar energy. A healthy, eco-friendly snack.",
                        Category = ProductCategory.Fruits,
                        DateCreated = DateTime.Now.AddDays(-10),
                        CreatedByUserId = createdUsers[0].Id
                    },
                    new Product
                    {
                        productName = "Biogas Fertilizer",
                        Description = "Organic fertilizer produced from farm biogas digesters. Boosts soil health and sustainability.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-7),
                        CreatedByUserId = createdUsers[0].Id
                    },

                    // Golden Harvest Organics
                    new Product
                    {
                        productName = "Grass-Fed Beef",
                        Description = "Premium beef from cattle grazing on organic pastures. No hormones or antibiotics.",
                        Category = ProductCategory.Livestock,
                        DateCreated = DateTime.Now.AddDays(-7),
                        CreatedByUserId = createdUsers[1].Id
                    },
                    new Product
                    {
                        productName = "Organic Apples",
                        Description = "Crisp, sweet apples grown with rainwater harvesting and natural pest control.",
                        Category = ProductCategory.Fruits,
                        DateCreated = DateTime.Now.AddDays(-4),
                        CreatedByUserId = createdUsers[1].Id
                    },
                    new Product
                    {
                        productName = "Wind-Powered Wheat Flour",
                        Description = "Stone-ground flour from wheat processed using wind energy. High in fiber.",
                        Category = ProductCategory.Grains,
                        DateCreated = DateTime.Now.AddDays(-6),
                        CreatedByUserId = createdUsers[1].Id
                    },
                    new Product
                    {
                        productName = "Raw Honey",
                        Description = "Unfiltered honey from hives supporting local pollinators and biodiversity.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-12),
                        CreatedByUserId = createdUsers[1].Id
                    },

                    // Eco-Farms
                    new Product
                    {
                        productName = "Hydroponic Lettuce",
                        Description = "Crisp lettuce grown in a closed-loop hydroponic system. Uses 90% less water.",
                        Category = ProductCategory.Vegetables,
                        DateCreated = DateTime.Now.AddDays(-3),
                        CreatedByUserId = createdUsers[2].Id
                    },
                    new Product
                    {
                        productName = "Solar-Powered Microgreens",
                        Description = "Nutrient-dense microgreens grown under solar-powered LED lights.",
                        Category = ProductCategory.Vegetables,
                        DateCreated = DateTime.Now.AddDays(-2),
                        CreatedByUserId = createdUsers[2].Id
                    },
                    new Product
                    {
                        productName = "Biofuel Pellets",
                        Description = "Renewable energy pellets made from agricultural waste. Clean and efficient.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-20),
                        CreatedByUserId = createdUsers[2].Id
                    },

                    // Sustainable Roots
                    new Product
                    {
                        productName = "Rainwater Harvesting System",
                        Description = "Complete system for collecting and storing rainwater for irrigation.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-22),
                        CreatedByUserId = createdUsers[3].Id
                    },
                    new Product
                    {
                        productName = "Organic Fertilizer",
                        Description = "Fertilizer made from composted plant material. Improves soil health and reduces waste.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-18),
                        CreatedByUserId = createdUsers[3].Id
                    },
                    new Product
                    {
                        productName = "Solar Panels (Farm Use)",
                        Description = "High-efficiency solar panels designed for agricultural use. Power your farm sustainably.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-25),
                        CreatedByUserId = createdUsers[3].Id
                    },
                    new Product
                    {
                        productName = "Wind Turbine",
                        Description = "Small-scale wind turbine for generating clean electricity on the farm.",
                        Category = ProductCategory.Other,
                        DateCreated = DateTime.Now.AddDays(-30),
                        CreatedByUserId = createdUsers[3].Id
                    }
                };

                try
                {
                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Products created successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving products: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
        }
    }
}