using System.Reflection;
using NA.PMS.Service.APIRequestServices;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NA.PMS.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NA.PMS.Web.App_Start.NinjectWebCommon), "Stop")]

namespace NA.PMS.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using NA.PMS.Service;
    using NA.PMS.Service.Property;
    using NA.PMS.Service.BusinessRuleEngine;
    using NA.PMS.Service.TemplateParser;
    using NA.PMS.Service.AccountWS;
    using NA.PMS.Service.Reports;
    

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //Business engine
            kernel.Bind<IPaymentEngine>().To<PaymentEngine>();
            kernel.Bind<IAllotmentEngine>().To<AllotmentEngine>();
            //Doc parser
            kernel.Bind<ITemplateParserService>().To<TemplateParserService>();
            kernel.Bind<IAccountWSService>().To<AccountWSService>();

            kernel.Bind<ILoginService>().To<LoginService>();
            kernel.Bind<IManageUsersService>().To<ManageUsersService>();
            kernel.Bind<IManageRolesService>().To<ManageRolesService>();
            kernel.Bind<IMenuMappingService>().To<MenuMappingService>();
            kernel.Bind<IMastersService>().To<MastersService>();
            kernel.Bind<IGeneralService>().To<GeneralService>();
            kernel.Bind<ISchemeService>().To<SchemeService>();
            kernel.Bind<IPropertyAllotmentService>().To<PropertyAllotmentService>();
            kernel.Bind<IAllotmentService>().To<AllotmentService>();
            kernel.Bind<IPropertyRegistrationService>().To<PropertyRegistrationService>();
            kernel.Bind<ICompletionService>().To<CompletionService>();
            kernel.Bind<IPossessionService>().To<PossessionService>();
            kernel.Bind<ICICService>().To<CICService>();            
            kernel.Bind<IPropertyCancellationService>().To<PropertyCancellationService>();
            kernel.Bind<IMergeSplitPropertyService>().To<MergeSplitPropertyService>();
            kernel.Bind<IReportService>().To<ReportService>();
            kernel.Bind<IApiRequestService>().To<ApiRequestService>();
            kernel.Bind<ICitizenRequestsService>().To<CitizenRequestsService>();

            kernel.Bind<IOnlineService>().To<OnlineService>();
            kernel.Bind<IRevenueService>().To<RevenueService>();
            kernel.Bind<IRequestService>().To<RequestService>();

            //for customer
            kernel.Bind<ICustomerService>().To<CustomerService>();

            kernel.Bind<IGraphService>().To<GraphService>();
            kernel.Bind<INICService>().To<NICService>();
            kernel.Bind<INICFormService>().To<NICFormService>();

            ServiceLayerBootstrapper.Bootstrap(kernel);
            
        }        
    }
}
