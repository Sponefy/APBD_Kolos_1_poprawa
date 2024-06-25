using Kolos_1_poprawa.Dtos;
using Microsoft.Data.SqlClient;

namespace Kolos_2_poprawa.Services;

public class ClientService : IClientService
{
    private readonly IConfiguration _configuration;

    public ClientService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<ClientDto?> GetClient(int clientId)
    {
        var query = @"
            Select 
                C.ID,
                C.FirstName,
                C.LastName,
                C.Address,
                CA.VIN,
                CL.Name AS Color,
                M.Name AS Model,
                CR.DateFrom,
                CR.DateTo,
                CR.TotalPrice
            from clients C
            join car_rentals CR on C.ID = CR.ClientID
            join cars CA on CA.ID = CR.CarID
            join models M on CA.ModelID = M.ID
            join colors CL on CA.ColorID = CL.ID
            where C.ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", clientId);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        ClientDto result = null;

        while (await reader.ReadAsync())
        {
            if (result == null)
            {
                result = new ClientDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),

                    Rentals = new List<RentalDto>
                    {
                        new RentalDto
                        {
                            Vin = reader.GetString(reader.GetOrdinal("VIN")),
                            Color = reader.GetString(reader.GetOrdinal("Color")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                        }
                    }
                };
            }
            else
            {
                result.Rentals.Add(new RentalDto
                {
                    Vin = reader.GetString(reader.GetOrdinal("VIN")),
                    Color = reader.GetString(reader.GetOrdinal("Color")),
                    Model = reader.GetString(reader.GetOrdinal("Model")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                });
            }
        }

        return result;
    }

    public async Task<int> AddClient(AddRentalDto rentalDto)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        
        var queryGetCar = @"
                Select PricePerDay
                From cars
                Where cars.ID = @ID";
        
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = queryGetCar;
        command.Parameters.AddWithValue("@ID", rentalDto.CarID);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        int pricePerDay = 0;
        pricePerDay = reader.GetInt32(reader.GetOrdinal("TotalPrice"));

        if (pricePerDay == 0)
        {
            return 0;
        }
        
        var queryAddClient = @"
                Insert into clients (FirstName, LastName, Address)
                output Inserted.PK
                Values (@FN, @LN, @AD)";

        var queryAddRental = @"
                Insert into car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice, Discount)
                VALUES (@CLID, @CID, @DF, @DT, @TP, null)";
        
        await using SqlCommand commandAddClient = new SqlCommand(queryAddClient, connection);
        commandAddClient.Parameters.AddWithValue("@FN", rentalDto.Client.FirstName);
        commandAddClient.Parameters.AddWithValue("@LN", rentalDto.Client.LastName);
        commandAddClient.Parameters.AddWithValue("@AD", rentalDto.Client.Address);

        var clientID = (int) await commandAddClient.ExecuteScalarAsync();


        var totalPrice = (rentalDto.DateFrom - rentalDto.DateTo).Days * pricePerDay;


        await using SqlCommand commandAddRental = new SqlCommand(queryAddRental, connection);
        commandAddRental.Parameters.AddWithValue("@CLID", clientID);
        commandAddRental.Parameters.AddWithValue("@CID", rentalDto.CarID);
        commandAddRental.Parameters.AddWithValue("@DF", rentalDto.DateFrom);
        commandAddRental.Parameters.AddWithValue("@DT", rentalDto.DateTo);
        commandAddRental.Parameters.AddWithValue("@TP", totalPrice);


        await commandAddRental.ExecuteNonQueryAsync();

        return 1;
    }
}