﻿using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface ICapsuleRepository<DbContextType> : IGeneralRepository<Capsule> where DbContextType : BaseContext { }
}
