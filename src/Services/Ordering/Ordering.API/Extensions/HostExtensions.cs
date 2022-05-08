using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ordering.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postresql database.");

                    using var connection = new NpgsqlConnection
                        (configuration.GetValue<string>("ConnectionStrings:DefaultConnection"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS Orders";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Orders(Id SERIAL PRIMARY KEY, 
                                                                UserName TEXT, TotalPrice INT, FirstName TEXT, LastName TEXT, EmailAddress TEXT, AddressLine TEXT, Country TEXT,
                                                                State TEXT, ZipCode TEXT, CardName TEXT, CardNumber TEXT, Expiration TEXT, CVV TEXT, PaymentMethod INT, CreatedBy TEXT,
                                                                CreatedDate DATE, LastModifiedBy TEXT, LastModifiedDate DATE)";
                    command.ExecuteNonQuery();

                    //command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    //command.ExecuteNonQuery();

                    //command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    //command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postresql database.");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}