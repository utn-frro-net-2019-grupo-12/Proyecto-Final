﻿using Presentation.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Library.Api.Endpoints.Interfaces {
    public interface IHorarioConsultaFechadoEndpoint : IEndpoint<HorarioConsultaFechado> {
        Task<IEnumerable<HorarioConsultaFechado>> GetByNewSearch(string descMateria, string descProfesor, string token);
    }
}
