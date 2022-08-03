using CapstoneProject_.NETFSD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneProject_.NETFSD.Data.Services
{
   public interface IMedicinesService
    {
        Task<IEnumerable<Medicine>> GetAll();
        Task<Medicine> GetByID(int id);
        Task Add(Medicine medicine);
        Task<Medicine> Update(int id, Medicine newMedicine);
        Task Delete(int id);
        
    }
}
