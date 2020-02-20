using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Presentation.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult ErrorCodes()
        {
            var assembly = Assembly.GetAssembly(typeof(BaseException));
            var classes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace == "Core.Exceptions").ToList();

            var exceptions = new List<BaseException>();
            foreach (var type in classes)
            {
                var obj = Activator.CreateInstance(type);

                var item = obj as BaseException;
                if (item != null)
                {
                    exceptions.Add(item);
                }
            }
            exceptions = exceptions.OrderBy(t => t.ErrorCode).ToList();
            ViewBag.Exceptions = exceptions;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}
