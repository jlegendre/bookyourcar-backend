using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using TestAuthentification.Models;
using TestAuthentification.Services.DbConfig;

namespace TestAuthentification.Services
{
    public class LocationService
    {
        public BookYourCarContext _context;

        public LocationService(BookYourCarContext context, CustomIdentityErrorDescriber errors = null)
        {
            _context = context;
        }

        public List<Vehicle> GetAvailableVehicleForLocation(DateTime dateDebut, DateTime dateFin, int? poleDebut, int? poleFin)
        {
            try
            {
                List<MySqlParameter> parametres = new List<MySqlParameter>
                {
                    new MySqlParameter(
                        "DATEDEBUT", MySqlDbType.DateTime, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, dateDebut),
                    new MySqlParameter(
                        "DATEFIN", MySqlDbType.DateTime, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, dateFin),
                    new MySqlParameter(
                        "POLESTART", MySqlDbType.Int32, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, poleDebut),
                    new MySqlParameter(
                        "POLEEND", MySqlDbType.Int32, Int32.MaxValue, ParameterDirection.Input, false, 1,1, "",DataRowVersion.Current, poleFin)
                };
                using (var dbm = new DbManager())
                {
                    return dbm.ExecuteList<Vehicle>("getAvailableVehicle", parametres);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }





        /**using (var connection = new MySqlConnection("server=mvinet.fr;port=3306;database=BookYourCar;uid=a5d;password=pwtk@[gh$!7Z#&wX"))
        {
            var command = new MySqlCommand("getAvailableVehicle", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new MySqlParameter("DATEDEBUT", dateDebut));
            command.Parameters.Add(new MySqlParameter("DATEFIN", dateFin));
            command.Parameters.Add(new MySqlParameter("POLESTART", poleDebut));
            command.Parameters.Add(new MySqlParameter("POLEEND", poleFin));
            command.Connection.Open();
            var result = command.ExecuteReader();
            command.Connection.Close();

            var listeVehicule = new List<Vehicle>();
            foreach (var vehicule in result)
            {
                var veh = new Vehicle();
                while (result.HasRows)
                {
                    //TODO recuperer les valeurs et les mettres dans un objet Vehicule
                }

            }

        }**/

        //_context.Database.ExecuteSqlCommand("getAvailableVehicle @p0, @p1, @p2, @p3", parameters: new[] { DateTime.Now.ToString(), DateTime.Now.ToString(), 1.ToString(), 1.ToString() });

    }
}


