﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAO
{
    public interface ISupplyDao
    {
        List<Supply> GetAllSupplies();
        List<Supply> GetAllSuppliesByUser(int userId);
        bool AddSupplyToFarmList(SupplyListItem supplyToAdd);
        bool RemoveSupplyFromFarmList(SupplyListItem supplyToRemove);

        /* 
         * bool AddSupplyToDatabase();
           bool RemoveSupplyFromDatabase();
        */
    }
}
