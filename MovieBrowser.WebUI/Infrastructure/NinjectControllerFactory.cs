using System.Web.Mvc;
using System;
using System.Web.Routing;
using Ninject;
using Moq;
using System.Linq;
using System.Collections.Generic;
using MovieBrowser.Domain.Entities;
using MovieBrowser.Domain.Abstract;
using MovieBrowser.Domain.Concrete;


namespace MovieBrowser.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext
        requestContext, Type controllerType)
        {
            return controllerType == null
            ? null
            : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IMovieRepository>().To<EFMovieRepository>();
        }
    }
}