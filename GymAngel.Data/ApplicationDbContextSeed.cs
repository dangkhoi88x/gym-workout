using GymAngel.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymAngel.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ApplicationDbContext context)
        {
            // 1. Seed Roles
            var roles = new[] { "Admin", "Staff", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role { Name = role });
                }
            }

            // 2. Seed Admin
            if (await userManager.FindByEmailAsync("admin@gymangel.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@gymangel.com",
                    FullName = "System Admin",
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed=true,

                };

                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 3. Seed Staffs
            var staffs = new List<User>
            {
                new User { UserName = "staff1", Email = "staff1@gymangel.com", FullName = "Nguyen Van Staff 1", CreatedAt = DateTime.UtcNow },
                new User { UserName = "staff2", Email = "staff2@gymangel.com", FullName = "Tran Thi Staff 2", CreatedAt = DateTime.UtcNow }
            };

            foreach (var staff in staffs)
            {
                if (await userManager.FindByEmailAsync(staff.Email) == null)
                {
                    await userManager.CreateAsync(staff, "Staff@123");
                    await userManager.AddToRoleAsync(staff, "Staff");
                }
            }

            // 4. Seed Customers
            var customers = new List<User>
            {
                new User { UserName = "customer1", Email = "customer1@gymangel.com", FullName = "Nguyen Van A", CreatedAt = DateTime.UtcNow, MembershipStart = DateTime.UtcNow, MembershipExpiry = DateTime.UtcNow.AddMonths(6) },
                new User { UserName = "customer2", Email = "customer2@gymangel.com", FullName = "Tran Thi B", CreatedAt = DateTime.UtcNow, MembershipStart = DateTime.UtcNow, MembershipExpiry = DateTime.UtcNow.AddMonths(12) },
                new User { UserName = "customer3", Email = "customer3@gymangel.com", FullName = "Le Van C", CreatedAt = DateTime.UtcNow }
            };

            foreach (var customer in customers)
            {
                if (await userManager.FindByEmailAsync(customer.Email) == null)
                {
                    await userManager.CreateAsync(customer, "Customer@123");
                    await userManager.AddToRoleAsync(customer, "User");
                }
            }

            // 5. Seed Products
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Dumbbell 5kg", Price = 200000, Quantity = 20, Description = "Tạ đơn 5kg", ImageUrl = "/images/dumbbell5.jpg" },
                    new Product { Name = "Dumbbell 10kg", Price = 350000, Quantity = 15, Description = "Tạ đơn 10kg", ImageUrl = "/images/dumbbell10.jpg" },
                    new Product { Name = "Yoga Mat", Price = 150000, Quantity = 30, Description = "Thảm yoga chống trượt", ImageUrl = "/images/yogamat.jpg" },
                    new Product { Name = "Protein Powder", Price = 1200000, Quantity = 10, Description = "Whey protein 2kg", ImageUrl = "/images/protein.jpg" },
                    new Product { Name = "Shaker Bottle", Price = 80000, Quantity = 50, Description = "Bình lắc 600ml", ImageUrl = "/images/shaker.jpg" },
                    new Product { Name = "Treadmill", Price = 12000000, Quantity = 5, Description = "Máy chạy bộ điện", ImageUrl = "/images/treadmill.jpg" },
                    new Product { Name = "Resistance Band", Price = 60000, Quantity = 40, Description = "Dây kháng lực", ImageUrl = "/images/band.jpg" },
                    new Product { Name = "Pull-up Bar", Price = 500000, Quantity = 10, Description = "Xà đơn treo tường", ImageUrl = "/images/pullup.jpg" },
                    new Product { Name = "Kettlebell 12kg", Price = 450000, Quantity = 12, Description = "Tạ ấm 12kg", ImageUrl = "/images/kettlebell.jpg" },
                    new Product { Name = "Jump Rope", Price = 50000, Quantity = 60, Description = "Dây nhảy thể dục", ImageUrl = "/images/jumprope.jpg" }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // 6. Seed an Order (cho customer1)
            var customer1 = await userManager.FindByEmailAsync("customer1@gymangel.com");
            if (customer1 != null && !context.Orders.Any())
            {
                var firstProduct = context.Products.First();

                var order = new Order
                {
                    OrderNumber = "ORD-20260101-0001",
                    UserId = customer1.Id, // Now string from Identity
                    OrderDate = DateTime.UtcNow,
                    Status = "Delivered",
                    
                    SubtotalAmount = firstProduct.Price * 2,
                    DiscountAmount = 0,
                    TotalAmount = firstProduct.Price * 2,
                    
                    ReceiverName = "Nguyen Van A",
                    ReceiverPhone = "0123456789",
                    DeliveryAddress = "123 Street",
                    City = "Ho Chi Minh",
                    District = "District 1",
                    Ward = "Ward 1",
                    Notes = "Sample order",
                    
                    PaymentMethod = "COD",
                    PaymentStatus = "Paid",
                    CreatedAt = DateTime.UtcNow,
                    
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = firstProduct.Id,
                            ProductName = firstProduct.Name,
                            Quantity = 2,
                            UnitPrice = firstProduct.Price
                        }
                    }
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();
            }

            // 7. Seed Membership Plans
            if (!context.MembershipPlans.Any())
            {
                var plans = new List<MembershipPlan>
                {
                    new MembershipPlan
                    {
                        Name = "1 Month",
                        Description = "Perfect for trying out our gym",
                        DurationMonths = 1,
                        Price = 500000,
                        OriginalPrice = 500000,
                        IsPopular = false,
                        IsActive = true,
                        Features = "[\"Full gym access\",\"Basic locker usage\",\"Fitness consultation\"]"
                    },
                    new MembershipPlan
                    {
                        Name = "3 Months",
                        Description = "Great value for committed fitness enthusiasts",
                        DurationMonths = 3,
                        Price = 1200000,
                        OriginalPrice = 1500000,
                        IsPopular = false,
                        IsActive = true,
                        Features = "[\"Full gym access\",\"Locker + towel service\",\"1x Personal Training/week\",\"Nutrition consultation\"]"
                    },
                    new MembershipPlan
                    {
                        Name = "6 Months",
                        Description = "Most popular choice - Best value!",
                        DurationMonths = 6,
                        Price = 2100000,
                        OriginalPrice = 3000000,
                        IsPopular = true, // Highlight this plan
                        IsActive = true,
                        Features = "[\"Full gym access\",\"Premium locker + towel\",\"2x Personal Training/week\",\"Nutrition & meal planning\",\"Free supplements consultation\",\"Guest passes (2x/month)\"]"
                    },
                    new MembershipPlan
                    {
                        Name = "12 Months",
                        Description = "Maximum savings for serious athletes",
                        DurationMonths = 12,
                        Price = 3600000,
                        OriginalPrice = 6000000,
                        IsPopular = false,
                        IsActive = true,
                        Features = "[\"Full gym access\",\"VIP locker + premium towel\",\"Unlimited Personal Training\",\"Advanced nutrition planning\",\"Free supplements (monthly)\",\"Unlimited guest passes\",\"Priority class booking\",\"Free gym merchandise\"]"
                    }
                };

                context.MembershipPlans.AddRange(plans);
                await context.SaveChangesAsync();
            }
        }
    }
}
