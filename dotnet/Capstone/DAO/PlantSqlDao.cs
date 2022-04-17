﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;
using System.Transactions;

namespace Capstone.DAO
{
    public class PlantSqlDao : IPlantDao
    {
        private readonly string connectionString;
        
        public PlantSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Plant GetPlantById(int plantId)
        {
            Plant plant = new Plant();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM plants " +
                                                    "WHERE plant_id = @plant_id", conn);
                    cmd.Parameters.AddWithValue("@plant_id", plantId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        plant = GetPlantFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return plant;
        }

        //returns entire Plants table
        public List<Plant> GetAllPlants()
        {
            List<Plant> plants = new List<Plant>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM plants", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Plant plant = GetPlantFromReader(reader);
                        plants.Add(plant);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return plants;
        }


        // returns plants specific to their sun requirements (sun, part-shade, shade)
        public List<Plant> GetAllPlantsBySunRequirements(string sun_requirements)
        {
            List<Plant> plants = new List<Plant>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    "FROM plants " +
                                                    "WHERE sun_requirements = @sun_requirements", conn);
                    cmd.Parameters.AddWithValue("@sun_requirements", sun_requirements);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Plant plant = GetPlantFromReader(reader);
                        plants.Add(plant);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return plants;
        }

        //returns plants specific to a plot 
        public List<Plant> GetAllPlantsByPlot(int plot_id)
        {
            List<Plant> plants = new List<Plant>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT p.plant_id, p.common_name, p.description, p.square_area, p.cost, p.sun_requirements, p.image_url, p.temporary_usda_zones  " +
                                                    "FROM plants p " +
                                                    "JOIN plots_plants pp " +
                                                    "ON p.plant_id = pp.plant_id " +
                                                    "WHERE pp.plot_id = @plot_id", conn);
                    cmd.Parameters.AddWithValue("@plot_id", plot_id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Plant plant = GetPlantFromReader(reader);
                        plants.Add(plant);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return plants;
        }

        //add plant to data store. returns added plant
        public Plant AddPlant(Plant plantToAdd)
        {
            int plantId;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO plants (common_name, description, square_area, cost, sun_requirements, image_url, temporary_usda_zones) " +
                                                    "OUTPUT INSERTED.plant_id " +
                                                    "VALUES (@common_name, @description, @square_area, @cost, @sun_requirements, @image_url, @temporary_usda_zones);", conn);
                    cmd.Parameters.AddWithValue("@common_name", plantToAdd.CommonName);
                    cmd.Parameters.AddWithValue("@description", plantToAdd.Description);
                    cmd.Parameters.AddWithValue("@square_area", plantToAdd.SquareArea);
                    cmd.Parameters.AddWithValue("@cost", plantToAdd.Cost);
                    cmd.Parameters.AddWithValue("@sun_requirements", plantToAdd.SunRequirements);
                    cmd.Parameters.AddWithValue("@image_url", plantToAdd.ImageUrl);
                    cmd.Parameters.AddWithValue("@temporary_usda_zones", plantToAdd.TemporaryUsdaZones);
                    plantId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch
            {
                throw;
            }

            Plant addedPlant = GetPlantById(plantId);
            return addedPlant;
        }

       
       //update plant in data store. returns updated plant.
       public Plant UpdatePlant(int plantId, Plant plantToUpdate)
       {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE plants " +
                                                    "SET common_name = @common_name, description = @description, square_area = @square_area, " +
                                                    "cost = @cost, sun_requirements = @sun_requirements, image_url = @image_url, " +
                                                    "@temporary_usda_zones = temporary_usda_zones " +
                                                    "WHERE plant_id = @plant_id", conn);
                    cmd.Parameters.AddWithValue("@common_name", plantToUpdate.CommonName);
                    cmd.Parameters.AddWithValue("@description", plantToUpdate.Description);
                    cmd.Parameters.AddWithValue("@square_area", plantToUpdate.SquareArea);
                    cmd.Parameters.AddWithValue("@cost", plantToUpdate.Cost);
                    cmd.Parameters.AddWithValue("@sun_requirements", plantToUpdate.SunRequirements);
                    cmd.Parameters.AddWithValue("@image_url", plantToUpdate.ImageUrl);
                    cmd.Parameters.AddWithValue("@temporary_usda_zones", plantToUpdate.TemporaryUsdaZones);
                    cmd.Parameters.AddWithValue("@plant_id", plantId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }

            Plant addedPlant = GetPlantById(plantId);
            return addedPlant;
        }

        //
        public bool AddPlantToPlot(PlantedPlant plantReadyForPlanting)
        {
            bool isPlanted = false;
            int atPlanting;
            int inTheGround;
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO plots_plants (plot_id, plant_id) " +
                                                    "OUTPUT INSERTED.plots_plants_id " +
                                                    "VALUES(@plot_id, @plant_id)", conn);
                    cmd.Parameters.AddWithValue("@plot_id", plantReadyForPlanting.PlotId);
                    cmd.Parameters.AddWithValue("@plant_id", plantReadyForPlanting.PlantId);
                    atPlanting = Convert.ToInt32(cmd.ExecuteScalar());
                    // ???redundant but tests itself????
                    SqlCommand cmd1 = new SqlCommand("SELECT * FROM plots_plants WHERE plots_plants_id = @plots_plants_id", conn);
                    cmd1.Parameters.AddWithValue("@plots_plants_id", atPlanting);
                    inTheGround = Convert.ToInt32(cmd1.ExecuteScalar());
                }
            }
            catch
            {
                throw;
            }

            if (atPlanting == inTheGround)
                isPlanted = true;

            return isPlanted;
        }

        private Plant GetPlantFromReader(SqlDataReader reader)
        {
            Plant plant = new Plant();
            plant.PlantId = Convert.ToInt32(reader["plant_id"]);
            plant.CommonName = Convert.ToString(reader["common_name"]);
            plant.Description = Convert.ToString(reader["description"]);
            plant.SquareArea = Convert.ToDecimal(reader["square_area"]);
            plant.Cost = Convert.ToDecimal(reader["cost"]);
            plant.SunRequirements = Convert.ToString(reader["sun_requirements"]);
            plant.ImageUrl = Convert.ToString(reader["image_url"]);
            plant.TemporaryUsdaZones = Convert.ToString(reader["temporary_usda_zones"]);
            
            return plant;
        }
    }
}
