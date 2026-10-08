using NA.PMS.Repository;
using NA.PMS.Repository.AccountWS;
using NA.PMS.Repository.Property;
using NA.PMS.Repository.TemplateParser;
using NA.PMS.Service.APIRequestServices;
using Ninject;
using Ninject.Modules;

namespace NA.PMS.Service
{
    public class ServiceLayerBootstrapper
    {
        public static void Bootstrap(IKernel kernel)
        {
            //Business engine
            kernel.Bind<IBusinessRuleRepository>().To<BusinessRuleRepository>();
            kernel.Bind<IPaymentEngineRepository>().To<PaymentEngineRepository>();
            //Doc parser
            kernel.Bind<ITemplateParserRepository>().To<TemplateParserRepository>();
            kernel.Bind<IAccountWSRepository>().To<AccountWSRepository>();

            kernel.Bind<ILoginRepository>().To<LoginRepository>();
            kernel.Bind<IManageUsersRepository>().To<ManageUsersRepository>();
            kernel.Bind<IManageRolesRepository>().To<ManageRolesRepository>();
            kernel.Bind<IMenuMappingRepository>().To<MenuMappingRepository>();
            kernel.Bind<IMastersRepositiory>().To<MastersRepositiory>();
            kernel.Bind<ISchemeRepository>().To<SchemeRepository>();
            kernel.Bind<IAllotmentRepository>().To<AllotmentRepository>();
            kernel.Bind<IPropertyAllotmentRepository>().To<PropertyAllotmentRepository>();
            kernel.Bind<IPropertyRegistrationRepository>().To<PropertyRegistrationRepository>();
            kernel.Bind<IPossessionRepository>().To<PossessionRepository>();
            kernel.Bind<ICompletionRepository>().To<CompletionRepository>();
            kernel.Bind<IGeneralRepository>().To<GeneralRepository>();
            kernel.Bind<ICICRepository>().To<CICRepository>();
            kernel.Bind<IPropertyCancellationRepository>().To<PropertyCancellationRepository>();
            kernel.Bind<IMergeSplitPropertyRepository>().To<MergeSplitPropertyRepository>();

            kernel.Bind<IOnlineRepository>().To<OnlineRepository>();
            kernel.Bind<IRevenueRepository>().To<RevenueRepository>();
            kernel.Bind<IServiceRepository>().To<ServiceRepository>();

            //for customer
            kernel.Bind<ICustomerRepository>().To<CustomerRepository>();

            kernel.Bind<IGraphRepository>().To<GraphRepository>();
            // etc

            kernel.Bind<IPIMSAPIRepository>().To<PIMSAPIRepository>();
            kernel.Bind<INICRepository>().To<NICRepository>();
            kernel.Bind<INICFormRepository>().To<NICFormRepository>();
        }
    }
}
