using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using SocialSharing.Service;
using SocialSharing.Service.Facebook;
using SocialSharing.Service.SocialSharing;
using SocialSharing.Service.Twitter;
using SocialSharing.Service.LinkedIn;

namespace SocialSharing
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IFacebookService, FacebookService>();
            container.RegisterType<ITwitterService, TwitterService>();
            container.RegisterType<ILinkedInService, LinkedInService>();
            container.RegisterType<ISocialSharingService, SocialSharingService>();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}