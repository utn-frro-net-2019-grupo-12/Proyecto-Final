﻿using System.Collections.Generic;

namespace DataAccess.Repositories {
    public interface IMateriaRepository : IRepository<Materia> {
        IEnumerable<Materia> GetMateriasOrderedByName();
        IEnumerable<Materia> GetMateriasWithDepto();
        Materia GetMateriaWithDepto(int id);
    }
}