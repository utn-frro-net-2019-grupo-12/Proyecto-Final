﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebPresentationMVC.Models;
using WebPresentationMVC.ViewModels;

namespace WebPresentationMVC.Controllers
{
    public class MateriaController : Controller
    {
        // GET: Materia
        public ActionResult Index()
        {
            var response = GlobalApi.WebApiClient.GetAsync("materias/departamento").Result;

            IEnumerable<MvcMateriaModel> materias = response.Content.ReadAsAsync<IEnumerable<MvcMateriaModel>>().Result;

            return View(materias);
        }


        public ActionResult Details(int id)
        {
            var response = GlobalApi.WebApiClient.GetAsync("materias/" + id.ToString() + "/departamento").Result;

            if (!response.IsSuccessStatusCode)
            {
                return View(response.Content.ReadAsAsync<ModelState>().Result);
            }

            var materia = response.Content.ReadAsAsync<MvcMateriaModel>().Result;
    
            return View(materia);
        }


        // DELETE Materia/5
        public ActionResult Delete(int id)
        {
            var response = GlobalApi.WebApiClient.DeleteAsync("materias/" + id.ToString()).Result;

            // Search what is TempData!
            TempData["SuccessMessage"] = "Deleted Sucessfully";

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Create()
        {
            var departamentos = GetDepartamentos();

            var viewModel = new CreateMateriaViewModel(departamentos);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateMateriaViewModel viewModel)
        {
            var response = GlobalApi.WebApiClient.PostAsJsonAsync("materias", viewModel.Materia).Result;

            // Move this to an action filter
            if (!response.IsSuccessStatusCode)
            {
                var departamentos = GetDepartamentos();

                viewModel.SetDepartamentosAsSelectList(departamentos);

                ModelState.AddModelErrorsFromResponse(response);

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var response = GlobalApi.WebApiClient.GetAsync("materias/" + id).Result;

            if (!response.IsSuccessStatusCode)
            {
                return HttpNotFound();
            }

            MvcMateriaModel materia = response.Content.ReadAsAsync<MvcMateriaModel>().Result;

            var departamentos = GetDepartamentos();

            var viewModel = new EditMateriaViewModel(departamentos, materia);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind(Include = "...") is used to avoid overposting attacks
        public ActionResult Edit(EditMateriaViewModel viewModel)
        {
            var response = GlobalApi.WebApiClient.PutAsJsonAsync("materias/" + viewModel.Materia.Id, viewModel.Materia).Result;

            if (!response.IsSuccessStatusCode)
            {
                var departamentos= GetDepartamentos();
                viewModel.SetDepartamentosAsSelectList(departamentos);

                ModelState.AddModelErrorsFromResponse(response);

                return View (viewModel);
            }

            return RedirectToAction("Index");
        }

        public IEnumerable<MvcDepartamentoModel> GetDepartamentos()
        {
            var response = GlobalApi.WebApiClient.GetAsync("departamentos").Result;

            return response.Content.ReadAsAsync<IEnumerable<MvcDepartamentoModel>>().Result;
        }
    }
}