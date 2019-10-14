﻿using AutoMapper;
using Presentation.Library.Api.Endpoints.Interfaces;
using Presentation.Library.Api.Exceptions;
using Presentation.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Presentation.Web.MVC.Models;
using Presentation.Web.MVC.ViewModels;

namespace Presentation.Web.MVC.Controllers {

    // Note: This Controller Communicates with ViewModels (CreateInscripcionViewModel and EditInscripcionViewModel)
    [Authorize]
    public class InscripcionController : Controller {
        private IInscripcionEndpoint _inscripcionEndpoint;
        private IUsuarioEndpoint _usuarioEndpoint;
        private IHorarioConsultaFechadoEndpoint _horarioConsultaFechadoEndpoint;
        private IUserSession _userSession;
        private IMapper _mapper;

        public InscripcionController(IInscripcionEndpoint inscripcionEndpoint, IUserSession userSession, 
            IUsuarioEndpoint usuarioEndpoint, IHorarioConsultaFechadoEndpoint horarioConsultaFechadoEndpoint ,IMapper mapper)
        {
            _inscripcionEndpoint = inscripcionEndpoint;
            _usuarioEndpoint = usuarioEndpoint;
            _horarioConsultaFechadoEndpoint = horarioConsultaFechadoEndpoint;
            _userSession = userSession;
            _mapper = mapper;
        }

        // Index - GET Inscripcion
        public async Task<ActionResult> Index()
        {
            try
            {
                IEnumerable<Inscripcion> entities = await _inscripcionEndpoint.GetAll(_userSession.BearerToken);

                var inscripciones = _mapper.Map<IEnumerable<MvcInscripcionModel>>(entities);

                return View(inscripciones);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }
        }

        // Details - GET Inscripcion/ID
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                Inscripcion entity = await _inscripcionEndpoint.Get(id, _userSession.BearerToken);

                var inscripcion = _mapper.Map<MvcInscripcionModel>(entity);

                return View(inscripcion);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (NotFoundRequestException ex)
            {
                return Content($"{ex.StatusCode}: Elemento no encontrado");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }
        }

        // Delete - DELETE Inscripcion/ID
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _inscripcionEndpoint.Delete(id, _userSession.BearerToken);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (NotFoundRequestException ex)
            {
                return Content($"{ex.StatusCode}: Elemento no encontrado");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }

            // TempData may be used to check in the view whether the deletion was successful or not
            TempData["SuccessMessage"] = "Deleted Sucessfully";

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // Create (Default)
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            try
            {
                // These tasks run in parallel until they are awaited by Task.WhenAll method
                var alumnosTask = _usuarioEndpoint.GetAll(_userSession.BearerToken);
                var horariosConsultaFechadosTask = _horarioConsultaFechadoEndpoint.GetAll(_userSession.BearerToken);

                await Task.WhenAll(alumnosTask, horariosConsultaFechadosTask);

                var alumnos = _mapper.Map<IEnumerable<MvcUsuarioModel>>(source: alumnosTask.Result);
                var horarioConsultaFechados = _mapper.Map<IEnumerable<MvcHorarioConsultaFechadoModel>>(source: horariosConsultaFechadosTask.Result);

                var viewModel = new CreateInscripcionViewModel(horariosConsultaFechados: horarioConsultaFechados, alumnos: alumnos);

                return PartialView("_Create", viewModel);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }
        }

        // Create - Post Inscripcion
        [HttpPost]
        public async Task<ActionResult> Create(CreateInscripcionViewModel viewModel)
        {
            try
            {
                var entity = _mapper.Map<Inscripcion>(source: viewModel.Inscripcion);

                await _inscripcionEndpoint.Post(entity, _userSession.BearerToken);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (BadRequestException ex)
            {
                var alumnosTask = _usuarioEndpoint.GetAll(_userSession.BearerToken);
                var horariosConsultaFechadosTask = _horarioConsultaFechadoEndpoint.GetAll(_userSession.BearerToken);

                await Task.WhenAll(alumnosTask, horariosConsultaFechadosTask);

                var profesores = _mapper.Map<IEnumerable<MvcUsuarioModel>>(source: alumnosTask.Result);
                var materias = _mapper.Map<IEnumerable<MvcHorarioConsultaFechadoModel>>(source: horariosConsultaFechadosTask.Result);

                viewModel.SetAlumnosAsSelectList(profesores);
                viewModel.SetHorariosConsultaFechadosAsSelectList(materias);

                ModelState.AddModelErrors(ex.Errors);

                return PartialView("_Create", viewModel);
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }

            return Content("OK");
        }


        // Edit - GET Inscripcion/ID
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Content("Debe incluir el id");
            }

            try
            {
                var inscripcionTask = _inscripcionEndpoint.Get(id, _userSession.BearerToken);
                var alumnosTask = _usuarioEndpoint.GetAll(_userSession.BearerToken);
                var horariosConsultaFechadosTask = _horarioConsultaFechadoEndpoint.GetAll(_userSession.BearerToken);

                await Task.WhenAll(inscripcionTask, alumnosTask, horariosConsultaFechadosTask);

                var inscripcion = _mapper.Map<MvcInscripcionModel>(source: inscripcionTask.Result);
                var alumnos = _mapper.Map<IEnumerable<MvcUsuarioModel>>(source: alumnosTask.Result);
                var horariosConsultaFechados = _mapper.Map<IEnumerable<MvcHorarioConsultaFechadoModel>>(source: horariosConsultaFechadosTask.Result);

                var viewModel = new EditInscripcionViewModel(inscripcion: inscripcion, horariosConsultaFechados: horariosConsultaFechados, alumnos: alumnos);

                return PartialView("_Edit", viewModel);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (NotFoundRequestException ex)
            {
                return Content($"{ex.StatusCode}: Elemento no encontrado");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }
        }

        // Edit - PUT Inscripcion/ID (Secured)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind(Include = "...") is used to avoid overposting attacks
        public async Task<ActionResult> Edit(EditInscripcionViewModel viewModel)
        {
            try
            {
                var entity = _mapper.Map<Inscripcion>(viewModel.Inscripcion);

                await _inscripcionEndpoint.Put(entity, _userSession.BearerToken);
            }
            catch (UnauthorizedRequestException)
            {
                return Content("No tiene acceso");
            }
            catch (BadRequestException ex)
            {
                var profesoresTask = _usuarioEndpoint.GetAll(_userSession.BearerToken);
                var horariosConsultaFechadosTask = _horarioConsultaFechadoEndpoint.GetAll(_userSession.BearerToken); // May throw an exception, so that is why the modal is not showing in nico user

                await Task.WhenAll(profesoresTask, horariosConsultaFechadosTask);

                var alumnos = _mapper.Map<IEnumerable<MvcUsuarioModel>>(source: profesoresTask.Result);
                var horariosConsultaFechado = _mapper.Map<IEnumerable<MvcHorarioConsultaFechadoModel>>(source: horariosConsultaFechadosTask.Result);

                viewModel.SetAlumnosAsSelectList(alumnos);
                viewModel.SetHorariosConsultaFechadosAsSelectList(horariosConsultaFechado);

                ModelState.AddModelErrors(ex.Errors);

                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message} Ha ocurrido un error. Por favor contacte a soporte");
            }

            return Content("OK");
        }
    }
}