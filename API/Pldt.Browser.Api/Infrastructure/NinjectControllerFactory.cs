using Ninject;
using Pldt.Browser.Api.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Pldt.Browser.Api.Infrastructure
{
    public class NinjectControllerFactory : IHttpControllerActivator
    {
        IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            this._ninjectKernel = new StandardKernel();
            this.AddBindings();
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)_ninjectKernel.GetService(controllerType);

            return controller;
        }

        void AddBindings()
        {
            /*Types here will always be available from controllers' constructor*/
            this._ninjectKernel.Bind<IEFRepository>().To<EFRepository>();

            //this._ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }
    }
}